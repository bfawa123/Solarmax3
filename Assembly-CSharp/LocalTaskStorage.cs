using System;
using System.Collections.Generic;
using System.Linq;
using Solarmax;

public class LocalTaskStorage : Solarmax.Singleton<LocalTaskStorage>, ILocalStorage
{
	public string Name()
	{
		return "TASK_RECORD1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutString("Task", this.DicToString());
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		string @string = manager.GetString("Task", string.Empty);
		TaskConfigProvider taskConfigProvider = Solarmax.Singleton<TaskConfigProvider>.Get();
		taskConfigProvider.taskStatusChanged = (TaskConfigProvider.TaskStatusChanged)Delegate.Combine(taskConfigProvider.taskStatusChanged, new TaskConfigProvider.TaskStatusChanged(this.OnStatusChanged));
		if (string.IsNullOrEmpty(@string))
		{
			foreach (TaskConfig taskConfig in Solarmax.Singleton<TaskConfigProvider>.Get().achieveList)
			{
				this.dicTask[taskConfig.id] = (int)taskConfig.status;
			}
			return;
		}
		this.StringToDic(@string);
	}

	public void Clear(LocalStorageSystem manager)
	{
	}

	public void OnStatusChanged(string id, TaskStatus status)
	{
		if (!this.dicTask.ContainsKey(id))
		{
			return;
		}
		if (this.dicTask[id] < (int)status)
		{
			this.dicTask[id] = (int)status;
		}
	}

	public int GetStatus(string id)
	{
		if (this.dicTask.ContainsKey(id))
		{
			return this.dicTask[id];
		}
		return 0;
	}

	private string DicToString()
	{
		return string.Join(";", (from x in this.dicTask
		select x.Key + "=" + x.Value).ToArray<string>());
	}

	private void StringToDic(string str)
	{
		string[] array = str.Split(new char[]
		{
			';'
		});
		foreach (string text in array)
		{
			string[] array3 = text.Split(new char[]
			{
				'='
			});
			string key = array3[0];
			string text2 = array3[1];
			if (!Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(key))
			{
				continue;
			}
			if (text2.Equals("0"))
			{
				this.dicTask[key] = 0;
			}
			else if (text2.Equals("1"))
			{
				this.dicTask[key] = 1;
			}
			else if (text2.Equals("2"))
			{
				this.dicTask[key] = 2;
			}
			if (Solarmax.Singleton<TaskConfigProvider>.Get().dataList[key].status < (TaskStatus)this.dicTask[key])
			{
				Solarmax.Singleton<TaskConfigProvider>.Get().dataList[key].status = (TaskStatus)this.dicTask[key];
			}
		}
	}

	public int ver = 1;

	public string achieve = string.Empty;

	public Dictionary<string, int> dicTask = new Dictionary<string, int>();
}
