using System;
using UnityEngine;

namespace TencentMobileGaming
{
	public class QAVSDKInit
	{
		public static void InitSDK()
		{
			Debug.LogWarning("QAVSDKInit  InitSDK");
			if (!QAVSDKInit.inited)
			{
				AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
				Debug.Log("initOpensdk Complete." + new AndroidJavaObject("com.tencent.av.wrapper.OpensdkGameWrapper", new object[]
				{
					@static
				}).Call<bool>("initOpensdk", new object[0]).ToString());
				QAVSDKInit.inited = true;
			}
		}

		private static bool inited;
	}
}
