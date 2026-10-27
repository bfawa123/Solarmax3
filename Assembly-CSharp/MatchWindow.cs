using System;
using UnityEngine;

public class MatchWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMatch);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.windowAnim.Play("MatchWindow");
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnMatch)
		{
			this.waitTime = (int)args[0];
			this.waitTime++;
			this.UpdateWaitUserTime();
		}
	}

	private void UpdateWaitUserTime()
	{
		this.waitTime--;
		this.waitTimeLabel.text = string.Format("{0:D2}:{1:D2}", this.waitTime / 60, this.waitTime % 60);
		if (this.waitTime > 0)
		{
			base.Invoke("UpdateWaitUserTime", 1f);
		}
	}

	public UILabel waitTimeLabel;

	public Animator windowAnim;

	private int waitTime = 10;
}
