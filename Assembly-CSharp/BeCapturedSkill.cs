using System;
using Solarmax;

public class BeCapturedSkill : BaseNewSkill
{
	public override void OnBeCaptured(Node node, Team before, Team now)
	{
		NewSkillBuffConfig data = Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance.GetData(this.config.buffIds[0]);
		BaseNewBuff baseNewBuff = this.skillManager.NewBuff(data.logicId);
		if (baseNewBuff == null)
		{
			return;
		}
		baseNewBuff.SetInfo(before, now, node, data, this, this.sceneManager);
		baseNewBuff.SetToasts();
		baseNewBuff.Init();
		base.AddBuff(node, now, baseNewBuff, data);
		base.PlaySkillAudio();
	}
}
