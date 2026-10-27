using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Solarmax
{
	public static class UpgradeRequest
	{
		public static UpgradeResponse Response { get; private set; }

		private static void OnResponse()
		{
			if (UpgradeRequest.mRequestor == null || UpgradeRequest.mRequestor.Response == null)
			{
				if (UpgradeRequest.mOnResponseDelegate != null)
				{
					UpgradeRequest.mOnResponseDelegate();
					UpgradeRequest.mOnResponseDelegate = null;
				}
				return;
			}
			UpgradeRequest.Response = UpgradeRequest.mRequestor.Response;
			try
			{
				UpgradeRequest.mHostList = JsonUtility.FromJson<GameHostList>(UpgradeRequest.Response.host_list);
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetVersionSuccess", "host", UpgradeRequest.Response.host_list);
			}
			catch (Exception ex)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetVersionFailed", "host", UpgradeRequest.Response.host_list);
				Debug.LogErrorFormat("Parse GameHostList from {0} failed!\n{1}", new object[]
				{
					UpgradeRequest.Response.host_list,
					ex.ToString()
				});
			}
			finally
			{
				if (UpgradeRequest.mOnResponseDelegate != null)
				{
					UpgradeRequest.mOnResponseDelegate();
					UpgradeRequest.mOnResponseDelegate = null;
				}
			}
		}

		public static string[] GetGameHosts()
		{
			if (UpgradeRequest.Response == null)
			{
				Debug.LogError("GetGameHosts: null Response");
				return null;
			}
			GameHostList gameHostList = UpgradeRequest.mHostList;
			if (UpgradeRequest.mHostList == null)
			{
				Debug.LogError("GetGameHosts: null HostList");
				return null;
			}
			string[] hosts = UpgradeRequest.mHostList.Hosts;
			if (hosts == null || hosts.Length < 1)
			{
				Debug.LogError("GetGameHosts: null Hosts");
				return null;
			}
			return hosts;
		}

		public static void GetUpgradeInfo(Action onResponseDelegate)
		{
			UpgradeRequest.Response = null;
			UpgradeRequest.mHostList = null;
			VersionConfig versionConfig = UpgradeUtil.GetVersionConfig();
			if (versionConfig == null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetVersionError", "info", "NullVersionConfig");
				UpgradeRequest.OnResponse();
				return;
			}
			GameConfig gameConfig = UpgradeUtil.GetGameConfig();
			if (gameConfig == null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetVersionError", "info", "NullGameConfig");
				UpgradeRequest.OnResponse();
				return;
			}
			ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
			if (channelConfig == null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetVersionError", "info", "NullChannelConfig");
				UpgradeRequest.OnResponse();
				return;
			}
			UpgradeRequestParam obj = new UpgradeRequestParam
			{
				version_name = versionConfig.VersionName,
				version_code = versionConfig.VersionCode,
				channel = channelConfig.ChannelId,
				app_id = gameConfig.AppId,
				package_name = Application.identifier,
				platform = Solarmax.Singleton<EngineSystem>.Instance.GetPlatform(),
				system_version = string.Empty,
				imei = Solarmax.Singleton<EngineSystem>.Instance.GetUUID(),
				security_code = Solarmax.Singleton<LocalStorageSystem>.Instance.GetLastLoginAccountId()
			};
			string text = JsonUtility.ToJson(obj);
			UpgradeRequest.mOnResponseDelegate = onResponseDelegate;
			string tag = "GetVersion";
			//string[] versionServerUrls = gameConfig.VersionServerUrls;
			string[] versionServerUrls = UpgradeUtil.GetGameConfig().ServerUrls;
            string subPath = "new_versions";
			string param = text;
			bool encrypt = true;
			if (UpgradeRequest.tmp0 == null)
			{
				UpgradeRequest.tmp0 = new Action(UpgradeRequest.OnResponse);
			}
			UpgradeRequest.mRequestor = new WebRequestor<UpgradeResponse>(tag, versionServerUrls, subPath, param, encrypt, UpgradeRequest.tmp0);
			UpgradeRequest.mRequestor.StartRequest(-1);
		}

		private static WebRequestor<UpgradeResponse> mRequestor;

		private static Action mOnResponseDelegate;

		private static GameHostList mHostList;

		[CompilerGenerated]
		private static Action tmp0;
	}
}
