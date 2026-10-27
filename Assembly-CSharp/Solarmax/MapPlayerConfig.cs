using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MapPlayerConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.tag = element.GetAttribute("tag", string.Empty);
			this.ship = Convert.ToInt32(element.GetAttribute("ship", "0"));
			this.camption = Convert.ToInt32(element.GetAttribute("camption", "0"));
			return true;
		}

		public string id;

		public string tag;

		public int ship;

		public int camption;
	}
}
