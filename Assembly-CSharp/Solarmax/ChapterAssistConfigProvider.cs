using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace Solarmax
{
	public class ChapterAssistConfigProvider : Solarmax.Singleton<ChapterAssistConfigProvider>, IDataProvider
	{
		public void SetCoopPlayerNum(int playerNum)
		{
			if (playerNum != 2 && playerNum != 3 && playerNum != 4 && playerNum != 5)
			{
				playerNum = 4;
			}
			this.currentCoopPlayerNum = playerNum;
		}

		public int GetCoopPlayerNum()
		{
			return this.currentCoopPlayerNum;
		}

		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/coochapter.xml";
		}

		public void Load()
		{
			this.dataListByMode.Clear();
			this.dataByIdByMode.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("coos");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("coo"))
						{
							ChapterAssistConfig chapterAssistConfig = new ChapterAssistConfig();
							if (!chapterAssistConfig.Load(element))
							{
								continue;
							}
							int playerNum = chapterAssistConfig.playerNum;
							if (!this.dataListByMode.ContainsKey(playerNum))
							{
								this.dataListByMode[playerNum] = new List<ChapterAssistConfig>();
							}
							this.dataListByMode[playerNum].Add(chapterAssistConfig);
							if (!string.IsNullOrEmpty(chapterAssistConfig.id))
							{
								if (!this.dataByIdByMode.ContainsKey(playerNum))
								{
									this.dataByIdByMode[playerNum] = new Dictionary<string, ChapterAssistConfig>();
								}
								this.dataByIdByMode[playerNum][chapterAssistConfig.id] = chapterAssistConfig;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("data/coochapter resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<ChapterAssistConfig> GetAllData()
		{
			List<ChapterAssistConfig> result;
			if (this.dataListByMode.TryGetValue(this.currentCoopPlayerNum, out result))
			{
				return result;
			}
			if (this.currentCoopPlayerNum == 5 && this.dataListByMode.TryGetValue(4, out result))
			{
				return result;
			}
			return new List<ChapterAssistConfig>();
		}

		public List<ChapterAssistConfig> GetAllData(string chapterID)
		{
			List<ChapterAssistConfig> list = new List<ChapterAssistConfig>();
			List<ChapterAssistConfig> list2;
			if (!this.dataListByMode.TryGetValue(this.currentCoopPlayerNum, out list2) || list2 == null)
			{
				if (this.currentCoopPlayerNum != 5 || !this.dataListByMode.TryGetValue(4, out list2) || list2 == null)
				{
					return list;
				}
			}
			for (int i = 0; i < list2.Count; i++)
			{
				ChapterAssistConfig chapterAssistConfig = list2[i];
				if (chapterAssistConfig != null && chapterAssistConfig.dependChapter == chapterID)
				{
					list.Add(chapterAssistConfig);
				}
			}
			return list;
		}

		public ChapterAssistConfig GetData(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}
			Dictionary<string, ChapterAssistConfig> dictionary;
			ChapterAssistConfig result;
			if (this.dataByIdByMode.TryGetValue(this.currentCoopPlayerNum, out dictionary) && dictionary != null && dictionary.TryGetValue(id, out result))
			{
				return result;
			}
			if (this.currentCoopPlayerNum == 5 && this.dataByIdByMode.TryGetValue(4, out dictionary) && dictionary != null && dictionary.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public ChapterAssistConfig GetDataByLevelId(string levelId, int playerNum)
		{
			if (string.IsNullOrEmpty(levelId))
			{
				return null;
			}
			List<ChapterAssistConfig> list;
			if (!this.dataListByMode.TryGetValue(playerNum, out list) || list == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				ChapterAssistConfig chapterAssistConfig = list[i];
				if (chapterAssistConfig != null && (chapterAssistConfig.level1 == levelId || chapterAssistConfig.level2 == levelId || chapterAssistConfig.level3 == levelId || chapterAssistConfig.level4 == levelId))
				{
					return chapterAssistConfig;
				}
			}
			return null;
		}

		public bool TryGetPlayerControl(string levelId, int playerNum, out int[] camps)
		{
			camps = null;
			ChapterAssistConfig dataByLevelId = this.GetDataByLevelId(levelId, playerNum);
			if (dataByLevelId == null || string.IsNullOrEmpty(dataByLevelId.playerControl))
			{
				if (playerNum == 5)
				{
					camps = new int[]
					{
						1,
						2,
						3,
						4,
						6
					};
					return true;
				}
				return false;
			}
			string[] array = dataByLevelId.playerControl.Split(new char[]
			{
				',',
				'，'
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length != playerNum)
			{
				if (playerNum == 5)
				{
					camps = new int[]
					{
						1,
						2,
						3,
						4,
						6
					};
					return true;
				}
				return false;
			}
			int[] array2 = new int[array.Length];
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < array.Length; i++)
			{
				int num;
				if (!int.TryParse(array[i].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out num) || num <= 0 || num >= LocalPlayer.MaxTeamNum || hashSet.Contains(num))
				{
					return false;
				}
				hashSet.Add(num);
				array2[i] = num;
			}
			camps = array2;
			return true;
		}

		public void LoadExtraData()
		{
		}

		private int currentCoopPlayerNum = 4;

		private readonly Dictionary<int, List<ChapterAssistConfig>> dataListByMode = new Dictionary<int, List<ChapterAssistConfig>>();

		private readonly Dictionary<int, Dictionary<string, ChapterAssistConfig>> dataByIdByMode = new Dictionary<int, Dictionary<string, ChapterAssistConfig>>();
	}
}
