using System;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class LotteryRewardWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnLotteryResultDone);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		int takeAwardTime = Solarmax.Singleton<LuckModel>.Get().GetTakeAwardTime();
		if (takeAwardTime > 0)
		{
			this.extawrardCntLabel.text = LanguageDataProvider.Format(2318, new object[]
			{
				takeAwardTime
			});
		}
		else
		{
			this.extawrardCntLabel.text = string.Empty;
		}
		AwardItem awardItem;
		if (Solarmax.Singleton<LuckModel>.Get().rewardType == LuckModel.RewardType.Lottery)
		{
			int retId = (int)Solarmax.Singleton<LuckModel>.Get().lotteryInfo.retlId;
			awardItem = Solarmax.Singleton<LuckModel>.Get().GetLotteryWheelItem(retId);
		}
		else
		{
			int awardId = Solarmax.Singleton<LuckModel>.Get().lotteryAward.awardId;
			awardItem = Solarmax.Singleton<LuckModel>.Get().GetExtraItem(awardId);
		}
		this.numLabel.text = awardItem.itemNum.ToString();
		if (awardItem.type == 1)
		{
			this.iconSprite.spriteName = "icon_currency";
		}
		else
		{
			this.iconSprite.spriteName = awardItem.itemIcon;
			if (awardItem.needCount > 1)
			{
				this.fragMarkObj.SetActive(true);
			}
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnLotteryResultDone)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
		}
	}

	private void RefreshUI()
	{
	}

	public void OnAdAndReLottery()
	{
		int num = 7 - Solarmax.Singleton<LuckModel>.Get().lotteryNotes.todayAdLotteryNum;
		if (num <= 0)
		{
			Tips.Make(LanguageDataProvider.GetValue(2337));
			return;
		}
		AdManager.ShowAd(AdManager.ShowAdType.LotteryAd, delegate(object[] param)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogLotteryLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.LotteryFromServer(2, Solarmax.Singleton<LuckModel>.Get().lotteryNotes.currLotteryId);
		});
	}

	public void OnOK()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	public UILabel extawrardCntLabel;

	public UILabel numLabel;

	public GameObject fragMarkObj;

	public UISprite iconSprite;

	public UITexture iconTexture;
}
