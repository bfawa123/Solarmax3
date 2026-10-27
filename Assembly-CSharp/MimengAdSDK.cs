using System;
using UnityEngine;

public class MimengAdSDK : Solarmax.Singleton<MimengAdSDK>
{
	public void Init()
	{
	}

	public void LoadAd()
	{
		if (this.javaAdManager == null || this.mainActivity == null)
		{
			this.javaAdManager.Call("Init", new object[]
			{
				this.mainActivity,
				MimengAdSDK.debugOpen,
				MimengAdSDK.APP_ID,
				MimengAdSDK.APP_KEY,
				MimengAdSDK.APP_TOKEN,
				MimengAdSDK.AD_ID,
				MimengAdSDK.BANNER_ID,
				MimengAdSDK.INSERTER_ID,
				"Main Camera",
				"onVideoComplete",
				string.Empty,
				false
			});
		}
		else
		{
			this.javaAdManager.Call("CreateActivity", new object[]
			{
				this.mainActivity
			});
		}
	}

	public void ShowAd()
	{
		if (this.javaAdManager == null)
		{
			return;
		}
		this.javaAdManager.Call("ShowVideoAd", new object[0]);
	}

	public bool AdIsReady()
	{
		if (this.javaAdManager == null)
		{
			return false;
		}
		bool flag = this.javaAdManager.Call<bool>("IsVideoAdReady", new object[0]);
		if (!flag)
		{
			this.LoadAd();
		}
		return flag;
	}

	private static string APP_ID = "2882303761517966524";

	private static string APP_KEY = "fake_app_key";

	private static string APP_TOKEN = "fake_app_token";

	private static string AD_ID = "1166a5d9069c2cdd6361d7a2139ecc1e";

	private static string BANNER_ID = "802e356f1726f9ff39c69308bfd6f06a";

	private static string INSERTER_ID = "67b05e7cc9533510d4b8d9d4d78d0ae9";

	private static bool debugOpen = true;

	public AndroidJavaObject mainActivity;

	public AndroidJavaObject javaAdManager;

	private string AAR = "com.xiaomi.ad.demo.AdApplication";
}
