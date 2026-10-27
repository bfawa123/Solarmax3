using System;
using Solarmax;

public class NodeShipReliveBuff : BaseNewBuff
{
	protected override void Apply()
	{
		this.targetNode.shipsRelifeTag[(int)this.sendTeam.team] = 0;
		base.ShowToasts(new object[0]);
		base.Apply();
	}

	protected override void Enable()
	{
		base.Enable();
	}

	protected override void Disable()
	{
		int num = this.targetNode.shipsRelifeTag[(int)this.sendTeam.team];
		float num2 = float.Parse(this.config.arg0);
		num = Convert.ToInt32((float)num * num2);
		this.sceneManager.AddShip(this.targetNode, num, (int)this.sendTeam.team, true, true);
		Solarmax.Singleton<LoggerSystem>.Instance.Info("涅槃复活飞船数量:" + num, new object[0]);
		this.targetNode.shipsRelifeTag[(int)this.sendTeam.team] = -1;
		base.Disable();
	}
}
