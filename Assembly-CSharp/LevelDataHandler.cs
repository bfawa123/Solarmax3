using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LevelDataHandler : Solarmax.Singleton<LevelDataHandler>, IDataHandler, Lifecycle
{
	public LevelDataHandler()
	{
		this.currentChapter = null;
		this.currentLevel = null;
	}

	public bool Init()
	{
		string text = LoadResManager.LoadCustomTxt("data/unlock.txt");
		if (!string.IsNullOrEmpty(text) && text == "true")
		{
			LocalPlayer.LocalUnlock = true;
		}
		else
		{
			LocalPlayer.LocalUnlock = false;
		}
		this.chapterList = new List<ChapterInfo>();
		this.dicChapter = new Dictionary<string, ChapterInfo>();
		this.payChapterList = new List<ChapterInfo>();
		this.payMainList = new List<ChapterInfo>();
		this.coopertionList = new List<ChapterInfo>();
		this.dicLevels = new Dictionary<string, ChapterLevelInfo>();
		this.groupLevels = new Dictionary<string, ChapterLevelGroup>();
		this.Release();
		this.initChapterInfo();
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnLoadChaptersResult, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnLoadOneChapterResult, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnSetLevelStarResult, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		return true;
	}

	public static void AddDelegate()
	{
		AchievementModel achievementModel = Solarmax.Singleton<AchievementModel>.Get();
		achievementModel.onAchieveSuccess = (AchievementModel.OnAchieveSuccess)Delegate.Combine(achievementModel.onAchieveSuccess, new AchievementModel.OnAchieveSuccess(Solarmax.Singleton<LevelDataHandler>.Instance.SetLevelStar));
	}

	public ChapterLevelGroup FindGroupLevel(string groupID)
	{
		if (!this.groupLevels.ContainsKey(groupID))
		{
			return null;
		}
		return this.groupLevels[groupID];
	}

	public void Reset()
	{
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			this.chapterList[i].Reset();
		}
		for (int i = 0; i < this.payChapterList.Count; i++)
		{
			this.payChapterList[i].Reset();
		}
		for (int i = 0; i < this.coopertionList.Count; i++)
		{
			this.coopertionList[i].Reset();
		}
		this.evaluationMap.Clear();
		this.allStars = AchievementModel.GetALLCompletedStars();
	}

	public int InitCurrentChapterIndex()
	{
		if (this.currentChapterIndex == -1)
		{
			this.currentChapterIndex = PlayerPrefs.GetInt(string.Format("Chapter_id_{0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.userId.ToString()));
		}
		return this.currentChapterIndex;
	}

	public void Tick(float interval)
	{
	}

	public void Destroy()
	{
		this.Release();
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnLoadChaptersResult, this);
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnLoadOneChapterResult, this);
		Solarmax.Singleton<EventSystem>.Instance.UnRegisterEvent(EventId.OnSetLevelStarResult, this);
	}

	public void Release()
	{
		this.allStars = AchievementModel.GetALLCompletedStars();
		this.chapterList.Clear();
		this.payChapterList.Clear();
		this.payMainList.Clear();
		this.dicLevels.Clear();
		this.groupLevels.Clear();
	}

	private void initChapterInfo()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler   initChapterInfo", new object[0]);
		List<ChapterConfig> allData = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetAllData();
		if (allData.Count == 0)
		{
			Solarmax.Singleton<ChapterConfigProvider>.Instance.Load();
			allData = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetAllData();
		}
		if (allData.Count == 0)
		{
			return;
		}
		List<LevelConfig> allData2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetAllData();
		if (allData2.Count == 0)
		{
			Solarmax.Singleton<LevelConfigConfigProvider>.Instance.Load();
			allData2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetAllData();
		}
		if (allData2.Count == 0)
		{
			return;
		}
		int num = 1;
		for (int i = 0; i < allData.Count; i++)
		{
			ChapterConfig chapterConfig = allData[i];
			ChapterInfo chapterInfo = new ChapterInfo();
			chapterInfo.sn = num;
			chapterInfo.id = chapterConfig.id;
			chapterInfo.nPromotionPrice = chapterConfig.costGold;
			if (i == 0)
			{
				chapterInfo.unLock = true;
			}
			if (PlayerPrefs.GetInt("UnlockAllChapters", -1) != -1)
			{
				chapterInfo.unLock = true;
			}
			chapterInfo.type = chapterConfig.type;
			if (chapterConfig.type == 0)
			{
				this.chapterList.Add(chapterInfo);
			}
			else if (chapterConfig.type == 1)
			{
				this.payChapterList.Add(chapterInfo);
			}
			else if (chapterConfig.type == 2)
			{
				this.coopertionList.Add(chapterInfo);
			}
			int num2 = 0;
			for (int j = 0; j < allData2.Count; j++)
			{
				LevelConfig levelConfig = allData2[j];
				if (levelConfig.chapter == chapterConfig.id)
				{
					ChapterLevelInfo chapterLevelInfo = new ChapterLevelInfo();
					chapterLevelInfo.id = levelConfig.id;
					chapterLevelInfo.chapterId = chapterInfo.id;
					chapterLevelInfo.isMain = (levelConfig.mainLine > 0);
					if (levelConfig.difficult == 0)
					{
						int num3 = 0;
						int num4 = 0;
						if (chapterLevelInfo.isMain)
						{
							AchievementModel.GetDiffcultStar(levelConfig.levelGroup, out num3, out num4);
							chapterInfo.allstar += num3;
							chapterInfo.star += num4;
						}
						else
						{
							chapterInfo.allstar += levelConfig.maxStar;
						}
						if (chapterLevelInfo.isMain)
						{
							num2++;
						}
					}
					if (string.IsNullOrEmpty(levelConfig.dependLevel))
					{
						chapterLevelInfo.unLock = true;
					}
					if (chapterLevelInfo.star > 0)
					{
						chapterLevelInfo.unLock = true;
					}
					if (this.IsUnLock(levelConfig.dependLevel))
					{
						chapterLevelInfo.unLock = true;
					}
					chapterLevelInfo.difficult = levelConfig.difficult;
					ChapterLevelGroup chapterLevelGroup;
					if (string.IsNullOrEmpty(levelConfig.levelGroup))
					{
						chapterLevelGroup = new ChapterLevelGroup(levelConfig.id);
						this.groupLevels.Add(chapterLevelGroup.groupID, chapterLevelGroup);
					}
					else
					{
						chapterLevelGroup = this.FindGroupLevel(levelConfig.levelGroup);
						if (chapterLevelGroup == null)
						{
							chapterLevelGroup = new ChapterLevelGroup(levelConfig.levelGroup);
							this.groupLevels.Add(chapterLevelGroup.groupID, chapterLevelGroup);
						}
					}
					chapterLevelInfo.groupId = chapterLevelGroup.groupID;
					chapterLevelGroup.ModifyLevel(chapterLevelInfo);
					this.dicLevels.Add(chapterLevelInfo.id, chapterLevelInfo);
					chapterInfo.AddLevel(chapterLevelGroup);
				}
			}
			chapterInfo.mainLineNum = num2;
			num++;
		}
	}

	public void SetChapterStar()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler SetChapterStar ", new object[0]);
		foreach (ChapterInfo chapterInfo in this.chapterList)
		{
			if (chapterInfo.type == 0)
			{
				chapterInfo.allstar = 0;
				chapterInfo.star = 0;
				int num = 0;
				int num2 = 0;
				foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
				{
					AchievementModel.GetDiffcultStar(chapterLevelGroup.groupID, out num, out num2);
					chapterInfo.allstar += num;
					chapterInfo.star += num2;
				}
			}
		}
	}

	public void SetChapterChallenges()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  SetChapterChallenges", new object[0]);
		foreach (ChapterInfo chapterInfo in this.chapterList)
		{
			if (chapterInfo.type == 0)
			{
				chapterInfo.allChallenges = 0;
				foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
				{
					List<Achievement> challenges = AchievementModel.GetChallenges(chapterLevelGroup.groupID);
					chapterInfo.allChallenges += challenges.Count;
				}
			}
		}
	}

	public void SetLocalScore()
	{
		foreach (ChapterInfo chapterInfo in this.chapterList)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
			{
				foreach (ChapterLevelInfo chapterLevelInfo in chapterLevelGroup.mapList)
				{
					if (Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore.ContainsKey(chapterLevelInfo.id))
					{
						chapterLevelInfo.Score = Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[chapterLevelInfo.id];
						chapterLevelGroup.ModifyLevel(chapterLevelInfo);
					}
				}
			}
		}
		foreach (ChapterInfo chapterInfo2 in this.payChapterList)
		{
			foreach (ChapterLevelGroup chapterLevelGroup2 in chapterInfo2.levelList)
			{
				foreach (ChapterLevelInfo chapterLevelInfo2 in chapterLevelGroup2.mapList)
				{
					if (Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore.ContainsKey(chapterLevelInfo2.id))
					{
						chapterLevelInfo2.Score = Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[chapterLevelInfo2.id];
						chapterLevelGroup2.ModifyLevel(chapterLevelInfo2);
					}
				}
			}
		}
		foreach (ChapterInfo chapterInfo3 in this.coopertionList)
		{
			foreach (ChapterLevelGroup chapterLevelGroup3 in chapterInfo3.levelList)
			{
				foreach (ChapterLevelInfo chapterLevelInfo3 in chapterLevelGroup3.mapList)
				{
					if (Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore.ContainsKey(chapterLevelInfo3.id))
					{
						chapterLevelInfo3.Score = Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[chapterLevelInfo3.id];
						chapterLevelGroup3.ModifyLevel(chapterLevelInfo3);
					}
				}
			}
		}
	}

	public void AfterAccountLogin()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("AfterAccountLogin", new object[0]);
		foreach (ChapterInfo chapterInfo in this.chapterList)
		{
			int @int = PlayerPrefs.GetInt(string.Format("Chapter_id_passd_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo.id));
			chapterInfo.hasPassed = (@int == 1);
			if (PlayerPrefs.GetInt(string.Format("Chapter_id_is_watting_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo.id), -1) == 1)
			{
				chapterInfo.isWattingUnlock = true;
			}
			else
			{
				chapterInfo.isWattingUnlock = false;
			}
		}
		foreach (ChapterInfo chapterInfo2 in this.payChapterList)
		{
			int int2 = PlayerPrefs.GetInt(string.Format("Chapter_id_passd_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo2.id));
			chapterInfo2.hasPassed = (int2 == 1);
			if (PlayerPrefs.GetInt(string.Format("Chapter_id_is_watting_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo2.id), -1) == 1)
			{
				chapterInfo2.isWattingUnlock = true;
			}
			else
			{
				chapterInfo2.isWattingUnlock = false;
			}
		}
		foreach (ChapterInfo chapterInfo3 in this.coopertionList)
		{
			int int3 = PlayerPrefs.GetInt(string.Format("Chapter_id_passd_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo3.id));
			chapterInfo3.hasPassed = (int3 == 1);
			if (PlayerPrefs.GetInt(string.Format("Chapter_id_is_watting_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, chapterInfo3.id), -1) == 1)
			{
				chapterInfo3.isWattingUnlock = true;
			}
			else
			{
				chapterInfo3.isWattingUnlock = false;
			}
		}
	}

	public void SetLevelStar(string achieveId, bool success)
	{
		AchievementConfig achievementConfig = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[achieveId];
		string groupId = Solarmax.Singleton<AchievementModel>.Get().dicAchievements[achieveId].groupId;
		if (Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(groupId) != null)
		{
			ChapterLevelInfo level = Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(groupId).GetLevel(achievementConfig.difficult);
			level.star = AchievementModel.GetCompletedStarsByDiff(groupId, achievementConfig.difficult);
		}
	}

	public ChapterInfo QueryChapterInfo(string id)
	{
		return this.GetChapterInfo(id);
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == 98)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  OnEventHandler  OnLoadChaptersResult", new object[0]);
			for (int i = 0; i < this.chapterList.Count; i++)
			{
				string id = this.chapterList[i].id;
				int star = this.chapterList[i].star;
				int achieveLevels = this.chapterList[i].achieveLevels;
				this.SetChapterStars(id, star, achieveLevels);
			}
			this.UnlockChaptersbyStar();
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChaptersView, new object[]
			{
				1
			});
			return;
		}
		if (eventId != 99)
		{
			if (eventId == 101)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  OnEventHandler  OnSetLevelStarResult", new object[0]);
				string chapterID = (string)args[0];
				string levelId = (string)args[1];
				int star2 = (int)args[2];
				int score = (int)args[3];
				this.SetChapterLevelStarLocal(chapterID, levelId, star2, score);
			}
			return;
		}
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  OnEventHandler  OnLoadOneChaptersResult", new object[0]);
		string text = args[0] as string;
		ChapterInfo chapterInfo = this.GetChapterInfo(text);
		if (chapterInfo == null)
		{
			return;
		}
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		for (int j = 0; j < chapterInfo.levelList.Count; j++)
		{
			string id2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetDataByChapter(text).id;
			ChapterLevelInfo level2 = this.GetLevel(id2);
			int star3 = level2.star;
			int score2 = level2.Score;
			this.SetChapterLevelStarLocal(chapterInfo, id2, star3, score2);
			if (!dictionary.ContainsKey(id2))
			{
				dictionary.Add(id2, 0);
			}
		}
		int num = 0;
		foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
		{
			using (List<ChapterLevelInfo>.Enumerator enumerator2 = chapterLevelGroup.mapList.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ChapterLevelInfo level = enumerator2.Current;
					if (Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore.ContainsKey(level.id) && !dictionary.ContainsKey(level.id))
					{
						global::Coroutine.DelayDo((float)(++num), new EventDelegate(delegate()
						{
							LevelConfig data2 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(level.id);
							int num2 = Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[level.id];
							Solarmax.Singleton<NetSystem>.Instance.helper.RequestSetLevelSorce(data2.id, data2.levelGroup, Solarmax.Singleton<LocalAccountStorage>.Get().account, num2);
							Solarmax.Singleton<NetSystem>.Instance.helper.SetLevelStar(this.currentChapter.id, data2.id, level.star, num2);
						}));
					}
				}
			}
		}
		this.UnlockLevels(chapterInfo);
		chapterInfo.isNetSync = true;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
		{
			1,
			text
		});
		global::Coroutine.DelayDo(0.1f, new EventDelegate(delegate()
		{
			this.InChapterClaimReward();
		}));
	}

	public ChapterInfo GetChapterInfo(string chapterId)
	{
		ChapterInfo chapterInfo = null;
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			if (this.chapterList[i].id.Equals(chapterId))
			{
				chapterInfo = this.chapterList[i];
				break;
			}
		}
		if (chapterInfo != null)
		{
			return chapterInfo;
		}
		if (chapterInfo == null)
		{
			for (int j = 0; j < this.payChapterList.Count; j++)
			{
				if (this.payChapterList[j].id.Equals(chapterId))
				{
					chapterInfo = this.payChapterList[j];
					break;
				}
			}
		}
		if (chapterInfo != null)
		{
			return chapterInfo;
		}
		for (int k = 0; k < this.coopertionList.Count; k++)
		{
			if (this.coopertionList[k].id.Equals(chapterId))
			{
				chapterInfo = this.coopertionList[k];
				break;
			}
		}
		return chapterInfo;
	}

	public ChapterInfo GetPayChapterInfo(string chapterId)
	{
		ChapterInfo result = null;
		for (int i = 0; i < this.payChapterList.Count; i++)
		{
			if (this.payChapterList[i].id.Equals(chapterId))
			{
				result = this.payChapterList[i];
				break;
			}
		}
		return result;
	}

	public void UnlockChapters(bool auto = true)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler   UnlockChapters", new object[0]);
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			ChapterInfo chapterInfo = this.chapterList[i];
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
			LoggerSystem.CodeComments("change chapter unlock here");
			bool flag = true;
			if (!string.IsNullOrEmpty(data.dependChapter))
			{
				ChapterInfo chapterInfo2 = this.GetChapterInfo(data.dependChapter);
				if (chapterInfo2 != null)
				{
					flag = this.GetChapterInfo(data.dependChapter).achieveMainLine;
					if (chapterInfo2.star >= data.needStar && flag)
					{
						if (!chapterInfo.unLock && auto)
						{
							Solarmax.Singleton<LevelDataHandler>.Get().SaveNextChapterFirstUnlock();
							PlayerPrefs.SetInt("Chapter_unlock_tips", 1);
						}
						chapterInfo.unLock = true;
					}
				}
			}
			else if (flag)
			{
				if (!chapterInfo.unLock && auto)
				{
					Solarmax.Singleton<LevelDataHandler>.Get().SaveNextChapterFirstUnlock();
					Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2054), 3f);
				}
				chapterInfo.unLock = true;
			}
		}
	}

	public void UnlockChaptersbyStar()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  UnlockChaptersbyStar", new object[0]);
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			ChapterInfo chapterInfo = this.chapterList[i];
			if (chapterInfo != null && chapterInfo.star > 0)
			{
				chapterInfo.unLock = true;
			}
			else
			{
				ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
				if (!string.IsNullOrEmpty(data.dependChapter))
				{
					ChapterInfo chapterInfo2 = this.GetChapterInfo(data.dependChapter);
					if (chapterInfo2 != null)
					{
						bool achieveMainLine = chapterInfo2.achieveMainLine;
						if (chapterInfo2.star >= data.needStar && achieveMainLine)
						{
							chapterInfo.unLock = true;
						}
					}
				}
			}
		}
	}

	public void UnLockPayChapter(string chapterID)
	{
		if (string.IsNullOrEmpty(chapterID))
		{
			return;
		}
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			if (this.chapterList[i].id.Equals(chapterID))
			{
				this.chapterList[i].unLock = true;
				this.payMainList.Add(this.chapterList[i]);
				return;
			}
		}
		for (int j = 0; j < this.payChapterList.Count; j++)
		{
			if (this.payChapterList[j].id.Equals(chapterID))
			{
				this.payChapterList[j].unLock = true;
				this.payMainList.Add(this.payChapterList[j]);
				return;
			}
		}
		for (int k = 0; k < this.coopertionList.Count; k++)
		{
			if (this.coopertionList[k].id.Equals(chapterID))
			{
				this.coopertionList[k].unLock = true;
				this.payMainList.Add(this.coopertionList[k]);
				return;
			}
		}
	}

	public bool IsBuyChapter(string chapterID)
	{
		return true;
	}

	public void ResetChapterStars()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler   ResetChapterStars", new object[0]);
		for (int i = 0; i < this.chapterList.Count; i++)
		{
			ChapterInfo chapterInfo = this.chapterList[i];
			if (chapterInfo == null)
			{
				return;
			}
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
			if (data == null)
			{
				return;
			}
			if (data.type == 0)
			{
				this.allStars = AchievementModel.GetALLCompletedStars();
				int num = 0;
				int num2 = 0;
				foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
				{
					num += AchievementModel.GetCompletedStars(chapterLevelGroup.groupID);
					num2 += AchievementModel.GetCompletedChallenges(chapterLevelGroup.groupID);
				}
				chapterInfo.star = num;
				chapterInfo.completedChallenges = num2;
			}
		}
	}

	private void SetChapterStars(string chapterId, int star, int finishedLevelNum)
	{
		ChapterInfo chapterInfo = this.GetChapterInfo(chapterId);
		if (chapterInfo == null)
		{
			return;
		}
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
		if (data == null)
		{
			return;
		}
		int num = 0;
		if (data.type == 0)
		{
			this.allStars = AchievementModel.GetALLCompletedStars();
			int num2 = 0;
			foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
			{
				int completedStars = AchievementModel.GetCompletedStars(chapterLevelGroup.groupID);
				num2 += completedStars;
				foreach (ChapterLevelInfo chapterLevelInfo in chapterLevelGroup.mapList)
				{
					if (Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore.ContainsKey(chapterLevelInfo.id))
					{
						num++;
					}
				}
				chapterLevelGroup.star = completedStars;
				if (chapterLevelGroup.star > 0)
				{
					chapterLevelGroup.unLock = true;
				}
			}
			chapterInfo.star = num2;
			int num3 = 0;
			foreach (ChapterLevelGroup chapterLevelGroup2 in chapterInfo.levelList)
			{
				int completedChallenges = AchievementModel.GetCompletedChallenges(chapterLevelGroup2.groupID);
				num3 += completedChallenges;
			}
			chapterInfo.completedChallenges = num3;
		}
		else
		{
			chapterInfo.star = star;
		}
		if (num > finishedLevelNum)
		{
			finishedLevelNum = num;
		}
		chapterInfo.achieveLevels = finishedLevelNum;
		if (finishedLevelNum >= chapterInfo.mainLineNum)
		{
			chapterInfo.achieveMainLine = true;
		}
		bool flag = true;
		if (data.type > 0)
		{
			return;
		}
		if (!string.IsNullOrEmpty(data.dependChapter))
		{
			ChapterInfo chapterInfo2 = this.GetChapterInfo(data.dependChapter);
			if (chapterInfo2 != null)
			{
				flag = this.GetChapterInfo(data.dependChapter).achieveMainLine;
				if (chapterInfo2.star >= data.needStar && flag)
				{
					chapterInfo.unLock = true;
				}
			}
		}
		else if (flag)
		{
			chapterInfo.unLock = true;
		}
	}

	public int GetChapterCompletedChallenges()
	{
		int num = 0;
		foreach (ChapterInfo chapterInfo in this.chapterList)
		{
			num += chapterInfo.completedChallenges;
		}
		return num;
	}

	private void SetChapterLevelStarLocal(ChapterInfo chapter, string levelId, int star, int Score)
	{
		if (chapter == null)
		{
			return;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelId);
		if (data == null)
		{
			return;
		}
		string text = data.levelGroup;
		if (string.IsNullOrEmpty(text))
		{
			text = data.id;
		}
		ChapterLevelGroup chapterLevelGroup = this.FindGroupLevel(text);
		if (chapterLevelGroup == null)
		{
			return;
		}
		ChapterLevelInfo level = this.GetLevel(levelId);
		if (level == null)
		{
			return;
		}
		ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapter.id);
		if (data2.type != 2)
		{
			if (level.star <= star)
			{
				level.star = star;
			}
			if (level.Score <= Score)
			{
				level.Score = Score;
			}
		}
		else
		{
			level.star = star;
			level.Score = Score;
		}
		if (star > 0)
		{
			level.unLock = true;
		}
		chapterLevelGroup.ModifyLevel(level);
	}

	public void SetChapterLevelStarLocal(string chapterID, string levelId, int star, int Score)
	{
		if (string.IsNullOrEmpty(chapterID))
		{
			return;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelId);
		if (data == null)
		{
			return;
		}
		string text = data.levelGroup;
		if (string.IsNullOrEmpty(text))
		{
			text = data.id;
		}
		ChapterLevelGroup chapterLevelGroup = this.FindGroupLevel(text);
		if (chapterLevelGroup == null)
		{
			return;
		}
		ChapterLevelInfo level = this.GetLevel(levelId);
		if (level == null)
		{
			return;
		}
		level.star = star;
		level.Score = Score;
		level.unLock = true;
		chapterLevelGroup.ModifyLevel(level);
	}

	private void UnlockLevels(ChapterInfo chapter)
	{
		for (int i = 0; i < chapter.levelList.Count; i++)
		{
			chapter.levelList[i].UnLock();
		}
	}

	private void GenerateAchievement(ChapterInfo chapter)
	{
		chapter.achieveLevels = 0;
		chapter.achieveMainLine = true;
		for (int i = 0; i < chapter.levelList.Count; i++)
		{
			ChapterLevelGroup chapterLevelGroup = chapter.levelList[i];
			if (chapterLevelGroup.star > 0)
			{
				chapter.achieveLevels++;
			}
			if (chapterLevelGroup.isMain)
			{
				chapter.achieveMainLine &= (chapterLevelGroup.star > 0);
				if (!chapter.achieveMainLine)
				{
					return;
				}
			}
		}
	}

	public void SetLevelStarToLocalStorage(int star, int Score)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler   SetLevelStarToLocalStorage", new object[0]);
		if (this.currentLevel != null)
		{
			if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.currentChapter.id).type == 0)
			{
				star = AchievementModel.GetCompletedStars(this.GetCurrentGroupID());
			}
			bool flag = false;
			if (this.currentLevel.star < star || this.currentLevel.Score < Score)
			{
				if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.currentChapter.id).type == 0)
				{
					this.allStars = AchievementModel.GetALLCompletedStars();
				}
				if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.currentChapter.id).type == 0)
				{
					int num = 0;
					foreach (ChapterLevelGroup chapterLevelGroup in this.currentChapter.levelList)
					{
						num += AchievementModel.GetCompletedStars(chapterLevelGroup.groupID);
					}
					this.currentChapter.star = num;
					num = 0;
					foreach (ChapterLevelGroup chapterLevelGroup2 in this.currentChapter.levelList)
					{
						num += AchievementModel.GetCompletedChallenges(chapterLevelGroup2.groupID);
					}
					this.currentChapter.completedChallenges = num;
				}
				else
				{
					this.currentChapter.star += star - this.currentLevel.star;
				}
				if (this.currentLevel.star < star)
				{
					this.currentLevel.star = star;
				}
				if (this.currentLevel.Score < Score)
				{
					this.currentLevel.Score = Score;
				}
				this.currentLevel.unLock = true;
				ChapterLevelGroup chapterLevelGroup3 = this.currentChapter.levelList[this.currentLevelIndex];
				if (chapterLevelGroup3.Score < this.currentLevel.Score)
				{
					chapterLevelGroup3.Score = this.currentLevel.Score;
					if (Solarmax.Singleton<LevelDataHandler>.Get().onLevelScore != null)
					{
						Solarmax.Singleton<LevelDataHandler>.Get().onLevelScore(chapterLevelGroup3.groupID);
					}
				}
				flag = true;
			}
			this.UnlockLevels(this.currentChapter);
			if (flag)
			{
				this.GenerateAchievement(this.currentChapter);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateStar, new object[0]);
			}
			this.UnlockChapters(true);
		}
	}

	public bool IsNeedSend(int star, int Score)
	{
		return this.currentLevel != null && (this.currentLevel.star < star || this.currentLevel.Score < Score);
	}

	public bool IsNeedGiveRaward(string strLevel)
	{
		if (string.IsNullOrEmpty(strLevel))
		{
			return false;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(strLevel);
		if (data == null)
		{
			return false;
		}
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(data.chapter);
		if (chapterInfo != null && !chapterInfo.unLock)
		{
			return false;
		}
		string dependLevel = data.dependLevel;
		return (Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(dependLevel) == null && chapterInfo.unLock) || (chapterInfo != null && chapterInfo.unLock && this.IsUnLock(dependLevel));
	}

	public int QueryLevelStar(string levelId)
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelId);
		if (data == null)
		{
			return 0;
		}
		string text = data.levelGroup;
		if (string.IsNullOrEmpty(text))
		{
			text = data.id;
		}
		ChapterLevelGroup chapterLevelGroup = this.FindGroupLevel(text);
		if (chapterLevelGroup == null)
		{
			return 0;
		}
		return chapterLevelGroup.star;
	}

	public bool IsUnLock(string levelId)
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelId);
		if (data == null)
		{
			return false;
		}
		string text = data.levelGroup;
		if (string.IsNullOrEmpty(text))
		{
			text = data.id;
		}
		ChapterLevelGroup chapterLevelGroup = this.FindGroupLevel(text);
		return chapterLevelGroup != null && chapterLevelGroup.unLock && chapterLevelGroup.star > 0;
	}

	public ChapterLevelGroup GetLevelInfo(ChapterInfo chapter, string levelId)
	{
		if (chapter == null)
		{
			return null;
		}
		List<ChapterLevelGroup> levelList = chapter.levelList;
		for (int i = 0; i < levelList.Count; i++)
		{
			if (levelId.Equals(levelList[i].groupID))
			{
				return levelList[i];
			}
		}
		return null;
	}

	public void SetSelectChapter(string chapterId)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  SetSelectChapter", new object[0]);
		ChapterInfo chapterInfo = this.GetChapterInfo(chapterId);
		if (chapterInfo != null)
		{
			int index = 0;
			for (int i = 0; i < this.chapterList.Count; i++)
			{
				if (this.chapterList[i].id.Equals(chapterId))
				{
					index = i;
					break;
				}
			}
			this.SaveCurrentChapterIndex(index);
			this.currentChapter = chapterInfo;
			if (this.currentChapter.isNetSync)
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
				{
					1,
					chapterId
				});
				return;
			}
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestOneChapter(chapterId);
		}
	}

	private void InChapterClaimReward()
	{
		List<string> list = new List<string>();
		if (this.currentChapter.type == 0)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in this.currentChapter.levelList)
			{
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(chapterLevelGroup.groupID))
				{
					AchievementGroup achievementGroup = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[chapterLevelGroup.groupID];
					foreach (Achievement achievement in achievementGroup.achievements)
					{
						if (achievement.success && achievement.types[0] == AchievementType.PassDiffcult && Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(achievement.taskId))
						{
							TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Get().dataList[achievement.taskId];
							if (taskConfig.status != TaskStatus.Received)
							{
								list.Add(taskConfig.id);
							}
						}
					}
				}
			}
		}
		if (list.Count > 0)
		{
			Solarmax.Singleton<TaskModel>.Get().ClaimAllReward(list, null, 1);
		}
	}

	public void SaveCurrentChapterIndex(int index)
	{
		if (index >= 0 && this.currentChapterIndex != index)
		{
			this.currentChapterIndex = index;
			PlayerPrefs.SetInt(string.Format("Chapter_id_{0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.userId.ToString()), index);
		}
	}

	public void SetSelectLevel(string groupID, int nDif = 0)
	{
		if (string.IsNullOrEmpty(groupID))
		{
			Debug.LogError("levelId为空");
			return;
		}
		if (this.currentChapter == null)
		{
			return;
		}
		for (int i = 0; i < this.currentChapter.levelList.Count; i++)
		{
			if (this.currentChapter.levelList[i].groupID.Equals(groupID))
			{
				ChapterLevelInfo level = this.currentChapter.levelList[i].GetLevel(nDif);
				if (level != null)
				{
					this.currentLevelIndex = i;
					this.currentLevel = level;
					return;
				}
			}
		}
		if (this.currentLevel == null)
		{
			Debug.LogError("获取当前关卡失败:" + groupID);
		}
	}

	public void SetSelectLevel(string LevelID, string groudID)
	{
		if (this.currentChapter == null)
		{
			return;
		}
		for (int i = 0; i < this.currentChapter.levelList.Count; i++)
		{
			if (this.currentChapter.levelList[i].groupID.Equals(groudID))
			{
				this.currentLevelIndex = i;
			}
		}
		ChapterLevelInfo level = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevel(LevelID);
		if (level != null)
		{
			this.currentLevel = level;
		}
	}

	public bool BeUnlockChapter(string chapterId)
	{
		ChapterInfo chapterInfo = this.GetChapterInfo(chapterId);
		return chapterInfo != null && chapterInfo.unLock;
	}

	public ChapterLevelGroup GetNextLevelInfo()
	{
		if (this.currentLevelIndex == this.currentChapter.levelList.Count - 1)
		{
			return null;
		}
		return this.currentChapter.levelList[this.currentLevelIndex + 1];
	}

	public int GetChapterFirstNoFullStarsLevel(ChapterInfo chapterInfo)
	{
		for (int i = 0; i < chapterInfo.levelList.Count; i++)
		{
			ChapterLevelGroup chapterLevelGroup = chapterInfo.levelList[i];
			int completedStars = AchievementModel.GetCompletedStars(chapterLevelGroup.groupID);
			if (chapterLevelGroup.unLock && completedStars != chapterLevelGroup.maxStar)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetPayChapterFirstNoFullStarsLevel(ChapterInfo chapterInfo)
	{
		for (int i = 0; i < chapterInfo.levelList.Count; i++)
		{
			ChapterLevelGroup chapterLevelGroup = chapterInfo.levelList[i];
			if (chapterLevelGroup.unLock && chapterLevelGroup.star != chapterLevelGroup.maxStar)
			{
				return i;
			}
		}
		return -1;
	}

	public int GetChapterFirstNoStarsLevel(ChapterInfo chapterInfo)
	{
		for (int i = 0; i < chapterInfo.levelList.Count; i++)
		{
			ChapterLevelGroup chapterLevelGroup = chapterInfo.levelList[i];
			if (chapterLevelGroup.unLock && chapterLevelGroup.star == 0)
			{
				return i;
			}
		}
		return 0;
	}

	public ChapterLevelGroup GetLevelByIndex(int index)
	{
		if (index < 0 || this.currentChapter == null || index >= this.currentChapter.levelList.Count)
		{
			return null;
		}
		return this.currentChapter.levelList[index];
	}

	public void SaveNextChapterFirstUnlock()
	{
		this.SaveChapterIsWattingUnlock(this.currentChapterIndex + 1, true);
	}

	public void SaveChapterIsWattingUnlock(int index, bool waitting)
	{
		if (index >= this.chapterList.Count || index < 0)
		{
			return;
		}
		if (this.chapterList[index] == null)
		{
			return;
		}
		if (waitting)
		{
			PlayerPrefs.SetInt(string.Format("Chapter_id_is_watting_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, this.chapterList[index].id), 1);
		}
		else
		{
			PlayerPrefs.SetInt(string.Format("Chapter_id_is_watting_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, this.chapterList[index].id), 0);
		}
		this.chapterList[index].isWattingUnlock = waitting;
	}

	public void SaveNextLevelFirstUnlock()
	{
		PlayerPrefs.SetInt(string.Format("Level_id_is_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, this.currentChapter.levelList[this.currentLevelIndex].groupID), 1);
	}

	public bool NextLevelIsFirstUnlock()
	{
		int @int = PlayerPrefs.GetInt(string.Format("Level_id_is_unlock_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, this.currentChapter.levelList[this.currentLevelIndex].groupID), -1);
		return @int == -1;
	}

	public void SaveChapterAnimationStatus()
	{
		this.currentChapter.hasPassed = true;
		PlayerPrefs.SetInt(string.Format("Chapter_id_passd_{0}{1}", Solarmax.Singleton<LocalAccountStorage>.Get().account, this.currentChapter.id), 1);
	}

	public bool GetChapterAnimationStatus()
	{
		return this.currentChapter.hasPassed;
	}

	public bool CanPlayPassedChapterAnimator()
	{
		if (this.GetChapterAnimationStatus())
		{
			return false;
		}
		foreach (ChapterLevelGroup chapterLevelGroup in this.currentChapter.levelList)
		{
			if (chapterLevelGroup.star == 0 && chapterLevelGroup.isMain)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanPlayPassedEvaluation()
	{
		foreach (ChapterLevelGroup chapterLevelGroup in this.currentChapter.levelList)
		{
			if (chapterLevelGroup.star == 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool CanUnlockNextLevel()
	{
		ChapterLevelGroup nextLevelInfo = this.GetNextLevelInfo();
		if (nextLevelInfo != null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("LevelDataHandler   CanUnlockNextLevel   conditions: {0},{1},{2},{3}", new object[]
			{
				nextLevelInfo != null,
				nextLevelInfo.star == 0,
				nextLevelInfo.unLock,
				this.NextLevelIsFirstUnlock()
			}), new object[0]);
		}
		else
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LevelDataHandler  CanUnlockNextLevel  next null", new object[0]);
		}
		return nextLevelInfo != null && nextLevelInfo.star == 0 && nextLevelInfo.unLock && this.NextLevelIsFirstUnlock();
	}

	public ChapterLevelInfo GetLevel(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return null;
		}
		if (!this.dicLevels.ContainsKey(levelId))
		{
			return null;
		}
		return this.dicLevels[levelId];
	}

	public ChapterLevelInfo GetLevelByName(string levelName)
	{
		foreach (KeyValuePair<string, ChapterLevelInfo> keyValuePair in this.dicLevels)
		{
			LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(keyValuePair.Key);
			if (data.levelName.ToString() == levelName)
			{
				return keyValuePair.Value;
			}
		}
		return null;
	}

	public void ClaimedLevelTaskStatus(string levelId, int star)
	{
		if (!this.dicLevels.ContainsKey(levelId))
		{
			return;
		}
	}

	public bool HaveLevelTaskCompleted()
	{
		return false;
	}

	public string GetCurrentGroupID()
	{
		if (this.currentChapter == null || this.currentLevelIndex < 0 || this.currentLevelIndex >= this.currentChapter.levelList.Count || this.currentChapter.levelList[this.currentLevelIndex] == null)
		{
			return string.Empty;
		}
		return this.currentChapter.levelList[this.currentLevelIndex].groupID;
	}

	public int GetCurrentDiffcult()
	{
		if (this.currentLevel != null)
		{
			return this.currentLevel.difficult;
		}
		return 0;
	}

	public void RefrushChapterInfo(chapterScore score)
	{
		if (score != null && !string.IsNullOrEmpty(score.chapter_id))
		{
			ChapterInfo payChapterInfo = this.GetPayChapterInfo(score.chapter_id);
			if (payChapterInfo == null)
			{
				return;
			}
			payChapterInfo.fStrategyStart = score.strategy_score;
			payChapterInfo.fInterestStart = score.interest_score;
			payChapterInfo.ftotalStart = score.total_score;
			payChapterInfo.nBuyCount = score.buy_count;
		}
	}

	public void RefrushChapterInfoPrice(discountChapter score)
	{
		if (score != null && !string.IsNullOrEmpty(score.chapter_id))
		{
			ChapterInfo payChapterInfo = this.GetPayChapterInfo(score.chapter_id);
			if (payChapterInfo == null)
			{
				return;
			}
			payChapterInfo.nPromotionPrice = score.gold;
		}
	}

	public void InitEvaluationChapter(List<string> server)
	{
		this.evaluationMap.Clear();
		for (int i = 0; i < server.Count; i++)
		{
			this.evaluationMap.Add(server[i], true);
		}
	}

	public void AddEvaluation(string chapterID)
	{
		bool flag = false;
		if (!this.evaluationMap.TryGetValue(chapterID, out flag))
		{
			this.evaluationMap.Add(chapterID, true);
		}
	}

	public bool IsEvaluationed(string chapterID)
	{
		bool result = false;
		this.evaluationMap.TryGetValue(chapterID, out result);
		return result;
	}

	public bool IsHotChapter(string chapterID)
	{
		for (int i = 0; i < this.topChapterList.Count; i++)
		{
			if (chapterID.Equals(this.topChapterList[i]))
			{
				return true;
			}
		}
		return false;
	}

	public void AddBuyChapterInfo(string chapterID)
	{
		if (!string.IsNullOrEmpty(chapterID))
		{
			ChapterInfo payChapterInfo = this.GetPayChapterInfo(chapterID);
			if (payChapterInfo == null)
			{
				return;
			}
			payChapterInfo.nBuyCount++;
		}
	}

	private const string SAVE_LEVEL_KEY_IS_UNLOCK = "Level_id_is_unlock_{0}{1}";

	private const string SAVE_CHAPTER_PASSED = "Chapter_id_passd_{0}{1}";

	private const string SAVE_CHAPTER_KEY = "Chapter_id_{0}";

	private const string SAVE_CHAPTER_KEY_IS_WATTING_UNLOCK = "Chapter_id_is_watting_unlock_{0}{1}";

	public List<ChapterInfo> chapterList;

	public Dictionary<string, ChapterInfo> dicChapter;

	public Dictionary<string, bool> evaluationMap = new Dictionary<string, bool>();

	public List<ChapterInfo> payChapterList;

	public List<ChapterInfo> payMainList;

	public List<ChapterInfo> coopertionList;

	public List<string> topChapterList = new List<string>();

	public int allStars;

	public int currentChapterIndex = -1;

	public ChapterInfo currentChapter;

	public int currentLevelIndex = -1;

	public ChapterLevelInfo currentLevel;

	private Dictionary<string, ChapterLevelInfo> dicLevels;

	private Dictionary<string, ChapterLevelGroup> groupLevels;

	public LevelDataHandler.OnLevelScore onLevelScore;

	public delegate void OnLevelScore(string group);
}
