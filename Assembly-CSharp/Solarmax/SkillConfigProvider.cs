using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class SkillConfigProvider : Solarmax.Singleton<SkillConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/skill.txt";
		}

		public bool IsXML()
		{
			return false;
		}

		public void Load()
		{
			this.dataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				SkillConfig skillConfig = new SkillConfig();
				skillConfig.skillid = FileReader.ReadInt();
				skillConfig.name = FileReader.ReadString();
				skillConfig.icon = FileReader.ReadString();
				skillConfig.icondisable = FileReader.ReadString();
				skillConfig.iconfull = FileReader.ReadString();
				skillConfig.skilltype = FileReader.ReadInt();
				skillConfig.targettype = FileReader.ReadInt();
				skillConfig.skillparam = FileReader.ReadFloat();
				skillConfig.skillcost = FileReader.ReadInt();
				skillConfig.actiontime = FileReader.ReadFloat();
				skillConfig.starttime = FileReader.ReadFloat();
				skillConfig.reuse = FileReader.ReadInt();
				skillConfig.skilltime = FileReader.ReadFloat();
				skillConfig.skillCD = FileReader.ReadFloat();
				skillConfig.srcPath = FileReader.ReadString();
				skillConfig.desc = FileReader.ReadString();
				skillConfig.audio = FileReader.ReadString();
				skillConfig.tips = FileReader.ReadString();
				skillConfig.tipsdir = FileReader.ReadInt();
				skillConfig.tipsallshow = FileReader.ReadInt();
				this.dataList.Add(skillConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<SkillConfig> GetAllData()
		{
			return this.dataList;
		}

		public SkillConfig GetData(int skillid)
		{
			SkillConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].skillid.Equals(skillid))
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

		private List<SkillConfig> dataList = new List<SkillConfig>();
	}
}
