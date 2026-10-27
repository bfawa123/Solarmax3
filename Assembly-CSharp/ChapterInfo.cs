using System;
using System.Collections.Generic;
using Solarmax;

public class ChapterInfo
{
	public ChapterInfo()
	{
		this.trackChallenge = true;
		this.sn = 0;
		this.id = string.Empty;
		this.star = 0;
		this.levelList = new List<ChapterLevelGroup>();
		LoggerSystem.CodeComments("Set Chapter Unlock here");
		this.unLock = LocalPlayer.LocalUnlock;
		this.achieveMainLine = LocalPlayer.LocalUnlock;
		this.achieveLevels = 0;
		this.allstar = 0;
		this.isWattingUnlock = false;
		this.isNetSync = false;
		this.fInterestStart = 0f;
		this.fStrategyStart = 0f;
		this.nPromotionPrice = 0;
		this.ftotalStart = 0f;
	}

	public bool IsExist(string groupID)
	{
		for (int i = 0; i < this.levelList.Count; i++)
		{
			if (this.levelList[i].groupID.Equals(groupID))
			{
				return true;
			}
		}
		return false;
	}

	public void AddLevel(ChapterLevelGroup level)
	{
		if (!this.IsExist(level.groupID))
		{
			this.levelList.Add(level);
		}
	}

	public void Reset()
	{
		this.star = 0;
		this.unLock = false;
		this.achieveMainLine = false;
		this.achieveLevels = 0;
		this.allstar = 0;
		this.isWattingUnlock = false;
		for (int i = 0; i < this.levelList.Count; i++)
		{
			this.levelList[i].Reset();
		}
		this.isNetSync = false;
		this.allChallenges = 0;
		this.completedChallenges = 0;
		this.fInterestStart = 0f;
		this.fStrategyStart = 0f;
		this.nPromotionPrice = 0;
		this.ftotalStart = 0f;
	}

	public int GetLevelCount()
	{
		return this.levelList.Count;
	}

	public int sn;

	public string id;

	public List<ChapterLevelGroup> levelList;

	public int star;

	public bool achieveMainLine;

	public int achieveLevels;

	public bool unLock;

	public int mainLineNum;

	public int allstar;

	public int allChallenges;

	public int completedChallenges;

	public bool isWattingUnlock;

	public bool hasPassed;

	public bool isNetSync;

	public int type;

	public bool trackChallenge;

	public float fInterestStart;

	public float fStrategyStart;

	public float ftotalStart;

	public int nBuyCount;

	public int nPromotionPrice;
}
