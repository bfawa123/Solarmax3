using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class TwistEffect : EffectNode
{
	public float speed { get; set; }

	public float recycleTime { get; set; }

	public bool isMax { get; set; }

	public Vector3 beginPosition { get; set; }

	public Vector3 endPosition { get; set; }

	public Color selfColor { get; set; }

	public Color enemyColor { get; set; }

	public Node node { get; set; }

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

	public override void OnRecycle()
	{
		base.OnRecycle();
		this.beginPosition = Vector3.zero;
		this.endPosition = Vector3.zero;
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Remove(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("tongdao");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
		}
		base.go.transform.SetParent(this.node.GetGO().transform);
		base.go.transform.localPosition = Vector3.zero;
		base.go.SetActive(true);
		Transform transform = base.go.transform.Find("effect_1");
		Transform transform2 = transform.transform.Find("Sprites");
		if (this.isMax)
		{
			transform2.gameObject.SetActive(false);
		}
		else if (transform != null && !this.isMax)
		{
			transform2.gameObject.SetActive(true);
			ParticleSystem component = transform2.GetComponent<ParticleSystem>();
			MainModule mm = component.main;
			mm.startColor = this.enemyColor;
		}
		if (transform != null)
		{
			Transform transform3 = transform.transform.Find("spark2");
			ParticleSystem component2 = transform3.GetComponent<ParticleSystem>();
			MainModule mm = component2.main;
			mm.startColor = this.selfColor;
		}
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		float x = this.endPosition.x - this.beginPosition.x;
		float y = this.endPosition.y - this.beginPosition.y;
		float z = Mathf.Atan2(y, x) * 57.29578f;
		Vector3 localEulerAngles = new Vector3(0f, 0f, z);
		base.go.transform.localEulerAngles = localEulerAngles;
		float num = Vector3.Distance(this.endPosition, this.beginPosition);
		base.go.transform.localScale = new Vector3(1f * num / 3f, 1f, 1f);
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
				particleSystem.Pause(false);
			}
		}
	}

	private void SetEffectPlay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			if (!(particleSystem == null))
			{
				particleSystem.Play(false);
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

	private const int TwistEffectScale = 1;

	private const string TwistEffectName = "tongdao";

	public const float effectTime = 1.3f;

	public float lastDelta;

	private bool pause;

	private float delta;
}
