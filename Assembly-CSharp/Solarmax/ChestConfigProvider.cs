using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class ChestConfigProvider : Solarmax.Singleton<ChestConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/chest.txt";
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
				ChestConfig chestConfig = new ChestConfig();
				chestConfig.id = FileReader.ReadInt();
				chestConfig.type = FileReader.ReadInt();
				chestConfig.iconopen = FileReader.ReadString();
				chestConfig.icon = FileReader.ReadString();
				chestConfig.ladderlevel = FileReader.ReadInt();
				chestConfig.name = FileReader.ReadString();
				chestConfig.costtime = FileReader.ReadInt();
				chestConfig.costdiamond = FileReader.ReadInt();
				chestConfig.trophy = FileReader.ReadInt();
				chestConfig.dropitem = FileReader.ReadInt();
				chestConfig.dropcoin = FileReader.ReadInt();
				chestConfig.mincoin = FileReader.ReadInt();
				chestConfig.maxcoin = FileReader.ReadInt();
				chestConfig.itemnum = FileReader.ReadInt();
				chestConfig.itemgather = FileReader.ReadString();
				this.dataList.Add(chestConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<ChestConfig> GetAllData()
		{
			return this.dataList;
		}

		public ChestConfig GetData(int id)
		{
			ChestConfig result = null;
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

		private List<ChestConfig> dataList = new List<ChestConfig>();
	}
}
