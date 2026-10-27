using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class DailyTaskView : MonoBehaviour
{
	public void EnsureInit()
	{
		this.model = Solarmax.Singleton<TaskModel>.Get();
		this.degreeConfig = Solarmax.Singleton<TaskConfigProvider>.Get().GeDegreeData();
		this.degreeConfig.Sort(delegate(TaskConfig p1, TaskConfig p2)
		{
			if (p1.taskParameter >= p2.taskParameter)
			{
				return 1;
			}
			return -1;
		});
		for (int i = 0; i < this.degreeTask.Length; i++)
		{
			UIEventListener component = this.degreeTask[i].GetComponent<UIEventListener>();
			if (component != null)
			{
				component.onPress = new UIEventListener.BoolDelegate(this.OnBgPressed);
			}
		}
		this.RefrushDegreeStats();
		base.StartCoroutine(this.UpdateUI());
		DateTime dateTime = new DateTime(1970, 1, 1);
		string arg = dateTime.ToLocalTime().ToString("hh:mm");
		this.refreshTime.text = string.Format(LanguageDataProvider.GetValue(2076), arg);
	}

	public void EnsureDestroy()
	{
		this.ClearTable();
	}

	public void RefrushDegreeStats()
	{
		for (int i = 0; i < this.degreeConfig.Count; i++)
		{
			this.SetTaskStats(this.degreeTask[i], this.degreeConfig[i].status);
		}
		this.UpdateDegreeProcess();
	}

	private void Update()
	{
		this.fDeltAccTimer += Time.deltaTime;
		if (this.fDeltAccTimer >= 120f)
		{
			this.fDeltAccTimer = 0f;
			if (Solarmax.Singleton<TaskModel>.Get().AfterDayRefrushTask())
			{
				Solarmax.Singleton<LocalPlayer>.Get().mOnLineTime = 0f;
				Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree = 0;
				this.RefrushDegreeStats();
				base.StartCoroutine(this.UpdateUI());
			}
		}
	}

	private IEnumerator UpdateUI()
	{
		yield return null;
		this.unfinish.Clear();
		this.complete.Clear();
		this.received.Clear();
		this.table.transform.DestroyChildren();
		List<TaskConfig> tasks = Solarmax.Singleton<TaskConfigProvider>.Get().GetDailyData();
		foreach (TaskConfig taskConfig in tasks)
		{
			if (taskConfig.status == TaskStatus.Unfinished)
			{
				this.unfinish.Add(taskConfig);
			}
			if (taskConfig.status == TaskStatus.Completed)
			{
				this.complete.Add(taskConfig);
			}
			if (taskConfig.status == TaskStatus.Received)
			{
				this.received.Add(taskConfig);
			}
		}
		foreach (TaskConfig config in this.complete)
		{
			GameObject gameObject = this.table.gameObject.AddChild(this.template);
			gameObject.SetActive(true);
			DailyTaskTemplate component = gameObject.GetComponent<DailyTaskTemplate>();
			component.EnsureInit(config, false);
		}
		foreach (TaskConfig config2 in this.unfinish)
		{
			GameObject gameObject2 = this.table.gameObject.AddChild(this.template);
			gameObject2.SetActive(true);
			DailyTaskTemplate component2 = gameObject2.GetComponent<DailyTaskTemplate>();
			component2.EnsureInit(config2, false);
		}
		foreach (TaskConfig config3 in this.received)
		{
			GameObject gameObject3 = this.table.gameObject.AddChild(this.template);
			gameObject3.SetActive(true);
			DailyTaskTemplate component3 = gameObject3.GetComponent<DailyTaskTemplate>();
			component3.EnsureInit(config3, false);
		}
		this.table.Reposition();
		this.scroll.ResetPosition();
		yield break;
	}

	private void ClearTable()
	{
		for (int i = 0; i < this.table.transform.childCount; i++)
		{
			DailyTaskTemplate component = this.table.transform.GetChild(i).gameObject.GetComponent<DailyTaskTemplate>();
			component.EnsureDestroy();
			UnityEngine.Object.Destroy(this.table.transform.GetChild(i).gameObject);
		}
		this.table.Reposition();
	}

	public void OnClickDegreeTask(GameObject go)
	{
		int num = int.Parse(go.name);
		num--;
		if (num >= 0 && num < this.degreeConfig.Count)
		{
			TaskConfig taskConfig = this.degreeConfig[num];
			if (taskConfig.status == TaskStatus.Unfinished || taskConfig.status == TaskStatus.Received)
			{
				return;
			}
			Solarmax.Singleton<TaskModel>.Get().claimTask = taskConfig;
			Solarmax.Singleton<TaskModel>.Get().claimReward = new RewardTipsModel(taskConfig.rewardValue, global::RewardType.Money, false, 0);
			Solarmax.Singleton<TaskModel>.Get().ClaimReward(Solarmax.Singleton<TaskModel>.Get().claimTask.id, null, 1);
		}
	}

	private void UpdateDegreeProcess()
	{
		int mActivityDegree = Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree;
		int num = 150;
		float value = (float)mActivityDegree / ((float)num * 1f);
		this.curSlider.value = value;
		this.degree.text = mActivityDegree.ToString();
	}

	private void SetTaskStats(GameObject go, TaskStatus status)
	{
		UISprite component = go.transform.Find("icon").GetComponent<UISprite>();
		if (status == TaskStatus.Unfinished)
		{
			component.spriteName = "close";
		}
		if (status == TaskStatus.Completed)
		{
			component.spriteName = "open";
		}
		if (status == TaskStatus.Received)
		{
			component.spriteName = "opened";
		}
	}

	public void OnBgPressed(GameObject go, bool press)
	{
		int num = int.Parse(go.name);
		num--;
		if (!press)
		{
			if (num >= 0 && num < this.degreeConfig.Count)
			{
				this.tips[num].SetActive(false);
			}
		}
		else if (num >= 0 && num < this.degreeConfig.Count)
		{
			TaskConfig taskConfig = this.degreeConfig[num];
			if (taskConfig.status == TaskStatus.Completed)
			{
				return;
			}
			this.ShowTaskRewardInfo(this.tips[num], taskConfig);
		}
	}

	public void ShowTaskRewardInfo(GameObject go, TaskConfig config)
	{
		if (!go.activeSelf)
		{
			go.SetActive(true);
		}
		string text = string.Empty;
		UILabel component = go.transform.Find("tips").GetComponent<UILabel>();
		if (component != null)
		{
			if (config.rewardType == Solarmax.RewardType.Gold)
			{
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					LanguageDataProvider.GetValue(2032),
					"x ",
					config.rewardValue
				});
				UISprite component2 = go.transform.Find("icon").GetComponent<UISprite>();
				GameObject gameObject = go.transform.Find("Fragmark").gameObject;
				component2.spriteName = "icon_currency";
				gameObject.SetActive(false);
			}
			else if (config.rewardType == Solarmax.RewardType.Item)
			{
				ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Instance.GetData(config.itemid);
				if (data != null)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						LanguageDataProvider.GetValue(data.name),
						"x ",
						config.rewardValue
					});
					GameObject gameObject2 = go.transform.Find("Fragmark").gameObject;
					UISprite component3 = go.transform.Find("icon").GetComponent<UISprite>();
					component3.spriteName = data.icon;
					gameObject2.SetActive(true);
				}
			}
		}
		component.text = text;
	}

	public UITable table;

	public GameObject template;

	private TaskModel model;

	public UILabel refreshTime;

	public UIScrollView scroll;

	public UISlider curSlider;

	public GameObject[] degreeTask;

	public GameObject[] tips;

	public UILabel degree;

	private List<TaskConfig> degreeConfig = new List<TaskConfig>();

	private List<TaskConfig> unfinish = new List<TaskConfig>();

	private List<TaskConfig> complete = new List<TaskConfig>();

	private List<TaskConfig> received = new List<TaskConfig>();

	private float fDeltAccTimer;
}
