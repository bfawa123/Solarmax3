using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class LotteryConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetIntAttribute("id", -1);
			this.percent = element.GetFloatAttribute("percent", 0f);
			this.itemType = element.GetIntAttribute("itemType", 0);
			this.itemId = element.GetIntAttribute("itemId", 0);
			this.num = element.GetIntAttribute("num", 0);
			return true;
		}

		public int id;

		public float percent;

		public int itemType;

		public int itemId;

		public int num;
	}
}
