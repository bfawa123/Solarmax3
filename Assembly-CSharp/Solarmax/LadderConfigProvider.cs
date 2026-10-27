using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class LadderConfigProvider : Solarmax.Singleton<LadderConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/ladder.txt";
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
				LadderConfig ladderConfig = new LadderConfig();
				ladderConfig.ladderlevel = FileReader.ReadInt();
				ladderConfig.itemgather = FileReader.ReadString();
				ladderConfig.points = FileReader.ReadInt();
				ladderConfig.laddername = FileReader.ReadString();
				ladderConfig.icon = FileReader.ReadString();
				ladderConfig.winmaxcoin = FileReader.ReadInt();
				ladderConfig.winmincoin = FileReader.ReadInt();
				ladderConfig.defeatmaxcoin = FileReader.ReadInt();
				ladderConfig.defeatmincoin = FileReader.ReadInt();
				this.dataList.Add(ladderConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<LadderConfig> GetAllData()
		{
			return this.dataList;
		}

		public LadderConfig GetData(int ladderlevel)
		{
			LadderConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].ladderlevel.Equals(ladderlevel))
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

		private List<LadderConfig> dataList = new List<LadderConfig>();
	}
}
