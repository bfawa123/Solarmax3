using System;
using UnityEngine;

namespace Solarmax
{
	[Serializable]
	public class UpgradeResponse : IWebResponse
	{
		public bool HasError
		{
			get
			{
				return this.error_code != 200;
			}
		}

		public static UpgradeResponse TestData
		{
			get
			{
				return new UpgradeResponse
				{
					error_code = 200,
					version_name = "0.0.1",
					version_code = 1,
					version_status = 1,
					update_type = 0,
					update_url = "http://solarmax-cn.ks3-cn-shanghai.ksyun.com/solarmax/staging/0.0.2.2/res",
					host_list = JsonUtility.ToJson(GameHostList.TestData)
				};
			}
		}

		public int error_code;

		public string version_name;

		public int version_code;

		public int version_status;

		public int update_type;

		public string update_url;

		public string host_list;
	}
}
