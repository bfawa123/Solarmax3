using System;
using Solarmax;
using UnityEngine;

public class AchievementTaskTemplate : MonoBehaviour
{
	public void UpdateUI(TaskConfig config)
	{
		Achievement achievement = Solarmax.Singleton<AchievementModel>.Get().dicAchievements[config.levelId];
		if (achievement.success && config.status != TaskStatus.Received)
		{
			config.status = TaskStatus.Completed;
		}
		this.config = config;
		TaskStatus status = config.status;
		if (status != TaskStatus.Completed)
		{
			if (status != TaskStatus.Received)
			{
				if (status == TaskStatus.Unfinished)
				{
					this.doneBtn.SetActive(false);
					this.finishBtn.SetActive(false);
					this.unfinishBtn.SetActive(true);
				}
			}
			else
			{
				this.doneBtn.SetActive(false);
				this.finishBtn.SetActive(true);
				this.unfinishBtn.SetActive(false);
			}
		}
		else
		{
			this.doneBtn.SetActive(true);
			this.finishBtn.SetActive(false);
			this.unfinishBtn.SetActive(false);
		}
		AchievementConfig achievementConfig = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[config.levelId];
		achievementConfig.SetDesc();
		this.desc.text = string.Format("{0} | {1}\n{2}", LanguageDataProvider.GetValue(achievementConfig.chapterNameId), LanguageDataProvider.GetValue(achievementConfig.levelNameId), achievementConfig.chapterDesc);
		this.rewardNum.text = config.rewardValue.ToString();
	}

	public void OnDoneClicked()
	{
		if (!this.canClick)
		{
			return;
		}
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
		this.canClick = false;
		this.onCDTime = 0.02f;
		Solarmax.Singleton<TaskModel>.Get().ClaimReward(this.config.id, delegate(bool success)
		{
			if (success)
			{
			}
		}, 1);
	}

	private void FixedUpdate()
	{
		if (this.onCDTime > 1E-45f)
		{
			this.onCDTime += Time.deltaTime;
			if (this.onCDTime > 0.5f)
			{
				this.canClick = true;
				this.onCDTime = float.Epsilon;
			}
		}
	}

	public UILabel desc;

	public UILabel rewardNum;

	public GameObject unfinishBtn;

	public GameObject finishBtn;

	public GameObject doneBtn;

	public UILabel unfinishLabel;

	private const float CD_TIME = 0.5f;

	private float onCDTime;

	private bool canClick = true;

	private TaskConfig config;
}
