using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class MiGameAnalytics
{
	public static void InitSDK()
	{
		ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
		string channelId = channelConfig.ChannelId;
		string miAnalyticsAppID = channelConfig.MiAnalyticsAppID;
		string miAnalyticsAppKey = channelConfig.MiAnalyticsAppKey;
		Debug.Log("call MiGameSDK InitMiStatisticsSDK...");
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("InitMiStatisticsSDK", new object[]
		{
			miAnalyticsAppID,
			miAnalyticsAppKey,
			channelId
		});
	}

	public static void MiAnalyticsNoticeEffect()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", SystemInfo.deviceUniqueIdentifier.ToString());
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"NoticeEvent",
			empty
		});
	}

	public static void MiAnalyticsSelectServer(string strRet)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", SystemInfo.deviceUniqueIdentifier.ToString());
		dictionary.Add("SelectServer", strRet);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"SelectServer",
			empty
		});
	}

	public static void MiAnalyticsSDKLoginStart(bool isVisitor)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("account", SystemInfo.deviceUniqueIdentifier.ToString());
		dictionary.Add("visitor", isVisitor.ToString());
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"SDKLoginStart",
			empty
		});
	}

	public static void MiAnalyticsLoginSDKSuccess(string account, bool isVisitor)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("account", account);
		dictionary.Add("visitor", isVisitor.ToString());
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"SDKLoginSuccess",
			empty
		});
	}

	public static void MiAnalyticsLoginSDKFailed(string info)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("channel", MiGameAnalytics.channel);
		dictionary.Add("info", info);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"SDKLoginFailed",
			empty
		});
	}

	public static void MiAnalyticsUserDataInit()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("UserId", playerData.userId.ToString());
		dictionary.Add("UserName", playerData.name);
		dictionary.Add("Money", playerData.money.ToString());
		dictionary.Add("DeviceModel", SystemInfo.deviceModel.ToString());
		dictionary.Add("DeviceUniqueIdentifier", SystemInfo.deviceUniqueIdentifier.ToString());
		dictionary.Add("OperatingSystem", SystemInfo.operatingSystem);
		dictionary.Add("ProcessorType", SystemInfo.processorType);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"UserDataInit",
			empty
		});
	}

	public static void MiAnalyticsMoneyCostEvent(string costType, string szReason, string strValue)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("CostType", costType);
		dictionary.Add("CostReason", szReason);
		dictionary.Add("CostValue", strValue);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"MoneyCostEvent",
			empty
		});
	}

	public static void MiAnalyticsBattleEndEvent(string strLevel, string szRet, string score, string star, string destroy, string lost, string totalTime)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Level", strLevel);
		dictionary.Add("Type", szRet);
		dictionary.Add("Score", score);
		dictionary.Add("Star", star);
		dictionary.Add("Destroy", destroy);
		dictionary.Add("Lost", lost);
		dictionary.Add("TotalTime", totalTime);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"EndbattleEvent",
			empty
		});
	}

	public static void MiAnalyticsPVPBattleEndEvent(string matchType, string strLevel, string score, string destroy, string lost, string totalTime)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("MatchType", matchType);
		dictionary.Add("Level", strLevel);
		dictionary.Add("Score", score);
		dictionary.Add("Destroy", destroy);
		dictionary.Add("Lost", lost);
		dictionary.Add("TotalTime", totalTime);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"EndbattlePvpEvent",
			empty
		});
	}

	public static void MiAnalyticsPVPBattleMatchEvent(string matchType, string strLevel, string matchState, string matchTime, string roomID)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("MatchType", matchType);
		dictionary.Add("Level", strLevel);
		dictionary.Add("MatchState", matchState);
		dictionary.Add("MatchTime", matchTime);
		dictionary.Add("RoomID", roomID);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"MatchBattlePvpEvent",
			empty
		});
	}

	public static void MiAnalyticsPaymentEvent(string productName, string productId, int num, string currency, double price, string transactionId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"PaymentEvent",
			empty
		});
	}

	public static void MiAnalyticsAdsClickEvent()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"AdsClick",
			empty
		});
	}

	public static void MiAnalyticsAdsEndEvent()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"AdsEnd",
			empty
		});
	}

	public static void MiAnalyticsOpenADSEvent(string rewardCoin)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		dictionary.Add("RewardCoin", rewardCoin);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"AdsOpen",
			empty
		});
	}

	public static void MiAnalyticsRewardCoinADSEvent(string rewardCoin)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		dictionary.Add("RewardCoin", rewardCoin);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"AdsReward",
			empty
		});
	}

	public static void MiAnalyticsFinishTaskEvent(string taskId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("channel", MiGameAnalytics.channel);
		dictionary.Add("taskId", taskId);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"finishTask",
			empty
		});
	}

	public static void MiAnalyticsCreateOrderIDEvent(string productName, string productId, int num, string currency, double price, string transactionId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("ProductId", productId);
		dictionary.Add("ProductName", productName);
		dictionary.Add("ProductNum", num.ToString());
		dictionary.Add("Currency", currency);
		dictionary.Add("Price", price.ToString());
		dictionary.Add("OrderId", transactionId);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"CreateOrder",
			empty
		});
	}

	public static void MiAnalyticsLogPayStart(string productName, string productId, int num, string currency, double price, string orderId, string notifyUrl)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("ProductId", productId);
		dictionary.Add("ProductName", productName);
		dictionary.Add("ProductNum", num.ToString());
		dictionary.Add("Currency", currency);
		dictionary.Add("Price", price.ToString());
		dictionary.Add("OrderId", orderId);
		dictionary.Add("NotifyUrl", notifyUrl);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"PayStart",
			empty
		});
	}

	public static void MiAnalyticsLogPaySuccess(string info)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Info", info);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"PaySuccess",
			empty
		});
	}

	public static void MiAnalyticsLogPayFailed(string info)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("Info", info);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"PayFailed",
			empty
		});
	}

	public static void MiAnalyticsLogOrderComplete(string productName, string productId, int num, string currency, double price, string orderId)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Account", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		dictionary.Add("ProductId", productId);
		dictionary.Add("ProductName", productName);
		dictionary.Add("ProductNum", num.ToString());
		dictionary.Add("Currency", currency);
		dictionary.Add("Price", price.ToString());
		dictionary.Add("OrderId", orderId);
		dictionary.Add("Channel", MiGameAnalytics.channel);
		string empty = string.Empty;
		MiGameAnalytics.miGameAnalyticsClass.CallStatic("LogEvent", new object[]
		{
			"OrderComplete",
			empty
		});
	}

	private static AndroidJavaClass miGameAnalyticsClass = new AndroidJavaClass("com.xiaomi.gamecenter.sdk.mi.LoginPlatformActivity");

	private static string channel = string.Empty;
}
