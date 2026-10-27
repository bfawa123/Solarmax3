using System;
using Solarmax;
using UnityEngine;

public class LaserLineNode : EffectNode
{
	public Vector3 beginPosition { get; set; }

	public Vector3 endPosition { get; set; }

	private SpriteRenderer sprite { get; set; }

	private LineRenderer m_lineRender { get; set; }

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
		this.minAlpha = 0f;
		this.maxAlpha = 1f;
		this.currentAlpha = 0f;
		this.beginPosition = Vector3.zero;
		this.endPosition = Vector3.zero;
		this.color = Color.white;
		this.m_Tide = false;
	}

	public override void InitEffectNode(bool anim = true)
	{
		if (base.go == null)
		{
			UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_LaserLineNew");
			base.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
			this.m_lineRender = base.go.GetComponentInChildren<LineRenderer>();
			base.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		}
		base.go.SetActive(anim);
		float x = this.endPosition.x - this.beginPosition.x;
		float y = this.endPosition.y - this.beginPosition.y;
		float num = Vector3.Distance(this.endPosition, this.beginPosition);
		float num2 = Mathf.Atan2(y, x);
		Vector3 one = Vector3.one;
		one.x = num * 6f;
		one.y = 0.5f;
		base.go.transform.localScale = one;
		if (this.m_lineRender != null)
		{
			this.m_lineRender.enabled = true;
			this.m_lineRender.positionCount = 2;
			this.m_lineRender.sortingLayerID = SortingLayer.NameToID("Node");
			this.m_lineRender.sortingOrder = 1;
			this.m_lineRender.SetPosition(0, this.beginPosition);
			this.m_lineRender.SetPosition(1, this.endPosition);
		}
		this.currentAlpha = this.minAlpha;
		this.color.a = this.currentAlpha;
		this.m_lineRender.startColor = this.color;
		this.m_lineRender.endColor = this.color;
		base.isActive = true;
		this.m_Tide = false;
	}

	private void UpdateHalo(int frame, float dt)
	{
		if (!base.isActive)
		{
			return;
		}
		if (!this.m_Tide)
		{
			this.currentAlpha += dt * 15f;
			this.color.a = this.currentAlpha;
			this.m_lineRender.enabled = true;
			this.m_lineRender.startColor = this.color;
			this.m_lineRender.endColor = this.color;
			if (this.currentAlpha >= this.maxAlpha)
			{
				this.m_Tide = true;
			}
		}
		else
		{
			this.currentAlpha = 0f;
			this.color.a = this.currentAlpha;
			this.m_lineRender.startColor = this.color;
			this.m_lineRender.endColor = this.color;
			if (this.currentAlpha <= this.minAlpha)
			{
				base.Recycle(this);
			}
		}
	}

	private float minAlpha;

	private float maxAlpha = 1f;

	public Color color;

	private float currentAlpha;

	private bool m_Tide;
}
