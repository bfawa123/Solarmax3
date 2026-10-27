using System;
using Solarmax;
using UnityEngine;

internal class BuglyTools : MonoSingleton<BuglyTools>
{
	public void InitSDK()
	{
		BuglyAgent.ConfigDebugMode(false);
		try
		{
			ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
			string appVersion = UpgradeUtil.GetAppVersion();
			BuglyAgent.ConfigDefault(channelConfig.ChannelId, appVersion, "Unknown", 0L);
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("BuglyTools: BuglyAgent.ConfigDefault failed!\n{0}", new object[]
			{
				ex.ToString()
			});
		}
		BuglyAgent.InitWithAppId("4f7fde19be");
		BuglyAgent.EnableExceptionHandler();
		BuglyAgent.SetScene(3450);
		BuglyAgent.PrintLog(LogSeverity.LogInfo, "Init the bugly sdk", new object[0]);
	}

	public void SetUserId(string userId)
	{
		BuglyAgent.SetUserId(userId);
	}

	private const string BuglyAppIDForiOS = "805efc498b";

	private const string BuglyAppIDForAndroid = "4f7fde19be";
}
