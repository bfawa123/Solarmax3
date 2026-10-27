using System;

public class TeamStopBeCaptureBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetTeam.SetAttribute(TeamAttr.StopBeCapture, 1f, true);
		base.ShowToasts(new object[0]);
		base.Apply();
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
}
