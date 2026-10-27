using System;
using System.Collections.Generic;
using Solarmax;

public class AttackPlanetShip : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		int i = 0;
		int count = usefulNodeList.Count;
		while (i < count)
		{
			Node node = usefulNodeList[i];
			if ((node.GetPosition() - this.owerNode.GetPosition()).magnitude < this.fAttackRange)
			{
				this.targetTeamList.Clear();
				for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
				{
					Team team = this.sceneManager.teamManager.GetTeam((TEAM)j);
					if (!castTeam.IsFriend(team.groupID))
					{
						this.targetTeamList.Add(team);
					}
				}
			}
			if (node != null)
			{
				int k = 0;
				int count2 = this.targetTeamList.Count;
				while (k < count2)
				{
					BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
					if (baseNewBuff == null)
					{
						return false;
					}
					baseNewBuff.SetInfo(castTeam, this.targetTeamList[k], node, data, this, this.sceneManager);
					baseNewBuff.Init();
					node.skillBuffLogic.Add(baseNewBuff);
					i++;
				}
			}
			i++;
		}
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}

	private List<Team> targetTeamList = new List<Team>();

	private float fAttackRange;
}
