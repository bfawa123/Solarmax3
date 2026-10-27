using System;

public class TeamQuickMoveBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetTeam.SetAttribute(TeamAttr.QuickMove, 1f, true);
		base.ShowToasts(new object[0]);
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		this.targetTeam.SetAttribute(TeamAttr.QuickMove, 0f, true);
		base.Disable();
	}
}
