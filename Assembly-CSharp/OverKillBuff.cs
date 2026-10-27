using System;

public class OverKillBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetNode.ReturnToNeutral();
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		base.Disable();
	}
}
