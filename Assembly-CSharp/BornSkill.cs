using System;
using System.Collections.Generic;
using Solarmax;

public class BornSkill : BaseNewSkill
{
	public override void OnBorn(Team t)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		if (data.targetType == 0)
		{
			List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
			int i = 0;
			int count = usefulNodeList.Count;
			while (i < count)
			{
				Node node = usefulNodeList[i];
				if (node.currentTeam == t)
				{
					BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
					if (baseNewBuff == null)
					{
						return;
					}
					baseNewBuff.SetInfo(t, t, node, data, this, this.sceneManager);
					baseNewBuff.Init();
					node.skillBuffLogic.Add(baseNewBuff);
				}
				i++;
			}
		}
		else if (data.targetType == 2)
		{
			BaseNewBuff baseNewBuff2 = this.skillManager.NewBuff(data.logicId);
			if (baseNewBuff2 == null)
			{
				return;
			}
			baseNewBuff2.SetInfo(t, t, null, data, this, this.sceneManager);
			baseNewBuff2.Init();
			t.skillBuffLogic.Add(baseNewBuff2);
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
	}
}
