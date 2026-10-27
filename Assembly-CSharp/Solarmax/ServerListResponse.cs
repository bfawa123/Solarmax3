using System;

namespace Solarmax
{
	[Serializable]
	public class ServerListResponse : IWebResponse
	{
		public bool HasError
		{
			get
			{
				return this.ErrorCode != 200;
			}
		}

		public static ServerListResponse TestData
		{
			get
			{
				return new ServerListResponse
				{
					ErrorCode = 200,
					Servers = new ServerListItemConfig[]
					{
						new ServerListItemConfig
						{
							Name = "Test1",
							Url = "http://120.92.132.77:8192"
						},
						new ServerListItemConfig
						{
							Name = "Test2",
							Url = "http://120.92.132.78:8192"
						}
					}
				};
			}
		}

		public int ErrorCode;

		public ServerListItemConfig[] Servers;
	}
}
