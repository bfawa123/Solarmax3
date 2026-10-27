using System;
using Solarmax;
using UnityEngine;

public class Ship : IPoolObject<Ship>, Lifecycle2
{
	public SceneManager sceneManager { get; set; }

	public Node currentNode { get; set; }

	public TEAM team
	{
		get
		{
			return (this.currentTeam != null) ? this.currentTeam.team : TEAM.Neutral;
		}
	}

	public Team currentTeam
	{
		get
		{
			return this.realTeam;
		}
		set
		{
			if (this.realTeam == value)
			{
				return;
			}
			this.realTeam = value;
			if (this.realTeam == null)
			{
				return;
			}
			this.SetColor(this.realTeam.color);
			this.SetAlpha(1f);
		}
	}

	private Team realTeam { get; set; }

	private ShipManager pool { get; set; }

	private bool isALive { get; set; }

	public EntityShip entity { get; set; }

	public float hp { get; set; }

	public int num
	{
		get
		{
			return this.shipNum;
		}
		set
		{
			if (value == this.shipNum)
			{
				return;
			}
			this.shipNum = value;
			if (this.entity != null)
			{
				if (this.shipNum == 1)
				{
					this.entity.ReSize(1f);
				}
				else if (this.shipNum >= 2 && this.shipNum < 5)
				{
					this.entity.ReSize(1.2f);
				}
				else
				{
					this.entity.ReSize(1.5f);
				}
			}
		}
	}

	public bool Init()
	{
		if (this.sceneManager != null)
		{
			this.currentTeam = this.sceneManager.teamManager.GetTeam(TEAM.Neutral);
		}
		this.isALive = false;
		this.currentNode = null;
		this.num = 1;
		this.forceShow = false;
		this.safeCrossRemain = -1;
		this.sideInited = false;
		this.lastSideSign = 0f;
		return true;
	}

	public void Tick(int frame, float interval)
	{
		if (!this.isALive)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.Tick(frame, interval);
		}
		this.HealthHp(frame, interval);
	}

	public void UpdateRender(float interval)
	{
		if (!this.isALive)
		{
			return;
		}
		if (this.entity != null)
		{
			this.entity.UpdateRender(interval);
		}
	}

	public void Destroy()
	{
		if (this.sceneManager != null)
		{
			this.currentTeam = this.sceneManager.teamManager.GetTeam(TEAM.Neutral);
		}
		this.isALive = false;
		this.currentNode = null;
		this.num = 1;
		this.entity.OnRecycle();
		this.forceShow = false;
		this.safeCrossRemain = -1;
		this.sideInited = false;
		this.lastSideSign = 0f;
	}

	public override void OnRecycle()
	{
		if (this.currentNode != null)
		{
			this.currentNode.RemoveShip(this.team, this, true);
		}
		this.pool.RemoveFlyShip(this);
		this.entity.OnRecycle();
	}

	public void PoolRecovery()
	{
		this.pool.Recycle(this);
	}

	public bool IsActive()
	{
		return this.isALive;
	}

	public void InitShip(ShipManager sm, bool vertical, bool noAnim)
	{
		this.pool = sm;
		this.sceneManager = sm.sceneManager;
		if (this.entity == null)
		{
			this.entity = sm.GetFreeShipEnity();
			if (this.entity == null)
			{
				this.entity = new EntityShip("ship", this.sceneManager.battleData.silent);
			}
			if (this.entity != null)
			{
				this.entity.Init();
			}
		}
		else
		{
			this.entity.SilentMode(this.sceneManager.battleData.silent);
		}
		this.entity.InitShipEntity(this, vertical, noAnim);
		this.hp = this.currentTeam.hpMax;
		if (!noAnim)
		{
			Solarmax.Singleton<EffectManager>.Get().AddMakeEffect(this.currentNode, this.entity.GetPosition(), this.entity.GetColor(), true, false);
		}
		this.currentTeam.SetAttribute(TeamAttr.Population, 1f, true);
		this.num = 1;
		this.safeCrossRemain = -1;
		this.sideInited = false;
		this.lastSideSign = 0f;
		this.isALive = true;
	}

	public void InitShip(ShipManager sm)
	{
		this.pool = sm;
		this.sceneManager = sm.sceneManager;
		if (this.entity == null)
		{
			this.entity = new EntityShip("ship", this.sceneManager.battleData.silent);
			if (this.entity != null)
			{
				this.entity.Init();
			}
		}
		this.isALive = false;
	}

	public void Bomb(NodeType eType = NodeType.None)
	{
		this.currentTeam.SetAttribute(TeamAttr.Population, -1f, true);
		this.currentTeam.playerData.skillPower++;
		this.num--;
		if (this.temNum > 0)
		{
			this.temNum--;
		}
		if (this.currentNode != null && this.currentNode.shipsRelifeTag[(int)this.team] > -1)
		{
			this.currentNode.shipsRelifeTag[(int)this.team]++;
		}
		if (eType == NodeType.Castle || eType == NodeType.Tower)
		{
			Solarmax.Singleton<EffectManager>.Get().AddBomberNoLimit(this.currentNode, this.entity.GetPosition(), this.currentTeam.color, true, false);
		}
		else if (eType == NodeType.Defense)
		{
			Solarmax.Singleton<EffectManager>.Get().AddBlackholeHitEffect(this.entity.GetPosition(), this.currentTeam.color, true, false);
		}
		else
		{
			Solarmax.Singleton<EffectManager>.Get().AddBomber(this.currentNode, this.entity.GetPosition(), this.currentTeam.color, true, false);
		}
		this.hp = this.currentTeam.hpMax;
		if (this.num == 0)
		{
			if (this.currentNode != null)
			{
				this.currentNode.RemoveShip(this.team, this, false);
			}
			this.pool.RemoveFlyShip(this);
			base.Recycle(this);
		}
	}

	public void AddOne()
	{
		this.currentTeam.SetAttribute(TeamAttr.Population, 1f, true);
		this.num++;
		if (this.temNum > 0)
		{
			this.temNum++;
		}
		this.hp = this.currentTeam.hpMax;
	}

	public void MoveTo(Node node, bool warp)
	{
		if (!warp)
		{
			this.pool.AddFlyShip(this);
		}
		this.entity.MoveTo(node, warp);
	}

	public void EnterNode(Node node, bool warp = false)
	{
		if (node != null)
		{
			node = node.nodeManager.GetNode(node.tag);
		}
		this.pool.RemoveFlyShip(this);
		if (node != null)
		{
			node.EnterNode(this, this.currentNode, warp);
		}
	}

	public void SetColor(Color color)
	{
		if (this.entity == null)
		{
			return;
		}
		this.entity.SetColor(color);
	}

	public void SetAlpha(float a)
	{
		if (this.entity == null)
		{
			return;
		}
		this.entity.SetAlpha(a);
	}

	public Vector3 GetPosition()
	{
		if (this.entity == null)
		{
			return Vector3.zero;
		}
		return this.entity.GetPosition();
	}

	public void SetPosition(Vector3 position)
	{
		if (this.entity == null)
		{
			return;
		}
		this.entity.SetPosition(position);
	}

	public void SetParent(Transform parent)
	{
		this.entity.SetParent(parent);
	}

	protected void HealthHp(int frame, float dt)
	{
		if (this.currentNode.state != NodeState.Idle)
		{
			return;
		}
		if (this.hp >= this.currentTeam.hpMax)
		{
			return;
		}
		this.hp += 2f;
		if (this.hp >= this.currentTeam.hpMax)
		{
			this.hp = this.currentTeam.hpMax;
		}
	}

	private int shipNum;

	public int temNum;

	public bool forceShow;

	/// <summary>摧毁线安全穿越剩余次数（-1 表示未初始化；&gt;=0 时每穿越一次减 1，小于 0 则被摧毁）。</summary>
	public int safeCrossRemain = -1;

	public float lastSideSign;

	public bool sideInited;
}
