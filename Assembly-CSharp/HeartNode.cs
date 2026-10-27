using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑暗之心 Boss 天体。
/// 特性：
/// 1. 无法被飞船破坏占领条，也无法被己方飞船修复占领条；
///    只有当 mapconfig 中 connected="tag1,tag2,..." 关联的天体被占领成中立/别的势力那一刻，
///    该 connected 天体及其驻留飞船立即变回 Heart 势力，同时 Heart 占领条 -25%。
/// 2. 每次 -25% 时，半径 2.5 同化周围天体，其余势力生产/破坏/飞行速度 *0.9（可叠加），并弹出提示。
/// 3. 占领条归零时，清空该天体与驻留飞船并播放炸星特效。
/// </summary>
public class HeartNode : PlanetNode
{
	public HeartNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Heart;
		}
	}

	private List<string> connectedTags = new List<string>();

	/// <summary>Heart 与每个子天体之间的连线（LineRenderer，下标与 connectedTags 对应）。</summary>
	private List<HeartLine> connectedLines = new List<HeartLine>();

	private bool destroyed = false;

	/// <summary>已叠加的减速层数（0 表示未减速，速度倍率 = 0.9^debuffStack）。</summary>
	private int debuffStack = 0;

	/// <summary>
	/// 设置 connected 关联的天体 tag 列表（由 SceneManager.CreateNode 在创建节点后注入）。
	/// </summary>
	public void SetConnectedTags(string connected)
	{
		this.connectedTags.Clear();
		if (string.IsNullOrEmpty(connected))
		{
			return;
		}
		string[] array = connected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (!string.IsNullOrEmpty(text))
			{
				this.connectedTags.Add(text);
			}
		}
	}

	/// <summary>
	/// 在 Heart 与每个子天体之间画一条连线（在子天体节点都创建完成后调用）。
	/// 使用 LineRenderer 直接绘制，不参与 AI 寻路，且每帧跟随子天体位置（支持轨道）。
	/// </summary>
	public void CreateConnectedLines()
	{
		this.ClearConnectedLines();
		if (this.nodeManager == null)
		{
			return;
		}
		for (int i = 0; i < this.connectedTags.Count; i++)
		{
			Node connected = this.nodeManager.GetNode(this.connectedTags[i]);
			if (connected == null)
			{
				continue;
			}
			HeartLine line = new HeartLine();
			line.connected = connected;
			line.go = this.CreateLineGameObject();
			if (line.go != null)
			{
				line.renderer = line.go.GetComponentInChildren<LineRenderer>();
				if (line.renderer != null)
				{
					line.renderer.positionCount = 2;
					line.renderer.sortingLayerID = SortingLayer.NameToID("Node");
					line.renderer.sortingOrder = 1;
				}
			}
			this.connectedLines.Add(line);
		}
		this.UpdateConnectedLines();
	}

	private GameObject CreateLineGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_LaserLineNew");
		if (resources == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		GameObject root = Solarmax.Singleton<BattleSystem>.Instance.battleData.root;
		gameObject.transform.SetParent(root != null ? root.transform : null);
		return gameObject;
	}

	/// <summary>
	/// 每帧刷新连线的端点与颜色（端点跟随 Heart / 子天体当前位置，颜色跟随子天体当前势力）。
	/// </summary>
	private void UpdateConnectedLines()
	{
		if (this.connectedLines.Count == 0)
		{
			return;
		}
		Vector3 heartPos = this.GetPosition();
		for (int i = 0; i < this.connectedLines.Count; i++)
		{
			HeartLine line = this.connectedLines[i];
			if (line == null || line.connected == null || line.renderer == null)
			{
				continue;
			}
			Vector3 connectedPos = line.connected.GetPosition();
			line.renderer.SetPosition(0, heartPos);
			line.renderer.SetPosition(1, connectedPos);
			Color color = (line.connected.currentTeam != null) ? line.connected.currentTeam.color : Color.white;
			color.a = 1f;
			line.renderer.startColor = color;
			line.renderer.endColor = color;
		}
	}

	/// <summary>
	/// 销毁 Heart 与子天体之间的连线。
	/// </summary>
	private void ClearConnectedLines()
	{
		for (int i = 0; i < this.connectedLines.Count; i++)
		{
			HeartLine line = this.connectedLines[i];
			if (line != null && line.go != null)
			{
				UnityEngine.Object.Destroy(line.go);
			}
		}
		this.connectedLines.Clear();
	}

	/// <summary>
	/// 节点销毁（退出关卡 / 切图重建场景）时，清理 Heart 与子天体之间的连线，
	/// 否则这些挂在 battleData.root 下的 LineRenderer 会残留到下一次战斗。
	/// </summary>
	public override void Destroy()
	{
		this.ClearConnectedLines();
		base.Destroy();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		this.TickHeart(frame);
	}

	protected override void UpdateCapturing(int frame, float dt)
	{
		// 飞船无法破坏占领条
	}

	protected override void UpdateOccupied(int frame, float dt)
	{
		// 己方飞船无法修复占领条
	}

	private void TickHeart(int frame)
	{
		if (this.destroyed)
		{
			return;
		}
		if (this.hp <= 0f)
		{
			this.OnHeartDestroyed();
			return;
		}
		if (this.nodeManager == null || this.sceneManager == null)
		{
			return;
		}
		if (this.connectedTags.Count == 0)
		{
			return;
		}
		// 刷新连线的端点与颜色
		this.UpdateConnectedLines();
		TEAM heartTeamId = this.team;
		Team heartTeam = this.sceneManager.teamManager.GetTeam(heartTeamId);
		if (heartTeam == null)
		{
			return;
		}

		// 必须所有 connected 天体「同时」处于 != heart（中立/别的势力）才触发
		for (int i = 0; i < this.connectedTags.Count; i++)
		{
			Node connected = this.nodeManager.GetNode(this.connectedTags[i]);
			if (connected == null || !connected.CanBeTarget())
			{
				continue;
			}
			if (connected.team == heartTeamId)
			{
				return; // 还有 connected 是 Heart 势力，未同时满足
			}
		}

		// 全部同时 != heart -> 全部变回 Heart 势力 + Heart -25%
		for (int i = 0; i < this.connectedTags.Count; i++)
		{
			Node connected = this.nodeManager.GetNode(this.connectedTags[i]);
			if (connected == null || !connected.CanBeTarget())
			{
				continue;
			}
			if (connected.team != heartTeamId)
			{
				connected.AssimilateTo(heartTeam);
				if (connected.appear > 0)
				{
					connected.AddShip((int)heartTeamId, connected.appear, true, true);
				}
			}
		}
		this.ApplyHeartDamage(heartTeamId);
	}

	private void ApplyHeartDamage(TEAM heartTeamId)
	{
		if (this.destroyed || this.hp <= 0f)
		{
			return;
		}
		this.hp -= this.hpMax * 0.25f;
		if (this.hp < 0f)
		{
			this.hp = 0f;
		}

		// 3. 半径 2.5 同化周围天体
		// 注意：同化中心就是 Heart 自己，AssimilateTo 会把 hp 重置回满血，必须先保存、同化后恢复，
		// 保证 Heart 的占领条只能按 connected 机制减少、且不会被同化回血。
		float damagedHp = this.hp;
		this.sceneManager.AssimilateAround(this.GetPosition(), 2.5f, heartTeamId);
		this.hp = damagedHp;

		if (this.hp <= 0f)
		{
			// 最后一次 -25% -> 恢复减速 + 炸星（炸星文本用 "这股压力消失了"）
			this.RecoverDebuff(heartTeamId);
			this.OnHeartDestroyed();
		}
		else
		{
			// 提示（用 Heart 势力颜色）
			Team heartTeamForColor = this.sceneManager.teamManager.GetTeam(heartTeamId);
			if (heartTeamForColor != null)
			{
				Color32 heartColor = heartTeamForColor.color;
				string hex = heartColor.r.ToString("X2") + heartColor.g.ToString("X2") + heartColor.b.ToString("X2");
				Tips.Make("[" + hex + "]你受到一股无形的侵蚀[-]");
			}
			else
			{
				Tips.Make("你受到一股无形的侵蚀");
			}
			// 还有血 -> 其余势力生产/破坏/飞行速度 *= 0.9（乘算可叠加）
			this.debuffStack++;
			this.ApplyDebuff(heartTeamId);
		}
	}

	private void ApplyDebuff(TEAM heartTeamId)
	{
		// 速度倍率 = 0.9^debuffStack
		float addPercent = Mathf.Pow(0.9f, (float)this.debuffStack) - 1f;
		this.SetOtherTeamsDebuff(heartTeamId, addPercent);
	}

	private void RecoverDebuff(TEAM heartTeamId)
	{
		this.debuffStack = 0;
		this.SetOtherTeamsDebuff(heartTeamId, 0f);
	}

	private void SetOtherTeamsDebuff(TEAM heartTeamId, float addPercent)
	{
		TeamManager teamManager = this.sceneManager.teamManager;
		if (teamManager == null)
		{
			return;
		}
		Team heartTeam = teamManager.GetTeam(heartTeamId);
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = teamManager.GetTeam((TEAM)i);
			if (team == null || !team.Valid())
			{
				continue;
			}
			if (team.team == heartTeamId)
			{
				continue;
			}
			if (heartTeam != null && heartTeam.IsFriend((int)team.team))
			{
				continue; // 跳过 Heart 的盟友，与同化逻辑保持一致
			}
			this.SetDebuffAttr(team, TeamAttr.ProduceSpeed, addPercent);
			this.SetDebuffAttr(team, TeamAttr.CapturedSpeed, addPercent);
			this.SetDebuffAttr(team, TeamAttr.Speed, addPercent);
		}
	}

	private void SetDebuffAttr(Team team, TeamAttr attr, float addPercent)
	{
		AttributeObject attributeObject = team.GetAttribute(attr);
		if (attributeObject == null)
		{
			return;
		}
		attributeObject.addPercent = addPercent;
		attributeObject.Calculate();
	}

	private void OnHeartDestroyed()
	{
		if (this.destroyed)
		{
			return;
		}
		this.destroyed = true;
		// 移除与子天体的连线
		this.ClearConnectedLines();
		// 炸星提示（无色）
		Tips.Make("这股压力消失了");
		if (this.sceneManager != null)
		{
			this.sceneManager.ExplodeNode(this);
		}
	}

	/// <summary>Heart 与某个子天体之间的一条连线。</summary>
	private class HeartLine
	{
		public Node connected;

		public GameObject go;

		public LineRenderer renderer;
	}
}
