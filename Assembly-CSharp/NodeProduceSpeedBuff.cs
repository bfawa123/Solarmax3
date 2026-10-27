using System;

public class NodeProduceSpeedBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float num = float.Parse(this.config.arg0);
		this.targetNode.SetAttribute(NodeAttr.ProduceSpeed, num, false);
		base.ShowToasts(new object[0]);
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		float num = float.Parse(this.config.arg0);
		this.targetNode.SetAttribute(NodeAttr.ProduceSpeed, -num, false);
		base.Disable();
	}
}
