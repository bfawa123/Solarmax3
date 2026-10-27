using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class DayRewardConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetIntAttribute("id", -1);
			this.rewardType = element.GetIntAttribute("rewardType", -1);
			this.misc = element.GetIntAttribute("misc", -1);
			this.desc = element.GetIntAttribute("desc", -1);
			this.icon = element.GetAttribute("icon", string.Empty);
			return true;
		}

		public int id;

		public int rewardType;

		public int misc;

		public int desc;

		public string icon;
	}
}
