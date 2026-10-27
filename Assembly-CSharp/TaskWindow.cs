using System;
using System.Collections.Generic;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class TaskWindow : BaseWindow
{
	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnTaskOkEvent)
		{
			List<string> list = (List<string>)args[0];
			List<string> list2 = (List<string>)args[1];
			if (list.Count > 0)
			{
				foreach (string key in list)
				{
					Solarmax.Singleton<TaskConfigProvider>.Get().dataList[key].status = TaskStatus.Received;
				}
			}
			LevelTaskView component = this.levelView.GetComponent<LevelTaskView>();
			DailyTaskView component2 = this.dailyView.GetComponent<DailyTaskView>();
			AchievementTaskView component3 = this.achievementView.GetComponent<AchievementTaskView>();
			if (component.gameObject.activeSelf)
			{
				component.EnsureDestroy();
				component.EnsureInit(Solarmax.Singleton<TaskConfigProvider>.Get().GetLevelData());
			}
			if (component2.gameObject.activeSelf && Solarmax.Singleton<TaskModel>.Get().claimTask != null)
			{
				int id = int.Parse(Solarmax.Singleton<TaskModel>.Get().claimTask.id);
				Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
				{
					RewardTipsWindow.ViewType.RewardTask,
					new RewardTipsModel(0, global::RewardType.Prop, false, id)
				}));
				component2.EnsureDestroy();
				component2.EnsureInit();
			}
			if (component3.gameObject.activeSelf)
			{
				component3.EnsureDestroy();
				component3.EnsureInit();
				Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalTask();
			}
			this.ShowRedDot();
		}
		else if (eventId == EventId.UpdateMoney)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("Gold");
			this.moneyChange = Solarmax.Singleton<LocalPlayer>.Get().playerData.money - int.Parse(this.money.text);
			this.moneyChangeSpeed = ((this.moneyChange / 40 <= 0) ? 1 : (this.moneyChange / 40));
		}
		else if (eventId == EventId.OnDoubleAdClicked)
		{
			AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
			{
				if (Solarmax.Singleton<TaskModel>.Get().claimTask != null)
				{
					MonoSingleton<FlurryAnalytis>.Instance.LogTaskLookAds();
					Solarmax.Singleton<TaskModel>.Get().ClaimReward(Solarmax.Singleton<TaskModel>.Get().claimTask.id, null, 2);
				}
				else
				{
					Solarmax.Singleton<LoggerSystem>.Instance.Error("双倍领取任务失败，task为空", new object[0]);
				}
			});
		}
		else if (eventId == EventId.OnDoubleAdCanceled)
		{
			DailyTaskView component4 = this.dailyView.GetComponent<DailyTaskView>();
			if (component4.gameObject.activeSelf)
			{
				Solarmax.Singleton<TaskModel>.Get().ClaimReward(Solarmax.Singleton<TaskModel>.Get().claimTask.id, delegate(bool success)
				{
				}, 1);
			}
		}
		else if (eventId == EventId.OnRefrushTaskDegree)
		{
			DailyTaskView component5 = this.dailyView.GetComponent<DailyTaskView>();
			if (component5.gameObject.activeSelf)
			{
				component5.RefrushDegreeStats();
			}
		}
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnChoosedAvatarEvent);
		base.RegisterEvent(EventId.OnTaskOkEvent);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnDoubleAdClicked);
		base.RegisterEvent(EventId.OnDoubleAdCanceled);
		base.RegisterEvent(EventId.OnRefrushTaskDegree);
		return true;
	}

	public void Awake()
	{
	}

	public void Update()
	{
		if (this.moneyChange > 0)
		{
			this.money.text = (int.Parse(this.money.text) + this.moneyChangeSpeed).ToString();
			this.moneyChange -= this.moneyChangeSpeed;
			if (this.moneyChange <= 0)
			{
				this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
		engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Combine(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
		this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		this.NetStatus((NetworkReachability)Solarmax.Singleton<EngineSystem>.Get().GetNetworkRechability());
		this.levelTask.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnLevelToggleChanged)));
		this.dailyTask.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnDailyToggleChanged)));
		this.achieveTask.onChange.Add(new EventDelegate(new EventDelegate.Callback(this.OnAchieveToggleChanged)));
		this.model = Solarmax.Singleton<TaskModel>.Get();
		this.ShowRedDot();
	}

	private void EnsureInit()
	{
	}

	private void OnLevelToggleChanged()
	{
		this.levelView.SetActive(this.levelTask.value);
		LevelTaskView component = this.levelView.GetComponent<LevelTaskView>();
		DailyTaskView component2 = this.dailyView.GetComponent<DailyTaskView>();
		if (this.levelTask.value)
		{
			component2.EnsureDestroy();
			component.EnsureInit(Solarmax.Singleton<TaskConfigProvider>.Get().GetLevelData());
		}
		else if (Solarmax.Singleton<LevelDataHandler>.Get().HaveLevelTaskCompleted())
		{
			this.reddots[0].SetActive(false);
		}
	}

	private void OnDailyToggleChanged()
	{
		this.dailyView.SetActive(this.dailyTask.value);
		LevelTaskView component = this.levelView.GetComponent<LevelTaskView>();
		DailyTaskView component2 = this.dailyView.GetComponent<DailyTaskView>();
		AchievementTaskView component3 = this.achievementView.GetComponent<AchievementTaskView>();
		if (this.dailyTask.value)
		{
			component3.EnsureDestroy();
			component.EnsureDestroy();
			component2.EnsureInit();
		}
		else if (Solarmax.Singleton<TaskConfigProvider>.Get().HaveDailyTaskCompleted())
		{
		}
	}

	private void OnAchieveToggleChanged()
	{
		this.achievementView.SetActive(this.achieveTask.value);
		LevelTaskView component = this.levelView.GetComponent<LevelTaskView>();
		DailyTaskView component2 = this.dailyView.GetComponent<DailyTaskView>();
		AchievementTaskView component3 = this.achievementView.GetComponent<AchievementTaskView>();
		if (this.achieveTask.value)
		{
			component.EnsureDestroy();
			component2.EnsureDestroy();
			component3.EnsureInit();
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
		EngineSystem engineSystem = Solarmax.Singleton<EngineSystem>.Get();
		engineSystem.onNetStatusChanged = (EngineSystem.OnNetStatusChanged)Delegate.Remove(engineSystem.onNetStatusChanged, new EngineSystem.OnNetStatusChanged(this.NetStatus));
		LevelTaskView component = this.levelView.GetComponent<LevelTaskView>();
		DailyTaskView component2 = this.dailyView.GetComponent<DailyTaskView>();
		component.EnsureDestroy();
		component2.EnsureDestroy();
	}

	public void OnCloseClicked()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	private void ShowRedDot()
	{
		if (Solarmax.Singleton<TaskConfigProvider>.Get().HaveDailyTaskCompleted())
		{
			this.reddots[1].SetActive(false);
		}
		else
		{
			this.reddots[1].SetActive(false);
		}
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	private const string WIFI_ICON = "icon_net_wifi_03";

	private const string MOBILE_ICON = "icon_net_mobile_03";

	private const string NOT_REACHABLE_ICON = "icon_net_offline";

	public UIToggle levelTask;

	public UIToggle dailyTask;

	public UIToggle achieveTask;

	public UISprite netIcon;

	public GameObject levelView;

	public GameObject dailyView;

	public GameObject achievementView;

	public UILabel money;

	public GameObject[] reddots;

	private NetworkReachability netStatus;

	private TaskModel model;

	public int moneyChange;

	public int moneyChangeSpeed;
}
