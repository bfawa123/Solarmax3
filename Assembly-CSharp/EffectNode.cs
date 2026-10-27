using System;
using Solarmax;
using UnityEngine;

public class EffectNode : IPoolObject<EffectNode>, Lifecycle2
{
	protected bool isActive { get; set; }

	protected GameObject go { get; set; }

	public virtual bool Init()
	{
		return true;
	}

	public virtual void Tick(int frame, float interval)
	{
	}

	public virtual void Destroy()
	{
		this.isActive = false;
		if (this.go != null)
		{
			UnityEngine.Object.Destroy(this.go);
		}
		this.go = null;
	}

	public override void OnRecycle()
	{
		this.isActive = false;
		if (this.go != null)
		{
			this.go.SetActive(false);
		}
	}

	public bool IsActive()
	{
		return this.isActive;
	}

	public virtual void InitEffectNode(bool anim = true)
	{
	}
}
