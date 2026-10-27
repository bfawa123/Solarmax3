using System;

namespace Solarmax
{
	[Serializable]
	public class GatewayResponse : IWebResponse
	{
		public bool HasError
		{
			get
			{
				return this.ErrorCode != 200;
			}
		}

		public static GatewayResponse TestData
		{
			get
			{
				return new GatewayResponse
				{
					ErrorCode = 200,
					Host = "120.92.132.77:8192",
					PayNotifyUrl = "http://120.92.132.77:8299"
				};
			}
		}

		public int ErrorCode;

		public string Host;

		public string PayNotifyUrl;
	}
}
