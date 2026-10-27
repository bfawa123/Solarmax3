using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class TaskConfig
	{
		public TaskStatus status
		{
			get
			{
				return this.innerStatus;
			}
			set
			{
				this.innerStatus = value;
				if (Solarmax.Singleton<TaskConfigProvider>.Get().taskStatusChanged != null && this.taskType == TaskType.Level && this.subType == FinishConntion.DestroyShip)
				{
                    Solarmax.Singleton<TaskConfigProvider>.Get().taskStatusChanged(this.id, this.innerStatus);
				}
			}
		}

		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.icon = element.GetAttribute("icon", string.Empty);
			this.taskType = (TaskType)Convert.ToInt32(element.GetAttribute("TaskType", string.Empty));
			if (this.taskType == TaskType.Level)
			{
				this.levelId = element.GetAttribute("LevelID", string.Empty);
			}
			else
			{
				this.descId = element.GetIntAttribute("DictionaryID", 0);
				this.taskParameter = Convert.ToInt32(element.GetAttribute("TaskParameter", string.Empty));
			}
			this.title = element.GetIntAttribute("TaskTitle", 0);
			this.subType = (FinishConntion)Convert.ToInt32(element.GetAttribute("SubType", string.Empty));
			this.rewardValue = Convert.ToInt32(element.GetAttribute("RewardValue", string.Empty));
			this.rewardType = (RewardType)Convert.ToInt32(element.GetAttribute("RewardType", string.Empty));
			this.rewardMultiple = Convert.ToInt32(element.GetAttribute("RewardMultiple", "1"));
			this.reward2Value = Convert.ToInt32(element.GetAttribute("Reward2Value", "0"));
			this.reward2Type = (RewardType)Convert.ToInt32(element.GetAttribute("Reward2Type", "0"));
			this.itemid = Convert.ToInt32(element.GetAttribute("itemid", "0"));
			return true;
		}

		public string id;

		public int descId;

		public int title;

		public string levelId;

		public string icon;

		public TaskType taskType;

		public FinishConntion subType;

		public RewardType rewardType;

		public int taskParameter;

		public int rewardValue;

		public int rewardMultiple;

		public RewardType reward2Type;

		public int reward2Value;

		public int itemid;

		private TaskStatus innerStatus;
	}
}
