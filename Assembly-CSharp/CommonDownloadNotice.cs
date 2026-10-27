using System;
using Solarmax;

public class CommonDownloadNotice : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonWifiDialog);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.gameObject.SetActive(false);
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = false;
	}

	public override void OnHide()
	{
		this.onYes = null;
		this.onNo = null;
		this.yesBtn1.gameObject.SetActive(false);
		this.noBtn1.gameObject.SetActive(false);
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = true;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonWifiDialog)
		{
			long nSize = (long)args[0];
			string str = (string)args[1];
			if (args.Length > 2)
			{
				this.onYes = (EventDelegate)args[2];
			}
			if (args.Length > 3)
			{
				this.onNo = (EventDelegate)args[3];
			}
			this.SetInfo(str, nSize);
		}
	}

	private void SetInfo(string str, long nSize)
	{
		this.yesBtn1.gameObject.SetActive(true);
		this.noBtn1.gameObject.SetActive(true);
		this.tips.text = str;
		string value = LanguageDataProvider.GetValue(2176);
		string arg = this.Cover2String(nSize);
		this.noBtnLabel1.text = string.Format("{0}({1})", value, arg);
		base.gameObject.SetActive(true);
	}

	private string Cover2String(long nSize)
	{
		string arg = ((double)nSize / 1024.0 / 1024.0).ToString("0.00");
		return string.Format("{0}MB", arg);
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonWifiWindow");
	}

	public void OnNoClick()
	{
		if (this.onNo != null)
		{
			this.onNo.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonWifiWindow");
	}

	public void OnNoClose()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonWifiWindow");
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UIButton noBtn1;

	public UILabel noBtnLabel1;

	public UILabel tips;

	private EventDelegate onYes;

	private EventDelegate onNo;
}
