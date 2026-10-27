using System;
using Solarmax;

public class DefenseNode : Node
{
	public DefenseNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Defense;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.UpdateNodeSkill(frame, interval);
		}
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		base.UpdateCapturing(frame, interval);
		base.UpdateOccupied(frame, interval);
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.UpdateBattle(frame, interval);
		}
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.DefenseToShip(frame, interval);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
