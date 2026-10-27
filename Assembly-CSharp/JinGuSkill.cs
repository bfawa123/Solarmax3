using System;
using System.Collections.Generic;
using Solarmax;

public class JinGuSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
		if (baseNewBuff == null)
		{
			return false;
		}
		int num = 0;
		Node node = null;
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		int i = 0;
		int count = usefulNodeList.Count;
		while (i < count)
		{
			Node node2 = usefulNodeList[i];
			int num2 = 0;
			for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team = this.sceneManager.teamManager.GetTeam((TEAM)j);
				if (!castTeam.IsFriend(team.groupID))
				{
					num2 += node2.GetShipFactCount(j);
				}
			}
			if (num2 > num)
			{
				num = num2;
				node = node2;
			}
			i++;
		}
		if (node == null)
		{
			return false;
		}
		baseNewBuff.SetInfo(castTeam, null, node, data, this, this.sceneManager);
		baseNewBuff.SetToasts();
		baseNewBuff.Init();
		node.skillBuffLogic.Add(baseNewBuff);
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}
}
