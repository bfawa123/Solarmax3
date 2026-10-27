using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LevelTaskView : MonoBehaviour
{
	public void EnsureInit(List<TaskConfig> levelTasks)
	{
		if (levelTasks == null || levelTasks.Count == 0)
		{
			return;
		}
		List<TaskConfig> list = new List<TaskConfig>();
		List<TaskConfig> list2 = new List<TaskConfig>();
		Solarmax.Singleton<TaskModel>.Get().completedTasks.Clear();
		this.groups = (list2.Count + 2) / 3;
		this.lastGroupCount = list2.Count % 3;
		if (this.lastGroupCount == 0)
		{
			this.lastGroupCount = 3;
		}
		this.dicTasks.Clear();
		this.wrapContent.maxIndex = 0;
		this.wrapContent.minIndex = -(this.groups - 1);
		UIWrapContent uiwrapContent = this.wrapContent;
		uiwrapContent.onInitializeItem = (UIWrapContent.OnInitializeItem)Delegate.Combine(uiwrapContent.onInitializeItem, new UIWrapContent.OnInitializeItem(this.OnDrageItem));
		base.StartCoroutine(this.UpdateUI());
	}

	public void EnsureDestroy()
	{
	}

	public void OnClickedOneKeyClaim()
	{
		int num = (Solarmax.Singleton<TaskModel>.Get().completedTasks.Count + 19) / 20;
		List<string> list = new List<string>();
		for (int i = 0; i < num; i++)
		{
			list.Clear();
			for (int j = 0; j < 20; j++)
			{
				if (i * 20 + j >= Solarmax.Singleton<TaskModel>.Get().completedTasks.Count)
				{
					break;
				}
				list.Add(Solarmax.Singleton<TaskModel>.Get().completedTasks[i * 20 + j]);
			}
			Solarmax.Singleton<TaskModel>.Get().ClaimAllReward(list, null, 1);
		}
	}

	private void OnDrageItem(GameObject gameObject, int wrapIndex, int realIndex)
	{
		if (!this.dicTasks.ContainsKey(-realIndex))
		{
			return;
		}
		int key = -realIndex;
		if (gameObject != null)
		{
			LevelGroupTemplate component = gameObject.GetComponent<LevelGroupTemplate>();
			if (component != null)
			{
				component.EnsureInit(this.dicTasks[key], this.dicTasks[key].Count);
			}
		}
	}

	private IEnumerator UpdateUI()
	{
		for (int i = 0; i < this.wrapContent.transform.childCount; i++)
		{
			if (i != 0)
			{
				if (i != 1)
				{
					if (i == 2)
					{
						this.wrapContent.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0f, -1496.2f, 0f);
					}
				}
				else
				{
					this.wrapContent.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0f, -895.8f, 0f);
				}
			}
			else
			{
				this.wrapContent.transform.GetChild(i).gameObject.transform.localPosition = new Vector3(0f, -295.4f, 0f);
			}
		}
		yield return null;
		int max = (this.groups <= 3) ? this.groups : 3;
		for (int j = 0; j < this.wrapContent.transform.childCount; j++)
		{
			this.wrapContent.transform.GetChild(j).gameObject.SetActive(j < max);
			if (j < max)
			{
				LevelGroupTemplate component = this.wrapContent.transform.GetChild(j).gameObject.GetComponent<LevelGroupTemplate>();
				if (component != null)
				{
					component.EnsureInit(this.dicTasks[j], this.dicTasks[j].Count);
				}
			}
		}
		yield break;
	}

	private void ResetScrollView()
	{
		this.scrollView.gameObject.GetComponent<UIPanel>().clipOffset = Vector2.zero;
		this.scrollView.transform.localPosition = new Vector3(184f, -89.3f, 0f);
		for (int i = 0; i < this.wrapContent.transform.childCount; i++)
		{
			LevelGroupTemplate component = this.wrapContent.transform.GetChild(i).gameObject.GetComponent<LevelGroupTemplate>();
			if (component != null)
			{
				component.EnsureDestroy();
			}
		}
	}

	public GameObject template;

	public UIScrollView scrollView;

	public UIWrapContent wrapContent;

	private int groups;

	private int lastGroupCount;

	private Dictionary<int, List<TaskConfig>> dicTasks = new Dictionary<int, List<TaskConfig>>();
}
