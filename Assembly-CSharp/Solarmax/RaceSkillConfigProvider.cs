using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class RaceSkillConfigProvider : Solarmax.Singleton<RaceSkillConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/raceskill.txt";
		}

		public bool IsXML()
		{
			return false;
		}

		public void Load()
		{
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				RaceSkillConfig raceSkillConfig = new RaceSkillConfig();
				raceSkillConfig.id = FileReader.ReadInt("id");
				raceSkillConfig.type = FileReader.ReadInt("type");
				raceSkillConfig.image = FileReader.ReadString("image");
				raceSkillConfig.icon = FileReader.ReadString("icon");
				raceSkillConfig.name = FileReader.ReadString("name");
				raceSkillConfig.level = FileReader.ReadInt("level");
				raceSkillConfig.nextcostitemid = FileReader.ReadInt("nextcostitemid");
				raceSkillConfig.nextcostnum = FileReader.ReadInt("nextcostnum");
				raceSkillConfig.nextcostmoney = FileReader.ReadInt("nextcostmoney");
				raceSkillConfig.nextid = FileReader.ReadInt("nextid");
				raceSkillConfig.skillid = FileReader.ReadInt("skillid");
				raceSkillConfig.desc = FileReader.ReadString("desc");
				this.dataList.Add(raceSkillConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<RaceSkillConfig> GetAllData()
		{
			return this.dataList;
		}

		public RaceSkillConfig GetData(int id)
		{
			RaceSkillConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].id.Equals(id))
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

		private List<RaceSkillConfig> dataList = new List<RaceSkillConfig>();
	}
}
