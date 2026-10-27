using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class ChallengeLevelTemplate : MonoBehaviour
{
	public void EnsureInit(string level)
	{
		this.levelGroupId = level;
		this.UpdateUI();
	}

	private void UpdateUI()
	{
		this.challenges.Clear();
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(this.levelGroupId);
		if (chapterLevelGroup == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("challenge window: level is null", new object[0]);
			return;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(chapterLevelGroup.displayID);
		this.levelName.text = LanguageDataProvider.GetValue(data.levelName);
		List<Achievement> list = AchievementModel.GetChallenges(this.levelGroupId);
		foreach (Achievement achieve in list)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.challengeTemplate);
			gameObject.SetActive(true);
			ChallengeTemplate component = gameObject.GetComponent<ChallengeTemplate>();
			component.EnsurseInit(achieve);
			this.challenges.Add(component);
		}
		this.grid.Reposition();
	}

	public void RefreshUI()
	{
		foreach (ChallengeTemplate challengeTemplate in this.challenges)
		{
			challengeTemplate.RefreshUI();
		}
	}

	public GameObject challengeTemplate;

	public UIGrid grid;

	public UILabel levelName;

	private string levelGroupId;

	private List<ChallengeTemplate> challenges = new List<ChallengeTemplate>();
}
