using System;

public class StaticAILogic : BaseAILogic
{
	public override void Init(Team t, int aiStrategy, int level, int Difficulty, float Tick = 1f)
	{
		AIData aiData = base.GetAiData(t, true);
		aiData.Reset();
		aiData.aiStrategy = aiStrategy;
	}

	public override void Release(Team t)
	{
		base.GetAiData(t, false).Reset();
	}

	private bool Idle(Team t, float dt)
	{
		AIData aiData = base.GetAiData(t, false);
		aiData.aiStatus = AIStatus.Defend;
		return true;
	}

	private bool Defend(Team t, float dt)
	{
		return false;
	}

	private bool Attack(Team t, float dt)
	{
		return false;
	}

	private bool Gather(Team t, float dt)
	{
		return false;
	}
}
