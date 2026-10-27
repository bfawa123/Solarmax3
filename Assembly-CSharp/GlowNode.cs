using System;
using UnityEngine;

public class GlowNode : EffectNode
{
	public GlowNode.GlowState state { get; set; }

	public Node node { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateGlow(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		this.node.SetGlowEnable(false);
		this.node = null;
		this.color = Color.white;
		this.alpha = 0f;
		this.state = GlowNode.GlowState.None;
	}

	public override void InitEffectNode(bool anim = true)
	{
		this.alpha = 0f;
		this.state = GlowNode.GlowState.FadeIn;
		this.node.SetGlowEnable(true);
		base.isActive = true;
	}

	private void UpdateGlow(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		GlowNode.GlowState state = this.state;
		if (state != GlowNode.GlowState.FadeIn)
		{
			if (state != GlowNode.GlowState.FadeOut)
			{
				base.isActive = false;
				base.Recycle(this);
			}
			else
			{
				this.alpha -= dt * 2f;
				if (this.alpha <= 0f)
				{
					this.alpha = 0f;
					this.state = GlowNode.GlowState.None;
				}
				this.color.a = this.alpha;
				this.node.SetGlowColor(this.color);
			}
		}
		else
		{
			this.alpha += dt * 4f;
			if (this.alpha >= 0.8f)
			{
				this.alpha = 0.8f;
				this.state = GlowNode.GlowState.FadeOut;
			}
			this.color.a = this.alpha;
			this.node.SetGlowColor(this.color);
		}
	}

	public Color color;

	private float alpha;

	public enum GlowState
	{
		None,
		FadeIn,
		FadeOut
	}
}
