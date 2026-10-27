using System;

public class NodePopulationBuff : BaseNewBuff
{
	protected override void Apply()
	{
		int num = int.Parse(this.config.arg1);
		int population = this.targetNode.population;
		this.targetNode.SetBasePopulation(population + num);
		base.ShowToasts(new object[]
		{
			Math.Abs(num)
		});
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		int num = int.Parse(this.config.arg1);
		int population = this.targetNode.population;
		this.targetNode.SetBasePopulation(population - num);
		base.Disable();
	}
}
