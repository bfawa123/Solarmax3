using System;

public class TeamBeCapturedSpeedBuff : BaseNewBuff
{
	protected override void Apply()
	{
		if (this.targetTeam == null)
		{
			return;
		}
		float num = float.Parse(this.config.arg0);
		this.targetTeam.SetAttribute(TeamAttr.BeCapturedSpeed, num, false);
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
		this.targetTeam.SetAttribute(TeamAttr.BeCapturedSpeed, -num, false);
		base.Disable();
	}
}
