using System;
using Solarmax;

public class AddTowerNode : Node
{
	public AddTowerNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.AntiCaptureship;
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
		base.EyeView(frame, interval);
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.AttackToAddShip(frame, interval);
		}
		base.UpdateCapturing(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
