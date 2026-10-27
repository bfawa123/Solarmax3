using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Solarmax
{
	public class AchievementListProvider : Solarmax.Singleton<AchievementListProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/achievementlist.xml";
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
					IEnumerable<XElement> enumerable = XDocument.Parse(text).Elements("lists");
					if (enumerable != null)
					{
						foreach (XElement element in enumerable.Elements("list"))
						{
							AchievementListConfig achievementListConfig = new AchievementListConfig();
							if (achievementListConfig.Load(element))
							{
								this.dataList.Add(achievementListConfig.levelGroup, achievementListConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/AchievementList.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public static int GetAchievementCount(string groupId)
		{
			foreach (KeyValuePair<string, AchievementListConfig> keyValuePair in Solarmax.Singleton<AchievementListProvider>.Instance.dataList)
			{
				if (keyValuePair.Value.levelGroup.Equals(groupId))
				{
					return keyValuePair.Value.achieveList.Length;
				}
			}
			return 1;
		}

		public static List<int> GetAchievement(string groupId, int diffcult)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<string, AchievementListConfig> keyValuePair in Solarmax.Singleton<AchievementListProvider>.Instance.dataList)
			{
				if (keyValuePair.Value.levelGroup.Equals(groupId))
				{
                    Dictionary<string, AchievementConfig> allAchievement = Solarmax.Singleton<AchievementConfigProvider>.Get().dataList;

                    string[] achieveList = keyValuePair.Value.achieveList
						.Where(key => allAchievement.ContainsKey(key))
						.ToArray();
                    foreach (string key in achieveList)
					{
						AchievementConfig achievementConfig = allAchievement[key];
						if (achievementConfig.difficult == diffcult)
						{
							foreach (int item in achievementConfig.types)
							{
								if (!list.Contains(item))
								{
									list.Add(item);
								}
							}
						}
					}
                    return list;
				}
			}
			return list;
		}

		public Dictionary<string, AchievementListConfig> dataList = new Dictionary<string, AchievementListConfig>();
	}
}
