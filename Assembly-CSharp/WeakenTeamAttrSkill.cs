using System;
using Solarmax;

public class WeakenTeamAttrSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		if (Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]) == null)
		{
			return false;
		}
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.sceneManager.teamManager.GetTeam((TEAM)i);
			if (!castTeam.IsFriend(team.groupID))
			{
				NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
				BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
				if (baseNewBuff == null)
				{
					return false;
				}
				baseNewBuff.SetInfo(castTeam, team, null, data, this, this.sceneManager);
				baseNewBuff.Init();
				team.skillBuffLogic.Add(baseNewBuff);
			}
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return base.OnCast(castTeam);
	}
}
