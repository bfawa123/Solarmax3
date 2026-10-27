using System;

public class NodeShipCallBuff : BaseNewBuff
{
	protected override void Apply()
	{
		int population = this.targetNode.population;
		float num = float.Parse(this.config.arg0);
		int num2 = Convert.ToInt32((float)population * num);
		this.sceneManager.AddShip(this.targetNode, num2, (int)this.targetTeam.team, false, false);
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
		this.sceneManager.DestroyTeamTemporary(this.targetTeam.team);
		base.Disable();
	}
}
