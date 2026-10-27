using System;
using Solarmax;
using UnityEngine;

public class HomeTitleWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnUpdateName);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnUpdateTitleWindowLayer);
		return true;
	}

	public override void Release()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		this.RefreshPlayerInfo();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnUpdateName)
		{
			this.RefreshPlayerInfo();
		}
		if (eventId == EventId.UpdateMoney)
		{
			this.RefreshPlayerInfo();
		}
		if (eventId == EventId.OnUpdateTitleWindowLayer)
		{
			int nLayer = (int)args[0];
			this.parentWindow = (string)args[1];
			this.UpdateLayer(nLayer);
		}
	}

	public override void OnHide()
	{
	}

	private void RefreshPlayerInfo()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		if (playerData == null)
		{
			return;
		}
		string picUrl = string.Empty;
		if (!Solarmax.Singleton<LocalPlayer>.Get().playerData.icon.EndsWith(".png"))
		{
			picUrl = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon + ".png";
		}
		else
		{
			picUrl = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		}
		this.playerIcon.picUrl = picUrl;
		this.playerName.text = ((!string.IsNullOrEmpty(playerData.name)) ? playerData.name : "无名喵喵");
		this.playerMoney.text = playerData.money.ToString();
		this.playerScore.text = playerData.score.ToString();
	}

	public void OnBnStoreClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnClickAvatar()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("HomeTitleWindow");
		if (!string.IsNullOrEmpty(this.parentWindow))
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow(this.parentWindow);
		}
	}

	private void UpdateLayer(int nLayer)
	{
		if (nLayer == 0)
		{
			this.headIconGo.SetActive(true);
			this.playerNameGo.SetActive(true);
			this.backButtonGo.SetActive(false);
			this.rankGo.SetActive(true);
			this.starGo.SetActive(true);
			this.wifiGo.SetActive(true);
			this.moneyGo.SetActive(true);
			this.addMoneyGo.SetActive(true);
			this.settingGo.SetActive(true);
			this.leftTable.Reposition();
			this.rightTable.Reposition();
		}
	}

	public NetTexture playerIcon;

	public UILabel playerName;

	public UILabel playerScore;

	public UILabel playerMoney;

	public UILabel playerStar;

	public global::Ping PingView;

	private string parentWindow = string.Empty;

	public GameObject headIconGo;

	public GameObject playerNameGo;

	public GameObject backButtonGo;

	public GameObject rankGo;

	public GameObject starGo;

	public GameObject wifiGo;

	public GameObject moneyGo;

	public GameObject addMoneyGo;

	public GameObject settingGo;

	public UITable leftTable;

	public UITable rightTable;
}
