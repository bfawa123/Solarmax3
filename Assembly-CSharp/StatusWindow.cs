using System;
using Solarmax;
using UnityEngine;

public class StatusWindow : BaseWindow
{
	private void Awake()
	{
		this.storeBtn.onClick = new UIEventListener.VoidDelegate(this.ShowSelectMap);
		this.raceBtn.onClick = new UIEventListener.VoidDelegate(this.OnRaceClick);
		this.friendBtn.onClick = new UIEventListener.VoidDelegate(this.ShowRecord);
		this.fightBtn.onClick = new UIEventListener.VoidDelegate(this.OnFightClick);
		this.championBtn.onClick = new UIEventListener.VoidDelegate(this.OnChampionClick);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnStatusWindowTabSelect);
		base.RegisterEvent(EventId.OnCoinSync);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.SetPlayerInfo();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnStatusWindowTabSelect)
		{
			int num = (int)args[0];
			Transform parent = null;
			if (num == 0)
			{
				parent = this.storeBtn.transform;
			}
			else if (num == 1)
			{
				parent = this.raceBtn.transform;
			}
			else if (num == 2)
			{
				parent = this.fightBtn.transform;
			}
			else if (num == 3)
			{
				parent = this.friendBtn.transform;
			}
			else if (num == 4)
			{
				parent = this.championBtn.transform;
			}
			this.selectTab.transform.SetParent(parent, false);
		}
		else if (eventId == EventId.OnCoinSync)
		{
			this.SetCoinInfo();
		}
	}

	private void SetPlayerInfo()
	{
		this.userRewardLabel.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.score.ToString();
		this.userNameLabel.text = string.Format("Hi, {0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.name);
		this.userIconTexture.picUrl = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
	}

	private void ShowSelectMap(GameObject go)
	{
		this.selectTab.transform.SetParent(go.transform, false);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SelectMapWindow");
	}

	private void ShowRecord(GameObject go)
	{
	}

	private void OnRaceClick(GameObject go)
	{
		this.selectTab.transform.SetParent(go.transform, false);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RaceWindow");
	}

	private void OnFightClick(GameObject go)
	{
		this.selectTab.transform.SetParent(go.transform, false);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
	}

	private void OnChampionClick(GameObject go)
	{
		this.selectTab.transform.SetParent(go.transform, false);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomSelectWindowNew");
	}

	public void OnSettingClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void SetCoinInfo()
	{
	}

	public UILabel userRewardLabel;

	public UILabel userNameLabel;

	public NetTexture userIconTexture;

	public UILabel championLabel;

	public UILabel goldLabel;

	public UILabel jewelLabel;

	public UIEventListener storeBtn;

	public UIEventListener raceBtn;

	public UIEventListener fightBtn;

	public UIEventListener friendBtn;

	public UIEventListener championBtn;

	public GameObject selectTab;

	private int oldGold;

	private int oldJewel;
}
