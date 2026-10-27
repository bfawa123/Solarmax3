using System;
using Solarmax;
using UnityEngine;

public class RewardTipsWindow : BaseWindow
{
	public override bool Init()
	{
		base.RegisterEvent(EventId.OnShowRewardTipsWindow);
		return base.Init();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnShowRewardTipsWindow)
		{
			this.windowType = (RewardTipsWindow.ViewType)args[0];
			if (this.windowType == RewardTipsWindow.ViewType.AD)
			{
				this.adView.SetActive(true);
				UILabel component = this.adView.transform.Find("tips").GetComponent<UILabel>();
				if (component != null)
				{
					component.text = LanguageDataProvider.GetValue(2254);
				}
			}
			else if (this.windowType == RewardTipsWindow.ViewType.Reward2)
			{
				this.rewardTips.SetActive(true);
				RewardTipsModel rewardTipsModel = (RewardTipsModel)args[1];
				UILabel component2 = this.rewardTips.transform.Find("tips").GetComponent<UILabel>();
				if (component2 != null)
				{
					component2.text = string.Format(LanguageDataProvider.GetValue(2276), rewardTipsModel.count);
				}
			}
			else if (this.windowType == RewardTipsWindow.ViewType.Reward)
			{
				this.rewardTipsView.SetActive(true);
				RewardTipsModel rewardTipsModel2 = (RewardTipsModel)args[1];
				this.rewardIcon.spriteName = rewardTipsModel2.reward;
				this.rewardCount.text = string.Format("X{0}", rewardTipsModel2.count);
				if (UpgradeUtil.GetGameConfig().Oversea)
				{
					this.monthcardBtn.SetActive(rewardTipsModel2.showMonthCardBtn);
				}
				else
				{
					this.monthcardBtn.SetActive(false);
				}
				this.btnTable.Reposition();
			}
			else if (this.windowType == RewardTipsWindow.ViewType.Integral)
			{
				this.adView.SetActive(true);
				UILabel component3 = this.adView.transform.Find("tips").GetComponent<UILabel>();
				if (component3 != null)
				{
					component3.text = LanguageDataProvider.GetValue(2257);
				}
			}
			else if (this.windowType == RewardTipsWindow.ViewType.Pvp)
			{
				this.adView.SetActive(true);
				int num = (int)args[1];
				UILabel component4 = this.adView.transform.Find("tips").GetComponent<UILabel>();
				if (component4 != null)
				{
					string text = string.Format(LanguageDataProvider.GetValue(2258), num);
					component4.text = text;
				}
			}
			else if (this.windowType == RewardTipsWindow.ViewType.RewardItem)
			{
				this.CompositeView.SetActive(true);
				RewardTipsModel rewardTipsModel3 = (RewardTipsModel)args[1];
				this.HandleCompositeReward(rewardTipsModel3.itemID, rewardTipsModel3.count);
			}
			else if (this.windowType == RewardTipsWindow.ViewType.RewardTask)
			{
				this.CompositeView.SetActive(true);
				RewardTipsModel rewardTipsModel4 = (RewardTipsModel)args[1];
				TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(rewardTipsModel4.itemID.ToString());
				if (task != null)
				{
					this.HandleQuestReward(task.rewardType, task.itemid, task.rewardValue);
				}
			}
		}
	}

	public override void OnShow()
	{
		base.OnShow();
		this.adView.SetActive(false);
		this.rewardTipsView.SetActive(false);
	}

	public override void OnHide()
	{
	}

	public void OnBnClose()
	{
		Solarmax.Singleton<UISystem>.Instance.HideWindow("RewardTipsWindow");
		if (this.windowType == RewardTipsWindow.ViewType.AD || this.windowType == RewardTipsWindow.ViewType.Integral || this.windowType == RewardTipsWindow.ViewType.Pvp || this.windowType == RewardTipsWindow.ViewType.Reward2)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnDoubleAdCanceled, null);
		}
	}

	public void OnBnAdClicked()
	{
		Solarmax.Singleton<UISystem>.Instance.HideWindow("RewardTipsWindow");
		if (this.windowType == RewardTipsWindow.ViewType.AD || this.windowType == RewardTipsWindow.ViewType.Integral || this.windowType == RewardTipsWindow.ViewType.Pvp || this.windowType == RewardTipsWindow.ViewType.Reward2)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnDoubleAdClicked, null);
		}
	}

	public void OnBnMonthCardClicked()
	{
		Solarmax.Singleton<UISystem>.Instance.HideWindow("RewardTipsWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("StoreWindow", EventId.OnShowMonthCardEffect, null));
	}

	private void HandleCompositeReward(int typeid, int count)
	{
		ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(typeid);
		if (data != null)
		{
			GameObject gameObject = this.CompositeView.transform.Find("icon").gameObject;
			UISprite component = gameObject.GetComponent<UISprite>();
			GameObject gameObject2 = gameObject.transform.Find("FragMark").gameObject;
			UILabel component2 = gameObject.transform.Find("num").GetComponent<UILabel>();
			if (data.resultType == ProductType.Gold)
			{
				gameObject2.SetActive(false);
				component.spriteName = "icon_currency";
				count = Solarmax.Singleton<TaskModel>.Get().claimReward.count;
				component2.text = string.Format("{0}", data.Coprice * count);
			}
			else
			{
				gameObject2.SetActive(false);
				component.spriteName = data.icon;
				component2.text = string.Empty;
			}
		}
	}

	private void HandleQuestReward(Solarmax.RewardType type, int typeid, int count)
	{
		GameObject gameObject = this.CompositeView.transform.Find("icon").gameObject;
		UISprite component = gameObject.GetComponent<UISprite>();
		GameObject gameObject2 = gameObject.transform.Find("FragMark").gameObject;
		UILabel component2 = gameObject.transform.Find("num").GetComponent<UILabel>();
		if (type == Solarmax.RewardType.Gold)
		{
			gameObject2.SetActive(false);
			component.spriteName = "icon_currency";
			count = Solarmax.Singleton<TaskModel>.Get().claimReward.count;
			component2.text = string.Format("{0}", count);
		}
		else
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(typeid);
			if (data != null)
			{
				gameObject2.SetActive(true);
				component.spriteName = data.icon;
				component2.text = string.Format("{0}", count);
			}
		}
	}

	public GameObject CompositeView;

	public GameObject rewardTipsView;

	public GameObject rewardTips;

	public GameObject adView;

	public UISprite rewardIcon;

	public UILabel rewardCount;

	public UITable btnTable;

	public GameObject monthcardBtn;

	private RewardTipsWindow.ViewType windowType;

	public enum ViewType
	{
		AD,
		Reward,
		Integral,
		Pvp,
		Reward2,
		RewardItem,
		RewardTask
	}
}
