using System;

public class ChongWangBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetTeam.SetAttribute(TeamAttr.StopBeCapture, 1f, true);
		base.ShowToasts(new object[0]);
		base.Apply();
		this.allNode = this.sceneManager.nodeManager.GetUsefulNodeList();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		this.targetTeam.SetAttribute(TeamAttr.StopBeCapture, 0f, true);
		base.Disable();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
	}
}
