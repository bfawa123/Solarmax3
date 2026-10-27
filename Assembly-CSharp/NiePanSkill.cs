using System;
using System.Collections.Generic;
using Solarmax;

public class NiePanSkill : BaseNewSkill
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
			BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
			if (baseNewBuff == null)
			{
				return false;
			}
			baseNewBuff.SetInfo(castTeam, null, node, data, this, this.sceneManager);
			baseNewBuff.SetToasts();
			baseNewBuff.Init();
			base.AddBuff(node, null, baseNewBuff, data);
			i++;
		}
		base.PlaySkillAudio();
		return base.OnCast(castTeam);
	}
}
