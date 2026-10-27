using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Solarmax
{
	public static class ServerListRequest
	{
		public static ServerListResponse Response
		{
			get
			{
				return (ServerListRequest.mRequestor != null) ? ServerListRequest.mRequestor.Response : null;
			}
		}

		public static void GetServerList(Action onResponseDelegate)
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info("Request for game server", new object[0]);
            string[] array = UpgradeUtil.GetGameConfig().ServerUrls;
			if (array == null || array.Length < 1)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetServerListError", "info", "EmptyHosts");
				Debug.LogError("GetServerList: Empty hosts");
				if (onResponseDelegate != null)
				{
					onResponseDelegate();
				}
				return;
			}
			string languageNameConfig = Solarmax.Singleton<LanguageDataProvider>.Get().GetLanguageNameConfig();
			string param = string.Format("language={0}", UnityWebRequest.EscapeURL(languageNameConfig));
			ServerListRequest.mRequestor = new WebRequestor<ServerListResponse>("GetServerList", array, "serverList", param, false, onResponseDelegate);
            ServerListRequest.mRequestor.StartRequest(-1);
		}

		private static WebRequestor<ServerListResponse> mRequestor;
	}
}
