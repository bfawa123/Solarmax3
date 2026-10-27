using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MonthCheckConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetIntAttribute("id", -1);
			return true;
		}

		public int id;

		public Dictionary<int, DayRewardConfig> dayRewards = new Dictionary<int, DayRewardConfig>();
	}
}
