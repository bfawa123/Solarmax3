using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Solarmax
{
    [Serializable]
    public class AccountInfo {
        public string AccountName;
        public string Token;
    }

    [Serializable]
    public class CreateAccountResponse : IWebResponse
    {
        public bool HasError
        {
            get
            {
                return this.ErrorCode != 200;
            }
        }

        public int ErrorCode;

        public AccountInfo Account;
    }
    public static class CreateAccountRequest
    {
        public static AccountInfo Account { get; private set; }
        private static void OnResponse()
        {
            if (CreateAccountRequest.mRequestor == null || CreateAccountRequest.mRequestor.Response == null)
            {
                if (CreateAccountRequest.mOnResponseDelegate != null)
                {
                    CreateAccountRequest.mOnResponseDelegate();
                    CreateAccountRequest.mOnResponseDelegate = null;
                }
                return;
            }
            CreateAccountRequest.Account = mRequestor.Response.Account;
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnHttpCreateAccountResponse);
        }

        public static void CreateAccount(Action onResponseDelegate) {
            CreateAccountRequest.Account = null;
            string[] gameHosts = UpgradeUtil.GetGameConfig().ServerUrls;
            if (gameHosts == null)
            {
                MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetNoticeError", "info", "EmptyHosts");
                Debug.LogError("GetNotice: Empty hosts");
                CreateAccountRequest.OnResponse();
                return;
            }
            string uuid = Solarmax.Singleton<EngineSystem>.Instance.GetUUID();
            string text = string.Format("uuid={0}", UnityWebRequest.EscapeURL(uuid));
            CreateAccountRequest.mOnResponseDelegate = onResponseDelegate;
            string tag = "CreateAccount";
            string[] urls = gameHosts;
            string subPath = "createAccount";
            string param = text;
            bool encrypt = false;
            if (CreateAccountRequest.cache0 == null)
            {
                CreateAccountRequest.cache0 = new Action(CreateAccountRequest.OnResponse);
            }
            CreateAccountRequest.mRequestor = new WebRequestor<CreateAccountResponse>(tag, urls, subPath, param, encrypt, CreateAccountRequest.cache0);
            CreateAccountRequest.mRequestor.StartRequest(1);
        }

        private static WebRequestor<CreateAccountResponse> mRequestor;

        private static Action mOnResponseDelegate;

        private static Action cache0;
    }
}
