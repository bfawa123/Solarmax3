using System;

public class TeamPopulationMaxBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float num = float.Parse(this.config.arg0);
		this.targetTeam.SetAttribute(TeamAttr.PopulationMax, num, false);
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
		this.targetTeam.SetAttribute(TeamAttr.PopulationMax, -num, false);
		base.Disable();
	}
}
