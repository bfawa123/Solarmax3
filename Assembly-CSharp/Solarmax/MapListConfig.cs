using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MapListConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.mapID = element.GetAttribute("name", string.Empty);
			if (this.mapID == "DLAM01")
			{
				int num = 0;
				num++;
			}
			this.version = Convert.ToInt32(element.GetAttribute("version", "0"));
			this.nAdd = Convert.ToInt32(element.GetAttribute("addss", "0"));
			return true;
		}

		public string mapID;

		public int version;

		public int nAdd;
	}
}
