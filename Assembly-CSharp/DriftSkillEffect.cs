using System;
using UnityEngine;

public class DriftSkillEffect : EffectNode
{
	public Node hoodEntity { get; set; }

	public string effectName { get; set; }

	public float lifeTime { get; set; }

	public float scale { get; set; }

	public override bool Init()
	{
		this.scale = 1f;
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateEffect(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		this.recycleType = DriftSkillEffect.SkillEffectRecycleType.Unknow;
		this.lifeTime = 0f;
		base.OnRecycle();
		this.Destroy();
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.hoodEntity != null && this.hoodEntity.nodeManager != null)
		{
			Node node = this.hoodEntity.nodeManager.GetNode(this.hoodEntity.tag);
			if (node != null && node != this.hoodEntity)
			{
				this.hoodEntity = node;
			}
		}
		if (base.go == null)
		{
			if (string.IsNullOrEmpty(this.effectName))
			{
				Debug.Log("InitEffectNode is null");
				return;
			}
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(this.effectName);
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		}
		if (base.go == null || this.hoodEntity == null)
		{
			return;
		}
		base.go.SetActive(true);
		base.go.transform.position = this.hoodEntity.GetPosition();
		base.go.transform.localScale *= this.scale;
		ParticleSystem componentInChildren = base.go.GetComponentInChildren<ParticleSystem>();
		if (componentInChildren)
		{
			componentInChildren.Play();
		}
		base.isActive = true;
	}

	private void UpdateEffect(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (this.lifeTime > 0f)
		{
			this.lifeTime -= dt;
			if (this.lifeTime <= 0f)
			{
				base.isActive = false;
				base.Recycle(this);
			}
			else if (this.hoodEntity != null && this.hoodEntity.nodeManager != null)
			{
				Node node = this.hoodEntity.nodeManager.GetNode(this.hoodEntity.tag);
				if (node != null && node != this.hoodEntity)
				{
					this.hoodEntity = node;
				}
				if (this.hoodEntity != null && base.go != null)
				{
					base.go.transform.position = this.hoodEntity.GetPosition();
				}
			}
		}
	}

	public DriftSkillEffect.SkillEffectRecycleType recycleType;

	public enum SkillEffectRecycleType
	{
		Unknow,
		AnimateTime,
		BuffTime
	}
}
