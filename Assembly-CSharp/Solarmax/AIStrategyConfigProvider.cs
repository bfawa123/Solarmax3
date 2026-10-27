using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class AIStrategyConfigProvider : Solarmax.Singleton<AIStrategyConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "/data/aistrategy.xml";
		}

		public bool Delete(string name)
		{
			return true;
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			AIStrategyConfigProvider.aiConfig.Load();
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public List<int> GetAIActions(int strategy)
		{
			for (int i = 0; i < AIStrategyConfigProvider.aiConfig.aiStrategy.Count; i++)
			{
				if (AIStrategyConfigProvider.aiConfig.aiStrategy[i].id == strategy)
				{
					return AIStrategyConfigProvider.aiConfig.aiStrategy[i].aiActions;
				}
			}
			return null;
		}

		public List<int> GetAIActions(string strategy)
		{
			for (int i = 0; i < AIStrategyConfigProvider.aiConfig.aiStrategy.Count; i++)
			{
				if (AIStrategyConfigProvider.aiConfig.aiStrategy[i].name == strategy)
				{
					return AIStrategyConfigProvider.aiConfig.aiStrategy[i].aiActions;
				}
			}
			return null;
		}

		public int GetAIStrategyByName(string name)
		{
			for (int i = 0; i < AIStrategyConfigProvider.aiConfig.aiStrategy.Count; i++)
			{
				if (AIStrategyConfigProvider.aiConfig.aiStrategy[i].name == name)
				{
					return AIStrategyConfigProvider.aiConfig.aiStrategy[i].id;
				}
			}
			return -1;
		}

		public static AiConfig aiConfig = new AiConfig();
	}
}
