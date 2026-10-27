using System;
using System.Collections.Generic;
using Solarmax;

public class Achievement
{
	public Achievement()
	{
		AchievementModel achievementModel = Solarmax.Singleton<AchievementModel>.Get();
		achievementModel.onLanguageChanged = (AchievementModel.OnLanguageChanged)Delegate.Combine(achievementModel.onLanguageChanged, new AchievementModel.OnLanguageChanged(this.OnChangeLanguage));
	}

	public bool success
	{
		get
		{
			return this.completed;
		}
		set
		{
			this.completed = value;
			if (Solarmax.Singleton<AchievementModel>.Get().onAchieveSuccess != null)
			{
                Solarmax.Singleton<AchievementModel>.Get().onAchieveSuccess(this.id, this.completed);
			}
		}
	}

	public void OnChangeLanguage()
	{
		AchievementConfig achievementConfig = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[this.id];
		achievementConfig.SetDesc();
		this.levelDesc = achievementConfig.levelDesc;
		this.chapterDesc = achievementConfig.chapterDesc;
	}

	public string id;

	public string chapterDesc;

	public string levelDesc;

	public bool completed;

	public AchievementDifficult diffcult;

	public List<AchievementType> types;

	public List<bool> achieveSuccess;

	public AchievementConfig config;

	public List<int> currentCompleted;

	public string groupId;

	public string levelNameId;

	public string taskId;
}
