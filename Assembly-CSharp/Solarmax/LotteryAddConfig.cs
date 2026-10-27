using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class LotteryAddConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetIntAttribute("id", -1);
			this.count = element.GetIntAttribute("count", 0);
			this.type = element.GetIntAttribute("tpye", 0);
			this.itemId = element.GetIntAttribute("itemId", 0);
			this.num = element.GetIntAttribute("num", 0);
			return true;
		}

		public int id;

		public int count;

		public int type;

		public int itemId;

		public int num;
	}
}
