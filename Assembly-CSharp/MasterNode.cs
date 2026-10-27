using System;

public class MasterNode : Node
{
	public MasterNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Master;
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
		base.UpdateCapturing(frame, interval);
		base.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		base.UpdatePurify(frame, interval);
		base.UpdateLost(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
