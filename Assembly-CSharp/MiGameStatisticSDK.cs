using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MiGameStatisticSDK : Solarmax.Singleton<MiGameStatisticSDK>
{
	public static void Init()
	{
	}

	private static IEnumerator FetchToken()
	{
		MiGameStatisticSDK.TokenRequest request = new MiGameStatisticSDK.TokenRequest();
		request.appId = "528288548881694720";
		if (string.IsNullOrEmpty(Solarmax.Singleton<LocalAccountStorage>.Get().account))
		{
			request.accountId = SystemInfo.deviceUniqueIdentifier;
		}
		else
		{
			request.accountId = Solarmax.Singleton<LocalAccountStorage>.Get().account;
		}
		request.timestamp = 0UL;
		request.roleId = request.accountId;
		request.tpOpenId = request.accountId;
		request.platform = 0;
		string request_json = JsonUtility.ToJson(request);
		Debug.Log("MiGameStatisticSDK fetch token from json: " + request_json);
		byte[] request_json_bytes = Encoding.UTF8.GetBytes(request_json);
		string secret = "21e95a0f259f49229ce5d8217f198faf";
		secret = (secret ?? string.Empty);
		byte[] scrKey = Encoding.ASCII.GetBytes(secret);
		HMACSHA256 hmac = new HMACSHA256(scrKey);
		byte[] hash = hmac.ComputeHash(request_json_bytes);
		string strHash = MiGameStatisticSDK.ToHexString(hash);
		string signature = strHash.ToLower();
		string url = "https://gamecommunity.mgp.mi.com/gamecomm/personalcenter/v1/gettoken?signature=hmac-sha256:" + signature;
		Debug.Log("MiGameStatisticSDK fetch token from url: " + url);
		WWW www = new WWW(url, request_json_bytes, new Dictionary<string, string>
		{
			{
				"Content-Type",
				"application/json"
			}
		});
		yield return www;
		Debug.Log("MiGameStatisticSDK: fetch token and result with http text=" + www.text);
		MiGameStatisticSDK.TokenResponse response = JsonUtility.FromJson<MiGameStatisticSDK.TokenResponse>(www.text);
		MiGameStatisticSDK.token_ = response.resultBody;
		Debug.Log("MiGameStatisticSDK: fetch token and result with http token=" + MiGameStatisticSDK.token_);
		global::Coroutine.Start(MiGameStatisticSDK.FetchConfig());
		yield break;
	}

	public static string ToHexString(byte[] bytes)
	{
		string result = string.Empty;
		if (bytes != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append(bytes[i].ToString("X2"));
			}
			result = stringBuilder.ToString();
		}
		return result;
	}

	private static IEnumerator FetchConfig()
	{
		WWW www = new WWW("https://gamecommunity.mgp.mi.com/gamecomm/personalcenter/v1/getconfig?token=" + MiGameStatisticSDK.token_);
		yield return www;
		MiGameStatisticSDK.ConfigResponse config = JsonUtility.FromJson<MiGameStatisticSDK.ConfigResponse>(www.text);
		if (string.IsNullOrEmpty(config.resultBody.gameCommunityWebUrl))
		{
			yield break;
		}
		MiGameStatisticSDK.feedback_url_ = config.resultBody.gameCommunityWebUrl;
		Debug.Log("MiGameStatisticSDK: fetch url_ and result with http Url=" + MiGameStatisticSDK.feedback_url_);
		MiGameStatisticSDK.InitAndroidSdk();
		yield break;
	}

	private static bool InitAndroidSdk()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		if (androidJavaClass == null)
		{
			return false;
		}
		MiGameStatisticSDK.current_activity_ = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.xiaomi.migamechannel.MiGameChannel");
		AndroidJavaObject @static = androidJavaClass2.GetStatic<AndroidJavaObject>("XiaoMi");
		AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.xiaomi.migamechannel.MiGameRoleConfig");
		MiGameStatisticSDK.MiGameStatistics = new AndroidJavaClass("com.xiaomi.migamechannel.MiGameStatistics");
		if (MiGameStatisticSDK.MiGameStatistics == null)
		{
			return false;
		}
		MiGameStatisticSDK.MiGameStatistics.CallStatic<bool>("Initialize", new object[]
		{
			MiGameStatisticSDK.current_activity_,
			"528288548881694720",
			@static
		});
		AndroidJavaObject miGameStatistics = MiGameStatisticSDK.MiGameStatistics;
		string methodName = "OfficeWebsiteInitialize";
		object[] array = new object[2];
		array[0] = MiGameStatisticSDK.token_;
		miGameStatistics.CallStatic(methodName, array);
		Debug.Log("MiGameStatisticSDK: InitAndroid OK!!!");
		return true;
	}

	public static void OpenWebView()
	{
	}

	private static bool initialized_;

	private static AndroidJavaClass MiGameStatistics;

	private static AndroidJavaObject current_activity_;

	private static string token_;

	private static string feedback_url_;

	public const string APPID = "528288548881694720";

	[Serializable]
	public class TokenRequest
	{
		public string appId;

		public ulong timestamp;

		public string accountId;

		public string roleId;

		public string tpOpenId;

		public int platform;
	}

	[Serializable]
	public class TokenResponse
	{
		public string result;

		public string resultCode;

		public string resultMsg;

		public string resultBody;
	}

	[Serializable]
	public class BizConfig
	{
		public string code;

		public string url;

		public string note;
	}

	[Serializable]
	public class ConfigBody
	{
		public int entryStatus;

		public int communityStatus;

		public string gameCommunityWebUrl;

		public List<MiGameStatisticSDK.BizConfig> bizConfig;
	}

	public class ConfigResponse
	{
		public bool result;

		public string resultCode;

		public string resultMsg;

		public MiGameStatisticSDK.ConfigBody resultBody;
	}
}
