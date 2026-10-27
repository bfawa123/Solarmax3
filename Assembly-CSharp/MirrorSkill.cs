using System;
using Solarmax;

public class MirrorSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		if (this.owerNode.currentTeam == null)
		{
			return false;
		}
		if (this.owerNode.GetShipFactCount((int)this.owerNode.currentTeam.team) == 0)
		{
			return false;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == this.owerNode.tag);
		if (mapBuildingConfig == null)
		{
			return false;
		}
		this.owerNode.RotateNodeImage(mapBuildingConfig.fAngle);
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return true;
	}
}
