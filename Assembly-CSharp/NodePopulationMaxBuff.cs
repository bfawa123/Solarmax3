using System;

public class NodePopulationMaxBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float num = float.Parse(this.config.arg0);
		float num2 = (float)this.targetNode.population * num;
		this.targetTeam.SetAttribute(TeamAttr.PopulationMax, num2, true);
		base.ShowToasts(new object[]
		{
			Math.Abs(num2)
		});
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		float num = float.Parse(this.config.arg0);
		float num2 = (float)this.targetNode.population * num;
		this.targetTeam.SetAttribute(TeamAttr.PopulationMax, -num2, true);
		base.ShowToasts(new object[]
		{
			Math.Abs(-num2)
		});
		base.Disable();
	}
}
