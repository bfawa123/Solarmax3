using System;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class MiGameSDKCallbackImpl : LoginSDKCallback
{
	public override void onInitResult(string strResult)
	{
	}

	public override void onLoginResult(string resuldCode)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("On Login Result", new object[0]);
		Debug.LogFormat("onLoginResult: {0}", new object[]
		{
			resuldCode
		});
		if (string.IsNullOrEmpty(resuldCode))
		{
			MonoSingleton<FlurryAnalytis>.Instance.FlurryLoginSDKFailed(string.Empty);
			MiGameAnalytics.MiAnalyticsLoginSDKFailed(string.Empty);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLoginFailed();
			return;
		}
		string[] array = resuldCode.Split(new char[]
		{
			','
		});
		if (array.Length < 2)
		{
			MonoSingleton<FlurryAnalytis>.Instance.FlurryLoginSDKFailed(resuldCode);
			MiGameAnalytics.MiAnalyticsLoginSDKFailed(resuldCode);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLoginFailed();
			return;
		}
		if (string.IsNullOrEmpty(array[0]) || string.IsNullOrEmpty(array[1]))
		{
			if (array.Length > 2)
			{
				string text = array[2];
				if (text != null && text == "1002")
				{
					Tips.Make(Tips.TipsType.Bottom, LanguageDataProvider.GetValue(2239), 1f);
				}
			}
			MonoSingleton<FlurryAnalytis>.Instance.FlurryLoginSDKFailed(resuldCode);
			MiGameAnalytics.MiAnalyticsLoginSDKFailed(resuldCode);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLoginFailed();
			return;
		}
		if (array.Length > 2)
		{
			bool flag = array[2] == "1";
			MiPlatformSDK.IsVisitor = flag;
			Debug.LogFormat("Login visitor: {0}", new object[]
			{
				flag
			});
		}
		else
		{
			MiPlatformSDK.IsVisitor = false;
			Debug.LogFormat("Login visitor: {0}", new object[]
			{
				false
			});
		}
		Solarmax.Singleton<LocalStorageSystem>.Instance.SetLastLoginAsVisitor(MiPlatformSDK.IsVisitor);
		MonoSingleton<FlurryAnalytis>.Instance.FlurryLoginSDKSuccess(array[0], MiPlatformSDK.IsVisitor);
		MiGameAnalytics.MiAnalyticsLoginSDKSuccess(array[0], MiPlatformSDK.IsVisitor);
		Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLogin(array[0], array[1]);
	}

	public override void onSwitchAccount(string resuldCode)
	{
		string[] array = resuldCode.Split(new char[]
		{
			','
		});
		if (!string.IsNullOrEmpty(array[0]))
		{
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKSwitch(array[0], array[1]);
		}
		else
		{
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLoginFailed();
		}
	}

	public override void onPrivacyResult(string privacyResult)
	{
		Debug.LogFormat("Login -》 onPrivacyResult: {0}", new object[]
		{
			privacyResult
		});
		Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKOpenPrivacty(privacyResult);
	}

	public override void OnGetProductInfo(string json)
	{
		if (string.IsNullOrEmpty(json))
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", "SDKFailed");
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
			return;
		}
		SDKProductInfoList sdkproductInfoList;
		try
		{
			sdkproductInfoList = JsonUtility.FromJson<SDKProductInfoList>(json);
		}
		catch (Exception exception)
		{
			string text = string.Format("ParseJsonFailed - {0}", json);
			Debug.LogErrorFormat("GetProductInfoFailed: {0}", new object[]
			{
				text
			});
			Debug.LogException(exception);
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", text);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
			return;
		}
		if (sdkproductInfoList.Products == null)
		{
			string text2 = string.Format("NullProducts - {0}", json);
			Debug.LogErrorFormat("GetProductInfoFailed: {0}", new object[]
			{
				text2
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", text2);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
			return;
		}
		if (sdkproductInfoList.Products.Length < 1)
		{
			string text3 = string.Format("EmptyProducts - {0}", json);
			Debug.LogErrorFormat("GetProductInfoFailed: {0}", new object[]
			{
				text3
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", text3);
			Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
			return;
		}
		for (int i = 0; i < sdkproductInfoList.Products.Length; i++)
		{
			SDKProductInfo sdkproductInfo = sdkproductInfoList.Products[i];
			if (string.IsNullOrEmpty(sdkproductInfo.currencyType))
			{
				string text4 = string.Format("EmptyCurrencyType - {0}", json);
				Debug.LogErrorFormat("GetProductInfoFailed: {0}", new object[]
				{
					text4
				});
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", text4);
				Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
				return;
			}
			if (sdkproductInfo.feeValue < 1E-06f)
			{
				string text5 = string.Format("InvalidPrice - {0}", json);
				Debug.LogErrorFormat("GetProductInfoFailed: {0}", new object[]
				{
					text5
				});
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoFailed", "info", text5);
				Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoFailed();
				return;
			}
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetProductInfoSuccess", "info", json);
		Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKGetProductInfoSuccess(sdkproductInfoList);
	}

	public override void onSinglePayResult(string resuldCode)
	{
		string[] array = resuldCode.Split(new char[]
		{
			','
		});
		if (array.Length >= 2)
		{
			Solarmax.Singleton<OrderManager>.Get().VerificationOrder();
			MonoSingleton<FlurryAnalytis>.Instance.LogPaySuccess(resuldCode);
			AppsFlyerTool.FlyerPaySuccessEvent(resuldCode);
		}
		else
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1137), 1f);
			MonoSingleton<FlurryAnalytis>.Instance.LogPayFailed(resuldCode);
		}
	}

	public override void onPayResult(string resuldCode)
	{
	}

	public override void onExitResult(string resuldCode)
	{
	}

	public override void onVideoComplete(string resuldCode)
	{
		if (string.IsNullOrEmpty(resuldCode))
		{
			return;
		}
		if (resuldCode.Equals("VideoOpened"))
		{
			string rewardbyNum = Solarmax.Singleton<CollectionModel>.Get().GetRewardbyNum(Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum);
			MonoSingleton<FlurryAnalytis>.Instance.FlurryOpenADSEvent(rewardbyNum);
			AppsFlyerTool.FlyerOpenADSEvent();
			return;
		}
		if (!resuldCode.Equals("Completed") && !resuldCode.Equals("MimengCompleted"))
		{
			return;
		}
		MonoSingleton<FlurryAnalytis>.Instance.FlurryAdsEndEvent();
		AppsFlyerTool.FlyerAdsEndEvent();
		Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Ads, 1);
		if (AdManager.currentShowType == AdManager.ShowAdType.ShopAd)
		{
			if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
			{
				Solarmax.Singleton<ReconnectHandler>.Get().StartReconnect();
				if (Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum <= Solarmax.Singleton<storeConfigProvider>.Get().MAX_SHOW_ADS_NUM)
				{
					Solarmax.Singleton<NetSystem>.Instance.helper.StartCSAdRewardCache();
				}
			}
			else if (Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum <= Solarmax.Singleton<storeConfigProvider>.Get().MAX_SHOW_ADS_NUM)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.StartCSAdReward();
			}
		}
		else if (AdManager.currentShowType == AdManager.ShowAdType.DoubleAd)
		{
			if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() != ConnectionStatus.CONNECTED)
			{
				Solarmax.Singleton<ReconnectHandler>.Get().StartReconnect();
			}
			AdManager.OnSuccessCallBack(null);
		}
		else if (AdManager.currentShowType == AdManager.ShowAdType.LotteryAd)
		{
			AdManager.OnSuccessCallBack(null);
		}
	}
}
