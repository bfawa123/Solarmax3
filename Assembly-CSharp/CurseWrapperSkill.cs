using System;
using Solarmax;

public class CurseWrapperSkill : BaseNewSkill
{
	public override bool OnCast(Team castTeam)
	{
		Solarmax.Singleton<EffectManager>.Get().PlayCurseEffect(this.owerNode);
		return base.OnCast(castTeam);
	}
}
