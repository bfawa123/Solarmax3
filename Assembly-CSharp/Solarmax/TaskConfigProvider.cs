using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class TaskConfigProvider : Solarmax.Singleton<TaskConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/Task.Xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					IEnumerable<XElement> enumerable = XDocument.Parse(text).Elements("configs");
					if (enumerable == null)
					{
						return;
					}
					foreach (XElement element in enumerable.Elements("config"))
					{
						TaskConfig taskConfig = new TaskConfig();
						if (taskConfig.Load(element))
						{
							if (taskConfig.taskType == TaskType.Daily)
							{
								if (taskConfig.subType == FinishConntion.Degree)
								{
									this.degreeList.Add(taskConfig);
								}
								else
								{
									this.dailyList.Add(taskConfig);
								}
							}
							else if (taskConfig.taskType == TaskType.Level && taskConfig.subType == FinishConntion.Achievement)
							{
								this.achieveList.Add(taskConfig);
								this.achieveToTask.Add(taskConfig.levelId, taskConfig);
							}
							else if (taskConfig.taskType == TaskType.Level && taskConfig.subType == FinishConntion.Level)
							{
								this.levelList.Add(taskConfig);
							}
							this.dataList.Add(taskConfig.id, taskConfig);
						}
					}
				}
				AchievementModel achievementModel = Solarmax.Singleton<AchievementModel>.Get();
				achievementModel.onAchieveSuccess = (AchievementModel.OnAchieveSuccess)Delegate.Combine(achievementModel.onAchieveSuccess, new AchievementModel.OnAchieveSuccess(this.OnChangeAchievementState));
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Task.Xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<string, TaskConfig> GetAllData()
		{
			return this.dataList;
		}

		public List<TaskConfig> GetLevelData()
		{
			return this.levelList;
		}

		public List<TaskConfig> GetDailyData()
		{
			return this.dailyList;
		}

		public List<TaskConfig> GeDegreeData()
		{
			return this.degreeList;
		}

		public List<TaskConfig> GetAchieveData()
		{
			return this.achieveList;
		}

		public TaskConfig GetTask(string id)
		{
			if (!this.dataList.ContainsKey(id))
			{
				return null;
			}
			return this.dataList[id];
		}

		public bool HaveDailyTaskCompleted()
		{
			foreach (TaskConfig taskConfig in this.dailyList)
			{
				if (taskConfig.status == TaskStatus.Completed)
				{
					return true;
				}
			}
			foreach (TaskConfig taskConfig2 in this.degreeList)
			{
				if (taskConfig2.status == TaskStatus.Completed)
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveAchievementTaskCompleted()
		{
			foreach (TaskConfig taskConfig in this.achieveList)
			{
				if (taskConfig.status == TaskStatus.Completed)
				{
					return true;
				}
			}
			return false;
		}

		public void OnChangeAchievementState(string achieveId, bool success)
		{
			if (this.achieveToTask.ContainsKey(achieveId) && this.achieveToTask[achieveId].status == TaskStatus.Unfinished && success)
			{
				this.achieveToTask[achieveId].status = TaskStatus.Completed;
			}
		}

		public Dictionary<string, TaskConfig> dataList = new Dictionary<string, TaskConfig>();

		public Dictionary<string, TaskConfig> achieveToTask = new Dictionary<string, TaskConfig>();

		public List<TaskConfig> levelList = new List<TaskConfig>();

		public List<TaskConfig> dailyList = new List<TaskConfig>();

		public List<TaskConfig> achieveList = new List<TaskConfig>();

		public List<TaskConfig> degreeList = new List<TaskConfig>();

		public TaskConfigProvider.TaskStatusChanged taskStatusChanged;

		public delegate void TaskStatusChanged(string id, TaskStatus status);
	}
}
