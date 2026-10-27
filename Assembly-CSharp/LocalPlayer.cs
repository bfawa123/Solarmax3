using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LocalPlayer : Solarmax.Singleton<LocalPlayer>
{
	public int battleRace { get; set; }

	public List<PlayerData> mathPlayer { get; set; }

	public string battleMap { get; set; }

	public string GetLocalAccount()
	{
		if (string.IsNullOrEmpty(this.localAccount))
		{
			this.localAccount = Solarmax.Singleton<LocalAccountStorage>.Get().account;
		}
		return this.localAccount;
	}

	public string GenerateLocalAccount(bool forceChange = false)
	{
		this.localAccount = SystemInfo.deviceUniqueIdentifier;
		if (forceChange)
		{
			int num = UnityEngine.Random.Range(0, 10000);
			this.localAccount = this.localAccount + "__force__" + num.ToString();
		}
		Solarmax.Singleton<LocalAccountStorage>.Get().account = this.localAccount;
		return this.localAccount;
	}

	public void ChangeMoney(int nVariety)
	{
		this.playerData.money += nVariety;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMoney, new object[0]);
	}

	public void SetMoney(int curMoney)
	{
		this.playerData.money = curMoney;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMoney, new object[0]);
	}

	public void Tick(float interval)
	{
		if (this.nextSasonStart > 0.0)
		{
			this.nextSasonStart -= (double)interval;
		}
		this.mOnLineTime += interval;
		this.refrushOnline += interval;
		if (this.refrushOnline > 60f)
		{
			this.refrushOnline = 0f;
			Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.OnLine, (int)this.mOnLineTime);
		}
		this.showAdsRefreshTime -= interval;
		if (this.showAdsRefreshTime <= 0f)
		{
			this.showAdsRefreshTime = 0f;
		}
		if (this.IsBeFangChenMI)
		{
			this.UpdateAntiAddiction(interval);
		}
		if (this.isAccountTokenOver && this.player_Offline_time > 0f)
		{
			this.player_Offline_time -= interval;
		}
		if (this.nextSasonStart <= 0.0 && Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			this.nextSasonStart = 60.0;
            Solarmax.Singleton<LocalPlayer>.Get().playerData.score = 0;
            Solarmax.Singleton<NetSystem>.Instance.helper.RequestUserInit();
		}
	}

	public bool IsInSeason(CooperationType eType)
	{
		return true;
	}

	public bool IsInSeason()
	{
		return true;
	}

	public bool IsBuyed(string productID)
	{
		return true;
	}

	public void AddBuy(string productID)
	{
		this.buyProduct.Add(productID);
	}

	public bool IsRechargeRewardCard()
	{
		DateTime d = new DateTime(1970, 1, 1);
		return (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds <= this.month_card_end;
	}

	public void InitAntiConfig(long noLine, long offLine)
	{
		if (noLine == -1L && offLine == -1L)
		{
			this.IsBeFangChenMI = false;
		}
		else
		{
			this.IsBeFangChenMI = true;
			this.IsShowOpenAntiWindow = false;
		}
		this.player_Online_time = (float)noLine;
		this.player_Offline_time = (float)offLine;
		this.ShowTipsFrequency = 3600f - this.player_Online_time % 3600f;
		if (this.ShowTipsFrequency == 0f)
		{
			this.ShowTipsFrequency = 3600f;
		}
		int num = (int)(this.player_Online_time / 3600f);
		this.histroyTips.Clear();
		for (int i = 0; i < num; i++)
		{
			this.histroyTips.Add(i + 1, true);
		}
	}

	private void UpdateAntiAddiction(float dt)
	{
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
		{
			return;
		}
		this.player_Online_time += dt;
		if (!this.IsCanOpenAntiWindow)
		{
			return;
		}
		if (this.player_Online_time >= 0f && this.player_Online_time < 10800f)
		{
			this.ShowTipsFrequency -= dt;
			if (this.ShowTipsFrequency <= 0f)
			{
				this.ShowTipsFrequency = 3600f;
				this.ShowFangChenMIInfo(LocalPlayer.FCM_TIP.Nor, false);
				return;
			}
		}
		else if (this.player_Online_time >= 10800f)
		{
			if (this.IsShowOpenAntiWindow)
			{
				return;
			}
			this.ShowFangChenMIInfo(LocalPlayer.FCM_TIP.Warring, false);
		}
	}

	public void ShowFangChenMIInfo(LocalPlayer.FCM_TIP tip, bool fouce = false)
	{
		int num = (int)(this.player_Online_time / 3600f);
		num = -1;
		if (num <= 0)
		{
			return;
		}
		bool flag = false;
		this.histroyTips.TryGetValue(num, out flag);
		if (flag && !fouce)
		{
			return;
		}
		if (!flag)
		{
			this.histroyTips.Add(num, true);
		}
		string text = string.Empty;
		if (tip == LocalPlayer.FCM_TIP.Nor)
		{
			text = string.Format(LanguageDataProvider.GetValue(2244), (int)(this.player_Online_time / 3600f));
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogAntiWindow");
			Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogAntiWindow", new object[]
			{
				2,
				text,
				0
			});
			return;
		}
		if (tip == LocalPlayer.FCM_TIP.Warring)
		{
			this.IsShowOpenAntiWindow = true;
			text = LanguageDataProvider.GetValue(2243);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogAntiWindow");
			Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogAntiWindow", new object[]
			{
				1,
				text,
				(int)this.player_Offline_time,
				new EventDelegate(new EventDelegate.Callback(this.QuitGame))
			});
		}
	}

	public void QuitGame()
	{
		this.IsBeFangChenMI = false;
		this.isAccountTokenOver = true;
		Solarmax.Singleton<NetSystem>.Instance.Close();
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LogoWindow");
	}

	public void LeaveBattle()
	{
		int num = (int)(this.player_Online_time / 3600f);
		num = -1;
		if (num <= 0)
		{
			return;
		}
		bool flag = false;
		this.histroyTips.TryGetValue(num, out flag);
		if (flag)
		{
			return;
		}
		this.histroyTips.Add(num, true);
		if (num >= 3 || this.isAccountTokenOver)
		{
			this.IsShowOpenAntiWindow = true;
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogAntiWindow");
			Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogAntiWindow", new object[]
			{
				1,
				LanguageDataProvider.GetValue(2243),
				(int)this.player_Offline_time,
				new EventDelegate(new EventDelegate.Callback(this.QuitGame))
			});
			return;
		}
		string text = string.Format(LanguageDataProvider.GetValue(2244), (int)(this.player_Online_time / 3600f));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogAntiWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogAntiWindow", new object[]
		{
			2,
			text,
			0
		});
	}

	public void AccountTokenOver()
	{
		this.IsShowOpenAntiWindow = true;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogAntiWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogAntiWindow", new object[]
		{
			1,
			LanguageDataProvider.GetValue(2243),
			(int)this.player_Offline_time,
			new EventDelegate(new EventDelegate.Callback(this.QuitGame))
		});
	}

	public string GetLocalName()
	{
		if (string.IsNullOrEmpty(this.localName))
		{
			this.localName = Solarmax.Singleton<LocalAccountStorage>.Get().name;
		}
		return this.localName;
	}

	private string localAccount;

	public PlayerData playerData = new PlayerData();

	public bool isAccountTokenOver;

	public bool isRandomUpdate;

	public float showAdsRefreshTime;

	public int CurBattlePlayerNum;

	public bool IsCanOpenAntiWindow = true;

	private bool IsBeFangChenMI;

	private bool IsShowOpenAntiWindow;

	public float player_Online_time;

	public float player_Offline_time;

	private Dictionary<int, bool> histroyTips = new Dictionary<int, bool>();

	public int SeasonType;

	public int SeasonStartTime;

	public int SeasonEndTime;

	public double nextSasonStart;

	public int nMaxAccumulMoney = 100;

	public int nCurAccumulMoney;

	public long month_card_end;

	public bool IsMonthCardReceive;

	public int mActivityDegree;

	public float mOnLineTime;

	private List<string> buyProduct = new List<string>();

	public string HomeWindow = string.Empty;

	private float refrushOnline;

	private const int ONEHOURSECOND = 3600;

	private float ShowTipsFrequency = 3600f;

	private string localName;

	public static string LocalNotice;

	public static bool LocalUnlock;

	public static int MaxTeamNum = 64;

	public enum FCM_TIP
	{
		Nor,
		Warring
	}
}
