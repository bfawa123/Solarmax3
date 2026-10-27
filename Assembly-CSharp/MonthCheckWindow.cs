using System;
using System.Collections.Generic;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class MonthCheckWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMonthCheckSuccess);
		base.RegisterEvent(EventId.OnDoubleAdClicked);
		base.RegisterEvent(EventId.OnDoubleAdCanceled);
		base.RegisterEvent(EventId.UpdateMothCard);
		return true;
	}

	private void Awake()
	{
	}

	public void Update()
	{
		this.TICK_NUM++;
		if (this.TICK_NUM <= 30)
		{
			return;
		}
		this.TICK_NUM = 0;
		if (this.IsShowNextCheckTime)
		{
			long checkedTime = Solarmax.Singleton<MonthCheckModel>.Get().GetCheckedTime();
			DateTime d = new DateTime(1970, 1, 1);
			TimeSpan t = d.AddSeconds((double)checkedTime) - d;
			long num = (long)this.MAX_DAY_SECOND - checkedTime % (long)this.MAX_DAY_SECOND;
			TimeSpan t2 = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
			long num2 = (long)((int)(t2 - t).TotalSeconds);
			num -= num2;
			if (num < 0L)
			{
				this.IsShowNextCheckTime = false;
				this.GrandDely.text = string.Empty;
				Solarmax.Singleton<NetSystem>.Get().helper.RequstMonthCheck();
				return;
			}
			int num3 = (int)(num / 3600L);
			int minutes = (int)((num - (long)(num3 * 3600)) / 60L);
			int seconds = (int)(num % 60L);
			TimeSpan timeSpan = new TimeSpan(num3, minutes, seconds);
			string text = string.Format(LanguageDataProvider.GetValue(2261), timeSpan.ToString());
			this.GrandDely.text = text;
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		this.IsShowNextCheckTime = false;
		this.GrandDely.text = string.Empty;
		this.rewardTemplate.SetActive(false);
		this.checkModel = Solarmax.Singleton<MonthCheckModel>.Get();
		this.RefreshUI();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnMonthCheckSuccess)
		{
			if (this.sendTemplate != null)
			{
				this.sendTemplate.RefreshUI(this.sendTemplate.GetRewardConfig(), CheckType.Checked);
				if (this.sendTemplate == this.choosedTemplate)
				{
					this.EnableCheckBtn(this.sendTemplate.checkType);
				}
				this.OnTemplateClicked(this.sendTemplate.gameObject);
				this.CheckSuceessCallBack();
				MonthCheckTemplate monthCheckTemplate = null;
				this.id2ItemTemplate.TryGetValue(this.sendTemplate.GetRewardConfig().id + 1, out monthCheckTemplate);
				if (monthCheckTemplate != null)
				{
					DayRewardConfig rewardConfig = monthCheckTemplate.GetRewardConfig();
					if (rewardConfig.id == this.checkModel.checkedId + 1 && rewardConfig.id <= this.checkModel.couldRepairId)
					{
						monthCheckTemplate.RefreshUI(rewardConfig, CheckType.RepairCheck);
					}
				}
			}
			int num = (int)args[0];
			int num2 = (int)args[1];
			int num3 = (int)args[2];
			Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
			{
				RewardTipsWindow.ViewType.Reward,
				new RewardTipsModel(this.sendTemplate.GetRewardConfig().misc * num3, global::RewardType.Money, !this.checkModel.isDouble, 0)
			}));
		}
		else if (eventId == EventId.OnDoubleAdClicked)
		{
			AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogCheckLookAds();
				Solarmax.Singleton<NetSystem>.Get().helper.MonthCheck(this.checkModel.currentMonth, this.checkModel.nextCheckId, 2 * ((!this.checkModel.isDouble) ? 1 : 2), false);
			});
		}
		else if (eventId == EventId.OnDoubleAdCanceled)
		{
			Solarmax.Singleton<NetSystem>.Get().helper.MonthCheck(this.checkModel.currentMonth, this.checkModel.nextCheckId, (!this.checkModel.isDouble) ? 1 : 2, false);
		}
		else if (eventId == EventId.UpdateMothCard)
		{
			this.table.transform.DestroyChildren();
			this.RefreshUI();
		}
	}

	public override void OnHide()
	{
		this.table.transform.DestroyChildren();
	}

	private void RefreshUI()
	{
		this.RefreshRightBar(null);
		if (!Solarmax.Singleton<MonthCheckConfgiProvider>.Get().dataList.ContainsKey(this.checkModel.currentMonth))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[{0}]签到月份为空{0}", new object[]
			{
				base.GetName(),
				this.checkModel.currentMonth
			});
			return;
		}
		MonthCheckConfig monthCheckConfig = Solarmax.Singleton<MonthCheckConfgiProvider>.Get().dataList[this.checkModel.currentMonth];
		int count = monthCheckConfig.dayRewards.Values.Count;
		int num = 0;
		int num2 = 0;
		foreach (DayRewardConfig dayRewardConfig in monthCheckConfig.dayRewards.Values)
		{
			GameObject gameObject = this.table.gameObject.AddChild(this.rewardTemplate);
			gameObject.SetActive(true);
			UIEventListener.Get(gameObject).onClick = new UIEventListener.VoidDelegate(this.OnTemplateClicked);
			gameObject.GetComponent<UIEventListener>().autoZoom = false;
			MonthCheckTemplate component = gameObject.GetComponent<MonthCheckTemplate>();
			this.id2ItemTemplate[dayRewardConfig.id] = component;
			if (dayRewardConfig.id <= this.checkModel.checkedId)
			{
				num2++;
				component.RefreshUI(dayRewardConfig, CheckType.Checked);
				if (dayRewardConfig.id == this.checkModel.checkedId)
				{
					this.OnTemplateClicked(gameObject);
				}
			}
			else if (dayRewardConfig.id == this.checkModel.nextCheckId && this.checkModel.needCheck)
			{
				num = dayRewardConfig.id;
				component.RefreshUI(dayRewardConfig, CheckType.WaittingCheck);
				this.OnTemplateClicked(gameObject);
			}
			else if (dayRewardConfig.id == this.checkModel.checkedId + 1 && dayRewardConfig.id <= this.checkModel.couldRepairId)
			{
				component.RefreshUI(dayRewardConfig, CheckType.RepairCheck);
			}
			else if (dayRewardConfig.id <= this.checkModel.todayCheckId)
			{
				component.RefreshUI(dayRewardConfig, CheckType.LossCheck);
			}
			else
			{
				component.RefreshUI(dayRewardConfig, CheckType.UnReachCheck);
			}
		}
		if (num2 > 0)
		{
			string text = string.Format(LanguageDataProvider.GetValue(2260), num2);
			this.GrandDays.text = text;
		}
		else
		{
			this.GrandDays.text = string.Empty;
		}
		if (num <= 0)
		{
			this.IsShowNextCheckTime = true;
		}
		else
		{
			this.IsShowNextCheckTime = false;
		}
		this.table.Reposition();
		this.scrollView.ResetPosition();
		this.Scroll(count, num);
		string text2 = string.Format(LanguageDataProvider.GetValue(2284), this.checkModel.repair_check_num);
		this.repairTimeLabel.text = text2;
		DateTime d = new DateTime(1970, 1, 1);
		long num3 = (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds;
		long num4 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num3) / 86400L;
		long num5 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num3) % 86400L;
		if (num5 > 0L)
		{
			num4 += 1L;
		}
		if (num4 < 0L)
		{
			num4 = 0L;
		}
		if (UpgradeUtil.GetGameConfig().EnablePay)
		{
			this.monthCardTip.SetActive(true);
			this.monthCardRestLabel.text = string.Format(LanguageDataProvider.GetValue(2283), num4);
		}
		else
		{
			this.monthCardTip.SetActive(false);
		}
	}

	private void CheckSuceessCallBack()
	{
		MonthCheckConfig monthCheckConfig = Solarmax.Singleton<MonthCheckConfgiProvider>.Get().dataList[this.checkModel.currentMonth];
		int count = monthCheckConfig.dayRewards.Values.Count;
		int num = 0;
		int num2 = 0;
		foreach (DayRewardConfig dayRewardConfig in monthCheckConfig.dayRewards.Values)
		{
			if (dayRewardConfig.id <= this.checkModel.checkedId)
			{
				num2++;
			}
			else if (dayRewardConfig.id == this.checkModel.nextCheckId && this.checkModel.needCheck)
			{
				num = dayRewardConfig.id;
			}
		}
		if (num2 > 0)
		{
			string text = string.Format(LanguageDataProvider.GetValue(2260), num2);
			this.GrandDays.text = text;
		}
		else
		{
			this.GrandDays.text = string.Empty;
		}
		if (num <= 0)
		{
			this.IsShowNextCheckTime = true;
		}
		else
		{
			this.IsShowNextCheckTime = false;
		}
		string text2 = string.Format(LanguageDataProvider.GetValue(2284), this.checkModel.repair_check_num);
		this.repairTimeLabel.text = text2;
	}

	private void Scroll(int sum, int index)
	{
		int num = sum / 5;
		int num2 = index / 5;
		float num3 = (float)num - 3f;
		float value = (float)(num2 - 1) * 1f / num3;
		this.scrollView.verticalScrollBar.value = value;
		this.scrollView.UpdatePosition();
	}

	private void RefreshRightBar(DayRewardConfig config)
	{
		if (config == null)
		{
			this.EnableCheckBtn(CheckType.Unkown);
			this.rewardIcon.spriteName = string.Empty;
			this.rewardName.text = string.Empty;
			this.rewardDesc.text = string.Empty;
			Solarmax.Singleton<LoggerSystem>.Instance.Error("[{0}]reward config is null", new object[]
			{
				base.GetName()
			});
			return;
		}
		int rewardType = config.rewardType;
		if (rewardType != 1)
		{
			this.rewardIcon.spriteName = string.Empty;
			this.rewardName.text = string.Empty;
		}
		else
		{
			this.rewardIcon.spriteName = config.icon;
			this.rewardName.text = string.Format("{0} x{1}", LanguageDataProvider.GetValue(2032), config.misc);
		}
		this.rewardDesc.text = LanguageDataProvider.GetValue(config.desc);
	}

	private void EnableCheckBtn(CheckType checkType)
	{
		this.checkStateLabel.gameObject.SetActive(false);
		this.repairCheckBtn.gameObject.SetActive(false);
		this.checkBtn.gameObject.SetActive(false);
		this.adCheckBtn.gameObject.SetActive(false);
		if (checkType == CheckType.WaittingCheck)
		{
			this.checkBtn.gameObject.SetActive(true);
			this.adCheckBtn.gameObject.SetActive(true);
		}
		else if (checkType == CheckType.Checked)
		{
			this.checkStateLabel.gameObject.SetActive(true);
			this.checkStateLabel.text = LanguageDataProvider.GetValue(2280);
		}
		else if (checkType == CheckType.RepairCheck)
		{
			this.repairCheckBtn.gameObject.SetActive(true);
		}
		else if (checkType == CheckType.LossCheck)
		{
			this.checkStateLabel.gameObject.SetActive(true);
			this.checkStateLabel.text = LanguageDataProvider.GetValue(2279);
		}
		else if (checkType == CheckType.UnReachCheck)
		{
			this.checkStateLabel.gameObject.SetActive(true);
			this.checkStateLabel.text = LanguageDataProvider.GetValue(2281);
		}
	}

	public void OnBnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("MonthCheckWindow");
	}

	public void OnTemplateClicked(GameObject go)
	{
		go.GetComponent<UIToggle>().value = true;
		this.choosedTemplate = go.GetComponent<MonthCheckTemplate>();
		this.EnableCheckBtn(this.choosedTemplate.checkType);
		DayRewardConfig rewardConfig = this.choosedTemplate.GetRewardConfig();
		this.RefreshRightBar(rewardConfig);
	}

	public void OnBnAdClick()
	{
		this.sendTemplate = this.choosedTemplate;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogCheckLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.MonthCheck(this.checkModel.currentMonth, this.checkModel.nextCheckId, 2 * ((!this.checkModel.isDouble) ? 1 : 2), false);
		});
	}

	public void OnRechargeBtnClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.MonthRecharge))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.MonthRecharge));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("StoreWindow", EventId.OnShowMonthCardEffect, null));
	}

	public void OnBnCheckClick()
	{
		this.sendTemplate = this.choosedTemplate;
		Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
		{
			RewardTipsWindow.ViewType.AD
		}));
	}

	public void onBtnRepairClick()
	{
		this.sendTemplate = this.choosedTemplate;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogRepairCheckLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.MonthCheck(this.checkModel.currentMonth, this.choosedTemplate.GetRewardConfig().id, (!this.checkModel.isDouble) ? 1 : 2, true);
		});
	}

	[HideInInspector]
	public static string MONEY_ICON = "icon_currency";

	public UISprite rewardIcon;

	public UILabel rewardName;

	public UILabel rewardDesc;

	public GameObject rewardTemplate;

	public UIScrollView scrollView;

	public UITable table;

	public UIButton adCheckBtn;

	public UIButton checkBtn;

	public UIButton repairCheckBtn;

	public UILabel GrandDays;

	public UILabel GrandDely;

	public UILabel repairTimeLabel;

	private MonthCheckModel checkModel;

	public UILabel checkStateLabel;

	private MonthCheckTemplate choosedTemplate;

	private MonthCheckTemplate sendTemplate;

	public GameObject monthCardTip;

	public UILabel monthCardRestLabel;

	private bool IsShowNextCheckTime;

	private Dictionary<int, MonthCheckTemplate> id2ItemTemplate = new Dictionary<int, MonthCheckTemplate>();

	private int TICK_NUM;

	private int MAX_DAY_SECOND = 86400;
}
