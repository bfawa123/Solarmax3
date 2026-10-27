using System;
using System.Collections.Generic;
using Solarmax;

public class AchievementModel : Solarmax.Singleton<AchievementModel>
{
	public void Init(bool needPull = true)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("AchievementModel  Init", new object[0]);
		foreach (AchievementListConfig achievementListConfig in Solarmax.Singleton<AchievementListProvider>.Get().dataList.Values)
		{
			AchievementGroup achievementGroup = new AchievementGroup();
			achievementGroup.groupid = achievementListConfig.levelGroup;
			achievementGroup.achievements = new List<Achievement>();
			achievementGroup.dicAchievements = new Dictionary<AchievementDifficult, List<Achievement>>();
			achievementGroup.idAchievements = new Dictionary<string, Achievement>();
			foreach (string key in achievementListConfig.achieveList)
			{
				AchievementConfig achievementConfig = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList[key];
				Achievement achievement = new Achievement();
				achievement.id = achievementConfig.id;
				achievement.success = Solarmax.Singleton<LocalAchievementStorage>.Get().GetStatus(achievement.id);
				achievement.levelDesc = achievementConfig.levelDesc;
				achievement.chapterDesc = achievementConfig.chapterDesc;
				achievement.diffcult = (AchievementDifficult)achievementConfig.difficult;
				achievement.config = achievementConfig;
				achievement.achieveSuccess = new List<bool>();
				achievement.types = new List<AchievementType>();
				achievement.currentCompleted = new List<int>();
				achievement.groupId = achievementGroup.groupid;
				achievement.levelNameId = achievementConfig.levelNameId.ToString();
				achievement.taskId = achievementConfig.taskId;
				foreach (int num in achievementConfig.types)
				{
					achievement.types.Add((AchievementType)num);
					if (num == 8)
					{
						achievement.achieveSuccess.Add(true);
					}
					else
					{
						achievement.achieveSuccess.Add(false);
					}
					achievement.currentCompleted.Add(0);
				}
				achievementGroup.achievements.Add(achievement);
				if (!achievementGroup.dicAchievements.ContainsKey(achievement.diffcult))
				{
					achievementGroup.dicAchievements[achievement.diffcult] = new List<Achievement>();
				}
				achievementGroup.dicAchievements[achievement.diffcult].Add(achievement);
				achievementGroup.idAchievements[achievement.id] = achievement;
				this.dicAchievements[achievement.id] = achievement;
			}
			this.achievementGroups[achievementGroup.groupid] = achievementGroup;
		}
		foreach (KeyValuePair<string, Achievement> keyValuePair in this.dicAchievements)
		{
			if (keyValuePair.Value.success)
			{
				Solarmax.Singleton<LevelDataHandler>.Get().SetLevelStar(keyValuePair.Key, keyValuePair.Value.success);
			}
		}
		LevelDataHandler.AddDelegate();
		if (needPull)
		{
			this.PullAllAchievements();
		}
		Solarmax.Singleton<LevelDataHandler>.Get().SetChapterStar();
		Solarmax.Singleton<LevelDataHandler>.Get().SetChapterChallenges();
		Solarmax.Singleton<LevelDataHandler>.Get().ResetChapterStars();
		Solarmax.Singleton<LevelDataHandler>.Get().UnlockChapters(false);
		Solarmax.Singleton<LevelDataHandler>.Get().UnlockChaptersbyStar();
	}

	public void PullAllAchievements()
	{
		float num = 0f;
		AchievementModel.requestCount = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList.Count;
		AchievementModel.responseCount = 0;
		AchievementModel.achieveDic.Clear();
		foreach (ChapterInfo chapterInfo in Solarmax.Singleton<LevelDataHandler>.Instance.chapterList)
		{
			List<string> groupIds = new List<string>();
			foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
			{
				groupIds.Add(chapterLevelGroup.groupID);
			}
			num += 0.1f;
			Coroutine.DelayDo(num, new EventDelegate(delegate()
			{
				this.RequestAchievement(groupIds);
			}));
		}
	}

	public void RequestAchievement(List<string> groupIds)
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.PullAchievements(groupIds);
	}

	public void SendAchievement(string group, string achieveId)
	{
		List<string> list = new List<string>();
		list.Add(achieveId);
		Solarmax.Singleton<NetSystem>.Instance.helper.SendAchievementSet(group, list);
	}

	public void SendAchievementWithoutReward(string group, string achieveId)
	{
		List<string> list = new List<string>();
		list.Add(achieveId);
		Solarmax.Singleton<NetSystem>.Instance.helper.SendAchievementSet(group, list);
	}

	public void SendAchievement(string group, List<string> achieveIds)
	{
		if (achieveIds.Count == 0)
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.SendAchievementSet(group, achieveIds);
	}

	public void SetAchievement(string groupID, string achieveID, bool success, bool claimTask = false)
	{
		if (this.achievementGroups.ContainsKey(groupID) && this.achievementGroups[groupID].idAchievements.ContainsKey(achieveID))
		{
			this.achievementGroups[groupID].idAchievements[achieveID].success = success;
			if (claimTask && this.achievementGroups[groupID].idAchievements[achieveID].types[0] == AchievementType.PassDiffcult)
			{
				TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(this.dicAchievements[achieveID].taskId);
				if (task != null && task.status != TaskStatus.Received)
				{
					Solarmax.Singleton<TaskModel>.Get().ClaimReward(task.id, null, 1);
				}
			}
		}
	}

	public static int GetGroupStars(string levelGroup)
	{
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(levelGroup))
		{
			int num = 0;
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[levelGroup].achievements)
			{
				if (achievement.success && achievement.types[0] == AchievementType.PassDiffcult)
				{
					num++;
				}
			}
			return num;
		}
		return 1;
	}

	public static int GetCompletedStars(string levelGroup)
	{
		int num = 1;
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(levelGroup))
		{
			num = 0;
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[levelGroup].achievements)
			{
				if (achievement.success && achievement.types[0] == AchievementType.PassDiffcult)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static int GetCompletedChallenges(string levelGroup)
	{
		int num = 1;
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(levelGroup))
		{
			num = 0;
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[levelGroup].achievements)
			{
				if (achievement.success && achievement.types[0] != AchievementType.PassDiffcult && achievement.types[0] != AchievementType.Ads)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static int GetALLCompletedStars()
	{
		int num = 0;
		foreach (KeyValuePair<string, AchievementGroup> keyValuePair in Solarmax.Singleton<AchievementModel>.Get().achievementGroups)
		{
			num += AchievementModel.GetCompletedStars(keyValuePair.Key);
		}
		return num;
	}

	public static int GetCompletedStarsByDiff(string groupID, int diff)
	{
		int num = 0;
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(groupID))
		{
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[groupID].achievements)
			{
				if (achievement.diffcult == (AchievementDifficult)diff && achievement.success)
				{
					num++;
				}
			}
		}
		return num;
	}

	public static void GetDiffcultStar(string groupID, out int all, out int completed)
	{
		all = 0;
		completed = 0;
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(groupID))
		{
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[groupID].achievements)
			{
				if (achievement.types[0] == AchievementType.PassDiffcult)
				{
					all++;
					if (achievement.success)
					{
						completed++;
					}
				}
			}
		}
	}

	public static List<Achievement> GetChallenges(string groupID)
	{
		List<Achievement> list = new List<Achievement>();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(groupID))
		{
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[groupID].achievements)
			{
				if (achievement.types[0] != AchievementType.PassDiffcult)
				{
					if (achievement.types[0] != AchievementType.Ads)
					{
						list.Add(achievement);
					}
				}
			}
		}
		return list;
	}

	public static bool CheckUnRecievedTask(string levelGroup)
	{
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(levelGroup))
		{
			foreach (Achievement achievement in Solarmax.Singleton<AchievementModel>.Get().achievementGroups[levelGroup].achievements)
			{
				if (Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(achievement.taskId) != null && Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(achievement.taskId).status == TaskStatus.Completed)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public void Reset()
	{
		foreach (Achievement achievement in this.dicAchievements.Values)
		{
			for (int i = 0; i < achievement.currentCompleted.Count; i++)
			{
				achievement.currentCompleted[i] = 0;
			}
			if (!achievement.success)
			{
				for (int j = 0; j < achievement.types.Count; j++)
				{
					switch (achievement.types[j])
					{
					case AchievementType.PassDiffcult:
						achievement.achieveSuccess[j] = false;
						break;
					case AchievementType.LessLoss:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.LessTime:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.MoreTime:
						achievement.achieveSuccess[j] = false;
						break;
					case AchievementType.NoAccupied:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.Accupied:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.LessKill:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.MoreKill:
						achievement.achieveSuccess[j] = false;
						break;
					case AchievementType.LessPeople:
						achievement.achieveSuccess[j] = true;
						break;
					case AchievementType.SecondAccupied:
						achievement.achieveSuccess[j] = false;
						break;
					}
				}
			}
		}
	}

	public Dictionary<string, AchievementGroup> achievementGroups = new Dictionary<string, AchievementGroup>();

	public Dictionary<string, Achievement> dicAchievements = new Dictionary<string, Achievement>();

	public AchievementModel.OnAchieveSuccess onAchieveSuccess;

	public AchievementModel.OnLanguageChanged onLanguageChanged;

	public static int requestCount = 0;

	public static int responseCount = 0;

	public static Dictionary<string, Dictionary<string, bool>> achieveDic = new Dictionary<string, Dictionary<string, bool>>();

	public delegate void OnAchieveSuccess(string id, bool success);

	public delegate void OnLanguageChanged();
}
