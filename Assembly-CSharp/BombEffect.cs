using System;
using Solarmax;
using UnityEngine;

public class BombEffect : EffectNode
{
	public Node node { get; set; }

	public Vector3 bombPosition { get; set; }

	public Color color { get; set; }

	private Animator animator { get; set; }

	private SpriteRenderer[] sprites { get; set; }

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
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		if (this.animator != null)
		{
			this.animator.enabled = false;
		}
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.node != null && this.node.nodeManager != null)
		{
			Node node = this.node.nodeManager.GetNode(this.node.tag);
			if (node != null && node != this.node)
			{
				this.node = node;
			}
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(BombEffect.bombEffect);
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.animator = base.go.GetComponent<Animator>();
			if (this.animator == null)
			{
				Debug.Log("Bomb Effect : animator is null!");
				return;
			}
			this.sprites = base.go.GetComponentsInChildren<SpriteRenderer>();
		}
		if (this.node != null && this.node.GetGO() != null)
		{
			base.go.transform.SetParent(this.node.GetGO().transform);
			float num = 0.4f;
			base.go.transform.localScale = new Vector3(num / this.node.GetGO().transform.localScale.x, num / this.node.GetGO().transform.localScale.y, num / this.node.GetGO().transform.localScale.z);
		}
		else
		{
			base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			base.go.transform.localScale = Vector3.one;
		}
		base.go.transform.position = this.bombPosition;
		for (int i = 0; i < this.sprites.Length; i++)
		{
			this.sprites[i].color = this.color;
		}
		this.animator.enabled = anim;
		base.go.SetActive(anim);
		if (anim)
		{
			this.animator.speed = Solarmax.Singleton<EffectManager>.Get().fPlayAniSpeed;
			this.animator.Play(BombEffect.bombAnimatorName);
		}
		base.isActive = anim;
	}

	private void UpdateEffect(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (this.animator)
		{
			if (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
			{
				base.isActive = false;
				this.animator.enabled = false;
				base.Recycle(this);
			}
		}
		else
		{
			base.isActive = false;
			base.Recycle(this);
		}
	}

	private static string bombEffect = "Effect_Explosion";

	private static string bombAnimatorName = "Effect_Explosion_in";
}
