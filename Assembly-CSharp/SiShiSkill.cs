using System;
using System.Collections.Generic;
using Solarmax;

public class SiShiSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		int num = this.sceneManager.battleData.rand.Range(0, 100);
		bool flag = false;
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		int i = 0;
		int count = usefulNodeList.Count;
		while (i < count)
		{
			Node node = usefulNodeList[i];
			if (node.team == castTeam.team)
			{
				num--;
				flag = true;
				if (num < 0)
				{
					BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
					if (baseNewBuff == null)
					{
						return false;
					}
					baseNewBuff.SetInfo(castTeam, castTeam, node, data, this, this.sceneManager);
					baseNewBuff.SetToasts();
					baseNewBuff.Init();
					base.AddBuff(node, castTeam, baseNewBuff, data);
					break;
				}
			}
			i++;
			if (i == count && flag)
			{
				i = 0;
			}
		}
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}
}
