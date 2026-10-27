using System;

namespace Solarmax
{
	[Serializable]
	public class GameHostList
	{
		public static GameHostList TestData
		{
			get
			{
				return new GameHostList
				{
					Hosts = new string[]
					{
						"http://120.92.132.77:8192"
					}
				};
			}
		}

		public string[] Hosts;
	}
}
