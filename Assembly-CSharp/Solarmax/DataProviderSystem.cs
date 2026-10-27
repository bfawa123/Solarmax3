using System;
using System.Collections.Generic;
using System.IO;

namespace Solarmax
{
	public class DataProviderSystem : Solarmax.Singleton<DataProviderSystem>, Lifecycle
	{
		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("DataProviderSystem   init   begin", new object[0]);
			this.RegisterDataProvider(Solarmax.Singleton<NameFilterConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<InnateSkillConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<LadderConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<NameConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<RaceSkillConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<SkillConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<UIWindowConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<MapNodeConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<GameVariableConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<NewSkillConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<NewSkillBuffConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<NewSkillEffectConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<GuideDataProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<LevelConfigConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<CampConfigConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<AIStrategyConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<SkinConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<storeConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<SeasonRewardProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<MonthCheckConfgiProvider>.Instance);

			this.RegisterDataProvider(Solarmax.Singleton<RaceConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<RaceSkillConfigProvider>.Instance);
			if (!this.Load())
			{
				return false;
			}
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("DataProviderSystem   init   end", new object[0]);
			return true;
		}

		public bool InitDelay()
		{
			this.RegisterDataProvider(Solarmax.Singleton<MapConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<ChapterConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<ChapterAssistConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<TaskConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<AchievementConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<AchievementListProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<ItemConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<LotteryConfigProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<LotteryAddProvider>.Instance);
			this.RegisterDataProvider(Solarmax.Singleton<LotteryRotateProvider>.Instance);
            Solarmax.Singleton<MapConfigProvider>.Instance.Load();
            Solarmax.Singleton<ChapterConfigProvider>.Instance.Load();
            Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.Load();
            Solarmax.Singleton<TaskConfigProvider>.Instance.Load();
            Solarmax.Singleton<AchievementConfigProvider>.Instance.Load();
            Solarmax.Singleton<AchievementListProvider>.Instance.Load();
            Solarmax.Singleton<ItemConfigProvider>.Instance.Load();
            Solarmax.Singleton<LotteryConfigProvider>.Instance.Load();
            Solarmax.Singleton<LotteryAddProvider>.Instance.Load();
            Solarmax.Singleton<LotteryRotateProvider>.Instance.Load();
			return true;
		}

		public void Tick(float interval)
		{
		}

		public void Destroy()
		{
			this.mDataProvider.Clear();
		}

		private bool Load()
		{
			for (int i = 0; i < this.mDataProvider.Count; i++)
			{
				IDataProvider dataProvider = this.mDataProvider[i];
				if (dataProvider != null)
				{
					if (!dataProvider.IsXML())
					{
						FileReader.LoadPath(dataProvider.Path());
					}
					dataProvider.Load();
					if (!dataProvider.Verify())
					{
						return false;
					}
					if (!dataProvider.IsXML())
					{
						FileReader.UnLoad();
					}
				}
			}
			return true;
		}

		private void RegisterDataProvider(IDataProvider dataProvider)
		{
			this.mDataProvider.Add(dataProvider);
		}

		public static string FormatDataProviderPath(string datapath)
		{
			return Path.Combine(UtilTools.GetStreamAssetsByPlatform(), datapath);
		}

		public void LoadExtraData(string path)
		{
		}

		private List<IDataProvider> mDataProvider = new List<IDataProvider>();

		private LanguageDataProvider languageProvider = Solarmax.Singleton<LanguageDataProvider>.Instance;
	}
}
