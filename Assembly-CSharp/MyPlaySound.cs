using System;
using UnityEngine;

public class MyPlaySound : MonoBehaviour
{
	private bool canPlay
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			UIButton component = base.GetComponent<UIButton>();
			return component == null || component.isEnabled || this.forcePlay;
		}
	}

	private void OnEnable()
	{
		if (this.trigger == MyPlaySound.Trigger.OnEnable)
		{
            Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
		}
	}

	private void OnDisable()
	{
		if (this.trigger == MyPlaySound.Trigger.OnDisable)
		{
            Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.trigger == MyPlaySound.Trigger.OnMouseOver)
		{
			if (this.mIsOver == isOver)
			{
				return;
			}
			this.mIsOver = isOver;
		}
		if (this.canPlay && ((isOver && this.trigger == MyPlaySound.Trigger.OnMouseOver) || (!isOver && this.trigger == MyPlaySound.Trigger.OnMouseOut)))
		{
            Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.trigger == MyPlaySound.Trigger.OnPress)
		{
			if (this.mIsOver == isPressed)
			{
				return;
			}
			this.mIsOver = isPressed;
		}
		if (this.canPlay && ((isPressed && this.trigger == MyPlaySound.Trigger.OnPress) || (!isPressed && this.trigger == MyPlaySound.Trigger.OnRelease)))
		{
            Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
		}
	}

	private void OnClick()
	{
		if (this.canPlay && this.trigger == MyPlaySound.Trigger.OnClick)
		{
            Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
		}
	}

	public void Play()
	{
        Solarmax.Singleton<AudioManger>.Get().PlayEffect(this.audioClip, this.volume);
	}

	public AudioClip audioClip;

	public MyPlaySound.Trigger trigger;

	[Range(0f, 1f)]
	public float volume = 1f;

	public bool forcePlay = true;

	private bool mIsOver;

	public enum Trigger
	{
		OnClick,
		OnMouseOver,
		OnMouseOut,
		OnPress,
		OnRelease,
		Custom,
		OnEnable,
		OnDisable
	}
}
