using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class LobbyChapterWindowCell : MonoBehaviour
{
	public void SetInfo(ChapterInfo info)
	{
		this.data = info;
		ChapterConfig chapterConfig = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(info.id);
		if (chapterConfig == null)
		{
			Debug.LogError("error read config " + info.id);
			return;
		}
		ResourcesUtil.SetStarChartTexture(this.galaxyBg.GetComponent<UITexture>(), chapterConfig.starChart);
		foreach (KeyValuePair<string, OneChapterPointConfig> keyValuePair in chapterConfig.oneChapterPointConfigs)
		{
			LevelConfig levelConfig = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(keyValuePair.Value.level);
			GameObject gameObject = this.startsGroup.AddChild(this.startsItemTemplate);
			gameObject.SetActive(true);
			Vector3 localPosition = new Vector3(keyValuePair.Value.x, keyValuePair.Value.y, keyValuePair.Value.z);
			Vector3 scale = new Vector3(keyValuePair.Value.size, keyValuePair.Value.size, 1f);
			gameObject.transform.localPosition = localPosition;
			LobbyChapterWindowCellStarsItemTemPlate component = gameObject.GetComponent<LobbyChapterWindowCellStarsItemTemPlate>();
			gameObject.GetComponent<BoxCollider>().enabled = false;
			int levelStar = this.GetLevelStar(keyValuePair.Value.level, this.data);
			bool bNor = false;
			if (levelConfig != null)
			{
				bNor = (levelConfig.mainLine <= 0);
			}
			component.SetLevel(bNor, levelStar, scale, this.data.unLock);
		}
		foreach (OneChapterLineConfig oneChapterLineConfig in chapterConfig.oneChapterLineConfigs)
		{
			GameObject gameObject2 = this.galaxyBg.AddChild(this.lineTemplate);
			gameObject2.SetActive(true);
			OneChapterPointConfig point = chapterConfig.GetPoint(oneChapterLineConfig.point1);
			OneChapterPointConfig point2 = chapterConfig.GetPoint(oneChapterLineConfig.point2);
			Vector3 startPos = new Vector3(point.x, point.y, point.z);
			Vector3 endPos = new Vector3(point2.x, point2.y, point2.z);
			int levelStar2 = this.GetLevelStar(point.level, info);
			int levelStar3 = this.GetLevelStar(point2.level, info);
			string spriteName = string.Empty;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < point.linkPointList.Length; i++)
			{
				if (point.linkPointList[i] == string.Empty)
				{
					flag = true;
				}
				else
				{
					OneChapterPointConfig point3 = chapterConfig.GetPoint(point.linkPointList[i]);
					if (this.GetLevelStar(point3.level, info) == 0)
					{
						flag = false;
						break;
					}
					flag = true;
				}
			}
			for (int j = 0; j < point2.linkPointList.Length; j++)
			{
				if (point2.linkPointList[j] == string.Empty)
				{
					flag2 = true;
				}
				else
				{
					OneChapterPointConfig point4 = chapterConfig.GetPoint(point2.linkPointList[j]);
					if (this.GetLevelStar(point4.level, info) == 0)
					{
						flag2 = false;
						break;
					}
					flag2 = true;
				}
			}
			if (levelStar2 > 0 && levelStar3 > 0)
			{
				spriteName = "Line_CopyUI_OpenState";
			}
			else if (flag && flag2 && (point.level == "0" || point2.level == "0"))
			{
				spriteName = "Line_CopyUI_OpenState";
			}
			else
			{
				spriteName = "Line_CopyUI_UnopenState";
			}
			gameObject2.GetComponent<LobbyChapterCellLineItemTemplate>().SetSpriteLine(startPos, endPos, spriteName, "LobbyWindow");
		}
		if (this.eventListener != null)
		{
			this.eventListener.onClick = new UIEventListener.VoidDelegate(this.OnClickItem);
		}
	}

	private void OnClickItem(GameObject go)
	{
		if (Solarmax.Singleton<LevelDataHandler>.Instance.BeUnlockChapter(this.data.id))
		{
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectChapter(this.data.id);
			Solarmax.Singleton<UISystem>.Get().HideWindow("LobbyWindowView");
		}
	}

	public int GetLevelStar(string groupID, ChapterInfo chapterInfo)
	{
		int result = 0;
		foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
		{
			if (groupID == chapterLevelGroup.groupID)
			{
				result = chapterLevelGroup.star;
				return result;
			}
		}
		return result;
	}

	public UIEventListener eventListener;

	public GameObject galaxyBg;

	public GameObject startsItemTemplate;

	public GameObject startsGroup;

	public GameObject lineTemplate;

	private ChapterInfo data;

	private const string GALAXY_TEXTURE_PATH = "gameres/texture_4/galaxyicon/";
}
