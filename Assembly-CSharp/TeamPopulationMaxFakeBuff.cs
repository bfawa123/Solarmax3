using System;

public class TeamPopulationMaxFakeBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.pop = this.targetNode.population;
		this.sendTeam.SetAttribute(TeamAttr.PopulationMax, (float)this.pop, true);
		base.ShowToasts(new object[]
		{
			Math.Abs(this.pop)
		});
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		this.sendTeam.SetAttribute(TeamAttr.PopulationMax, (float)(-(float)this.pop), true);
		base.Disable();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
		if (this.targetNode.currentTeam == this.sendTeam)
		{
			this.lastTime = -1f;
		}
	}

	private int pop;
}
