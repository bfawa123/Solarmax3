using System;

namespace Solarmax
{
	[Serializable]
	public class GameConfig
	{
		public string AppId;

		public string[] VersionServerUrls;

		public string[] ServerUrls;

		public bool EnablePay;

		public bool EnableFetchProducts;

		public bool EnableVisitorLogin;

		public bool Oversea;
	}
}
