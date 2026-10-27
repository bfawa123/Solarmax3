using System;
using Solarmax;
using UnityEngine;

public class GuideEffect : EffectNode
{
	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.liveTime += interval;
		if (this.liveTime >= 30f)
		{
			base.Recycle(this);
			return;
		}
		this.UpdateEffect(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		if (this.animator != null)
		{
			this.animator.enabled = false;
		}
		if (this.animator != null && this.animator.gameObject != null)
		{
			this.animator.gameObject.SetActive(false);
		}
		this.color.a = 0f;
		this.homeNode.CalcGlowShape(this.color);
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.homeNode != null && this.homeNode.nodeManager != null)
		{
			Node node = this.homeNode.nodeManager.GetNode(this.homeNode.tag);
			if (node != null && node != this.homeNode)
			{
				this.homeNode = node;
			}
		}
		if (string.IsNullOrEmpty(this.effectName))
		{
			Debug.Log("InitEffectNode is null");
			return;
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(this.effectName);
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		}
		this.animator = base.go.GetComponent<Animator>();
		if (this.animator == null)
		{
			Debug.Log("InitEffectNode Animator is null");
			return;
		}
		base.go.SetActive(anim);
		base.go.transform.SetParent(this.homeNode.GetGO().transform);
		base.go.transform.localScale = Vector3.one * 0.6f;
		base.go.transform.localPosition = Vector3.zero;
		this.animator.speed = Solarmax.Singleton<EffectManager>.Get().fPlayAniSpeed;
		this.animator.Play(this.animationName);
		this.totalTime = 0f;
		this.flickStatus = 0;
		this.color = Color.white;
		this.color.a = 0f;
		this.liveTime = 0f;
		base.isActive = true;
	}

	private void UpdateEffect(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		this.totalTime += dt;
		if (this.flickStatus == 0)
		{
			if (this.totalTime < this.flickDuring)
			{
				this.color.a = this.color.a + 0.016f;
			}
			else
			{
				this.flickStatus = 1;
				this.totalTime = 0f;
				this.color = Color.white;
			}
		}
		else if (this.flickStatus == 1)
		{
			if (this.totalTime < this.flickDuring)
			{
				this.color.a = this.color.a - 0.016f;
			}
			else
			{
				this.flickStatus = 2;
				this.totalTime = 0f;
				this.color.a = 0f;
			}
		}
		else if (this.flickStatus == 2 && this.totalTime >= this.flickInterval)
		{
			this.flickStatus = 0;
			this.totalTime = 0f;
		}
		this.homeNode.CalcGlowShape(this.color);
	}

	public Node homeNode;

	private Animator animator;

	public string effectName;

	public string animationName;

	public float flickInterval;

	public float flickDuring;

	private float totalTime;

	private Color color;

	private int flickStatus;

	private float liveTime = 30f;
}
