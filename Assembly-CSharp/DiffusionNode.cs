using System;

public class DiffusionNode : Node
{
	public DiffusionNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Diffusion;
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
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
