using System;
using System.Collections;
using Solarmax;
using UnityEngine;

public class LobbyAchieveView : MonoBehaviour
{
	private void Awake()
	{
	}

	private void OnEnable()
	{
		UIEventListener uieventListener = UIEventListener.Get(this.bg);
		uieventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onPress, new UIEventListener.BoolDelegate(this.OnBgPressed));
	}

	private void OnDisable()
	{
		this.Clear();
		UIEventListener uieventListener = UIEventListener.Get(this.bg);
		uieventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Remove(uieventListener.onPress, new UIEventListener.BoolDelegate(this.OnBgPressed));
	}

	public void OnBgPressed(GameObject go, bool press)
	{
		this.OnBgClicked();
	}

	public void OnBgClicked()
	{
		Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnAchieveBgClicked, null);
	}

	public void Show(string group)
	{
		this.levelGroup = group;
	}

	public void ChangeLanguage()
	{
		AchievementGroup achievementGroup = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[this.levelGroup];
		for (int i = 0; i < achievementGroup.achievements.Count; i++)
		{
		}
	}

	private IEnumerator ShowHandler()
	{
		yield return null;
		AchievementGroup temp = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[this.levelGroup];
		for (int i = 0; i < temp.achievements.Count; i++)
		{
			Achievement achievement = temp.achievements[i];
			GameObject gameObject = this.table.gameObject.AddChild(this.template);
			gameObject.SetActive(true);
			UILabel component = gameObject.transform.GetChild(0).GetComponent<UILabel>();
			component.text = achievement.chapterDesc;
			TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(achievement.taskId);
			if (task == null)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("成就任务为空：" + achievement.taskId, new object[0]);
			}
			else
			{
				UILabel component2 = gameObject.transform.Find("RewardLabel").GetComponent<UILabel>();
				component2.text = task.rewardValue.ToString();
				LobbyAchieveTemplate component3 = gameObject.GetComponent<LobbyAchieveTemplate>();
				component3.Init(task);
			}
		}
		this.table.Reposition();
		this.title.text = string.Format("{0}{1}/{2}", LanguageDataProvider.GetValue(2182), AchievementModel.GetCompletedStars(this.levelGroup).ToString(), AchievementModel.GetGroupStars(this.levelGroup).ToString());
		yield break;
	}

	private void Clear()
	{
		for (int i = 0; i < this.table.transform.childCount; i++)
		{
			LobbyAchieveTemplate component = this.table.transform.GetChild(i).GetComponent<LobbyAchieveTemplate>();
			component.Destroy();
			this.table.Reposition();
		}
		this.table.transform.DestroyChildren();
		this.scroll.ResetPosition();
	}

	public UIScrollView scroll;

	public UITable table;

	public GameObject template;

	public UILabel title;

	public GameObject bg;

	private string levelGroup;
}
