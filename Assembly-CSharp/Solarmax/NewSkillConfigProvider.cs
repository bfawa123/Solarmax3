using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class NewSkillConfigProvider : Solarmax.Singleton<NewSkillConfigProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return false;
		}

		public string Path()
		{
			return "data/NewSkill.txt";
		}

		public void Load()
		{
			this.dataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				NewSkillConfig newSkillConfig = new NewSkillConfig();
				newSkillConfig.skillId = FileReader.ReadInt("skillId");
				newSkillConfig.name = FileReader.ReadString("name");
				newSkillConfig.desc = FileReader.ReadString("desc");
				newSkillConfig.icon = FileReader.ReadString("icon");
				newSkillConfig.iconDisable = FileReader.ReadString("iconDisable");
				newSkillConfig.iconFull = FileReader.ReadString("iconFull");
				newSkillConfig.type = FileReader.ReadInt("type");
				newSkillConfig.cost = FileReader.ReadInt("cost");
				newSkillConfig.logicId = FileReader.ReadInt("logicId");
				newSkillConfig.buffs = FileReader.ReadString("buffs");
				newSkillConfig.firstcd = FileReader.ReadFloat("firstcd");
				newSkillConfig.reuse = FileReader.ReadBoolean("reuse");
				newSkillConfig.cd = FileReader.ReadFloat("cd");
				newSkillConfig.effectId = FileReader.ReadInt("effectId");
				newSkillConfig.tips = FileReader.ReadString("tips");
				newSkillConfig.tipsdir = FileReader.ReadInt("tipsdir");
				newSkillConfig.tipsallshow = FileReader.ReadBoolean("tipsallshow");
				newSkillConfig.buffIds = Converter.ConvertNumberList<int>(newSkillConfig.buffs);
				this.dataList.Add(newSkillConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public NewSkillConfig GetData(int skillId)
		{
			NewSkillConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].skillId == skillId)
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

		private List<NewSkillConfig> dataList = new List<NewSkillConfig>();
	}
}
