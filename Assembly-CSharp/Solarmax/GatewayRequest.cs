using System;
using UnityEngine;

namespace Solarmax
{
	public static class GatewayRequest
	{
		public static GatewayResponse Response
		{
			get
			{
				return (GatewayRequest.mRequestor != null) ? GatewayRequest.mRequestor.Response : null;
			}
		}

		public static void GetGateway(ServerListItemConfig serverConfig, Action onResponseDelegate)
		{
            if (serverConfig == null || string.IsNullOrEmpty(serverConfig.Url))
            {
                MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetGatewayError", "url", string.Empty);
                Debug.LogError("GetGateway: Empty server url");
                if (onResponseDelegate != null)
                {
                    onResponseDelegate();
                }
                return;
            }
            GatewayRequest.mRequestor = new WebRequestor<GatewayResponse>("GetGateway", new string[]
            {
                serverConfig.Url,
                serverConfig.IPAddress
            }, "gateway", null, false, onResponseDelegate);
            GatewayRequest.mRequestor.StartRequest(-1);
            //onResponseDelegate();
		}

		private static WebRequestor<GatewayResponse> mRequestor;
	}
}
