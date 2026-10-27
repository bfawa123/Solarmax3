using System;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class StoreWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnChoosedAvatarEvent);
		base.RegisterEvent(EventId.OnBuySkinRespose);
		base.RegisterEvent(EventId.OnAdsShowEvent);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.UpdateMothCard);
		base.RegisterEvent(EventId.OnSDKGetProductInfoSuccess);
		base.RegisterEvent(EventId.OnSDKGetProductInfoFailed);
		base.RegisterEvent(EventId.OnShowMonthCardEffect);
		this.storeBehavior = this.storeView.GetComponent<storeView>();
		return true;
	}

	public void Awake()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
		engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Combine(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
		this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		this.NetStatus((NetworkReachability)Solarmax.Singleton<EngineSystem>.Get().GetNetworkRechability());
		Solarmax.Singleton<ThirdPartySystem>.Instance.GetProductsInfo();
		if (this.storeBehavior != null)
		{
			this.storeBehavior.EnsureDestroy();
			this.storeBehavior.EnsureInit();
		}
		StoreWindow.hasOpened = true;
	}

	private void EnsureInit()
	{
	}

	private void NetStatus(NetworkReachability reachability)
	{
		this.netStatus = reachability;
		if (reachability != NetworkReachability.NotReachable)
		{
			if (reachability != NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				if (reachability == NetworkReachability.ReachableViaLocalAreaNetwork)
				{
					this.netIcon.spriteName = "icon_net_wifi_03";
				}
			}
			else
			{
				this.netIcon.spriteName = "icon_net_mobile_03";
			}
		}
		else
		{
			this.netIcon.spriteName = "icon_net_offline";
		}
	}

	public override void OnHide()
	{
		EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
		engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Remove(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
		MiPlatformSDK.StopAds();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnBuySkinRespose)
		{
			int id = (int)args[0];
			Solarmax.Singleton<CollectionModel>.Get().UnLock(id);
			if (base.gameObject.activeSelf && this.storeBehavior.gameObject.activeSelf)
			{
				this.storeBehavior.EnsureDestroy();
				this.storeBehavior.EnsureInit();
			}
		}
		else if (eventId == EventId.OnAdsShowEvent)
		{
			if (base.gameObject.activeSelf && this.storeBehavior.gameObject.activeSelf)
			{
				this.storeBehavior.CoodDown();
			}
		}
		else if (eventId == EventId.UpdateMoney)
		{
			this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			if (base.gameObject.activeSelf && this.storeBehavior.gameObject.activeSelf)
			{
				this.storeBehavior.EnsureDestroy();
				this.storeBehavior.EnsureInit();
			}
		}
		else if (eventId == EventId.UpdateMothCard)
		{
			this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			if (base.gameObject.activeSelf && this.storeBehavior.gameObject.activeSelf)
			{
				this.storeBehavior.RefreshMothGo();
			}
		}
		else if (eventId == EventId.OnSDKGetProductInfoSuccess)
		{
			this.storeBehavior.RefreshProducts();
		}
		else if (eventId != EventId.OnSDKGetProductInfoFailed)
		{
			if (eventId == EventId.OnShowMonthCardEffect)
			{
				this.storeBehavior.ShowMonthCardEffect();
			}
		}
	}

	public void OnCloseClicked()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	public void RefreshView()
	{
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	private const string WIFI_ICON = "icon_net_wifi_03";

	private const string MOBILE_ICON = "icon_net_mobile_03";

	private const string NOT_REACHABLE_ICON = "icon_net_offline";

	private NetworkReachability netStatus;

	public UILabel money;

	public UISprite netIcon;

	public GameObject storeView;

	private storeView storeBehavior;

	public static bool hasOpened;
}
