using System;
using Plugin;
using Solarmax;
using UnityEngine;

public class LasergunNode : Node
{
	public LasergunNode(string name) : base(name)
	{
	}

	public override NodeType type
	{
		get
		{
			return NodeType.Lasergun;
		}
	}

	public override bool Init()
	{
		bool result = base.Init();
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		this.ani = base.entity.GetGO().GetComponent<Animator>();
		return result;
	}

	public override void Tick(int frame, float interval)
	{
		AnimatorStateInfo currentAnimatorStateInfo = this.ani.GetCurrentAnimatorStateInfo(0);
		if (base.team != TEAM.Neutral && LasergunNode.hash != currentAnimatorStateInfo.shortNameHash)
		{
			this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
			this.ani.Play(LasergunNode.hash, 0, 0f);
		}
		base.Tick(frame, interval);
		base.UpdateOrbit(frame, interval);
		base.UpdateState(frame, interval);
		base.UpdateOccupied(frame, interval);
		base.UpdateBattle(frame, interval);
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript)
		{
			base.LasergunAttack(frame, interval);
		}
		base.UpdateNodeSkill(frame, interval);
		base.UpdateCapturing(frame, interval);
	}

	private void SetPause(bool pause)
	{
		if (pause)
		{
			this.ani.speed = 0f;
		}
		else
		{
			this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		}
	}

	private void SetEffectSpeed(float speed)
	{
		this.ani.speed = speed;
	}

	public override void Destroy()
	{
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		base.Destroy();
	}

	public Animator ani;

	private static readonly int hash = Animator.StringToHash("Entity_Speedship_emission");
}
