using System;

public class BarrierLineNode : Node
{
	public BarrierLineNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.BarrierLine;
		}
	}
}
