using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LevelTaskTemplate : MonoBehaviour
{
	public void EnsureInit(TaskConfig config)
	{
		this.task = null;
		if (config == null)
		{
			return;
		}
		this.task = config;
		this.model = Solarmax.Singleton<TaskModel>.Get();
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(this.task.levelId);
		base.name = this.task.levelId + "_" + LanguageDataProvider.GetValue(data.levelName);
		base.StartCoroutine(this.UpdateUI());
	}

	public void EnsureDestroy()
	{
	}

	public void OnClickedClaimReward()
	{
		this.model.ClaimReward(this.task.id, delegate(bool success)
		{
			if (success)
			{
			}
		}, 1);
	}

	private IEnumerator UpdateUI()
	{
		yield return null;
		for (int i = 0; i < 4; i++)
		{
			this.stars[i].SetActive(i < this.task.taskParameter);
		}
		this.starTable.Reposition();
		LevelConfig level = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(this.task.levelId);
		this.nameLabel.text = LanguageDataProvider.GetValue(level.levelName);
		this.rewardLabel.text = this.task.rewardValue.ToString();
		ChapterLevelInfo tempLevel = Solarmax.Singleton<LevelDataHandler>.Get().GetLevel(this.task.levelId);
		TaskConfig temp = Solarmax.Singleton<TaskConfigProvider>.Instance.GetTask(this.task.id);
		TaskStatus status = this.task.status;
		if (status != TaskStatus.Completed)
		{
			if (status != TaskStatus.Unfinished)
			{
				if (status == TaskStatus.Received)
				{
					this.claimReward.SetActive(false);
					this.uncompleted.SetActive(true);
					this.uncompletedInfo.text = LanguageDataProvider.GetValue(2081);
				}
			}
			else
			{
				this.claimReward.SetActive(false);
				this.uncompleted.SetActive(true);
				this.uncompletedInfo.text = LanguageDataProvider.GetValue(2080);
			}
		}
		else
		{
			this.claimReward.SetActive(true);
			this.uncompleted.SetActive(false);
		}
		yield break;
	}

	private void OnRequestTaskOk(List<string> success, List<string> failure)
	{
		foreach (string value in success)
		{
			if (this.task.id.Equals(value))
			{
				TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Instance.GetTask(this.task.id);
				taskConfig.status = TaskStatus.Received;
				this.claimReward.SetActive(false);
				this.uncompleted.SetActive(true);
				this.uncompletedInfo.text = LanguageDataProvider.GetValue(2081);
				return;
			}
		}
		foreach (string value2 in failure)
		{
			if (this.task.id.Equals(value2))
			{
				break;
			}
		}
	}

	private const int UNCOMLETED = 2080;

	private const int COMLETED = 2081;

	public UITable starTable;

	public GameObject[] stars;

	public UILabel nameLabel;

	public UILabel rewardLabel;

	public GameObject claimReward;

	public GameObject uncompleted;

	public UILabel uncompletedInfo;

	private TaskConfig task;

	private TaskModel model;
}
