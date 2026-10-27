using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class AchievementView : MonoBehaviour
{
	private void Awake()
	{
	}

	public void Init()
	{
		this.dicBehavior = new Dictionary<string, AchievementView.AchieveTemplate>();
		this.ShowAchievement();
		this.achievementInfo.gameObject.SetActive(false);
		AchievementManager achievementManager = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager.achieveStateChanged = (AchievementManager.AchieveStateChanged)Delegate.Combine(achievementManager.achieveStateChanged, new AchievementManager.AchieveStateChanged(this.OnAchieveStateChanged));
		AchievementManager achievementManager2 = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager2.battleEnd = (AchievementManager.BattleEnd)Delegate.Combine(achievementManager2.battleEnd, new AchievementManager.BattleEnd(this.OnDBattleEnd));
		this.OnClickAchievementBtn();
	}

	public void Destory()
	{
		this.dicBehavior.Clear();
		AchievementManager achievementManager = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager.achieveStateChanged = (AchievementManager.AchieveStateChanged)Delegate.Remove(achievementManager.achieveStateChanged, new AchievementManager.AchieveStateChanged(this.OnAchieveStateChanged));
		AchievementManager achievementManager2 = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager2.battleEnd = (AchievementManager.BattleEnd)Delegate.Remove(achievementManager2.battleEnd, new AchievementManager.BattleEnd(this.OnDBattleEnd));
	}

	public void OnClickAchievementBtn()
	{
		if (!this.achievementInfo.gameObject.activeSelf)
		{
			this.achievementInfo.gameObject.SetActive(true);
		}
		else
		{
			this.achievementInfo.gameObject.SetActive(false);
		}
	}

	public void ShowAchievement()
	{
		List<Achievement> achievementByDifficult = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID()].GetAchievementByDifficult((AchievementDifficult)Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentDiffcult(), false);
		if (achievementByDifficult.Count > 0)
		{
		}
		for (int i = 0; i < achievementByDifficult.Count; i++)
		{
			if (achievementByDifficult[i].types[0] != AchievementType.Ads)
			{
				GameObject gameObject = this.table.gameObject.AddChild(this.infoTemplate);
				gameObject.SetActive(true);
				AchievementTemplate component = gameObject.GetComponent<AchievementTemplate>();
				component.UpdateUI(achievementByDifficult[i].success, achievementByDifficult[i].levelDesc);
				AchievementView.AchieveTemplate achieveTemplate = new AchievementView.AchieveTemplate();
				achieveTemplate.ahievement = achievementByDifficult[i];
				achieveTemplate.behavior = component;
				this.dicBehavior[achievementByDifficult[i].id] = achieveTemplate;
			}
		}
		this.table.Reposition();
	}

	private IEnumerator RefreshScrollView()
	{
		yield return null;
		this.achievementInfo.ResetPosition();
		yield break;
	}

	public void Clear()
	{
		this.table.transform.DestroyChildren();
		this.table.Reposition();
	}

	public void OnAchieveStateChanged(string id)
	{
		if (this.dicBehavior.ContainsKey(id))
		{
			string desc = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[id].GetDesc(true, Solarmax.Singleton<AchievementModel>.Get().dicAchievements[id].currentCompleted);
			bool success = Solarmax.Singleton<AchievementManager>.Get().IsCompleted(id);
			if (Solarmax.Singleton<AchievementModel>.Get().dicAchievements[id].success)
			{
				success = true;
			}
			this.dicBehavior[id].behavior.UpdateUI(success, desc);
		}
	}

	public void OnDBattleEnd()
	{
		AchievementManager achievementManager = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager.achieveStateChanged = (AchievementManager.AchieveStateChanged)Delegate.Remove(achievementManager.achieveStateChanged, new AchievementManager.AchieveStateChanged(this.OnAchieveStateChanged));
		AchievementManager achievementManager2 = Solarmax.Singleton<AchievementManager>.Get();
		achievementManager2.battleEnd = (AchievementManager.BattleEnd)Delegate.Remove(achievementManager2.battleEnd, new AchievementManager.BattleEnd(this.OnDBattleEnd));
		this.dicBehavior.Clear();
	}

	public UIScrollView achievementInfo;

	public GameObject achievementBtn;

	public UITable table;

	public GameObject infoTemplate;

	public GameObject bg;

	private Dictionary<string, AchievementView.AchieveTemplate> dicBehavior;

	private class AchieveTemplate
	{
		public Achievement ahievement;

		public AchievementTemplate behavior;
	}
}
