using System;
using Solarmax;
using UnityEngine;

public class DarkPulse : EffectNode
{
	private float currentScale { get; set; }

	public float angle { get; set; }

	public Vector3 position { get; set; }

	public Vector3 scale { get; set; }

	public Node from { get; set; }

	public Node to { get; set; }

	public TEAM team { get; set; }

	public float moveRate { get; set; }

	public WarpType type { get; set; }

	private bool isMoveto { get; set; }

	private Animator anitor { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		this.UpdateDark(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void OnRecycle()
	{
		base.OnRecycle();
		if (this.anitor != null)
		{
			this.anitor.enabled = false;
		}
		this.currentScale = 0f;
		this.position = Vector3.zero;
		this.scale = Vector3.one;
		this.color = Color.white;
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (this.from != null && this.from.nodeManager != null)
		{
			Node node = this.from.nodeManager.GetNode(this.from.tag);
			if (node != null && node != this.from)
			{
				this.from = node;
			}
		}
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Effect_Door");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.anitor = base.go.GetComponent<Animator>();
			this.sprites = base.go.GetComponentsInChildren<SpriteRenderer>();
		}
		base.go.SetActive(anim);
		this.anitor.enabled = true;
		base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		base.go.transform.position = this.position;
		base.go.transform.localScale = this.scale;
		for (int i = 0; i < this.sprites.Length; i++)
		{
			this.sprites[i].color = this.color;
		}
		if (this.from != null)
		{
			base.go.transform.SetParent(this.from.GetGO().transform);
		}
		if (this.anitor == null)
		{
			Debug.Log("Animation is null");
			return;
		}
		this.anitor.speed = Solarmax.Singleton<EffectManager>.Get().fPlayAniSpeed;
		WarpType type = this.type;
		if (type != WarpType.WarpPulse)
		{
			if (type == WarpType.WarpArrive)
			{
				this.anitor.Play("Effect_Door_out");
			}
		}
		else
		{
			this.currentScale = 1f;
			this.isMoveto = true;
			this.anitor.Play("Effect_Door_in");
		}
		base.isActive = true;
	}

	private void UpdateDark(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (this.anitor == null)
		{
			base.isActive = false;
			base.Recycle(this);
			return;
		}
		WarpType type = this.type;
		if (type != WarpType.WarpPulse)
		{
			if (type == WarpType.WarpArrive)
			{
				if (this.anitor.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
				{
					base.isActive = false;
					base.Recycle(this);
				}
			}
		}
		else
		{
			this.currentScale -= dt;
			if ((double)this.currentScale < 0.5 && this.isMoveto)
			{
				this.isMoveto = false;
			}
			if (this.anitor.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
			{
				base.isActive = false;
				base.Recycle(this);
			}
		}
	}

	public Color color;

	private SpriteRenderer[] sprites;
}
