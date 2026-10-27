using System;

public class FixedWarpDoorNode : Node
{
	public FixedWarpDoorNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.FixedWarpDoor;
		}
	}
}
