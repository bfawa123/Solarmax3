using System;
using UnityEngine;

public class GroundNode : EffectNode
{
	public float liftTime { get; set; }

	public Node owner { get; set; }

	public float scale { get; set; }

	private SpriteRenderer sprite { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateGround(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		this.liftTime = -1f;
		this.scale = 1f;
		this.color = Color.white;
		base.isActive = false;
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.owner != null && this.owner.nodeManager != null)
		{
			Node node = this.owner.nodeManager.GetNode(this.owner.tag);
			if (node != null && node != this.owner)
			{
				this.owner = node;
			}
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Select");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.sprite = base.go.GetComponentInChildren<SpriteRenderer>();
		}
		base.go.SetActive(anim);
		base.go.transform.SetParent(this.owner.GetGO().transform);
		this.scale = 0.85f;
		base.go.transform.localPosition = Vector3.zero;
		base.go.transform.localScale = Vector3.one * this.scale;
		this.color.a = 0.1f;
		this.sprite.color = this.color;
		base.isActive = true;
	}

	private void UpdateGround(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (this.liftTime == -1f)
		{
			if (this.color.a < 0.5f)
			{
				this.color.a = this.color.a + dt;
				if (this.sprite != null)
				{
					this.sprite.color = this.color;
				}
			}
			return;
		}
		this.liftTime -= dt;
		if (this.liftTime < 0f)
		{
			this.liftTime = 0f;
			base.isActive = false;
		}
		this.color.a = this.liftTime;
		if (this.sprite != null)
		{
			this.sprite.color = this.color;
		}
		if (this.liftTime <= 0f)
		{
			base.Recycle(this);
		}
	}

	public Color color;
}
