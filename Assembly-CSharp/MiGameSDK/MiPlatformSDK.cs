using System;
using NetMessage;
using Solarmax;
using UnityEngine;

namespace MiGameSDK
{
	public class MiPlatformSDK
	{
		public static bool IsVisitor { get; set; }

		public static void InitSDK()
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Debug("call MiGameSDK InitSDK...", new object[0]);
			MiPlatformSDK.androidClass = new AndroidJavaClass("com.xiaomi.gamecenter.sdk.mi.LoginPlatformActivity");
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("initSDK", new object[0]);
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Debug("call MiGameSDK androidClass == null", new object[0]);
			}
		}

		public static void InitAds()
		{
			ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
			if (MiPlatformSDK.androidClass != null && !string.IsNullOrEmpty(channelConfig.AdAppId))
			{
				MiPlatformSDK.androidClass.CallStatic("initAds", new object[]
				{
					channelConfig.AdAppId,
					channelConfig.AdCodeId
				});
			}
		}

		public static void loginAccount()
		{
			MiPlatformSDK.IsVisitor = false;
			Debug.Log("call MiGameSDK loginAccount...");
			MonoSingleton<FlurryAnalytis>.Instance.FlurrySDKLoginStart(false);
			MiGameAnalytics.MiAnalyticsSDKLoginStart(false);
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("login", new object[0]);
			}
		}

		public static void switchAccount()
		{
			MiPlatformSDK.IsVisitor = false;
			Solarmax.Singleton<LoggerSystem>.Instance.Debug("call MiGameSDK switchAccount...", new object[0]);
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("login", new object[0]);
			}
		}

		public static void pay(string productID, string productName, int productNum, string currency, float price, string productDesc, string orderID, string notifyUrl)
		{
			Debug.Log("buy -> BuyProduct pay---" + orderID);
			string account = Solarmax.Singleton<LocalAccountStorage>.Get().account;
			if (MiPlatformSDK.IsVisitor)
			{
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
				EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
				EventId id = EventId.OnCommonDialog;
				object[] array = new object[3];
				array[0] = 2;
				array[1] = LanguageDataProvider.GetValue(2172);
				array[2] = new EventDelegate(delegate()
				{
					MiPlatformSDK.VisitorBind();
				});
				instance.FireEvent(id, array);
				return;
			}
			MonoSingleton<FlurryAnalytis>.Instance.LogPayStart(productName, productID, productNum, currency, (double)price, orderID, notifyUrl);
			MiGameAnalytics.MiAnalyticsLogPayStart(productName, productID, productNum, currency, (double)price, orderID, notifyUrl);
			AppsFlyerTool.FlyerPayStartEvent(productName, productID, productNum, currency, (double)price, orderID);
			if (MiPlatformSDK.androidClass != null)
			{
				if (UpgradeUtil.GetGameConfig().Oversea)
				{
					MiPlatformSDK.androidClass.CallStatic("pay", new object[]
					{
						productID,
						account,
						productNum,
						productDesc,
						orderID,
						notifyUrl
					});
				}
				else
				{
					MiPlatformSDK.androidClass.CallStatic("pay", new object[]
					{
						productID,
						account,
						productNum,
						Mathf.RoundToInt(price),
						productName,
						productDesc,
						orderID,
						notifyUrl
					});
				}
			}
		}

		public static void ShowAds()
		{
			if (UpgradeUtil.GetGameConfig().Oversea)
			{
				if (MiPlatformSDK.androidClass != null)
				{
					MiPlatformSDK.androidClass.CallStatic("ShowAds", new object[0]);
				}
			}
			else
			{
				if (Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel.ContainsKey(AdChannel.AD_PANGOLIN) && MiPlatformSDK.androidClass != null)
				{
					Debug.Log("---播放穿山甲广告----");
					MiPlatformSDK.androidClass.CallStatic("ShowAds", new object[0]);
					return;
				}
				Tips.Make(LanguageDataProvider.GetValue(2155));
			}
		}

		public static void StopAds()
		{
			if (UpgradeUtil.GetGameConfig().Oversea)
			{
				if (MiPlatformSDK.androidClass != null)
				{
					MiPlatformSDK.androidClass.CallStatic("StopAds", new object[0]);
				}
			}
			else if (Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel.ContainsKey(AdChannel.AD_PANGOLIN) && MiPlatformSDK.androidClass != null)
			{
				Debug.Log("---停止穿山甲广告----");
				MiPlatformSDK.androidClass.CallStatic("StopAds", new object[0]);
				return;
			}
		}

		private static AndroidJavaObject ToJavaArray(string[] values)
		{
			AndroidJavaObject result;
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.reflect.Array"))
			{
				AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("newInstance", new object[]
				{
					new AndroidJavaClass("java.lang.String"),
					values.Length
				});
				for (int i = 0; i < values.Length; i++)
				{
					androidJavaClass.CallStatic("set", new object[]
					{
						androidJavaObject,
						i,
						new AndroidJavaObject("java.lang.String", new object[]
						{
							values[i]
						})
					});
				}
				result = androidJavaObject;
			}
			return result;
		}

		public static bool GetProductsInfo(string[] productIds)
		{
			if (!UpgradeUtil.GetGameConfig().EnableFetchProducts)
			{
				return false;
			}
			if (MiPlatformSDK.androidClass != null)
			{
				using (AndroidJavaObject androidJavaObject = MiPlatformSDK.ToJavaArray(productIds))
				{
					MiPlatformSDK.androidClass.CallStatic("fetchProducts", new object[]
					{
						androidJavaObject
					});
				}
				return true;
			}
			return false;
		}

		public static void VisitorLogin()
		{
			if (!UpgradeUtil.GetGameConfig().EnableVisitorLogin)
			{
				return;
			}
			MiPlatformSDK.IsVisitor = false;
			Debug.Log("Call VisitorLogin");
			MonoSingleton<FlurryAnalytis>.Instance.FlurrySDKLoginStart(true);
			MiGameAnalytics.MiAnalyticsSDKLoginStart(true);
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("touristLogin", new object[0]);
			}
		}

		public static void VisitorBind()
		{
			if (!UpgradeUtil.GetGameConfig().EnableVisitorLogin)
			{
				return;
			}
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("bindTouristLogin", new object[0]);
			}
		}

		public static void SetCurrentVersionGoOnline()
		{
		}

		public static void Restart(int delay)
		{
			using (MiPlatformSDK.androidClass = new AndroidJavaClass("com.xiaomi.gamecenter.sdk.mi.LoginPlatformActivity"))
			{
				Debug.LogFormat("Call doRestart...", new object[0]);
				MiPlatformSDK.androidClass.CallStatic("doRestart", new object[]
				{
					delay
				});
			}
		}

		public static void OpenPrivactyWidnow()
		{
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("openPrivacty", new object[0]);
			}
		}

		public static void OpenPrivactyDetails()
		{
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("openPrivactyDetails", new object[0]);
			}
		}

		public static void OpenUserCenter()
		{
			if (MiPlatformSDK.androidClass != null)
			{
				MiPlatformSDK.androidClass.CallStatic("openUserCenter", new object[0]);
			}
		}

		private const string SDK_JAVA_CLASS = "com.xiaomi.gamecenter.sdk.mi.LoginPlatformActivity";

		public static AndroidJavaClass androidClass;
	}
}
