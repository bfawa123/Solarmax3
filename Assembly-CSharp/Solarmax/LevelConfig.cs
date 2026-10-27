using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class LevelConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.levelGroup = element.GetAttribute("levelGroup", string.Empty);
			this.dependLevel = element.GetAttribute("dependlevel", string.Empty);
			this.chapter = element.GetAttribute("chapter", string.Empty);
			this.map = element.GetAttribute("map", string.Empty);
			this.ifBlackStrike = Convert.ToInt32(element.GetAttribute("IfBlackStrike", "0"));
			this.blackStrikeThreshold = Convert.ToInt32(element.GetAttribute("IfBlackStrikeThreshold", "220"));
			this.mainLine = Convert.ToInt32(element.GetAttribute("mainline", "0"));
			this.easyAI = Convert.ToInt32(element.GetAttribute("easyAI", "0"));
			this.generalAI = Convert.ToInt32(element.GetAttribute("generalAI", "0"));
			this.hardAI = Convert.ToInt32(element.GetAttribute("hardAI", "0"));
			this.maxStar = Convert.ToInt32(element.GetAttribute("maxstar", "3"));
			this.aiType = element.GetAttribute("aitype", "normal");
			this.aiParam = Convert.ToInt32(element.GetAttribute("aiparam", "1"));
			this.dyncDiffType = element.GetAttribute("dyncdifftype", "normal");
			this.playerTeam = Convert.ToInt32(element.GetAttribute("playerTeam", "1"));
			this.difficult = Convert.ToInt32(element.GetAttribute("difficult", "1"));
			this.winType = element.GetAttribute("wintype", "killall");
			this.winTypeParam1 = element.GetAttribute("wintypeparam1", string.Empty);
			this.winTypeParam2 = element.GetAttribute("wintypeparam2", string.Empty);
			string text = element.GetAttribute("surival", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				text = element.GetAttribute("survival", string.Empty);
			}
			if (!string.IsNullOrEmpty(text))
			{
				int num = text.IndexOf('(');
				if (num >= 0)
				{
					int num2 = text.IndexOf(')', num + 1);
					if (num2 > num)
					{
						text = text.Substring(num + 1, num2 - num - 1);
					}
				}
				float num3;
				if (float.TryParse(text.Trim(), out num3) && num3 > 0f)
				{
					this.winType = "alive";
					this.winTypeParam1 = num3.ToString();
					this.winTypeParam2 = string.Empty;
				}
			}
			this.loseType = element.GetAttribute("losetype", "otherswin");
			this.loseTypeParam1 = element.GetAttribute("losetypeparam1", string.Empty);
			this.loseTypeParam2 = element.GetAttribute("losetypeparam2", string.Empty);
			this.levelName = Convert.ToInt32(element.GetAttribute("levelname", "0"));
			this.awardPower = Convert.ToInt32(element.GetAttribute("awardpower", "0"));
			this.awardGold = Convert.ToInt32(element.GetAttribute("awardgold", "0"));
			this.starType = element.GetAttribute("starType", string.Empty);
			this.star1Gold = Convert.ToInt32(element.GetAttribute("star1gold", "0"));
			this.star2Gold = Convert.ToInt32(element.GetAttribute("star2gold", "0"));
			this.star3Gold = Convert.ToInt32(element.GetAttribute("star3gold", "0"));
			this.star4Gold = Convert.ToInt32(element.GetAttribute("star4gold", "0"));
			this.star4Dead = Convert.ToInt32(element.GetAttribute("star4dead", "-1"));
			this.star3Dead = Convert.ToInt32(element.GetAttribute("star3dead", "100"));
			this.star2Dead = Convert.ToInt32(element.GetAttribute("star2dead", "200"));
			this.scoreper = Convert.ToInt32(element.GetAttribute("scoreper", "1"));
			this.costpower = Convert.ToInt32(element.GetAttribute("costpower", "0"));
			this.musicName = element.GetAttribute("musicName", string.Empty);
			this.insideMap = element.GetAttribute("inside_map", string.Empty);
			this.insideMusicName = element.GetAttribute("inside_musicName", string.Empty);
			return true;
		}

		public bool IfBlackStrike()
		{
			return this.ifBlackStrike > 0;
		}

		public int GetBlackStrikeThreshold()
		{
			return (this.blackStrikeThreshold > 0) ? this.blackStrikeThreshold : 220;
		}

		public string id;

		public string levelGroup;

		public string dependLevel;

		public string chapter;

		public string map;

		public int ifBlackStrike;

		public int blackStrikeThreshold;

		public int mainLine;

		public int easyAI;

		public int generalAI;

		public int hardAI;

		public int maxStar;

		public string aiType;

		public int aiParam;

		public string dyncDiffType;

		public int playerTeam;

		public int difficult;

		public string winType;

		public string winTypeParam1;

		public string winTypeParam2;

		public string loseType;

		public string loseTypeParam1;

		public string loseTypeParam2;

		public float xPositionOfStar;

		public float yPositionOfStar;

		public float zPositionOfStar;

		public int levelName;

		public int awardPower;

		public int awardGold;

		public string starType;

		public int star1Gold;

		public int star2Gold;

		public int star3Gold;

		public int star4Gold;

		public int star4Dead;

		public int star3Dead;

		public int star2Dead;

		public int scoreper;

		public int costpower;

		public string musicName;

		public string insideMap;

		public string insideMusicName;

		/// <summary>
		/// 按逗号拆分 inside_map，支持链式地图切换（如 "500107.1,500107.2"）。
		/// </summary>
		public string[] GetInsideMapList()
		{
			if (string.IsNullOrEmpty(this.insideMap))
			{
				return new string[0];
			}
			return this.insideMap.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 按逗号拆分 inside_musicName，与 GetInsideMapList 按下标一一对应。
		/// </summary>
		public string[] GetInsideMusicNameList()
		{
			if (string.IsNullOrEmpty(this.insideMusicName))
			{
				return new string[0];
			}
			return this.insideMusicName.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public List<OneChapterLevelConfig> listOneChapterLevelConfig = new List<OneChapterLevelConfig>();
	}
}
