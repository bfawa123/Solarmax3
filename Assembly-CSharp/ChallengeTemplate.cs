using System;
using Solarmax;
using UnityEngine;

public class ChallengeTemplate : MonoBehaviour
{
	public void EnsurseInit(Achievement achieve)
	{
		if (achieve == null)
		{
			Solarmax.Singleton<LoggerSystem>.Get().Error("challenge window: achieve is null", new object[0]);
			return;
		}
		this.achieve = achieve;
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		if (this.achieve.diffcult == AchievementDifficult.Hard)
		{
			this.diffcult.text = string.Format("[{0}]", LanguageDataProvider.GetValue(2105));
		}
		if (this.achieve.diffcult == AchievementDifficult.Hell)
		{
			this.diffcult.text = string.Format("[{0}]", LanguageDataProvider.GetValue(2106));
		}
		AchievementConfig achievementConfig = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[this.achieve.id];
		achievementConfig.SetDesc();
		this.info.text = achievementConfig.chapterDescWithoutDiff;
		TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(this.achieve.taskId);
		if (task == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("chanllenge 成就任务为空：" + this.achieve.taskId, new object[0]);
		}
		this.rewardNumber.text = task.rewardValue.ToString();
		this.RefreshUI();
	}

	public void RefreshUI()
	{
		TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(this.achieve.taskId);
		if (task == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("chanllenge 成就任务为空：" + this.achieve.taskId, new object[0]);
			return;
		}
		this.completed.SetActive(false);
		float num = 1f;
		if (task.status == TaskStatus.Unfinished && this.achieve.success)
		{
			task.status = TaskStatus.Completed;
		}
		TaskStatus status = task.status;
		if (status != TaskStatus.Unfinished)
		{
			if (status != TaskStatus.Completed)
			{
				if (status == TaskStatus.Received)
				{
					this.completed.SetActive(true);
					num = 1f;
				}
			}
			else
			{
				num = 0.9f;
			}
		}
		else
		{
			num = 0.5f;
		}
		this.info.alpha = num;
		this.diffcult.alpha = num;
		this.rewardNumber.alpha = num;
		this.bg.GetComponent<UISprite>().alpha = 0.28f * num;
		this.moneyIcon.GetComponent<UISprite>().alpha = num;
		this.layoutTable.Reposition();
	}

	public UILabel diffcult;

	public UILabel info;

	public UILabel rewardNumber;

	public GameObject completed;

	public GameObject moneyIcon;

	public GameObject bg;

	public UITable layoutTable;

	private Achievement achieve;
}
