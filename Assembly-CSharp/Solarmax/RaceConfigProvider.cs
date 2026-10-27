using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class RaceConfigProvider : Solarmax.Singleton<RaceConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/race.txt";
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
				RaceConfig raceConfig = new RaceConfig();
				raceConfig.id = FileReader.ReadInt();
				raceConfig.name = FileReader.ReadString();
				raceConfig.icon = FileReader.ReadString();
				raceConfig.desc = FileReader.ReadString();
				raceConfig.defaultskill1 = FileReader.ReadInt();
				raceConfig.defaultskill2 = FileReader.ReadInt();
				raceConfig.defaultskill3 = FileReader.ReadInt();
				raceConfig.defaultskill4 = FileReader.ReadInt();
				raceConfig.defaultskill5 = FileReader.ReadInt();
				raceConfig.defaultskill6 = FileReader.ReadInt();
				this.dataList.Add(raceConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<RaceConfig> GetAllData()
		{
			return this.dataList;
		}

		public RaceConfig GetData(int id)
		{
			RaceConfig result = null;
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

		private List<RaceConfig> dataList = new List<RaceConfig>();
	}
}
