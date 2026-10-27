using System;

public class PlanetShipNumAllEnemyBuff : BaseNewBuff
{
	protected override void Apply()
	{
		float percentAdd = 0f;
		if (!string.IsNullOrEmpty(this.config.arg0))
		{
			percentAdd = float.Parse(this.config.arg0);
		}
		int numAdd = 0;
		if (!string.IsNullOrEmpty(this.config.arg1))
		{
			numAdd = int.Parse(this.config.arg1);
		}
		int value = 0;
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.sceneManager.teamManager.GetTeam((TEAM)i);
			if (!team.IsFriend(this.sendTeam.groupID))
			{
				this.AddShip(team, this.targetNode, percentAdd, numAdd, ref value);
			}
		}
		base.ShowToasts(new object[]
		{
			Math.Abs(value)
		});
		base.Apply();
	}

	private void AddShip(Team targetTeam, Node tragetNode, float percentAdd, int numAdd, ref int totalNum)
	{
		int shipFactCount = this.targetNode.GetShipFactCount((int)targetTeam.team);
		int num = Convert.ToInt32((float)shipFactCount * percentAdd);
		num += numAdd;
		if (num > 0)
		{
			if (targetTeam.current + num > targetTeam.currentMax)
			{
				num = targetTeam.currentMax - targetTeam.current;
			}
			this.sceneManager.AddShip(this.targetNode, num, (int)targetTeam.team, true, true);
		}
		else if (num < 0)
		{
			if (Math.Abs(num) > shipFactCount)
			{
				num = -shipFactCount;
			}
			this.targetNode.BombShipNum(targetTeam.team, this.sendTeam.team, -num);
		}
		totalNum += Math.Abs(num);
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
