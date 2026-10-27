using System;
using System.Collections.Generic;
using Solarmax;

public class TianShangSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		int num = 0;
		Node node = null;
		List<Team> list = new List<Team>();
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		int i = 0;
		int count = usefulNodeList.Count;
		while (i < count)
		{
			Node node2 = usefulNodeList[i];
			int num2 = 0;
			list.Clear();
			for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team = this.sceneManager.teamManager.GetTeam((TEAM)j);
				if (!castTeam.IsFriend(team.groupID))
				{
					num2 += node2.GetShipFactCount(j);
				}
				list.Add(team);
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
		int k = 0;
		int count2 = list.Count;
		while (k < count2)
		{
			BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
			if (baseNewBuff == null)
			{
				return false;
			}
			baseNewBuff.SetInfo(castTeam, list[k], node, data, this, this.sceneManager);
			baseNewBuff.Init();
			node.skillBuffLogic.Add(baseNewBuff);
			k++;
		}
		base.PlaySkillAudio();
		float num3 = float.Parse(data.arg0);
		int value = (int)((float)num * num3);
		base.ShowToasts(new object[]
		{
			Math.Abs(value)
		});
		return base.OnCast(castTeam);
	}
}
