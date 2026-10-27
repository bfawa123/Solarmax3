using System;

public class PlanetShipNumBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float num = 0f;
		if (!string.IsNullOrEmpty(this.config.arg0))
		{
			num = float.Parse(this.config.arg0);
		}
		int num2 = 0;
		if (!string.IsNullOrEmpty(this.config.arg1))
		{
			num2 = int.Parse(this.config.arg1);
		}
		int shipFactCount = this.targetNode.GetShipFactCount((int)this.targetTeam.team);
		int num3 = Convert.ToInt32((float)shipFactCount * num);
		num3 += num2;
		float num4 = 0f;
		if (!string.IsNullOrEmpty(this.config.arg2))
		{
			num4 = float.Parse(this.config.arg2);
		}
		int population = this.targetNode.population;
		num3 += Convert.ToInt32((float)population * num4);
		if (num3 > 0)
		{
			if (this.targetTeam.current + num3 > this.targetTeam.currentMax)
			{
				num3 = this.targetTeam.currentMax - this.targetTeam.current;
			}
			this.sceneManager.AddShip(this.targetNode, num3, (int)this.targetTeam.team, true, true);
		}
		else if (num3 < 0)
		{
			if (Math.Abs(num3) > shipFactCount)
			{
				num3 = -shipFactCount;
			}
			this.targetNode.BombShipNum(this.targetTeam.team, this.sendTeam.team, -num3);
		}
		base.ShowToasts(new object[]
		{
			Math.Abs(num3)
		});
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		base.Disable();
	}
}
