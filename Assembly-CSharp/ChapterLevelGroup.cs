using System;
using System.Collections.Generic;
using Solarmax;

public class ChapterLevelGroup
{
	public ChapterLevelGroup(string gID)
	{
		this.groupID = gID;
		this.displayID = string.Empty;
		this.isMain = false;
		this.star = 0;
		this.Score = 0;
		LoggerSystem.CodeComments("Change Level Unlock here.");
		this.unLock = LocalPlayer.LocalUnlock;
		this.mapList = new List<ChapterLevelInfo>();
		this.maxStar = AchievementListProvider.GetAchievementCount(this.groupID);
	}

	public ChapterLevelInfo GetLevel(int nDif)
	{
		int count = this.mapList.Count;
		if (nDif >= count)
		{
			for (int i = 0; i < this.mapList.Count; i++)
			{
				if (!this.mapList[i].unLock)
				{
					return this.mapList[i];
				}
			}
			return this.mapList[0];
		}
		for (int j = 0; j < this.mapList.Count; j++)
		{
			if (this.mapList[j].difficult == nDif)
			{
				return this.mapList[j];
			}
		}
		return null;
	}

	public int GetMaxDiffuse()
	{
		int result = this.mapList.Count - 1;
		for (int i = 0; i < this.mapList.Count; i++)
		{
			if (i == 0 && this.mapList[i].star == 0)
			{
				result = 0;
				break;
			}
			if (!this.mapList[i].unLock)
			{
				result = this.mapList[i].difficult;
				break;
			}
		}
		return result;
	}

	public void ModifyLevel(ChapterLevelInfo levelInfo)
	{
		if (levelInfo == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < this.mapList.Count; i++)
		{
			if (this.mapList[i].id.Equals(levelInfo.id))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			this.mapList.Add(levelInfo);
		}
		if (!this.isMain)
		{
			this.isMain = levelInfo.isMain;
		}
		if (!this.unLock)
		{
			this.unLock = levelInfo.unLock;
		}
		if (string.IsNullOrEmpty(this.displayID))
		{
			this.displayID = levelInfo.id;
		}
		if (levelInfo.Score > this.Score)
		{
			this.Score = levelInfo.Score;
			if (Solarmax.Singleton<LevelDataHandler>.Get().onLevelScore != null)
			{
				Solarmax.Singleton<LevelDataHandler>.Get().onLevelScore(this.groupID);
			}
		}
		this.CalcAchievents();
		if (this.star > 0)
		{
			this.unLock = true;
		}
	}

	public void UnLock()
	{
		string norLevelID = this.GetNorLevelID();
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(norLevelID);
		string dependLevel = data.dependLevel;
		if (string.IsNullOrEmpty(dependLevel))
		{
			this.unLock = true;
		}
		this.CalcAchievents();
		if (this.star > 0)
		{
			this.unLock = true;
		}
		if (Solarmax.Singleton<LevelDataHandler>.Get().IsUnLock(dependLevel))
		{
			this.unLock = true;
		}
	}

	public void Reset()
	{
	}

	public string GetNorLevelID()
	{
		if (this.mapList.Count == 1)
		{
			return this.mapList[0].id;
		}
		ChapterLevelInfo level = this.GetLevel(0);
		if (level != null)
		{
			return level.id;
		}
		return string.Empty;
	}

	public void CalcAchievents()
	{
		this.star = 0;
		this.star += this.mapList[0].star;
	}

	public string groupID;

	public string displayID;

	public bool isMain;

	public int star;

	public int maxStar;

	public int Score;

	public bool unLock;

	public List<ChapterLevelInfo> mapList;
}
