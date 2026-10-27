using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class EntityBarrierLine : EntityNode
{
	public EntityBarrierLine(string name) : base(name)
	{
	}

	public override void Tick(int frame, float interval)
	{
		if (this.fadeStatus == EntityBarrierLine.fadeline.fade_idle)
		{
			return;
		}
		float a = 0f;
		this.fAphlaTime += interval;
		if (this.fadeStatus == EntityBarrierLine.fadeline.fade_in)
		{
			a = Mathf.Lerp(0.2f, 1f, this.fAphlaTime);
		}
		else if (this.fadeStatus == EntityBarrierLine.fadeline.fade_out)
		{
			a = Mathf.Lerp(1f, 0.2f, this.fAphlaTime);
		}
		if (this.fAphlaTime > this.duration)
		{
			this.fAphlaTime = 1f;
			if (this.fadeStatus == EntityBarrierLine.fadeline.fade_in)
			{
				base.go.SetActive(true);
			}
			this.fadeStatus = EntityBarrierLine.fadeline.fade_idle;
		}
		this.startColor.a = a;
		this.m_lineRender.startColor = this.startColor;
		this.m_lineRender.endColor = this.startColor;
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_BarrierLine");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	protected override void InitGameObject()
	{
		base.InitGameObject();
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.ImageTransform.Rotate(Vector3.forward, -90f);
		}
	}

	public override void AddPoint(List<Vector3> ls)
	{
		if (this.m_lineRender != null)
		{
			this.m_lineRender.enabled = true;
			this.m_lineRender.positionCount = 2;
			this.m_lineRender.sortingLayerID = SortingLayer.NameToID("Node");
			this.m_lineRender.sortingOrder = 1;
			for (int i = 0; i < ls.Count; i++)
			{
				this.m_lineRender.SetPosition(i, ls[i]);
			}
		}
	}

	private LineRenderer m_lineRender;

	private float fAphlaTime;

	private float duration;

	private EntityBarrierLine.fadeline fadeStatus;

	private Color startColor = new Color(1f, 0f, 0f, 0f);

	private enum fadeline
	{
		fade_idle,
		fade_in,
		fade_out
	}
}
