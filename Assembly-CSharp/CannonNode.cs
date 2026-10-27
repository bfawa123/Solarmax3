using System;

public class CannonNode : Node
{
	public CannonNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Cannon;
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
		base.UpdateCapturing(frame, interval);
		base.UpdateCannon(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
