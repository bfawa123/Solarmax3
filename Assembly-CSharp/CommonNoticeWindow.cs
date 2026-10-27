using System;
using Solarmax;

public class CommonNoticeWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonDialog);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.gameObject.SetActive(false);
	}

	public override void OnHide()
	{
		this.onYes = null;
		this.onNo = null;
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
	}

	private void InvokeYes2CountDown()
	{
		if (this.yesCountDown <= 0)
		{
			this.yesBtnLabel2.text = LanguageDataProvider.GetValue(16);
			this.yesBtn2.isEnabled = true;
			base.CancelInvoke("InvokeYes2CountDown");
			return;
		}
		this.yesBtnLabel2.text = string.Format(LanguageDataProvider.GetValue(17), this.yesCountDown);
		this.yesCountDown--;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialog)
		{
			this.type = (int)args[0];
			string str = (string)args[1];
			if (this.type == 1)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
			}
			else if (this.type == 2)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
				if (args.Length > 3)
				{
					this.onNo = (EventDelegate)args[3];
				}
			}
			else if (this.type == 3)
			{
				if (args.Length > 2)
				{
					this.onYes = (EventDelegate)args[2];
				}
				if (args.Length > 3)
				{
					this.onNo = (EventDelegate)args[3];
				}
				if (args.Length > 4)
				{
					this.yesCountDown = (int)args[4];
				}
			}
			else if (this.type == 4 && args.Length > 2)
			{
				this.onYes = (EventDelegate)args[2];
			}
			this.SetInfo(this.type, str);
		}
	}

	private void SetInfo(int type, string str)
	{
		this.yesBtn1.gameObject.SetActive(false);
		this.yesBtn2.gameObject.SetActive(false);
		this.noBtn2.gameObject.SetActive(false);
		if (type == 1)
		{
			this.yesBtn1.gameObject.SetActive(true);
		}
		else if (type == 2)
		{
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
		}
		else if (type == 3)
		{
			this.yesBtn2.gameObject.SetActive(true);
			this.noBtn2.gameObject.SetActive(true);
			if (this.yesCountDown > 0)
			{
				this.yesBtn2.isEnabled = false;
				base.InvokeRepeating("InvokeYes2CountDown", 0f, 1f);
			}
		}
		else if (type == 4)
		{
			this.yesBtn1.gameObject.SetActive(true);
		}
		this.tips.text = str;
		base.gameObject.SetActive(true);
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		if (this.type != 4)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonNoticeWindow");
		}
	}

	public void OnNoClick()
	{
		if (this.onNo != null)
		{
			this.onNo.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonNoticeWindow");
	}

	public void OnClose()
	{
		if (this.yesBtn2.gameObject.activeSelf && this.noBtn2.gameObject.activeSelf)
		{
			this.OnNoClick();
		}
		else if (this.yesBtn1.gameObject.activeSelf)
		{
			this.OnYesClick();
		}
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UIButton yesBtn2;

	public UILabel yesBtnLabel2;

	public UIButton noBtn2;

	public UILabel noBtnLabel2;

	public UILabel tips;

	private int type;

	private EventDelegate onYes;

	private EventDelegate onNo;

	private int yesCountDown;
}
