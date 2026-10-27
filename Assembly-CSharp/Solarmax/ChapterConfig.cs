using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class ChapterConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", "0");
			this.type = Convert.ToInt32(element.GetAttribute("type", "0"));
			this.posX = Convert.ToSingle(element.GetAttribute("posX", "0"));
			this.posY = Convert.ToSingle(element.GetAttribute("posY", "0"));
			this.costGold = Convert.ToInt32(element.GetAttribute("costGold", "0"));
			this.costGold = 0;
			this.needStar = Convert.ToInt32(element.GetAttribute("needstar", "0"));
			this.dependChapter = element.GetAttribute("dependchapter", string.Empty);
			this.name = Convert.ToInt32(element.GetAttribute("name", "0"));
			this.describe = Convert.ToInt32(element.GetAttribute("describe", "0"));
			this.starChart = element.GetAttribute("starchart", string.Empty);
			this.chapterBG = element.GetAttribute("chapterBg", string.Empty);
			this.comment = Convert.ToInt32(element.GetAttribute("comment", "0"));
			this.commentMultiple = Convert.ToInt32(element.GetAttribute("commentMultiple", "0"));
			return true;
		}

		public OneChapterPointConfig GetPoint(string id)
		{
			OneChapterPointConfig result = null;
			if (this.oneChapterPointConfigs.ContainsKey(id))
			{
				result = this.oneChapterPointConfigs[id];
			}
			return result;
		}

		public string id;

		public int type;

		public float posX;

		public float posY;

		public int costGold;

		public int needStar;

		public string dependChapter;

		public int name;

		public int describe;

		public string starChart;

		public string chapterBG;

		public int comment;

		public int commentMultiple;

		public List<OneChapterLineConfig> oneChapterLineConfigs = new List<OneChapterLineConfig>();

		public Dictionary<string, OneChapterPointConfig> oneChapterPointConfigs = new Dictionary<string, OneChapterPointConfig>();
	}
}
