using System;
using System.Collections.Generic;
using Solarmax;

public class CloneToSkill : BaseNewSkill
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
		int num = 0;
		string text = string.Empty;
		List<Node> usefulNodeList = this.sceneManager.nodeManager.GetUsefulNodeList();
		int i = 0;
		int count = usefulNodeList.Count;
		while (i < count)
		{
			Node node = usefulNodeList[i];
			if (node != this.owerNode)
			{
				if (node.currentTeam.Valid())
				{
					if (node.GetShipFactCount((int)node.currentTeam.team) > num)
					{
						num = node.GetShipFactCount((int)node.currentTeam.team);
						text = node.tag;
					}
				}
			}
			i++;
		}
		if (text != string.Empty)
		{
			Node node2 = this.sceneManager.nodeManager.GetNode(text);
			if (node2 != null)
			{
				data.arg0 = text;
				baseNewBuff.SetInfo(node2.currentTeam, this.owerNode.currentTeam, this.owerNode, data, this, this.sceneManager);
				baseNewBuff.Init();
				this.owerNode.skillBuffLogic.Add(baseNewBuff);
			}
		}
		base.PlaySkillAudio();
		base.ShowToasts(new object[0]);
		return base.OnCast(castTeam);
	}
}
