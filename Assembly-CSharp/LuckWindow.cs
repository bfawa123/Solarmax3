using System;
using System.Collections.Generic;
using DG.Tweening;
using MiGameSDK;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LuckWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnLotteryExtraAwardAccepted);
		base.RegisterEvent(EventId.OnLotteryNotesDone);
		base.RegisterEvent(EventId.OnLotteryResultDone);
		base.RegisterEvent(EventId.UpdateMoney);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		Solarmax.Singleton<NetSystem>.Get().helper.LotteryNotesFromServer();
		this.moneyLabel.text = playerData.money.ToString();
		this.RefreshUI();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		switch (eventId)
		{
		case EventId.OnLotteryResultDone:
			if (Solarmax.Singleton<LuckModel>.Get().lotteryInfo.errcode == ErrCode.EC_Ok)
			{
				this.OnLotteryResultDone();
			}
			this.lotteryRequesting = false;
			Solarmax.Singleton<NetSystem>.Get().helper.LotteryNotesFromServer();
			break;
		case EventId.OnLotteryExtraAwardAccepted:
			Solarmax.Singleton<NetSystem>.Get().helper.LotteryNotesFromServer();
			if (Solarmax.Singleton<LuckModel>.Get().lotteryAward.errcode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<LuckModel>.Get().rewardType = LuckModel.RewardType.ExtraAward;
				Solarmax.Singleton<UISystem>.Get().ShowWindow("LotteryRewardWindow");
			}
			break;
		case EventId.OnLotteryNotesDone:
			this.RefreshUI();
			break;
		default:
			if (eventId == EventId.UpdateMoney)
			{
				this.moneyLabel.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			}
			break;
		}
	}

	private void RefreshUI()
	{
		SCLotterNotes lotteryNotes = Solarmax.Singleton<LuckModel>.Get().lotteryNotes;
		if (lotteryNotes == null)
		{
			return;
		}
		Dictionary<int, LotteryRotateConfig> dataDict = Solarmax.Singleton<LotteryRotateProvider>.Get().dataDict;
		LotteryRotateConfig lotteryRotateConfig = dataDict[lotteryNotes.currLotteryId];
		Dictionary<int, LotteryConfig> dataDict2 = Solarmax.Singleton<LotteryConfigProvider>.Get().dataDict;
		List<int> lottery_ids = lotteryRotateConfig.lottery_ids;
		for (int i = 0; i < 8; i++)
		{
			LotteryItemBh component = this.wheelItems[i].GetComponent<LotteryItemBh>();
			component.SetWheelId(lottery_ids[i]);
		}
		Dictionary<int, LotteryAddConfig> dataDict3 = Solarmax.Singleton<LotteryAddProvider>.Get().dataDict;
		List<int> lottery_add_ids = lotteryRotateConfig.lottery_add_ids;
		for (int j = 0; j < 5; j++)
		{
			LotteryAwardItemBh component2 = this.extraItems[j].GetComponent<LotteryAwardItemBh>();
			component2.SetExtraId(lottery_add_ids[j]);
		}
		if (Solarmax.Singleton<LuckModel>.Get().lotteryNotes != null)
		{
			string arg = "[0fd3fd]" + lotteryNotes.totalTimes + "[-]";
			this.lotteryCntLabel.text = string.Format(LanguageDataProvider.GetValue(2315), arg);
			this.freeRestTimeLabel.text = Solarmax.Singleton<LuckModel>.Get().GetFreeRestTime();
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			for (int k = 0; k < lotteryNotes.lotteryBoxInfo.Count; k++)
			{
				LotteryBoxInfo lotteryBoxInfo = lotteryNotes.lotteryBoxInfo[k];
				dictionary[lotteryBoxInfo.awardId] = lotteryBoxInfo.get;
			}
			for (int l = 0; l < 5; l++)
			{
				bool hasTook = false;
				bool canTake = false;
				LotteryAwardItemBh component3 = this.extraItems[l].GetComponent<LotteryAwardItemBh>();
				if (component3.awardInfo.count <= lotteryNotes.totalTimes)
				{
					if (dictionary.ContainsKey(component3.awardInfo.id))
					{
						hasTook = true;
					}
					else
					{
						canTake = true;
					}
				}
				component3.SetCanTake(canTake);
				component3.SetHasTook(hasTook);
			}
			int num = 7 - Solarmax.Singleton<LuckModel>.Get().lotteryNotes.todayAdLotteryNum;
			this.adLotteryBtn.GetComponentInChildren<UILabel>().text = LanguageDataProvider.Format(2323, new object[]
			{
				num
			});
		}
		this.UpdateContentState();
	}

	public void ToDraw()
	{
		if (Solarmax.Singleton<LuckModel>.Get().lotteryNotes == null)
		{
			return;
		}
		if (this.tweener != null && this.tweener.IsPlaying())
		{
			return;
		}
		if (this.lotteryRequesting)
		{
			return;
		}
		this.lotteryRequesting = true;
		Solarmax.Singleton<NetSystem>.Get().helper.LotteryFromServer(1, Solarmax.Singleton<LuckModel>.Get().lotteryNotes.currLotteryId);
	}

	public void CostLottery()
	{
		if (Solarmax.Singleton<LuckModel>.Get().lotteryNotes == null)
		{
			return;
		}
		if (this.tweener != null && this.tweener.IsPlaying())
		{
			return;
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < 50)
		{
			Tips.Make(LanguageDataProvider.GetValue(1102));
			return;
		}
		if (this.lotteryRequesting)
		{
			return;
		}
		this.lotteryRequesting = true;
		Solarmax.Singleton<NetSystem>.Get().helper.LotteryFromServer(3, Solarmax.Singleton<LuckModel>.Get().lotteryNotes.currLotteryId);
	}

	public void AdLottery()
	{
		if (Solarmax.Singleton<LuckModel>.Get().lotteryNotes == null)
		{
			return;
		}
		if (this.tweener != null && this.tweener.IsPlaying())
		{
			return;
		}
		int num = 7 - Solarmax.Singleton<LuckModel>.Get().lotteryNotes.todayAdLotteryNum;
		if (num <= 0)
		{
			Tips.Make(LanguageDataProvider.GetValue(2337));
			return;
		}
		if (this.lotteryRequesting)
		{
			return;
		}
		AdManager.ShowAd(AdManager.ShowAdType.LotteryAd, delegate(object[] param)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogLotteryLookAds();
			this.lotteryRequesting = true;
			Solarmax.Singleton<NetSystem>.Get().helper.LotteryFromServer(2, Solarmax.Singleton<LuckModel>.Get().lotteryNotes.currLotteryId);
		});
	}

	public void OnLotteryResultDone()
	{
		int num = (int)Solarmax.Singleton<LuckModel>.Get().lotteryInfo.retlId;
		int num2 = 0;
		for (int i = 1; i <= 8; i++)
		{
			if (this.wheelItems[i - 1].GetComponent<LotteryItemBh>().wheelID == num)
			{
				num2 = i;
			}
		}
		float z = this.wheelObj.transform.eulerAngles.z;
		int num3 = 45 * (num2 - 1);
		float z2 = (float)num3 - z - 1800f;
		this.tweener = this.wheelObj.transform.DOLocalRotate(new Vector3(0f, 0f, z2), 5f, RotateMode.WorldAxisAdd);
		this.tweener.OnComplete(new TweenCallback(this.OnWheelRotateDone));
		Solarmax.Singleton<LuckModel>.Get().tweener = this.tweener;
	}

	public void OnWheelRotateDone()
	{
		Debug.Log("OnWheelRotateDone");
		Solarmax.Singleton<LuckModel>.Get().rewardType = LuckModel.RewardType.Lottery;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LotteryRewardWindow");
	}

	public void OnCloseClicked()
	{
		if (this.tweener != null && this.tweener.IsPlaying())
		{
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow(base.GetName());
	}

	public void Update()
	{
		this.TICK_NUM++;
		if (this.TICK_NUM <= 30)
		{
			return;
		}
		this.TICK_NUM = 0;
		this.UpdateContentState();
	}

	private void UpdateContentState()
	{
		this.freeRestTimeLabel.text = Solarmax.Singleton<LuckModel>.Get().GetFreeRestTime();
		if (Solarmax.Singleton<LuckModel>.Get().canFreeLottery)
		{
			this.freeLotteryBtn.SetActive(true);
			this.freeCountDownObj.SetActive(false);
		}
		else
		{
			this.freeLotteryBtn.SetActive(false);
			this.freeCountDownObj.SetActive(true);
		}
		this.retSetLabel.text = Solarmax.Singleton<LuckModel>.Get().GetUpdateRestTime();
	}

	public GameObject wheelObj;

	public List<GameObject> wheelItems;

	public GameObject freeLotteryBtn;

	public GameObject freeCountDownObj;

	public GameObject adLotteryBtn;

	public GameObject costLotteryBtn;

	public List<GameObject> extraItems;

	public UILabel freeRestTimeLabel;

	public UILabel lotteryCntLabel;

	public UILabel retSetLabel;

	public UILabel moneyLabel;

	private Tweener tweener;

	private bool lotteryRequesting;

	private int TICK_NUM;
}
