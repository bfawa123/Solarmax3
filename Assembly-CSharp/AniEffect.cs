using System;
using Solarmax;
using UnityEngine;

public class AniEffect : EffectNode
{
	private SpriteRenderer[] sprites { get; set; }

	public Node hoodEntity { get; set; }

	public Vector3 position { get; set; }

	private Animator anitor { get; set; }

	public string effectName { get; set; }

	public string AnimetorName { get; set; }

	public float lifeTime { get; set; }

	public float scale { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateEffect(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
		this.color = Color.white;
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		if (this.anitor != null)
		{
			this.anitor.enabled = false;
		}
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
			this.anitor = base.go.GetComponent<Animator>();
			if (this.anitor == null)
			{
				Debug.Log("InitEffectNode Animator is null");
				return;
			}
		}
		base.go.SetActive(true);
		if (this.hoodEntity != null)
		{
			base.go.transform.SetParent(this.hoodEntity.GetGO().transform);
		}
		else
		{
			base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		}
		base.go.transform.position = this.position;
		base.go.transform.localScale = Vector3.one * this.scale;
		if (anim)
		{
			this.anitor.speed = Solarmax.Singleton<EffectManager>.Get().fPlayAniSpeed;
			this.anitor.enabled = true;
			this.anitor.Play(this.AnimetorName);
		}
		base.isActive = true;
	}

	private void UpdateEffect(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = this.anitor.GetCurrentAnimatorStateInfo(0);
		if (currentAnimatorStateInfo.normalizedTime > 1f && !currentAnimatorStateInfo.loop)
		{
			base.isActive = false;
			base.Recycle(this);
		}
		if (this.lifeTime > 0f)
		{
			this.lifeTime -= dt;
			if (this.lifeTime <= 0f)
			{
				base.isActive = false;
				base.Recycle(this);
			}
		}
	}

	public Color color;
}
