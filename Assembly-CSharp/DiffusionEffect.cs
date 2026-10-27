using System;
using System.Collections.Generic;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class DiffusionEffect : EffectNode
{
	public float speed { get; set; }

	public float angle { get; set; }

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
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_Lightning1");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.effectList = base.go.GetComponentsInChildren<ParticleSystem>();
			this.aniList = base.go.GetComponentsInChildren<Animator>();
			this.planeList.Add(base.go.transform.Find("Plane1"));
			this.planeList.Add(base.go.transform.Find("Plane2"));
			this.planeList.Add(base.go.transform.Find("Plane3"));
			this.materials.Add(this.planeList[0].gameObject.GetComponent<MeshRenderer>().materials[0]);
			this.materials.Add(this.planeList[1].gameObject.GetComponent<MeshRenderer>().materials[0]);
			this.materials.Add(this.planeList[2].gameObject.GetComponent<MeshRenderer>().materials[0]);
			this.originScale = this.planeList[2].localScale;
		}
		base.go.SetActive(true);
		base.go.transform.SetParent(this.node.GetGO().transform);
		this.planeList[0].gameObject.SetActive(false);
		this.planeList[1].gameObject.SetActive(false);
		this.planeList[2].gameObject.SetActive(false);
		int num = UnityEngine.Random.Range(0, 3);
		this.planeList[num].gameObject.SetActive(true);
		this.planeList[num].localScale = new Vector3(this.originScale.x / this.scale, 1f, this.originScale.z / this.scale);
		base.go.transform.localEulerAngles = new Vector3(0f, 0f, this.angle);
		base.go.transform.localPosition = new Vector3(this.radius * Mathf.Cos(this.radian), this.radius * Mathf.Sin(this.radian), 0f);
		this.materials[num].SetColor("_TintColor", this.node.entity.GetColor());
		if (num != 0)
		{
			if (num != 1)
			{
				if (num == 2)
				{
					this.aniList[0].Play("EFF_XJ_Lightning3");
				}
			}
			else
			{
				this.aniList[0].Play("EFF_XJ_Lightning2");
			}
		}
		else
		{
			this.aniList[0].Play("EFF_XJ_Lightning");
		}
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		BattleSystem battleSystem = Solarmax.Singleton<BattleSystem>.Get();
		battleSystem.onPauseDelegate = (BattleSystem.OnPauseDelegate)Delegate.Combine(battleSystem.onPauseDelegate, new BattleSystem.OnPauseDelegate(this.SetPause));
		this.delta = 0f;
		this.lastDelta = 0f;
		this.isBreak = false;
		this.pause = false;
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
				MainModule mm = particleSystem.main;
				mm.simulationSpeed = speed;
			}
		}
		if (base.go != null && this.aniList != null)
		{
			foreach (Animator animator in this.aniList)
			{
				animator.speed = speed;
			}
		}
	}

	private void SetEffectPause(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			particleSystem.Pause(true);
		}
	}

	private void SetEffectPlay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
		{
			particleSystem.Play(true);
		}
	}

	private void SetEffectDelay(ParticleSystem[] list)
	{
		foreach (ParticleSystem particleSystem in list)
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
		this.lastDelta = this.delta;
	}

	private const string EFFECT_NAME = "EFF_XJ_Lightning1";

	private const string EFFECT_ANIMATOR_NAME1 = "EFF_XJ_Lightning";

	private const string EFFECT_ANIMATOR_NAME2 = "EFF_XJ_Lightning2";

	private const string EFFECT_ANIMATOR_NAME3 = "EFF_XJ_Lightning3";

	public float lastDelta;

	public float scale;

	private float delta;

	public bool pause = true;

	public float radius;

	public float radian;

	private bool isBreak;

	private List<Material> materials = new List<Material>();

	private List<Transform> planeList = new List<Transform>();

	private Vector3 originScale;
}
