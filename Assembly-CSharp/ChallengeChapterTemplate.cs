using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class ChallengeChapterTemplate : MonoBehaviour
{
	public void EnsureInit(int index, ChapterInfo chapter)
	{
		this.chapter = chapter;
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Get().GetData(chapter.id);
		this.chapterName.text = LanguageDataProvider.GetValue(data.name);
		ResourcesUtil.SetStarChartTexture(this.chapterTexture, data.starChart);
		this.completedPercent.text = string.Format("{0}/{1}", chapter.completedChallenges, chapter.allChallenges);
		this.RefreshReddotVisible();
	}

	public void RefreshReddotVisible()
	{
		this.reddot.SetActive(false);
		foreach (ChapterLevelGroup chapterLevelGroup in this.chapter.levelList)
		{
			List<Achievement> challenges = AchievementModel.GetChallenges(chapterLevelGroup.groupID);
			foreach (Achievement achievement in challenges)
			{
				if (!Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(achievement.id))
				{
					Solarmax.Singleton<LoggerSystem>.Instance.Error("challenge window 任务为空: " + achievement.id, new object[0]);
				}
				else
				{
					TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Get().dataList[achievement.taskId];
					if (taskConfig.status == TaskStatus.Completed)
					{
						this.reddot.SetActive(true);
						return;
					}
				}
			}
		}
	}

	public bool ReddotVisible()
	{
		return this.reddot.activeSelf;
	}

	public UITexture chapterTexture;

	public UILabel chapterName;

	public UILabel completedPercent;

	public GameObject reddot;

	private ChapterInfo chapter;
}
