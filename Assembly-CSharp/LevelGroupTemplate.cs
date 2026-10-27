using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LevelGroupTemplate : MonoBehaviour
{
	public void EnsureInit(List<TaskConfig> config, int count)
	{
		if (config == null || count == 0)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if (i < count)
			{
				this.template[i].SetActive(true);
				this.template[i].gameObject.GetComponent<LevelTaskTemplate>().EnsureInit(config[i]);
			}
			else
			{
				this.template[i].SetActive(false);
			}
		}
	}

	public void EnsureDestroy()
	{
		for (int i = 0; i < 3; i++)
		{
			this.template[i].gameObject.GetComponent<LevelTaskTemplate>().EnsureDestroy();
		}
	}

	private void UpdateUI(TaskConfig config)
	{
	}

	private const int MAX_TEMPLATE = 3;

	public UITable table;

	public GameObject[] template;
}
