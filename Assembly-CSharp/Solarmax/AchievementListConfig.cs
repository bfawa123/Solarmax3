using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class AchievementListConfig
	{
		public bool Load(XElement element)
		{
			this.levelGroup = element.GetAttribute("levelGroup", string.Empty);
			this.achieveList = element.GetStringArray("list", ',');
			return true;
		}

		public string levelGroup;

		public string[] achieveList;
	}
}
