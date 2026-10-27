using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class AchievementConfigProvider : Solarmax.Singleton<AchievementConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/achievement.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					IEnumerable<XElement> enumerable = XDocument.Parse(text).Elements("achievements");
					if (enumerable != null)
					{
						foreach (XElement element in enumerable.Elements("achievement"))
						{
							AchievementConfig achievementConfig = new AchievementConfig();
							if (achievementConfig.Load(element))
							{
								this.dataList.Add(achievementConfig.id, achievementConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Achievement.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public void SetDescs()
		{
			foreach (AchievementConfig achievementConfig in this.dataList.Values)
			{
				achievementConfig.SetDesc();
			}
		}

		public Dictionary<string, AchievementConfig> dataList = new Dictionary<string, AchievementConfig>();
	}
}
