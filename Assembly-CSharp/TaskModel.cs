using System;
using System.Collections.Generic;
using Solarmax;

public class TaskModel : Solarmax.Singleton<TaskModel>
{
	public void Init()
	{
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		if (Solarmax.Singleton<LocalPvpStorage>.Get().days != timeSpan.Days)
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().Clear();
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalPvp(timeSpan.Days, 0, 0);
		}
		else
		{
			foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().dailyList)
			{
				if (taskConfig.subType == FinishConntion.Level && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin >= taskConfig.taskParameter)
				{
					if (taskConfig.status != TaskStatus.Received)
					{
						taskConfig.status = TaskStatus.Completed;
					}
				}
				else if (taskConfig.subType == FinishConntion.Achievement && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy >= taskConfig.taskParameter)
				{
					if (taskConfig.status != TaskStatus.Received)
					{
						taskConfig.status = TaskStatus.Completed;
					}
				}
				else if (taskConfig.subType == FinishConntion.Pve && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPvpStorage>.Get().pve >= taskConfig.taskParameter)
				{
					if (taskConfig.status != TaskStatus.Received)
					{
						taskConfig.status = TaskStatus.Completed;
					}
				}
				else if (taskConfig.subType == FinishConntion.OnLine && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPlayer>.Get().mOnLineTime / 60f >= (float)taskConfig.taskParameter)
				{
					if (taskConfig.status != TaskStatus.Received)
					{
						taskConfig.status = TaskStatus.Completed;
					}
				}
				else if (taskConfig.subType == FinishConntion.Ads && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPvpStorage>.Get().lookAds >= taskConfig.taskParameter && taskConfig.status != TaskStatus.Received)
				{
					taskConfig.status = TaskStatus.Completed;
				}
			}
			foreach (TaskConfig taskConfig2 in Solarmax.Singleton<TaskConfigProvider>.Get().degreeList)
			{
				if (taskConfig2.subType == FinishConntion.Degree && taskConfig2.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree >= taskConfig2.taskParameter && taskConfig2.status != TaskStatus.Received)
				{
					taskConfig2.status = TaskStatus.Completed;
				}
			}
		}
		this.PullCompleted();
	}

	public void PullCompleted()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.PullTaskOk();
	}

	public void ClaimReward(string taskId, TaskModel.RequestResult result, int multiply = 1)
	{
		this.ClaimAllReward(new List<string>
		{
			taskId
		}, result, multiply);
		MonoSingleton<FlurryAnalytis>.Instance.FlurryFinishTaskEvent(taskId);
		MiGameAnalytics.MiAnalyticsFinishTaskEvent(taskId);
	}

	public void ClaimAllReward(List<string> taskId, TaskModel.RequestResult result, int multiply = 1)
	{
		if (taskId.Count == 0)
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestTaskOk(taskId, multiply);
	}

	public void WinTeam(Team team)
	{
		if (team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			DateTime d = new DateTime(1970, 1, 1);
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
			if (Solarmax.Singleton<LocalPvpStorage>.Get().days != timeSpan.Days)
			{
				Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin = 1;
			}
			else
			{
				Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin++;
			}
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalPvp(Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin, Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy);
			foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().dailyList)
			{
				if (taskConfig.subType == FinishConntion.Level && taskConfig.status == TaskStatus.Unfinished && Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin >= taskConfig.taskParameter)
				{
					taskConfig.status = TaskStatus.Completed;
				}
			}
		}
	}

	public void PvpEndDataHandler(BattleEndData data)
	{
		if (data == null)
		{
			return;
		}
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(data.team);
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		if (Solarmax.Singleton<LocalPvpStorage>.Get().days != timeSpan.Days)
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy = team.hitships;
		}
		else
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy += team.hitships;
		}
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalPvp(Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin, Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy);
		foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().dailyList)
		{
			if (taskConfig.subType == FinishConntion.Achievement && Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy >= taskConfig.taskParameter && taskConfig.status == TaskStatus.Unfinished)
			{
				taskConfig.status = TaskStatus.Completed;
			}
		}
	}

	public void FinishTaskEvent(FinishConntion co, int nParam = 0)
	{
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		if (Solarmax.Singleton<LocalPvpStorage>.Get().days != timeSpan.Days)
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().Clear(null);
		}
		if (co == FinishConntion.Degree)
		{
			Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree += nParam;
			this.FinishDegreeQuest(co);
			return;
		}
		if (co == FinishConntion.Ads)
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().lookAds += nParam;
		}
		else if (co == FinishConntion.Pve)
		{
			Solarmax.Singleton<LocalPvpStorage>.Get().pve += nParam;
		}
		foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().dailyList)
		{
			if (taskConfig.subType == co && taskConfig.status == TaskStatus.Unfinished)
			{
				if (co == FinishConntion.Ads && Solarmax.Singleton<LocalPvpStorage>.Get().lookAds >= taskConfig.taskParameter)
				{
					taskConfig.status = TaskStatus.Completed;
				}
				else if (co == FinishConntion.OnLine && nParam / 60 >= taskConfig.taskParameter)
				{
					taskConfig.status = TaskStatus.Completed;
				}
				else if (co == FinishConntion.Pve && Solarmax.Singleton<LocalPvpStorage>.Get().pve >= taskConfig.taskParameter)
				{
					taskConfig.status = TaskStatus.Completed;
				}
				else if (co == FinishConntion.Login)
				{
					taskConfig.status = TaskStatus.Completed;
				}
			}
		}
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalPvp(Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin, Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy);
	}

	private void FinishDegreeQuest(FinishConntion co)
	{
		int mActivityDegree = Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree;
		foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().degreeList)
		{
			if (taskConfig.subType == co && taskConfig.status == TaskStatus.Unfinished && co == FinishConntion.Degree && mActivityDegree >= taskConfig.taskParameter)
			{
				taskConfig.status = TaskStatus.Completed;
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefrushTaskDegree, new object[0]);
	}

	public bool AfterDayRefrushTask()
	{
		DateTime d = new DateTime(1970, 1, 1);
		TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
		if (Solarmax.Singleton<LocalPvpStorage>.Get().days != timeSpan.Days)
		{
			foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().dailyList)
			{
				taskConfig.status = TaskStatus.Unfinished;
			}
			foreach (TaskConfig taskConfig2 in Solarmax.Singleton<TaskConfigProvider>.Get().degreeList)
			{
				taskConfig2.status = TaskStatus.Unfinished;
			}
			Solarmax.Singleton<LocalPvpStorage>.Get().Clear();
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalPvp(timeSpan.Days, 0, 0);
			return true;
		}
		return false;
	}

	public TaskModel.RequestResult requestResult;

	public TaskModel.OnRequestTaskOk onRequestTaskOk;

	public List<string> completedTasks = new List<string>();

	public TaskConfig claimTask;

	public RewardTipsModel claimReward;

	public delegate void RequestResult(bool success);

	public delegate void OnRequestTaskOk(List<string> success, List<string> failure);
}
