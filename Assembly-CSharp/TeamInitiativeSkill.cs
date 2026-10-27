using System;
using Solarmax;

public class TeamInitiativeSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
		if (baseNewBuff == null)
		{
			return false;
		}
		baseNewBuff.SetInfo(castTeam, castTeam, null, data, this, this.sceneManager);
		baseNewBuff.SetToasts();
		baseNewBuff.Init();
		castTeam.skillBuffLogic.Add(baseNewBuff);
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}
}
