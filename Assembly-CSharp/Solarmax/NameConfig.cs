using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class NameConfig : ICfgEntry
	{
		public string Path()
		{
			return "data/name.txt";
		}

		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", "0");
			this.type = Convert.ToInt32(element.GetAttribute("type", "0"));
			this.chinese = element.GetAttribute("chinese", string.Empty);
			this.english = element.GetAttribute("english", string.Empty);
			return true;
		}

		public string id;

		public int type;

		public string chinese;

		public string english;
	}
}
