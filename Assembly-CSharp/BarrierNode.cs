using System;

public class BarrierNode : Node
{
	public BarrierNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Barrier;
		}
	}
}
