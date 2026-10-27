using System;
using System.Collections.Generic;
using Solarmax;

public class InhibitSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
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
					if (node.currentTeam != castTeam)
					{
						if (!castTeam.IsFriend(node.currentTeam.groupID))
						{
							float num = this.owerNode.GetWidth() * this.owerNode.GetAttackRage();
							num *= num;
							if ((this.owerNode.GetPosition() - node.GetPosition()).sqrMagnitude <= num)
							{
								int j = 0;
								int count2 = this.config.buffIds.Count;
								while (j < count2)
								{
									NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[j]);
									if (data != null)
									{
										BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
										if (baseNewBuff != null)
										{
											baseNewBuff.SetInfo(castTeam, node.currentTeam, node, data, this, this.sceneManager);
											baseNewBuff.Init();
											node.skillBuffLogic.Add(baseNewBuff);
										}
									}
									j++;
								}
							}
						}
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
