using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class ConfigSystemProvider : Solarmax.Singleton<ConfigSystemProvider>, IDataProvider
	{
		public string Path()
		{
			return "config/config.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		private Dictionary<string, ConfigItem> mapDoctopmaru = new Dictionary<string, ConfigItem>();
	}
}
