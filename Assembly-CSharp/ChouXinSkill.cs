using System;
using Solarmax;

public class ChouXinSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			Team team = this.sceneManager.teamManager.GetTeam((TEAM)i);
			if (!castTeam.IsFriend(team.groupID))
			{
				BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
				if (baseNewBuff == null)
				{
					return false;
				}
				baseNewBuff.SetInfo(castTeam, team, null, data, this, this.sceneManager);
				baseNewBuff.SetToasts();
				baseNewBuff.Init();
				base.AddBuff(null, team, baseNewBuff, data);
			}
		}
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}
}
