using System;
using Solarmax;
using UnityEngine;

public class CollectionWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnChoosedAvatarEvent);
		base.RegisterEvent(EventId.OnBuySkinRespose);
		base.RegisterEvent(EventId.OnStartDownLoad2);
		base.RegisterEvent(EventId.OnUpdateDownLoad2);
		base.RegisterEvent(EventId.UpdateSkinStatus);
		base.RegisterEvent(EventId.OnBgPreviewShow);
		base.RegisterEvent(EventId.OnBgPreviewClose);
		return true;
	}

	public void Awake()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		if (!CollectionWindow.isShowBuyView)
		{
			this.topTitle.gameObject.SetActive(true);
			this.leftBar.SetActive(true);
			EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
			engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Combine(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
			this.NetStatus((NetworkReachability)Solarmax.Singleton<EngineSystem>.Get().GetNetworkRechability());
			this.imageToggleEvent = new EventDelegate(new EventDelegate.Callback(this.OnImageToggleChanged));
			this.sceneToggleEvent = new EventDelegate(new EventDelegate.Callback(this.OnSceneToggleChanged));
			this.coverToggleEvent = new EventDelegate(new EventDelegate.Callback(this.OnCoverToggleChanged));
			this.imageToggle.onChange.Add(this.imageToggleEvent);
			this.sceneToggle.onChange.Add(this.sceneToggleEvent);
			this.coverToggle.onChange.Add(this.coverToggleEvent);
			this.preview.SetActive(false);
			this.coverToggle.value = true;
			this.OnCoverToggleChanged();
		}
		else
		{
			CollectionWindow.isShowBuyView = false;
		}
		CollectionWindow.isShow = true;
	}

	private void EnsureInit()
	{
	}

	private void OnImageToggleChanged()
	{
		this.imageView.SetActive(this.imageToggle.value);
		AvatarView component = this.imageView.GetComponent<AvatarView>();
		BGView component2 = this.sceneView.GetComponent<BGView>();
		PersonalView component3 = this.coverView.GetComponent<PersonalView>();
		if (this.imageToggle.value)
		{
			component2.EnsureDestroy();
			component3.EnsureDestroy();
			component.EnsureInit();
		}
		else
		{
			component.EnsureDestroy();
		}
	}

	private void OnSceneToggleChanged()
	{
		this.sceneView.SetActive(this.sceneToggle.value);
		AvatarView component = this.imageView.GetComponent<AvatarView>();
		BGView component2 = this.sceneView.GetComponent<BGView>();
		PersonalView component3 = this.coverView.GetComponent<PersonalView>();
		if (this.sceneToggle.value)
		{
			component.EnsureDestroy();
			component3.EnsureDestroy();
			component2.EnsureInit();
		}
		else
		{
			component2.EnsureDestroy();
		}
	}

	private void OnCoverToggleChanged()
	{
		this.coverView.SetActive(this.coverToggle.value);
		AvatarView component = this.imageView.GetComponent<AvatarView>();
		BGView component2 = this.sceneView.GetComponent<BGView>();
		PersonalView component3 = this.coverView.GetComponent<PersonalView>();
		if (this.coverToggle.value)
		{
			component2.EnsureDestroy();
			component.EnsureDestroy();
			component3.EnsureInit();
		}
		else
		{
			component3.EnsureDestroy();
		}
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
		if (!CollectionWindow.isShowBuyView)
		{
			this.imageToggle.onChange.Remove(this.imageToggleEvent);
			this.sceneToggle.onChange.Remove(this.sceneToggleEvent);
			this.coverToggle.onChange.Remove(this.coverToggleEvent);
			EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
			engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Remove(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
			AvatarView component = this.imageView.GetComponent<AvatarView>();
			BGView component2 = this.sceneView.GetComponent<BGView>();
			PersonalView component3 = this.coverView.GetComponent<PersonalView>();
			PreviewView component4 = this.preview.GetComponent<PreviewView>();
			component2.EnsureDestroy();
			component3.EnsureDestroy();
			component.EnsureDestroy();
			component4.EnsureDestroy();
			BGManager.Inst.ApplyLastSkinConfig();
		}
		CollectionWindow.isShow = false;
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnChoosedAvatarEvent)
		{
			if (eventId == EventId.OnBuySkinRespose)
			{
				this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
				int id = (int)args[0];
				Solarmax.Singleton<CollectionModel>.Get().UnLock(id);
				AvatarView component = this.imageView.GetComponent<AvatarView>();
				BGView component2 = this.sceneView.GetComponent<BGView>();
				PreviewView component3 = this.preview.GetComponent<PreviewView>();
				if (base.gameObject.activeSelf && this.imageView.activeSelf)
				{
					component.EnsureDestroy();
					component.EnsureInit();
				}
				if (base.gameObject.activeSelf && this.sceneView.activeSelf)
				{
					component2.EnsureDestroy();
					component2.EnsureInit();
				}
				if (base.gameObject.activeSelf && this.preview.activeSelf)
				{
					component3.OnBuySkinResponse();
				}
			}
			else if (eventId == EventId.OnStartDownLoad2)
			{
				BGView component4 = this.sceneView.GetComponent<BGView>();
				if (component4.gameObject.activeSelf)
				{
					component4.StartDownLoad();
				}
				PreviewView component5 = this.preview.GetComponent<PreviewView>();
				if (component5.gameObject.activeSelf)
				{
					component5.StartDownLoad();
				}
			}
			else if (eventId == EventId.OnUpdateDownLoad2)
			{
				int nAddVuale = (int)args[0];
				BGView component6 = this.sceneView.GetComponent<BGView>();
				if (component6.gameObject.activeSelf)
				{
					component6.UpdateProgress(nAddVuale);
				}
				PreviewView component7 = this.preview.GetComponent<PreviewView>();
				if (component7.gameObject.activeSelf)
				{
					component7.UpdateProgress(nAddVuale);
				}
			}
			else if (eventId == EventId.UpdateSkinStatus)
			{
				BGView component8 = this.sceneView.GetComponent<BGView>();
				if (component8.gameObject.activeSelf)
				{
					component8.UpdateSkinStatus();
				}
			}
			else if (eventId == EventId.OnBgPreviewShow)
			{
				this.topTitle.gameObject.SetActive(false);
				this.leftBar.SetActive(false);
				this.sceneView.SetActive(false);
				this.preview.SetActive(true);
				PreviewView component9 = this.preview.GetComponent<PreviewView>();
				component9.EnsureInit();
			}
			else if (eventId == EventId.OnBgPreviewClose)
			{
				this.leftBar.SetActive(true);
				this.sceneView.SetActive(true);
			}
		}
	}

	public void OnCloseClicked()
	{
		if (this.preview.activeSelf)
		{
			this.topTitle.gameObject.SetActive(true);
			this.leftBar.SetActive(true);
			this.sceneView.SetActive(true);
			this.preview.SetActive(false);
			PreviewView component = this.preview.GetComponent<PreviewView>();
			component.EnsureDestroy();
			BGManager.Inst.ApplyLastSkinConfig();
			BGView component2 = this.sceneView.GetComponent<BGView>();
			component2.EnsureInit();
		}
		else
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
		}
	}

	public void RefreshView()
	{
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		CollectionWindow.isShowBuyView = true;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	private const string WIFI_ICON = "icon_net_wifi_03";

	private const string MOBILE_ICON = "icon_net_mobile_03";

	private const string NOT_REACHABLE_ICON = "icon_net_offline";

	private NetworkReachability netStatus;

	public UILabel money;

	public UISprite netIcon;

	public UIToggle imageToggle;

	public UIToggle sceneToggle;

	public UIToggle coverToggle;

	public GameObject imageView;

	public GameObject sceneView;

	public GameObject coverView;

	public GameObject leftBar;

	public GameObject topTitle;

	public GameObject preview;

	public static SkinConfig choosedBg;

	public static bool isShowBuyView;

	public EventDelegate imageToggleEvent;

	public EventDelegate sceneToggleEvent;

	public EventDelegate coverToggleEvent;

	public static bool isShow;
}
