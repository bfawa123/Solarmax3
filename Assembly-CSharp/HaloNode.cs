using System;
using Solarmax;
using UnityEngine;

public class HaloNode : EffectNode
{
	private float currentScale { get; set; }

	private float baseScale { get; set; }

	public Vector3 position { get; set; }

	public Vector3 scale { get; set; }

	private SpriteRenderer sprite { get; set; }

	public Node CurNode { get; set; }

	private HaloState haloState { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateHalo(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		this.minScale = 0.1f;
		this.maxScale = 1f;
		this.currentScale = 0f;
		this.position = Vector3.zero;
		this.scale = Vector3.one;
		this.color = Color.white;
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.CurNode != null && this.CurNode.nodeManager != null)
		{
			Node node = this.CurNode.nodeManager.GetNode(this.CurNode.tag);
			if (node != null && node != this.CurNode)
			{
				this.CurNode = node;
			}
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Halo");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.sprite = base.go.GetComponentInChildren<SpriteRenderer>();
		}
		base.go.SetActive(anim);
		base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		base.go.transform.position = this.position;
		base.go.transform.localScale = this.scale;
		if (this.CurNode != null)
		{
			base.go.transform.SetParent(this.CurNode.GetGO().transform);
			this.baseScale = base.go.transform.localScale.x;
		}
		this.color.a = 0.5f;
		this.sprite.color = this.color;
		this.haloState = HaloState.HaloAlphaUp;
		base.isActive = true;
	}

	private void UpdateHalo(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (base.go == null)
		{
			base.isActive = false;
			base.Recycle(this);
			return;
		}
		this.currentScale += dt;
		base.go.transform.localScale = Vector3.one * this.currentScale;
		HaloState haloState = this.haloState;
		if (haloState != HaloState.HaloAlphaUp)
		{
			if (haloState == HaloState.HaloAlphaDown)
			{
				this.color.a = this.color.a - dt;
			}
		}
		else
		{
			this.color.a = this.color.a + dt;
			if (this.color.a > 1f && this.currentScale > 0.8f)
			{
				this.haloState = HaloState.HaloAlphaDown;
			}
		}
		this.sprite.color = this.color;
		if (this.currentScale >= this.maxScale && this.haloState == HaloState.HaloAlphaDown && this.color.a < 0.01f)
		{
			base.isActive = false;
			base.Recycle(this);
		}
	}

	public float minScale = 0.1f;

	public float maxScale = 1f;

	public Color color;
}
