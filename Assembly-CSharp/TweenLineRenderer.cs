using System;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Tween LineRenderer")]
public class TweenLineRenderer : UITweener
{
	[Obsolete("Use 'value' instead")]
	public float fade
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	private void Cache()
	{
		this.mCached = true;
		this.fadeValue = 1f;
		this.useScale = false;
		this.useAlpha = false;
		this.havePositions = false;
		this.haveColors = false;
		this.mLr = base.GetComponent<LineRenderer>();
		if (this.mLr == null)
		{
			this.mLr = base.gameObject.GetComponentInChildren<LineRenderer>();
			if (this.mLr == null)
			{
				base.enabled = false;
			}
		}
	}

	public float value
	{
		get
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			return this.fadeValue;
		}
		set
		{
			if (!this.mCached)
			{
				this.Cache();
			}
			this.fadeValue = value;
			if (this.havePositions && this.useScale && this.mLr != null)
			{
				for (int i = 0; i < this.mPosition.Length; i++)
				{
					this.mLr.SetPosition(i, this.fadeValue * this.mPosition[i]);
				}
			}
			if (this.haveColors && this.useAlpha && this.mLr != null)
			{
				this.tempStartColor.a = this.startColor.a * this.fadeValue;
				this.tempEndColor.a = this.endColor.a * this.fadeValue;
				this.mLr.SetColors(this.tempStartColor, this.tempEndColor);
			}
		}
	}

	protected override void OnUpdate(float factor, bool isFinished)
	{
		this.value = Mathf.Lerp(this.from, this.to, factor);
	}

	public override void SetStartToCurrentValue()
	{
		this.from = this.value;
	}

	public override void SetEndToCurrentValue()
	{
		this.to = this.value;
	}

	public void SetPositions(Vector3[] positions)
	{
		if (!this.mCached)
		{
			this.Cache();
		}
		this.mPosition = (positions.Clone() as Vector3[]);
		this.havePositions = true;
	}

	public void SetColors(Color start, Color end)
	{
		if (!this.mCached)
		{
			this.Cache();
		}
		this.tempStartColor = start;
		this.startColor = start;
		this.tempEndColor = end;
		this.endColor = end;
		this.haveColors = true;
	}

	public void SetFadeFactor(bool scale, bool alpha)
	{
		if (!this.mCached)
		{
			this.Cache();
		}
		this.useScale = scale;
		this.useAlpha = alpha;
		if (this.havePositions && !this.useScale && this.mLr != null)
		{
			for (int i = 0; i < this.mPosition.Length; i++)
			{
				this.mLr.SetPosition(i, this.mPosition[i]);
			}
		}
		if (this.haveColors && !this.useAlpha && this.mLr != null)
		{
			this.mLr.SetColors(this.startColor, this.endColor);
		}
	}

	[Range(0f, 1f)]
	public float from = 1f;

	[Range(0f, 1f)]
	public float to = 1f;

	private bool mCached;

	private LineRenderer mLr;

	private Vector3[] mPosition;

	private Color startColor;

	private Color endColor;

	private Color tempStartColor;

	private Color tempEndColor;

	private bool useScale;

	private bool useAlpha;

	private bool havePositions;

	private bool haveColors;

	private float fadeValue;
}
