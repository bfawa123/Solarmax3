using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class SeasonRewardView : MonoBehaviour
{
	private void Start()
	{
		this.rewardTemplate.SetActive(false);
		this.UpdateUI();
	}

	public void UpdateUI()
	{
		SeasonRewardModel seasonRewardModel = Solarmax.Singleton<SeasonRewardModel>.Get();
		if (seasonRewardModel.outSeason)
		{
			return;
		}
		for (int i = 0; i < this.templates.Count; i++)
		{
			GameObject gameObject = this.templates[i].gameObject;
			gameObject.transform.parent = null;
			UnityEngine.Object.Destroy(gameObject);
		}
		SeasonRewardConfig seasonRewardConfig = Solarmax.Singleton<SeasonRewardProvider>.Get().dataList[seasonRewardModel.seasonId.ToString()];
		this.templates.Clear();
		for (int j = 1; j <= seasonRewardConfig.count; j++)
		{
			GameObject gameObject2 = this.table.gameObject.AddChild(this.rewardTemplate);
			gameObject2.SetActive(true);
			SeasonRewardTemplate component = gameObject2.GetComponent<SeasonRewardTemplate>();
			component.UpdateUI(j.ToString());
			this.templates.Add(component);
		}
		this.tips.transform.parent = this.table.transform;
		this.table.Reposition();
	}

	public void RefreshUI()
	{
		SeasonRewardModel seasonRewardModel = Solarmax.Singleton<SeasonRewardModel>.Get();
		if (seasonRewardModel.outSeason)
		{
			return;
		}
		foreach (SeasonRewardTemplate seasonRewardTemplate in this.templates)
		{
			seasonRewardTemplate.RefreshUI();
		}
	}

	public UITable table;

	public GameObject rewardTemplate;

	public GameObject tips;

	private List<SeasonRewardTemplate> templates = new List<SeasonRewardTemplate>();
}
