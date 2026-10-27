using System;
using Solarmax;

public class MonthRechargeTipsWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		DateTime d = new DateTime(1970, 1, 1);
		long num = (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds;
		long num2 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) / 86400L;
		long num3 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) % 86400L;
		if (num3 > 0L)
		{
			num2 += 1L;
		}
		if (num2 < 0L)
		{
			num2 = 0L;
		}
		this.contentLabel.text = string.Format(LanguageDataProvider.GetValue(2287), num2);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("MonthRechargeTipsWindow");
	}

	public void OnClickCharge()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("MonthRechargeTipsWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public UILabel contentLabel;
}
