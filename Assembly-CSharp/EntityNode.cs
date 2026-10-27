using System;
using Solarmax;
using UnityEngine;

public abstract class EntityNode : Entity
{
	public EntityNode(string name) : base(name, false)
	{
	}

	protected SpriteRenderer halo { get; set; }

	protected SpriteRenderer glow { get; set; }

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
		this.FadeCompent = null;
		this.ReleaseNode();
	}

	private void ReleaseNode()
	{
		this.halo = null;
		this.FadeCompent = null;
	}

	protected override void InitGameObject()
	{
		base.InitGameObject();
		this.halo = base.go.transform.Find("halo").GetComponent<SpriteRenderer>();
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.halo.transform.Rotate(Vector3.forward, 90f);
		}
		Transform transform = base.go.transform.Find("shape");
		if (transform != null)
		{
			this.glowObject = transform.gameObject;
			this.glow = transform.GetComponent<SpriteRenderer>();
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
			{
				this.glow.transform.Rotate(Vector3.forward, 90f);
			}
			this.glowObject.SetActive(false);
		}
		this.imageobj = base.go.transform.Find("image").gameObject;
		if (this.imageobj != null && base.image != null)
		{
			base.image.color = new Color(base.image.color.r, base.image.color.g, base.image.color.b, 0f);
			this.FadeCompent = this.imageobj.GetComponent<TweenAlpha>();
			if (this.FadeCompent == null)
			{
				this.FadeCompent = this.imageobj.AddComponent<TweenAlpha>();
			}
		}
		if (!Solarmax.Singleton<BattleSystem>.Instance.battleData.mapEdit)
		{
			base.go.SetActive(false);
		}
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		if (this.halo == null)
		{
			return;
		}
		this.halo.color = color;
	}

	public override void SetHaloColor(Color color)
	{
		if (this.halo == null)
		{
			return;
		}
		this.halo.color = color;
	}

	public override void SetAlpha(float alpha)
	{
		base.SetAlpha(alpha);
		if (this.halo == null)
		{
			return;
		}
		this.halo.color = new Color(this.halo.color.r, this.halo.color.g, this.halo.color.b, alpha);
	}

	public override void CalcGlowShape(Color color)
	{
		if (this.glow != null)
		{
			this.glow.color = color;
		}
	}

	public virtual void SetGlowEnable(bool b)
	{
		if (this.glowObject != null)
		{
			this.glowObject.SetActive(b);
		}
	}

	public override void FadeEntity(bool bFadeIn, float duration)
	{
		if (this.FadeCompent == null)
		{
			return;
		}
		this.FadeCompent.SetOnFinished(delegate()
		{
			if (bFadeIn)
			{
				this.SetAlpha(1f);
			}
		});
		this.FadeCompent.ResetToBeginning();
		if (bFadeIn)
		{
			this.FadeCompent.from = 0.2f;
			this.FadeCompent.to = 1f;
		}
		else
		{
			this.HideTitleAndHaloUI(false);
			this.FadeCompent.from = 1f;
			this.FadeCompent.to = 0f;
		}
		this.FadeCompent.duration = duration;
		this.FadeCompent.Play(true);
		if (bFadeIn && !base.go.activeSelf)
		{
			base.go.SetActive(true);
		}
	}

	public void HideTitleAndHaloUI(bool bFadeIn)
	{
		if (this.title == null)
		{
			this.title = base.go.transform.Find("HudRoot");
		}
		if (this.title != null)
		{
			this.title.gameObject.SetActive(bFadeIn);
		}
		if (this.uiHalo == null)
		{
			this.uiHalo = base.go.transform.Find("Entity_UI_Halo(Clone)");
			if (this.uiHalo != null)
			{
				this.uiHalo.gameObject.SetActive(false);
			}
		}
	}

	public GameObject GetImageGameobject()
	{
		return this.imageobj;
	}

	protected GameObject glowObject;

	private TweenAlpha FadeCompent;

	private TweenAlpha haloCompent;

	protected GameObject imageobj;

	protected GameObject haloobj;

	protected Transform title;

	private Transform uiHalo;
}
