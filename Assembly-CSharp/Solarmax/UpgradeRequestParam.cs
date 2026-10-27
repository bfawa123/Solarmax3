using System;

namespace Solarmax
{
	[Serializable]
	public class UpgradeRequestParam
	{
		public string version_name;

		public int version_code;

		public string channel;

		public string app_id;

		public string package_name;

		public string platform;

		public string system_version;

		public string imei;

		public string security_code;
	}
}
