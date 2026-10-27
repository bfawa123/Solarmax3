using System;
using System.Collections.Generic;
using Solarmax;

public class LasercannonSkill : BaseNewSkill
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
			if (node != this.owerNode)
			{
				float num = this.owerNode.GetWidth() * this.owerNode.GetAttackRage();
				num *= num;
				Node node2 = node;
				if ((this.owerNode.GetPosition() - node.GetPosition()).sqrMagnitude <= num)
				{
					try
					{
						TEAM team = TEAM.Team_1;
						while ((int)team < LocalPlayer.MaxTeamNum)
						{
							Team team2 = this.sceneManager.teamManager.GetTeam(team);
							if (this.owerNode.currentTeam.team != team && node.GetShipFactCount((int)team) != 0 && (this.hostTeam == null || !this.hostTeam.IsFriend(team2.groupID)))
							{
								BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
								if (baseNewBuff != null)
								{
									baseNewBuff.SetInfo(castTeam, team2, node2, data, this, this.sceneManager);
									baseNewBuff.Init();
									node2.skillBuffLogic.Add(baseNewBuff);
								}
							}
							team += 1;
						}
					}
					catch (Exception)
					{
					}
				}
			}
			i++;
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return base.OnCast(castTeam);
	}
}
