using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BlackholeHitEffect : EffectNode
{
	public float speed { get; set; }

	public float recycleTime { get; set; }

	public Color selfColor { get; set; }

	public Vector3 bombPosition { get; set; }

	private ParticleSystem[] effectList { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		if (!this.pause)
		{
			this.delta += interval * this.speed;
			if (this.recycleTime > 0f)
			{
				this.recycleTime -= interval * this.speed;
			}
			else
			{
				this.Destroy();
			}
		}
	}

	public override void Destroy()
	{
		base.Recycle(this);
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_XiShou");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
		}
		base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		base.go.transform.localPosition = this.bombPosition;
		base.go.SetActive(anim);
		Transform transform = base.go.transform.Find("Particle System");
		if (transform != null)
		{
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			ParticleSystem.MinMaxGradient startColor = main.startColor;
			startColor.colorMax = this.selfColor;
			main.startColor = startColor;
		}
		this.delta = 0f;
		this.lastDelta = 0f;
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		this.PlayEffect(this.effectList);
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
				if (!(particleSystem == null))
				{
                    MainModule mm = particleSystem.main;
					mm.simulationSpeed = speed;
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
	}

	private void SetEffectDelay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				ParticleSystem.MainModule main = particleSystem.main;
				float num = main.startDelay.constant;
				float num2 = (this.delta - this.lastDelta <= 0f) ? 0f : (this.delta - this.lastDelta);
				if (num - num2 > 0f)
				{
					num -= num2;
					main.startDelay = num;
				}
			}
		}
		this.lastDelta = this.delta;
	}

	private const int EffectScale = 1;

	private const string EffectName = "EFF_XJ_XiShou";

	public const float effectTime = 1.5f;

	public float lastDelta;

	private bool pause;

	private float delta;
}
