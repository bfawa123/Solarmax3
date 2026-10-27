using System;
using System.Collections.Generic;
using DG.Tweening;
using NetMessage;
using Solarmax;

public class LuckModel : Solarmax.Singleton<LuckModel>
{
	public LuckModel()
	{
		Dictionary<int, LotteryAddConfig> dataDict = Solarmax.Singleton<LotteryAddProvider>.Get().dataDict;
		foreach (int key in dataDict.Keys)
		{
			this.awardCntList.Add(dataDict[key].count);
		}
	}

	public string GetFreeRestTime()
	{
		if (this.lotteryNotes == null)
		{
			return string.Empty;
		}
		long num = this.lotteryNotes.lastTime + 86400L;
		long timeStamp = Solarmax.Singleton<TimeSystem>.Get().GetTimeStamp();
		if (timeStamp >= num)
		{
			this.canFreeLottery = true;
			return string.Empty;
		}
		this.canFreeLottery = false;
		int num2 = (int)(num - timeStamp);
		if (num2 >= 86400)
		{
			num2 = 86399;
		}
		return TimeSpan.FromSeconds((double)num2).ToString();
	}

	public string GetUpdateRestTime()
	{
		long num = 1574640000L;
		long timeStamp = Solarmax.Singleton<TimeSystem>.Get().GetTimeStamp();
		long num2 = timeStamp - num;
		long num3 = 604800L;
		num2 %= num3;
		long num4 = num3 - num2;
		TimeSpan timeSpan = TimeSpan.FromSeconds((double)num4);
		return string.Format(LanguageDataProvider.GetValue(2317), timeSpan.ToString());
	}

	public void SetLotteryData()
	{
	}

	public int GetTakeAwardTime()
	{
		int totalTimes = this.lotteryNotes.totalTimes;
		for (int i = 0; i < this.awardCntList.Count; i++)
		{
			if (totalTimes < this.awardCntList[i])
			{
				return this.awardCntList[i] - totalTimes;
			}
		}
		return 0;
	}

	public bool needRedDot()
	{
		if (this.lotteryNotes == null)
		{
			return false;
		}
		string freeRestTime = this.GetFreeRestTime();
		if (freeRestTime.Length == 0)
		{
			return true;
		}
		Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
		for (int i = 0; i < this.lotteryNotes.lotteryBoxInfo.Count; i++)
		{
			LotteryBoxInfo lotteryBoxInfo = this.lotteryNotes.lotteryBoxInfo[i];
			dictionary[lotteryBoxInfo.awardId] = lotteryBoxInfo.get;
		}
		Dictionary<int, LotteryRotateConfig> dataDict = Solarmax.Singleton<LotteryRotateProvider>.Get().dataDict;
		LotteryRotateConfig lotteryRotateConfig = dataDict[this.lotteryNotes.currLotteryId];
		List<int> lottery_add_ids = lotteryRotateConfig.lottery_add_ids;
		Dictionary<int, LotteryAddConfig> dataDict2 = Solarmax.Singleton<LotteryAddProvider>.Get().dataDict;
		foreach (int key in lottery_add_ids)
		{
			if (dataDict2[key].count <= this.lotteryNotes.totalTimes && !dictionary.ContainsKey(dataDict2[key].id))
			{
				return true;
			}
		}
		return false;
	}

	public AwardItem GetLotteryWheelItem(int retId)
	{
		LotteryConfig lotteryConfig = Solarmax.Singleton<LotteryConfigProvider>.Get().dataDict[retId];
		int itemId = lotteryConfig.itemId;
		AwardItem awardItem = new AwardItem();
		awardItem.itemId = itemId;
		awardItem.itemNum = lotteryConfig.num;
		awardItem.type = lotteryConfig.itemType;
		if (lotteryConfig.itemType != 1)
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(itemId);
			awardItem.itemIcon = data.icon;
			awardItem.needCount = data.needCount;
		}
		return awardItem;
	}

	public AwardItem GetExtraItem(int retId)
	{
		LotteryAddConfig lotteryAddConfig = Solarmax.Singleton<LotteryAddProvider>.Get().dataDict[retId];
		int itemId = lotteryAddConfig.itemId;
		AwardItem awardItem = new AwardItem();
		awardItem.itemId = itemId;
		awardItem.itemNum = lotteryAddConfig.num;
		awardItem.type = lotteryAddConfig.type;
		if (lotteryAddConfig.type != 1)
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(itemId);
			awardItem.itemIcon = data.icon;
			awardItem.needCount = data.needCount;
		}
		return awardItem;
	}

	public SCLotteryInfo lotteryInfo;

	public SCLotterNotes lotteryNotes;

	public SCLotteryAward lotteryAward;

	public bool canFreeLottery;

	public Tweener tweener;

	public List<int> awardCntList = new List<int>();

	public LuckModel.RewardType rewardType;

	public enum RewardType
	{
		Lottery = 1,
		ExtraAward
	}
}
