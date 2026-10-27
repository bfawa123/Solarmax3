using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MapBarrierPoingConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.tag = element.GetAttribute("tag", null);
			this.range = Convert.ToSingle(element.GetAttribute("range", "1.0"));
			return true;
		}

		public string tag;

		public float range;
	}
}
