using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class OneChapterLevelConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.dependLevel = element.GetAttribute("dependLevel", string.Empty);
			this.chapter = element.GetAttribute("chapter", string.Empty);
			this.maxStar = Convert.ToInt32(element.GetAttribute("maxstar", "3"));
			return true;
		}

		public string chapter;

		public string id;

		public string dependLevel;

		public int maxStar;
	}
}
