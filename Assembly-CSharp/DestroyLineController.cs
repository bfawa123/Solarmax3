using System.Collections.Generic;
using Solarmax;
using UnityEngine;

/// <summary>
/// 摧毁线（长地图）控制器：一条固定线段（Destroy1→Destroy2），整张星图缓慢向其漂移，
/// 飞船/天体碰到线即被摧毁（天体走炸星、飞船走黑洞吞噬）。配合 The_end 节点实现终点胜利/跳图。
/// </summary>
public class DestroyLineController
{
	private SceneManager sceneManager;

	private bool enabled;

	private Vector3 lineP1;

	private Vector3 lineP2;

	private Vector2 lineDir;

	private Vector2 driftDir;

	private float driftSpeed;

	private int safeCamp = -1;

	private int safeTimes;

	private string theEndTag;

	/// <summary>true 表示不漂移星图，仅保留一条固定摧毁线（适合放在地图中央）。</summary>
	private bool notMoved;

	/// <summary>本帧漂移增量（PreTick 计算，Node.ApplyDestroyLineDrift 消费）。</summary>
	private Vector2 driftDelta;

	/// <summary>漂移去重集合（Curse 等节点同时存在于多个列表，避免重复漂移）。</summary>
	private HashSet<Node> driftDedup = new HashSet<Node>();

	private GameObject lineGo;

	private LineRenderer lineRenderer;

	/// <summary>飞船判定为「碰到线」的半径（点到线段）。</summary>
	private const float ShipHitRadius = 0.3f;

	/// <summary>是否豁免 The_end 节点不被线摧毁（保留胜利路径）。</summary>
	private bool exemptTheEnd = true;

	public DestroyLineController(SceneManager sm)
	{
		this.sceneManager = sm;
	}

	public void Setup(MapConfig cfg)
	{
		this.Reset();
		this.driftDelta = Vector2.zero;
		if (cfg == null || !cfg.escapeEnabled)
		{
			return;
		}
		this.lineP1 = new Vector3(cfg.destroy1X, cfg.destroy1Y, 0f);
		this.lineP2 = new Vector3(cfg.destroy2X, cfg.destroy2Y, 0f);
		this.driftSpeed = cfg.destroyLineSpeed * 0.01f;
		this.safeCamp = cfg.safeShipCamp;
		this.safeTimes = cfg.safeShipTimes;
		this.theEndTag = cfg.the_end;
		this.notMoved = cfg.notMoved;

		Vector3 seg = this.lineP2 - this.lineP1;
		if (seg.sqrMagnitude < 0.0001f)
		{
			return;
		}
		Vector2 dir = new Vector2(seg.x, seg.y).normalized;
		this.lineDir = dir;

		// 静态线：不漂移，仅作为固定摧毁线
		if (this.notMoved)
		{
			this.enabled = true;
			this.CreateLineVisual();
			return;
		}

		if (this.driftSpeed <= 0f)
		{
			return;
		}
		// 垂直于连线的两个方向之一：perp = (dir.y, -dir.x)
		Vector2 perp = new Vector2(dir.y, -dir.x);
		// 根据天体质心所在侧，取「朝摧毁线」的垂直方向（把天体推向线）
		Vector3 centroid = this.ComputeCentroid();
		float signed = dir.x * (centroid.y - this.lineP1.y) - dir.y * (centroid.x - this.lineP1.x);
		this.driftDir = perp * (signed >= 0f ? 1f : -1f);

		this.enabled = true;
		this.CreateLineVisual();
	}

	private Vector3 ComputeCentroid()
	{
		List<Node> nodes = this.sceneManager.nodeManager.GetUsefulNodeList();
		if (nodes == null || nodes.Count == 0)
		{
			return Vector3.zero;
		}
		Vector3 sum = Vector3.zero;
		int count = 0;
		for (int i = 0; i < nodes.Count; i++)
		{
			Node node = nodes[i];
			if (node == null)
			{
				continue;
			}
			sum += node.GetPosition();
			count++;
		}
		if (count == 0)
		{
			return Vector3.zero;
		}
		return sum / (float)count;
	}

	/// <summary>每帧先于 nodeManager.Tick 调用：计算漂移增量并施加到所有天体（轨道参考系 + 静止实体）。</summary>
	public void PreTick(float interval)
	{
		if (!this.enabled || this.notMoved)
		{
			return;
		}
		this.driftDelta = this.driftDir * (this.driftSpeed * interval);
		this.driftDedup.Clear();
		this.DriftNodes(this.sceneManager.nodeManager.GetUsefulNodeList());
		this.DriftNodes(this.sceneManager.nodeManager.GetGlobalNodeList());
		this.DriftNodes(this.sceneManager.nodeManager.GetBarrierNodeList());
		this.sceneManager.nodeManager.ApplyDestroyLineDrift(this.driftDelta);
	}

	private void DriftNodes(List<Node> nodes)
	{
		if (nodes == null)
		{
			return;
		}
		for (int i = 0; i < nodes.Count; i++)
		{
			Node node = nodes[i];
			if (node != null && this.driftDedup.Add(node))
			{
				node.ApplyDestroyLineDrift(this.driftDelta);
			}
		}
	}

	/// <summary>每帧在 shipManager.Tick 之后调用：摧毁碰到线的天体与飞船。</summary>
	public void PostTick()
	{
		if (!this.enabled)
		{
			return;
		}
		// 天体：圆与线段相交 → 炸星（ExplodeNode 延迟移除并同步人口上限）
		List<Node> nodes = this.sceneManager.nodeManager.GetUsefulNodeList();
		for (int i = 0; i < nodes.Count; i++)
		{
			Node node = nodes[i];
			if (node == null)
			{
				continue;
			}
			if (this.exemptTheEnd && !string.IsNullOrEmpty(this.theEndTag) && node.tag == this.theEndTag)
			{
				continue;
			}
			if (this.sceneManager.nodeManager.IntersectCircle(this.lineP1, this.lineP2, node.GetPosition(), node.GetWidth()))
			{
				this.sceneManager.ExplodeNode(node);
			}
		}
		// 障碍点：碰到线则连同相连的障碍线(BarrierLine)一起移除
		List<Node> barriers = this.sceneManager.nodeManager.GetBarrierNodeList();
		List<string> removeBarrierTags = null;
		for (int i = 0; i < barriers.Count; i++)
		{
			Node barrier = barriers[i];
			if (barrier == null || (barrier.type != NodeType.Barrier && barrier.type != NodeType.BarrierPoint))
			{
				continue;
			}
			if (this.sceneManager.nodeManager.IntersectCircle(this.lineP1, this.lineP2, barrier.GetPosition(), barrier.GetWidth()))
			{
				if (removeBarrierTags == null)
				{
					removeBarrierTags = new List<string>();
				}
				removeBarrierTags.Add(barrier.tag);
			}
		}
		if (removeBarrierTags != null)
		{
			for (int i = 0; i < removeBarrierTags.Count; i++)
			{
				this.sceneManager.nodeManager.RemoveBarrierAndLines(removeBarrierTags[i]);
			}
		}
		// 飞船：倒序遍历（Bomb 可能在 num==0 时从 flyMap 移除元素）
		for (int t = 0; t < LocalPlayer.MaxTeamNum; t++)
		{
			List<Ship> fly = this.sceneManager.shipManager.GetFlyShip((TEAM)t);
			for (int i = fly.Count - 1; i >= 0; i--)
			{
				Ship ship = fly[i];
				if (ship != null)
				{
					this.CheckShip(ship);
				}
			}
		}
	}

	private void CheckShip(Ship ship)
	{
		Vector3 pos = ship.GetPosition();
		bool isSafe = this.safeCamp >= 0 && (int)ship.team == this.safeCamp;

		if (isSafe)
		{
			float signed = this.SignedDistance(pos);
			if (ship.safeCrossRemain < 0)
			{
				ship.safeCrossRemain = this.safeTimes;
				ship.sideInited = false;
			}
			if (!ship.sideInited)
			{
				ship.lastSideSign = signed;
				ship.sideInited = true;
			}
			else if (Mathf.Abs(signed) > 0.001f && Mathf.Abs(ship.lastSideSign) > 0.001f && Mathf.Sign(signed) != Mathf.Sign(ship.lastSideSign))
			{
				ship.safeCrossRemain--;
				ship.lastSideSign = signed;
			}
			if (ship.safeCrossRemain < 0)
			{
				ship.Bomb(NodeType.None);
			}
		}
		else if (this.sceneManager.nodeManager.IntersectCircle(this.lineP1, this.lineP2, pos, ShipHitRadius))
		{
			ship.Bomb(NodeType.None);
		}
	}

	private float SignedDistance(Vector3 pos)
	{
		return this.lineDir.x * (pos.y - this.lineP1.y) - this.lineDir.y * (pos.x - this.lineP1.x);
	}

	public void Reset()
	{
		this.enabled = false;
		this.driftDelta = Vector2.zero;
		this.driftDedup.Clear();
		if (this.lineGo != null)
		{
			UnityEngine.Object.Destroy(this.lineGo);
			this.lineGo = null;
			this.lineRenderer = null;
		}
	}

	private void CreateLineVisual()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_LaserLineNew");
		if (resources == null)
		{
			return;
		}
		GameObject go = UnityEngine.Object.Instantiate(resources) as GameObject;
		if (go == null)
		{
			return;
		}
		GameObject root = Solarmax.Singleton<BattleSystem>.Instance.battleData.root;
		go.transform.SetParent(root != null ? root.transform : null);
		this.lineRenderer = go.GetComponentInChildren<LineRenderer>();
		if (this.lineRenderer != null)
		{
			this.lineRenderer.positionCount = 2;
			this.lineRenderer.sortingLayerID = SortingLayer.NameToID("Node");
			this.lineRenderer.sortingOrder = 2;
			this.lineRenderer.widthMultiplier = 0.5f;
			this.lineRenderer.SetPosition(0, this.lineP1);
			this.lineRenderer.SetPosition(1, this.lineP2);
			this.lineRenderer.startColor = Color.red;
			this.lineRenderer.endColor = Color.red;
		}
		this.lineGo = go;
	}
}
