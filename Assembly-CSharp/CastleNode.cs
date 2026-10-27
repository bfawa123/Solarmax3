using System;
using Solarmax;

public class CastleNode : Node
{
	public CastleNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Castle;
		}
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		base.UpdateNodeSkill(frame, interval);
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		base.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		base.UpdateProduce(frame, interval, true);
		base.EyeView(frame, interval);
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.AttackToShip(frame, interval);
		}
		base.UpdateCapturing(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
