using System;

public class ResumingWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public override void OnHide()
	{
	}

	public UILabel tips;

	private float waitBeginTime;
}
