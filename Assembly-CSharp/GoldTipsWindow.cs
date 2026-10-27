using System;
using Solarmax;

public class GoldTipsWindow : BaseWindow
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

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnChoosedAvatarEvent)
		{
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("GoldTipsWindow");
	}

	public void OnClickTask()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("GoldTipsWindow");
	}

	public void OnClickCharge()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("GoldTipsWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}
}
