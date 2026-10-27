using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Solarmax
{
	public static class NoticeRequest
	{
		public static NoticeConfig Notice { get; private set; }

		private static void OnResponse()
		{
			if (NoticeRequest.mRequestor == null || NoticeRequest.mRequestor.Response == null)
			{
				if (NoticeRequest.mOnResponseDelegate != null)
				{
					NoticeRequest.mOnResponseDelegate();
					NoticeRequest.mOnResponseDelegate = null;
				}
				return;
			}
			string text = NoticeRequest.mRequestor.Response.Notice.Replace("\r", string.Empty).Replace("\n", "\\n");
			try
			{
				NoticeRequest.Notice = JsonUtility.FromJson<NoticeConfig>(text);
			}
			catch (Exception ex)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetNoticeFailed", "notice", text);
				Debug.LogErrorFormat("Parse NoticeConfig from {0} failed!\n{1}", new object[]
				{
					text,
					ex.ToString()
				});
				return;
			}
			finally
			{
				if (NoticeRequest.mOnResponseDelegate != null)
				{
					NoticeRequest.mOnResponseDelegate();
					NoticeRequest.mOnResponseDelegate = null;
				}
			}
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetNoticeSuccess");
		}

		public static void GetNotice(Action onResponseDelegate)
		{
			NoticeRequest.Notice = null;
            //string[] gameHosts = UpgradeRequest.GetGameHosts();
            string[] gameHosts = UpgradeUtil.GetGameConfig().ServerUrls;
            if (gameHosts == null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetNoticeError", "info", "EmptyHosts");
				Debug.LogError("GetNotice: Empty hosts");
				NoticeRequest.OnResponse();
				return;
			}
			ChannelConfig channelConfig = UpgradeUtil.GetChannelConfig();
			if (channelConfig == null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetNoticeError", "info", "NullChannelConfig");
				NoticeRequest.OnResponse();
				return;
			}
			string languageNameConfig = Solarmax.Singleton<LanguageDataProvider>.Get().GetLanguageNameConfig();
			string text = string.Format("channel={0}&language={1}", UnityWebRequest.EscapeURL(channelConfig.ChannelId), UnityWebRequest.EscapeURL(languageNameConfig));
			NoticeRequest.mOnResponseDelegate = onResponseDelegate;
			string tag = "GetNotice";
			string[] urls = gameHosts;
			string subPath = "notice";
			string param = text;
			bool encrypt = false;
			if (NoticeRequest.cache0 == null)
			{
				NoticeRequest.cache0 = new Action(NoticeRequest.OnResponse);
			}
			NoticeRequest.mRequestor = new WebRequestor<NoticeResponse>(tag, urls, subPath, param, encrypt, NoticeRequest.cache0);
			NoticeRequest.mRequestor.StartRequest(1);
		}

		private static WebRequestor<NoticeResponse> mRequestor;

		private static Action mOnResponseDelegate;

		[CompilerGenerated]
		private static Action cache0;
	}
}
