using System;
using Solarmax;
using UnityEngine;

public class MonthCheckModel : Solarmax.Singleton<MonthCheckModel>
{
	public void Init()
	{
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		int @int = PlayerPrefs.GetInt(string.Format("{0}_opened", Solarmax.Singleton<LocalPlayer>.Get().GetLocalAccount()), -1);
		this.firstOpen = (@int != timeSpan.Days);
		if (this.firstOpen)
		{
			PlayerPrefs.SetInt(string.Format("{0}_opened", Solarmax.Singleton<LocalPlayer>.Get().GetLocalAccount()), timeSpan.Days);
		}
		this.todayCheckId = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime().Day;
	}

	public void SetCheckedTime(long time)
	{
		this.checkTime = time;
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = d.AddSeconds((double)this.checkTime) - d;
		this.needCheck = ((Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).Days != timeSpan.Days);
		this.isDouble = Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard();
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMothCard, new object[0]);
	}

	public void SetCheckedTime()
	{
		this.isDouble = Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard();
	}

	public void SetCheckedTimeEX(long time)
	{
		this.checkTime = time;
	}

	public long GetCheckedTime()
	{
		return this.checkTime;
	}

	public int currentMonth;

	public int checkedId;

	public int nextCheckId;

	public int couldRepairId;

	public int todayCheckId;

	public bool needCheck;

	public bool isDouble;

	public bool firstOpen;

	private long checkTime;

	public int repair_check_num;
}
