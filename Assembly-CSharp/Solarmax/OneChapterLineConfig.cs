using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class OneChapterLineConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.point1 = element.GetAttribute("point1", string.Empty);
			this.point2 = element.GetAttribute("point2", string.Empty);
			return true;
		}

		public string point1;

		public string point2;
	}
}
