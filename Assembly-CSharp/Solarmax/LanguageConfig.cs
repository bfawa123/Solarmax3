using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class LanguageConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			return true;
		}

		public bool Load(XElement element, string language)
		{
			this.mID = Convert.ToInt32(element.GetAttribute("id", null));
			this.mData = element.GetAttribute(language, string.Empty);
			return true;
		}

		public int mID = -1;

		public string mData = string.Empty;
	}
}
