using System;
using UnityEngine;

public class LineFillAnimationBehaviour : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (this.filling)
		{
			this.delta += Time.deltaTime;
			this.everyPictureDelta += Time.deltaTime;
			if (this.delta <= 0.3f)
			{
				if (this.everyPictureDelta > 0.05f)
				{
					this.everyPictureDelta = 0f;
					this.line.fillAmount += 0.2f;
					this.line.color += new Color(0f, 0f, 0f, 0.08f);
				}
			}
			else
			{
				this.filling = false;
				this.delta = 0f;
			}
		}
		if (this.unlockFilling)
		{
			this.delta += Time.deltaTime;
			if (this.delta <= 0.5f)
			{
				this.everyPictureDelta += Time.deltaTime;
				if (this.everyPictureDelta > 0.05f)
				{
					this.everyPictureDelta = 0f;
					this.line.fillAmount += 0.15f;
				}
			}
			else
			{
				this.unlockFilling = false;
				this.delta = 0f;
			}
		}
		if (this.fading)
		{
			this.delta += Time.deltaTime;
			this.everyPictureDelta += Time.deltaTime;
			if (this.delta <= 0.6f)
			{
				if (this.everyPictureDelta > 0.05f)
				{
					this.everyPictureDelta = 0f;
					this.line.color -= new Color(0f, 0f, 0f, 0.04f);
				}
			}
			else
			{
				this.fading = false;
				this.delta = 0f;
			}
		}
		if (this.unlockFading)
		{
			this.delta += Time.deltaTime;
			this.everyPictureDelta += Time.deltaTime;
			if (this.delta <= 0.3f)
			{
				if (this.everyPictureDelta > 0.05f)
				{
					this.everyPictureDelta = 0f;
					this.line.fillAmount -= 0.2f;
					this.line.color -= new Color(0f, 0f, 0f, 0.2f);
				}
			}
			else
			{
				this.unlockFading = false;
				this.delta = 0f;
			}
		}
	}

	public void Fill(bool unlock = false)
	{
		this.filling = !unlock;
		this.unlockFilling = unlock;
		this.delta = 0f;
		this.everyPictureDelta = 0.05f;
	}

	public void Fade(bool unlock = false)
	{
		this.fading = !unlock;
		this.unlockFading = unlock;
		this.delta = 0f;
		this.everyPictureDelta = 0.05f;
		this.everyPictureAlpha = ((!unlock) ? 0.08f : 0.2f);
	}

	public UISprite line;

	private bool filling;

	private bool unlockFilling;

	private float delta;

	private float everyPictureDelta;

	private bool fading;

	private bool unlockFading;

	private float everyPictureAlpha = 0.08f;
}
