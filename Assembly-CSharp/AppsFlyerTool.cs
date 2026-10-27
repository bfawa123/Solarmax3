using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class AppsFlyerTool
{
	public static void InitSDK()
	{
		AppsFlyerTool.mEnabled = false;
		ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
		if (string.IsNullOrEmpty(channelConfig.AppsFlyerKey) || string.IsNullOrEmpty(channelConfig.AppsFlyerAppId))
		{
			return;
		}
		AppsFlyerTool.mEnabled = true;
		AppsFlyer.setAppsFlyerKey(channelConfig.AppsFlyerKey);
		AppsFlyer.setIsDebug(false);
		AppsFlyer.setAppID(channelConfig.AppsFlyerAppId);
		AppsFlyer.init(channelConfig.AppsFlyerKey, "AppsFlyerTrackerCallbacks");
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				@static.Call("runOnUiThread", new object[]
				{
					new AndroidJavaRunnable(AppsFlyerTool.init_startEvent)
				});
			}
		}
	}

	private static void init_startEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyer.trackRichEvent("libra_open_app", null);
	}

	public static void FlyerLoginEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyer.trackRichEvent("af_login", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPVPBattleStartEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("play_id", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		AppsFlyer.trackRichEvent("custom_pvp_start", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPVPBattleEndEvent(string lost)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("play_id", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		AppsFlyerTool.purchaseEvent.Add("play_result", lost);
		AppsFlyer.trackRichEvent("custom_pvp_end", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPveBattleStartEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("play_id", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		AppsFlyer.trackRichEvent("custom_pve_start", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPveBattleEndEvent(string strResult)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("play_id", Solarmax.Singleton<LocalAccountStorage>.Get().account);
		AppsFlyerTool.purchaseEvent.Add("play_result", strResult);
		AppsFlyer.trackRichEvent("custom_pve_end", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerMoneyCostEvent(string costType, string szReason, string strValue)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("af_price", strValue);
		AppsFlyerTool.purchaseEvent.Add("af_content_type", costType);
		AppsFlyerTool.purchaseEvent.Add("af_content_id", szReason);
		AppsFlyer.trackRichEvent("af_spent_credits", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerCreateOrderIDEvent(string productName, string productId, int num, string currency, double price, string orderId)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("af_content_id", orderId);
		AppsFlyerTool.purchaseEvent.Add("af_content_type", productId);
		AppsFlyerTool.purchaseEvent.Add("af_description", productName);
		AppsFlyerTool.purchaseEvent.Add("af_quantity", num.ToString());
		AppsFlyerTool.purchaseEvent.Add("af_price", price.ToString());
		AppsFlyerTool.purchaseEvent.Add("af_currency", currency);
		AppsFlyer.trackRichEvent("af_order_id", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPayStartEvent(string productName, string productId, int num, string currency, double price, string orderId)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("af_content_id", orderId);
		AppsFlyerTool.purchaseEvent.Add("af_content_type", productId);
		AppsFlyerTool.purchaseEvent.Add("af_description", productName);
		AppsFlyerTool.purchaseEvent.Add("af_quantity", num.ToString());
		AppsFlyerTool.purchaseEvent.Add("af_price", price.ToString());
		AppsFlyerTool.purchaseEvent.Add("af_currency", currency);
		AppsFlyer.trackRichEvent("af_pay_start", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPaySuccessEvent(string info)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("af_content_id", info);
		AppsFlyer.trackRichEvent("af_pay_success", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerPaymentEvent(string productName, string productId, string currency, double price, string orderId)
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyerTool.purchaseEvent.Add("af_content_id", orderId);
		AppsFlyerTool.purchaseEvent.Add("af_content_type", productId);
		AppsFlyerTool.purchaseEvent.Add("af_description", productName);
		AppsFlyerTool.purchaseEvent.Add("af_revenue", price.ToString());
		AppsFlyerTool.purchaseEvent.Add("af_currency", currency);
		AppsFlyer.trackRichEvent("af_purchase", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerAdsClickEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyer.trackRichEvent("af_ad_click", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerAdsEndEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyer.trackRichEvent("af_ad_end", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerOpenADSEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyer.trackRichEvent("af_ad_open", AppsFlyerTool.purchaseEvent);
	}

	public static void FlyerRewardCoinADSEvent()
	{
		if (!AppsFlyerTool.mEnabled)
		{
			return;
		}
		AppsFlyerTool.purchaseEvent.Clear();
		AppsFlyer.trackRichEvent("af_ad_reward", AppsFlyerTool.purchaseEvent);
	}

	private static Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();

	private static bool mEnabled;
}
