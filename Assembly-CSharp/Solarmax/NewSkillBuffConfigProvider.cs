using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class NewSkillBuffConfigProvider : Solarmax.Singleton<NewSkillBuffConfigProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return false;
		}

		public string Path()
		{
			return "data/NewSkillBuff.txt";
		}

		public void Load()
		{
			this.dataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				NewSkillBuffConfig newSkillBuffConfig = new NewSkillBuffConfig();
				newSkillBuffConfig.buffId = FileReader.ReadInt("buffId");
				newSkillBuffConfig.name = FileReader.ReadString("name");
				newSkillBuffConfig.desc = FileReader.ReadString("desc");
				newSkillBuffConfig.order = FileReader.ReadInt("order");
				newSkillBuffConfig.logicId = FileReader.ReadInt("logicId");
				newSkillBuffConfig.targetType = FileReader.ReadInt("targetType");
				newSkillBuffConfig.lastTime = FileReader.ReadFloat("lastTime");
				newSkillBuffConfig.actInterval = FileReader.ReadFloat("actInterval");
				newSkillBuffConfig.overcastType = FileReader.ReadInt("overcastType");
				newSkillBuffConfig.breakUpSkill = FileReader.ReadInt("breakUpSkill");
				newSkillBuffConfig.effectId = FileReader.ReadInt("effectId");
				newSkillBuffConfig.arg0 = FileReader.ReadString("arg0");
				newSkillBuffConfig.arg1 = FileReader.ReadString("arg1");
				newSkillBuffConfig.arg2 = FileReader.ReadString("arg2");
				newSkillBuffConfig.arg3 = FileReader.ReadString("arg3");
				newSkillBuffConfig.arg4 = FileReader.ReadString("arg4");
				newSkillBuffConfig.arg5 = FileReader.ReadString("arg5");
				this.dataList.Add(newSkillBuffConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public NewSkillBuffConfig GetData(int buffid)
		{
			NewSkillBuffConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].buffId == buffid)
				{
					result = this.dataList[i];
					break;
				}
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		private List<NewSkillBuffConfig> dataList = new List<NewSkillBuffConfig>();
	}
}
