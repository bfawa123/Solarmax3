using System;
using Plugin;
using Solarmax;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class UnknownStarEffect : EffectNode
{
	public float scale { get; set; }

	public float recycleTime { get; set; }

	public Node node { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		if (this.recycleTime > 0f)
		{
			this.recycleTime -= interval;
		}
		else
		{
			base.Recycle(this);
		}
	}

	public override void Destroy()
	{
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Remove(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
	}

	public override void InitEffectNode(bool anim = true)
	{
		Vector3 position = Camera.main.WorldToScreenPoint(this.node.GetPosition());
		Vector3 localPosition = UICamera.currentCamera.ScreenToWorldPoint(position);
		localPosition.z = 0f;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("EFF_XJ_BianHuan");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		}
		base.go.transform.localPosition = localPosition;
		base.go.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
		for (int i = 0; i < base.go.transform.childCount; i++)
		{
			Transform child = base.go.transform.GetChild(i);
			child.transform.localScale = new Vector3(this.scale, this.scale, this.scale);
		}
		LockStep lockStep = Solarmax.Singleton<BattleSystem>.Get().lockStep;
		lockStep.onPlaySpeedChange = (LockStep.OnPlaySpeedChange)Delegate.Combine(lockStep.onPlaySpeedChange, new LockStep.OnPlaySpeedChange(this.SetEffectSpeed));
		this.SetEffectSpeed(Solarmax.Singleton<BattleSystem>.Get().lockStep.playSpeed);
	}

	private void SetEffectSpeed(float speed)
	{
		if (base.go != null)
		{
			MainModule mm = base.go.GetComponent<ParticleSystem>().main;
			mm.simulationSpeed = speed;
			ParticleSystem[] componentsInChildren = base.go.GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				MainModule mm2 = particleSystem.main;
				mm2.simulationSpeed = speed;
			}
		}
	}

	private const string UNKNOWN_STAR_EFFECT = "EFF_XJ_BianHuan";
}
