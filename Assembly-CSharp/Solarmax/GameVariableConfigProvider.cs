using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class GameVariableConfigProvider : Solarmax.Singleton<GameVariableConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/gamevariable.txt";
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
				GameVariableConfig gameVariableConfig = new GameVariableConfig();
				gameVariableConfig.id = FileReader.ReadInt();
				gameVariableConfig.value = FileReader.ReadString();
				this.dataList.Add(gameVariableConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<GameVariableConfig> GetAllData()
		{
			return this.dataList;
		}

		public string GetData(int id)
		{
			GameVariableConfig gameVariableConfig = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].id.Equals(id))
				{
					gameVariableConfig = this.dataList[i];
					break;
				}
			}
			return gameVariableConfig.value;
		}

		public void LoadExtraData()
		{
		}

		private List<GameVariableConfig> dataList = new List<GameVariableConfig>();
	}
}
