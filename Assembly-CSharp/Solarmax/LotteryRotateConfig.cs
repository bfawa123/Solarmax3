using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class LotteryRotateConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetIntAttribute("id", -1);
			this.startTime = element.GetAttribute("starTime", string.Empty);
			this.endTime = element.GetAttribute("endTime", string.Empty);
			XElement xelement = element.Element("lottery_id");
			this.lottery_ids = new List<int>();
			IEnumerable<XAttribute> enumerable = xelement.Attributes();
			foreach (XAttribute xattribute in enumerable)
			{
				int item = 0;
				int.TryParse(xattribute.Value, out item);
				this.lottery_ids.Add(item);
			}
			this.lottery_add_ids = new List<int>();
			XElement xelement2 = element.Element("lottery_add_id");
			IEnumerable<XAttribute> enumerable2 = xelement2.Attributes();
			foreach (XAttribute xattribute2 in enumerable2)
			{
				int item2 = 0;
				int.TryParse(xattribute2.Value, out item2);
				this.lottery_add_ids.Add(item2);
			}
			return true;
		}

		public int id;

		public string startTime;

		public string endTime;

		public List<int> lottery_ids;

		public List<int> lottery_add_ids;
	}
}
