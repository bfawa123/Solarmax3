using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class NameFilterConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", "0");
			this.desc = element.GetAttribute("desc", "0");
			return true;
		}

		public string id;

		public string desc;
	}
}
