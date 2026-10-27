using System;
using UnityEngine;

public class GalaxyBgBehaviour : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (this.subAlphaing)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.27f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					this.mainTexture.color -= new Color(0f, 0f, 0f, 0.125f);
				}
			}
			else
			{
				this.subAlphaing = false;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
		if (this.addAlphaing)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.27f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					this.mainTexture.color += new Color(0f, 0f, 0f, 0.125f);
				}
			}
			else
			{
				this.addAlphaing = false;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
		if (this.alphaing)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.15f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					this.mainTexture.color += new Color(0f, 0f, 0f, 0.25f);
				}
			}
			else
			{
				this.alphaing = false;
				this.scaling = true;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
		if (this.scaling)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.68f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					base.gameObject.transform.localScale += new Vector3(0.0022f, 0.0022f, 0f);
					this.mainTexture.color -= new Color(0f, 0f, 0f, 0.05f);
				}
			}
			else
			{
				this.scaling = false;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
		if (this.unlockAlpha)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.15f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					this.mainTexture.color += new Color(0f, 0f, 0f, 0.22f);
				}
			}
			else
			{
				this.unlockAlpha = false;
				this.unlockScale = true;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
		if (this.unlockScale)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.3f)
			{
				this.frameDelta += Time.deltaTime;
				if (this.frameDelta >= 0.03f)
				{
					this.frameDelta = 0f;
					base.gameObject.transform.localScale += new Vector3(0.0025f, 0.0025f, 0f);
					this.mainTexture.color -= new Color(0f, 0f, 0f, 0.11f);
				}
			}
			else
			{
				this.unlockScale = false;
				this.delta = 0f;
				this.frameDelta = 0f;
			}
		}
	}

	public void StartAnimation()
	{
		this.alphaing = true;
		this.scaling = false;
		this.delta = 0f;
		this.frameDelta = 0f;
	}

	public void StartUnlockAnimation()
	{
		this.unlockAlpha = true;
		this.unlockScale = false;
		this.delta = 0f;
		this.frameDelta = 0f;
	}

	public void StartSubAlpha()
	{
		this.subAlphaing = true;
		this.delta = 0f;
		this.frameDelta = 0f;
	}

	public void StartAddAlpha()
	{
		this.addAlphaing = true;
		this.delta = 0f;
		this.frameDelta = 0f;
	}

	private bool alphaing;

	private bool subAlphaing;

	private bool addAlphaing;

	private bool scaling;

	private float delta;

	private float frameDelta;

	public UITexture mainTexture;

	public bool unlockAlpha;

	public bool unlockScale;
}
