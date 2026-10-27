using System;
using System.Collections.Generic;
using Solarmax;

public class OverKillSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		if (data == null)
		{
			return false;
		}
		BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
		if (baseNewBuff == null)
		{
			return false;
		}
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
				if ((this.owerNode.GetPosition() - node.GetPosition()).sqrMagnitude <= num)
				{
					baseNewBuff.SetInfo(castTeam, node.currentTeam, node, data, this, this.sceneManager);
					baseNewBuff.Init();
					node.skillBuffLogic.Add(baseNewBuff);
				}
			}
			i++;
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return base.OnCast(castTeam);
	}
}
