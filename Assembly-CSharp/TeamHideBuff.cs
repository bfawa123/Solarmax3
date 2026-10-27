using System;

public class TeamHideBuff : BaseNewBuff
{
	protected override void Apply()
	{
		if (this.targetTeam == null)
		{
			return;
		}
		this.targetTeam.SetAttribute(TeamAttr.HideFly, 1f, true);
		base.ShowToasts(new object[0]);
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		this.targetTeam.SetAttribute(TeamAttr.HideFly, 0f, true);
		base.Disable();
	}
}
