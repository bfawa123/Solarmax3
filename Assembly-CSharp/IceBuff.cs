using System;

public class IceBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetNode.SetAttribute(NodeAttr.Ice, 1f, true);
		base.ShowToasts(new object[0]);
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		this.targetNode.SetAttribute(NodeAttr.Ice, 0f, true);
		base.Disable();
	}
}
