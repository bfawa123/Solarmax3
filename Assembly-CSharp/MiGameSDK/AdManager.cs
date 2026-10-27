using System;

namespace MiGameSDK
{
	public class AdManager
	{
		public static int ShowAd(AdManager.ShowAdType type, AdManager.OnSuccess callback = null)
		{
			AdManager.showAdCount++;
			if (AdManager.showAdCount == 2147483647)
			{
				AdManager.showAdCount = 0;
			}
			AdManager.currentShowType = type;
			AdManager.onAdCompleted = callback;
			MonoSingleton<FlurryAnalytis>.Instance.FlurryAdsClickEvent();
			AppsFlyerTool.FlyerAdsClickEvent();
			MiGameAnalytics.MiAnalyticsAdsClickEvent();
			MiPlatformSDK.ShowAds();
			return AdManager.showAdCount;
		}

		public static void OnSuccessCallBack(params object[] args)
		{
			if (AdManager.onAdCompleted != null)
			{
				AdManager.onAdCompleted(args);
			}
		}

		public static int showAdCount;

		public static AdManager.ShowAdType currentShowType;

		private static AdManager.OnSuccess onAdCompleted;

		public enum ShowAdType
		{
			ShopAd,
			DoubleAd,
			LotteryAd
		}

		public delegate void OnSuccess(params object[] args);
	}
}
