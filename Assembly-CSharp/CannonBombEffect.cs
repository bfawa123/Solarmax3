using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CannonBombEffect : EffectNode
{
	public float speed { get; set; }

	public float recycleTime { get; set; }

	public Node node { get; set; }

	private ParticleSystem[] effectList { get; set; }

	private Animator[] animatorList { get; set; }

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
					base.Recycle(this);
				}
			}
			else if (this.shrinkTime < 1f)
			{
				Vector3 vector = base.go.transform.localScale;
				vector -= Vector3.one * interval * this.speed * 2f;
				if (vector.x >= 0f && vector.y >= 0f && vector.z >= 0f)
				{
					base.go.transform.localScale = vector;
				}
				this.shrinkTime += interval * this.speed;
			}
			else
			{
				this.isBreak = false;
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

	public void RecycleSelf()
	{
		this.isBreak = true;
	}

	public override void OnRecycle()
	{
		this.SetEffectStop(this.effectList);
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
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_BaoZha2");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
			this.animatorList = base.go.GetComponentsInChildren<Animator>();
		}
		base.go.transform.SetParent(this.node.GetGO().transform);
		base.go.transform.localPosition = Vector3.zero;
		float num = ((this.node.GetHalfNodeSize() <= 0.8f) ? this.node.GetHalfNodeSize() : 0.8f) / 0.4f;
		base.go.transform.localScale = new Vector3(num * 4f, num * 4f, num * 4f);
		base.go.SetActive(true);
		foreach (ParticleSystem particleSystem in this.effectList)
		{
			if (particleSystem != null)
			{
				particleSystem.transform.localScale = new Vector3(num * 4f, num * 4f, num * 4f);
			}
		}
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		this.delta = 0f;
		this.lastDelta = 0f;
		this.isBreak = false;
		this.CD = this.recycleTime;
		this.PlayEffect(this.effectList);
	}

	private void SetPause(bool pause)
	{
		if (base.go != null && this.effectList != null)
		{
			if (pause)
			{
				this.pause = pause;
				this.SetEffectPause(this.effectList);
			}
			else
			{
				this.pause = pause;
				this.SetEffectDelay(this.effectList);
				this.SetEffectPlay(this.effectList);
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
                MainModule mm = particleSystem.main;
				mm.simulationSpeed = speed;
			}
		}
		foreach (Animator animator in this.animatorList)
		{
			animator.speed = Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed;
		}
	}

	private void SetEffectPause(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Pause(false);
			}
		}
		foreach (Animator animator in this.animatorList)
		{
			if (animator != null)
			{
				animator.speed = 0f;
			}
		}
	}

	private void PlayEffect(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Simulate(0f, false, true);
				particleSystem.Play(false);
			}
		}
		foreach (Animator animator in this.animatorList)
		{
			if (animator != null)
			{
				animator.speed = Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed;
			}
		}
	}

	private void SetEffectPlay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Play(false);
			}
		}
		foreach (Animator animator in this.animatorList)
		{
			if (animator != null)
			{
				animator.speed = Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed;
			}
		}
	}

	private void SetEffectStop(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (particleSystem != null)
			{
				particleSystem.Stop(false);
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
		foreach (Animator animator in this.animatorList)
		{
			if (animator != null)
			{
				animator.speed = Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed;
			}
		}
	}

	private const string EFF_NAME = "EFF_XJ_BaoZha2";

	public const float effectTime = 10f;

	public float lastDelta;

	private bool pause;

	private float delta;

	private bool isBreak;

	private float shrinkTime;

	private float CD;
}
