using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class AchievementTaskView : MonoBehaviour
{
	public void EnsureInit()
	{
		this.tasks.Clear();
		this.wattingTasks.Clear();
		this.model = Solarmax.Singleton<TaskModel>.Get();
		int num = 0;
		foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().GetAchieveData())
		{
			if (taskConfig.status != TaskStatus.Received)
			{
				Achievement achievement = Solarmax.Singleton<AchievementModel>.Get().dicAchievements[taskConfig.levelId];
				if (achievement.success)
				{
					taskConfig.status = TaskStatus.Completed;
					this.tasks.Insert(num++, taskConfig);
					this.wattingTasks.Add(taskConfig.id);
				}
				else
				{
					this.tasks.Insert(this.tasks.Count, taskConfig);
				}
			}
		}
		this.UpdateUI();
		if (this.wattingTasks.Count == 0)
		{
			this.oneKey.enabled = false;
			this.oneKey.GetComponent<UISprite>().color = new Color(1f, 1f, 1f, 0.5f);
		}
		else
		{
			this.oneKey.enabled = true;
			this.oneKey.GetComponent<UISprite>().color = Vector4.one;
		}
	}

	public void EnsureDestroy()
	{
	}

	public void RefrushUI()
	{
	}

	private void UpdateUI()
	{
		this.wrapContent.maxIndex = 0;
		this.wrapContent.minIndex = -(this.tasks.Count - 1);
		int num;
		if (this.tasks.Count > 10)
		{
			UIWrapContent uiwrapContent = this.wrapContent;
			uiwrapContent.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(uiwrapContent.onInitializeItem, new UIWrapContent.OnInitializeItem(this.OnDrageItem));
			num = 10;
		}
		else
		{
			for (int i = this.tasks.Count; i < 10; i++)
			{
				this.wrapContent.transform.GetChild(i).gameObject.SetActive(false);
			}
			num = this.tasks.Count;
		}
		for (int j = 0; j < num; j++)
		{
			this.UpdateChildTemplate(j, j);
		}
	}

	public void OnClickedOneKeyClaim()
	{
		if (!this.canClick)
		{
			return;
		}
		this.canClick = false;
		this.onCDTime = 0.02f;
		List<string> list = new List<string>();
		int num = (this.wattingTasks.Count + 20) / 20;
		for (int i = 0; i < num; i++)
		{
			list.Clear();
			for (int j = 0; j < 20; j++)
			{
				int num2 = i * 20 + j;
				if (num2 >= this.wattingTasks.Count)
				{
					break;
				}
				list.Add(this.wattingTasks[num2]);
			}
			if (list.Count > 0)
			{
				Solarmax.Singleton<TaskModel>.Get().ClaimAllReward(list, null, 1);
			}
		}
	}

	private void OnDrageItem(GameObject gameObject, int wrapIndex, int realIndex)
	{
		this.UpdateChildTemplate(wrapIndex, realIndex);
	}

	private void UpdateChildTemplate(int wrapIndex, int realIndex)
	{
		realIndex = Mathf.Abs(realIndex);
		TaskConfig config = this.tasks[realIndex];
		Transform child = this.wrapContent.transform.GetChild(wrapIndex);
		AchievementTaskTemplate component = child.GetComponent<AchievementTaskTemplate>();
		component.UpdateUI(config);
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

	public UIScrollView scroll;

	public GameObject template;

	public UIWrapContent wrapContent;

	public UIButton oneKey;

	private const float CD_TIME = 0.5f;

	private const int MAX_ONE_CLAIM = 20;

	private const int MAX_REUSE = 10;

	private TaskModel model;

	private List<TaskConfig> tasks = new List<TaskConfig>();

	private List<string> wattingTasks = new List<string>();

	private float onCDTime;

	private bool canClick = true;
}
