using System;
using Solarmax;

public class CommonDialogTileWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnInviteDialog);
		return true;
	}

	private void InvokeNoCountDown()
	{
		if (this.noCountDown <= 0)
		{
			this.noBtnLabel2.text = LanguageDataProvider.GetValue(2116);
			base.CancelInvoke("InvokeNoCountDown");
			this.OnNoClick();
			return;
		}
		this.noBtnLabel2.text = string.Format("{0}({1})", LanguageDataProvider.GetValue(2116), this.noCountDown);
		this.noCountDown--;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
		this.onYes = null;
		this.onNo = null;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnInviteDialog)
		{
			this.curInviteUserID = (int)args[0];
			string text = (string)args[1];
			string text2 = (string)args[2];
			string text3 = (string)args[3];
			int num = (int)args[4];
			this.noCountDown = (int)args[5];
			if (args.Length > 2)
			{
				this.onYes = (EventDelegate)args[6];
			}
			if (args.Length > 3)
			{
				this.onNo = (EventDelegate)args[7];
			}
			if (!text2.EndsWith(".png"))
			{
				text2 += ".png";
			}
			this.tips.text = text;
			this.Icon.picPanel = null;
			this.Icon.picUrl = text2.ToLower();
			this.playerName.text = text3;
			this.playerSroce.text = num.ToString();
			base.InvokeRepeating("InvokeNoCountDown", 0f, 1f);
		}
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogTileWindow");
	}

	public void OnNoClick()
	{
		if (this.onNo != null)
		{
			this.onNo.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogTileWindow");
	}

	public void OnClose()
	{
		this.OnNoClick();
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UIButton noBtn2;

	public UILabel noBtnLabel2;

	public UILabel tips;

	public NetTexture Icon;

	public UILabel playerName;

	public UILabel playerSroce;

	private EventDelegate onYes;

	private EventDelegate onNo;

	private int noCountDown;

	private int curInviteUserID = -1;
}
