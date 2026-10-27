using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CurseEffect : EffectNode
{
	public float speed { get; set; }

	public float recycleTime { get; set; }

	public Node node { get; set; }

	private ParticleSystem[] effectList { get; set; }

	private Animator[] aniList { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		if (!this.pause)
		{
			if (!this.isBreak)
			{
				this.delta += interval * this.speed;
				if (this.recycleTime > 0f)
				{
					this.recycleTime -= interval * this.speed;
				}
				else
				{
					this.delta = 0f;
					this.isBreak = true;
					this.pause = true;
					base.Recycle(this);
				}
			}
			else if (this.recycleTime > 0f)
			{
				this.delta = 0f;
				this.pause = true;
				base.Recycle(this);
			}
		}
	}

	public override void Destroy()
	{
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		base.Destroy();
	}

	public void ForceRecycle()
	{
		this.isBreak = true;
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_SaoGuang_B");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			base.go.transform.SetParent(GameObject.Find("Battle").transform);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
			this.aniList = base.go.GetComponentsInChildren<Animator>();
		}
		base.go.SetActive(true);
		float x = Solarmax.Singleton<BattleSystem>.Get().battleData.rand.Range(-2f, 2f);
		float y = Solarmax.Singleton<BattleSystem>.Get().battleData.rand.Range(-1.5f, 1.5f);
		int num = Solarmax.Singleton<BattleSystem>.Get().battleData.rand.Range(0, 360);
		base.go.transform.position = new Vector3(x, y, 0f);
		base.go.transform.localEulerAngles = new Vector3(0f, 0f, (float)num);
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		CurseSkill componentInChildren = base.go.GetComponentInChildren<CurseSkill>();
		componentInChildren.EnsureInit((CurseNode)this.node, this);
		this.delta = 0f;
		this.lastDelta = 0f;
		this.isBreak = false;
		this.pause = false;
		this.PlayEffect(this.effectList);
	}

	public void Effect()
	{
	}

	private void PlayEffect(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Simulate(0f, false, true);
				particleSystem.Play(false);
				this.aniList[0].Play("EFF_XJ_SaoGuang_B30", 0, 0f);
			}
		}
	}

	public void PlayFireEffect()
	{
		this.aniList[0].Play("15", 0, 0f);
	}

	private void SetPause(bool pause)
	{
		if (base.go != null && this.effectList != null)
		{
			this.pause = pause;
			if (pause)
			{
				this.SetEffectPause(this.effectList);
				this.SetEffectSpeed(0f);
			}
			else
			{
				this.SetEffectDelay(this.effectList);
				this.SetEffectPlay(this.effectList);
				this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
			}
		}
	}

	private void SetEffectSpeed(float speed)
	{
		this.speed = speed;
		if (base.go != null && this.effectList != null)
		{
			foreach (ParticleSystem particleSystem in this.effectList)
			{
				if (!(particleSystem == null))
				{
					MainModule mm = particleSystem.main;
					mm.simulationSpeed = speed;
				}
			}
		}
		if (base.go != null && this.aniList != null)
		{
			foreach (Animator animator in this.aniList)
			{
				if (!(animator == null))
				{
					animator.speed = speed;
				}
			}
		}
	}

	private void SetEffectPause(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				particleSystem.Pause(true);
			}
		}
	}

	private void SetEffectPlay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				particleSystem.Play(true);
			}
		}
		if (base.go != null && this.aniList != null)
		{
			foreach (Animator animator in this.aniList)
			{
				if (!(animator == null))
				{
					animator.speed = 0f;
				}
			}
		}
	}

	private void SetEffectDelay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				if (!particleSystem.isPlaying)
				{
					ParticleSystem.MainModule main = particleSystem.main;
					float num = main.startDelay.constant;
					if (num - this.delta > 0f)
					{
						num -= this.delta;
						main.startDelay = num;
					}
				}
			}
		}
		this.lastDelta = this.delta;
	}

	private const string BLACK_HOLE_EFFECT = "EFF_XJ_SaoGuang_B";

	private const string Fire_ANIMATION = "15";

	private const string READY_ANIMATION_15 = "EFF_XJ_SaoGuang_B15";

	private const string READY_ANIMATION_30 = "EFF_XJ_SaoGuang_B30";

	public float lastDelta;

	private float delta;

	public bool pause = true;

	private bool isBreak;
}
