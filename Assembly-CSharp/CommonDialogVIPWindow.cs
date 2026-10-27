using System;
using Solarmax;

public class CommonDialogVIPWindow : BaseWindow
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
		this.yesBtn1.gameObject.SetActive(false);
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
				if (args.Length > 3)
				{
					this.costMoney = (int)args[3];
					this.desc = (string)args[4];
					this.chapterName = (string)args[5];
				}
			}
			this.SetInfo(this.type, str, this.costMoney, this.desc, this.chapterName);
		}
	}

	private void SetInfo(int type, string str, int costValue, string desc, string chapterName)
	{
		this.yesBtn1.gameObject.SetActive(false);
		if (type == 1)
		{
			this.yesBtn1.gameObject.SetActive(true);
		}
		this.tips.text = str;
		this.yesBtnLabel1.text = costValue.ToString();
		this.title.text = chapterName;
		this.tips.text = desc;
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
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogVIPWindow");
		}
	}

	public void OnNoClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogVIPWindow");
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogVIPWindow");
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UILabel tips;

	private int type;

	private int costMoney;

	private string desc;

	private string chapterName;

	public UILabel title;

	private EventDelegate onYes;

	private int yesCountDown;
}
