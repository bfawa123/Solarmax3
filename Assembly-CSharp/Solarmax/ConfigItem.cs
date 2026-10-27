using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class ConfigItem : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.configName = element.GetAttribute("Name", string.Empty);
			this.configValue = element.GetAttribute("Value", string.Empty);
			return true;
		}

		public string configName;

		public string configValue;
	}
}
