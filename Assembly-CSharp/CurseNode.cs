using System;
using Solarmax;

public class CurseNode : Node
{
	public CurseNode(string name) : base(name)
	{
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == base.tag);
		this.curseDelay = mapBuildingConfig.curseDelay;
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Curse;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		base.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		base.UpdateNodeSkill(frame, interval);
		base.UpdateCursing(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public float skillK;

	public float skillB;

	public float skillRange = 0.5f;

	public float curseDelay;

	public bool isFirst = true;

	public bool useSkill;

	public float skillDelay = 21f;

	public float skillEffective = 9f;

	public float cd;
}
