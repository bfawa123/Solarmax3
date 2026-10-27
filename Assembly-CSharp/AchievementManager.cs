using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class AchievementManager : Solarmax.Singleton<AchievementManager>
{
	public void Init(BattleData data)
	{
		this.stop = false;
		AchievementManager.isFirst = true;
		this.battleData = data;
		this.completeList.Clear();
		this.model = Solarmax.Singleton<AchievementModel>.Get();
		this.groupId = Solarmax.Singleton<LevelDataHandler>.Instance.GetCurrentGroupID();
		this.difficult = (AchievementDifficult)Solarmax.Singleton<LevelDataHandler>.Instance.GetCurrentDiffcult();
	}

	public void BattleTick(float interval)
	{
		if (this.battleData == null || (this.battleData.gameType != GameType.SingleLevel && this.battleData.gameType != GameType.GuildeLevel) || this.battleData.isReplay)
		{
			return;
		}
		if (this.stop)
		{
			return;
		}
		if (AchievementManager.isFirst)
		{
			AchievementManager.isFirst = false;
			this.model.Reset();
			if (this.model.achievementGroups.ContainsKey(this.groupId))
			{
				this.achieveList = this.model.achievementGroups[this.groupId].GetAchievementByDifficult(this.difficult, true);
			}
		}
		if (this.achieveList == null)
		{
			return;
		}
		foreach (Achievement achievement in this.achieveList)
		{
			for (int i = 0; i < achievement.types.Count; i++)
			{
				AchievementType t = achievement.types[i];
				List<string> misc = achievement.config.GetMisc((int)t);
				switch (t)
				{
				case AchievementType.PassDiffcult:
					achievement.achieveSuccess[i] = this.PassDifficultAchievement(achievement);
					break;
				case AchievementType.LessLoss:
				{
					int aims = 0;
					int.TryParse(misc[0], out aims);
					achievement.achieveSuccess[i] = this.LessLossAchievement(achievement, aims, i);
					break;
				}
				case AchievementType.LessTime:
				{
					float aims2 = 0f;
					float.TryParse(misc[0], out aims2);
					achievement.achieveSuccess[i] = this.LessTimeAchievement(achievement, aims2, i);
					break;
				}
				case AchievementType.MoreTime:
				{
					float aims3 = 0f;
					float.TryParse(misc[0], out aims3);
					achievement.achieveSuccess[i] = this.MoreTimeAchievement(achievement, aims3, i);
					break;
				}
				case AchievementType.NoAccupied:
					if (achievement.achieveSuccess[i])
					{
						achievement.achieveSuccess[i] = this.NoAccupyAchievement(achievement, misc);
					}
					break;
				case AchievementType.Accupied:
					if (achievement.achieveSuccess[i])
					{
						achievement.achieveSuccess[i] = this.AccupyAchievement(achievement, misc);
					}
					break;
				case AchievementType.LessKill:
				{
					int aims4 = 0;
					int.TryParse(misc[0], out aims4);
					achievement.achieveSuccess[i] = this.LessKillAchievement(achievement, aims4, i);
					break;
				}
				case AchievementType.MoreKill:
				{
					int aims5 = 0;
					int.TryParse(misc[0], out aims5);
					achievement.achieveSuccess[i] = this.MoreKillAchievement(achievement, aims5, i);
					break;
				}
				case AchievementType.LessPeople:
					if (achievement.achieveSuccess[i])
					{
						int aims6 = 0;
						int.TryParse(misc[0], out aims6);
						achievement.achieveSuccess[i] = this.LessPeopleAchievement(achievement, aims6, i);
					}
					break;
				case AchievementType.SecondAccupied:
					if (!achievement.achieveSuccess[i])
					{
						float time = 0f;
						float.TryParse(misc[0], out time);
						int aims7 = 0;
						int.TryParse(misc[1], out aims7);
						achievement.achieveSuccess[i] = this.LessSecondAccupiedAchievement(achievement, time, aims7);
					}
					break;
				}
				if (this.achieveStateChanged != null)
				{
					this.achieveStateChanged(achievement.id);
				}
			}
		}
	}

	public void SettlementAchievement(bool success)
	{
		if (this.battleData.gameType != GameType.SingleLevel && this.battleData.gameType != GameType.GuildeLevel)
		{
			return;
		}
		if (success)
		{
			this.BattleTick(0.02f);
			this.stop = true;
			bool flag = false;
			List<string> list = new List<string>();
			foreach (Achievement achievement in this.achieveList)
			{
				if (!achievement.success)
				{
					bool flag2 = this.IsCompleted(achievement.id);
					if (flag2)
					{
						flag = true;
						achievement.success = true;
						list.Add(achievement.id);
						this.completeList.Add(achievement);
					}
				}
			}
			if (flag)
			{
				this.model.SendAchievement(this.groupId, list);
				Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAchievement();
			}
		}
		if (this.battleEnd != null)
		{
			this.battleEnd();
		}
	}

	public void FinishADSAchievement(bool success)
	{
		if (this.battleData.gameType != GameType.SingleLevel && this.battleData.gameType != GameType.GuildeLevel)
		{
			return;
		}
		if (success)
		{
			this.BattleTick(0.02f);
			this.stop = true;
			bool flag = false;
			List<string> list = new List<string>();
			foreach (Achievement achievement in this.achieveList)
			{
				if (!achievement.success)
				{
					bool flag2 = achievement.types[0] == AchievementType.Ads;
					if (flag2)
					{
						flag = true;
						achievement.success = true;
						list.Add(achievement.id);
						this.completeList.Add(achievement);
					}
				}
			}
			if (flag)
			{
				this.model.SendAchievement(this.groupId, list);
				Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAchievement();
			}
		}
		if (this.battleEnd != null)
		{
			this.battleEnd();
		}
	}

	public Achievement GetAdsAchievement()
	{
		AchievementManager.isFirst = true;
		this.stop = false;
		this.BattleTick(0.02f);
		this.stop = true;
		if (this.achieveList == null)
		{
			return null;
		}
		foreach (Achievement achievement in this.achieveList)
		{
			if (!achievement.success)
			{
				if (achievement.types[0] == AchievementType.Ads)
				{
					return achievement;
				}
			}
		}
		return null;
	}

	public bool IsCompleted(string id)
	{
		bool result = true;
		foreach (Achievement achievement in this.achieveList)
		{
			if (!(achievement.id != id))
			{
				result = true;
				using (List<bool>.Enumerator enumerator2 = achievement.achieveSuccess.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (!enumerator2.Current)
						{
							result = false;
							break;
						}
					}
				}
			}
		}
		return result;
	}

	public bool PassDifficultAchievement(Achievement achieve)
	{
		return this.battleData.currentTeam == this.battleData.winTEAM;
	}

	public bool LessLossAchievement(Achievement achieve, int aims, int index)
	{
		achieve.currentCompleted[index] = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).destory;
		return Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).destory <= aims;
	}

	public bool LessTimeAchievement(Achievement achieve, float aims, int index)
	{
		achieve.currentCompleted[index] = (int)Mathf.Ceil(this.battleData.battleTime);
		return this.battleData.battleTime <= aims;
	}

	public bool MoreTimeAchievement(Achievement achieve, float aims, int index)
	{
		achieve.currentCompleted[index] = (int)Mathf.Ceil(this.battleData.battleTime);
		return this.battleData.battleTime >= aims;
	}

	public bool NoAccupyAchievement(Achievement achieve, List<string> aims)
	{
		foreach (string tag in aims)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.IsOccupiedPlanet((int)this.battleData.currentTeam, tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool AccupyAchievement(Achievement achieve, List<string> aims)
	{
		foreach (string tag in aims)
		{
			if (!Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.IsOccupiedPlanet((int)this.battleData.currentTeam, tag))
			{
				return false;
			}
		}
		return true;
	}

	public bool LessKillAchievement(Achievement achieve, int aims, int index)
	{
		achieve.currentCompleted[index] = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).hitships;
		return Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).hitships <= aims;
	}

	public bool MoreKillAchievement(Achievement achieve, int aims, int index)
	{
		achieve.currentCompleted[index] = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).hitships;
		return Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).hitships >= aims;
	}

	public bool LessPeopleAchievement(Achievement achieve, int aims, int index)
	{
		if (achieve.currentCompleted[index] < Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).currentMax)
		{
			achieve.currentCompleted[index] = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).currentMax;
			if (this.achieveStateChanged != null)
			{
				this.achieveStateChanged(achieve.id);
			}
		}
		return Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(this.battleData.currentTeam).currentMax <= aims;
	}

	public bool LessSecondAccupiedAchievement(Achievement achieve, float time, int aims)
	{
		achieve.currentCompleted[0] = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.CheckHaveNodeCount((int)this.battleData.currentTeam);
		if (achieve.currentCompleted.Count > 1)
		{
			achieve.currentCompleted[1] = (int)this.battleData.battleTime;
		}
		return this.battleData.battleTime <= time && (this.battleData.battleTime <= time && achieve.currentCompleted[0] >= aims);
	}

	public bool open;

	public string groupId;

	public AchievementDifficult difficult;

	public BattleData battleData;

	public AchievementModel model;

	public List<Achievement> completeList = new List<Achievement>();

	private bool stop;

	private static bool isFirst = true;

	private List<Achievement> achieveList;

	public AchievementManager.AchieveStateChanged achieveStateChanged;

	public AchievementManager.BattleEnd battleEnd;

	public delegate void AchieveStateChanged(string id);

	public delegate void BattleEnd();
}
