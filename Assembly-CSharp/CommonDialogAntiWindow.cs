using System;
using Solarmax;

public class CommonDialogAntiWindow : BaseWindow
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
	}

	public override void OnHide()
	{
		this.onYes = null;
	}

	private void Update()
	{
		float player_Online_time = Solarmax.Singleton<LocalPlayer>.Get().player_Online_time;
		float player_Offline_time = Solarmax.Singleton<LocalPlayer>.Get().player_Offline_time;
		if (player_Offline_time > 0f && player_Offline_time < 60f && player_Online_time >= 10800f)
		{
			this.AcceTime.text = string.Format("00:00:{0:D2}", (int)player_Offline_time);
		}
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialog)
		{
			int num = (int)args[0];
			string str = (string)args[1];
			int offtime = (int)args[2];
			if (num == 1 && args.Length > 2)
			{
				this.onYes = (EventDelegate)args[3];
			}
			this.SetInfo(num, str, offtime);
		}
	}

	private void SetInfo(int type, string str, int offtime)
	{
		this.yesBtn1.gameObject.SetActive(true);
		this.tips.text = str;
		if (offtime <= 0)
		{
			this.AcceLable.gameObject.SetActive(false);
			this.AcceTime.text = string.Empty;
		}
		else
		{
			int num = offtime / 3600;
			int num2 = (offtime - num * 3600) / 60;
			this.AcceTime.text = string.Format("{0}:{1}", num, num2);
		}
		base.gameObject.SetActive(true);
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogAntiWindow");
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UILabel tips;

	public UILabel AcceLable;

	public UILabel AcceTime;

	private EventDelegate onYes;

	private float mOfftime;

	private const int ONEHOURSECOND = 3600;
}
