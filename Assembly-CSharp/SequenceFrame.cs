using System;
using UnityEngine;

public class SequenceFrame : MonoBehaviour
{
	private void Start()
	{
		this.mSprite = base.gameObject.GetComponent<UISprite>();
		if (null == this.mSprite)
		{
			return;
		}
		this.ResetFrame();
		this.mPlaying = this.AutoPlay;
	}

	private void ResetFrame()
	{
		this.mCurFrameIndex = 0;
		this.mCurFrameElapsedTime = 0f;
		this.mDurationPerFrame = ((this.FrameRate <= 0.1f) ? 0f : (1f / this.FrameRate));
		this.mElapsedTime = 0f;
		this.UpdateTexture();
	}

	private void UpdateTexture()
	{
		if (this.FrameTextures == null)
		{
			return;
		}
		if (null == this.mSprite)
		{
			return;
		}
		if (this.mCurFrameIndex < 0 || this.mCurFrameIndex >= this.FrameTextures.Length)
		{
			return;
		}
		this.mSprite.spriteName = this.FrameTextures[this.mCurFrameIndex];
	}

	private void Update()
	{
		if (!this.mPlaying)
		{
			return;
		}
		if (this.FrameTextures == null || this.FrameTextures.Length < 1)
		{
			return;
		}
		if (this.mDurationPerFrame < Mathf.Epsilon)
		{
			return;
		}
		float num = this.mCurFrameElapsedTime * this.FrameRate;
		int num2 = (int)num;
		int num3 = (this.mCurFrameIndex + num2) % this.FrameTextures.Length;
		if (num3 != this.mCurFrameIndex)
		{
			this.mCurFrameIndex = num3;
			this.mCurFrameElapsedTime = (num - (float)num2) * this.mDurationPerFrame;
			this.UpdateTexture();
			if (this.Duration > 0.001f && this.mElapsedTime > this.Duration && num3 == this.FrameTextures.Length - 1)
			{
				this.Stop();
			}
		}
		this.mCurFrameElapsedTime += Time.deltaTime;
		this.mElapsedTime += Time.deltaTime;
	}

	public void Play(float duration)
	{
		if (this.mPlaying)
		{
			this.mElapsedTime = 0f;
			return;
		}
		this.Duration = duration;
		this.mPlaying = true;
	}

	public void Stop()
	{
		this.mPlaying = false;
		this.ResetFrame();
	}

	public float FrameRate;

	public string[] FrameTextures;

	public bool AutoPlay;

	public float Duration;

	private int mCurFrameIndex;

	private float mCurFrameElapsedTime;

	private float mDurationPerFrame;

	private UISprite mSprite;

	private bool mPlaying;

	private float mElapsedTime;
}
