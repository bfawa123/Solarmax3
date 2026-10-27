using System;

public class CreatureNode : Node
{
	public CreatureNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return this.nodeType;
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
		base.UpdateCapturing(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override bool CanBeTarget()
	{
		return !base.currentTeam.hideFly;
	}
}
