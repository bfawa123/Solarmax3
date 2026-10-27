using System;
using Solarmax;
using UnityEngine;

public class MonthCheckTemplate : MonoBehaviour
{
	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public void RefreshUI(DayRewardConfig reward, CheckType type)
	{
		this.rewardConfig = reward;
		this.checkType = type;
		this.haveChecked.SetActive(this.checkType == CheckType.Checked);
		this.monthDouble.SetActive(Solarmax.Singleton<MonthCheckModel>.Get().isDouble);
		int rewardType = reward.rewardType;
		if (rewardType == 1)
		{
			this.rewardIcon.spriteName = this.rewardConfig.icon;
		}
		this.rewardNumber.text = reward.misc.ToString();
		this.checkMark.SetActive(false);
		this.repairMark.SetActive(false);
		switch (type)
		{
		case CheckType.WaittingCheck:
			this.checkMark.SetActive(true);
			break;
		case CheckType.RepairCheck:
			this.repairMark.SetActive(true);
			break;
		}
	}

	public void OnClicked(GameObject go)
	{
	}

	public DayRewardConfig GetRewardConfig()
	{
		return this.rewardConfig;
	}

	public GameObject haveChecked;

	public GameObject monthDouble;

	public UILabel rewardNumber;

	public UISprite rewardIcon;

	public GameObject checkMark;

	public GameObject repairMark;

	[HideInInspector]
	public CheckType checkType;

	private DayRewardConfig rewardConfig;
}
