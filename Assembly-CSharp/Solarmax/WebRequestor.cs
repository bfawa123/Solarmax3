using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Solarmax
{
	public class WebRequestor<T> where T : class, IWebResponse
	{
		public WebRequestor(string tag, string[] urls, string subPath, string param, bool encrypt, Action responseCallback)
		{
			this.mTag = tag;
			this.mUrls = urls;
			this.mSubPath = subPath;
			this.mParam = param;
			this.mEncrypt = encrypt;
			this.mOnResponse = responseCallback;
		}

		public T Response { get; private set; }

		private void OnResponseCallback()
		{
            if (this.Response != null)
            {
                if (this.mOnResponse != null)
                {
                    this.mOnResponse();
                }
                return;
            }
            if (this.mRetryTimes == -1 || this.mCurRetryTimes < this.mRetryTimes)
            {
                this.mCurRetryTimes++;
                UpgradeUtil.CheckNetwork(new EventDelegate.Callback(this.StartDelayRequestCoroutine), 0L);
                return;
            }
            if (this.mOnResponse != null)
			{
				this.mOnResponse();
			}
		}

		private void StartRequestCoroutine()
		{
			global::Coroutine.Start(this.Request());
		}

		private void StartDelayRequestCoroutine()
		{
			global::Coroutine.Start(this.DelayRequest(3f));
		}

		public void StartRequest(int retryTimes)
		{
			this.mRetryTimes = retryTimes;
			this.mCurRetryTimes = 0;
			UpgradeUtil.CheckNetwork(new EventDelegate.Callback(this.StartRequestCoroutine), 0L);
		}

		private IEnumerator DelayRequest(float delay)
		{
			yield return new WaitForSecondsRealtime(delay);
			yield return this.Request();
			yield break;
		}

		public IEnumerator Request()
		{
			this.Response = default(T);
            if (this.mUrls == null)
            {
                this.LogFailed(string.Empty, "null urls");
                this.OnResponseCallback();
                yield break;
            }
            for (int i = 0; i < this.mUrls.Length; i++)
            {
                string url = this.mUrls[i];
                if (!string.IsNullOrEmpty(this.mSubPath))
                {
                    url = string.Format("{0}/{1}", url, this.mSubPath);
                }
                if (!string.IsNullOrEmpty(this.mParam))
                {
                    if (this.mEncrypt)
                    {
                        Debug.LogFormat("Request url {0} param before encrypt: {1}", new object[]
                        {
                            url,
                            this.mParam
                        });
                        string text = this.mParam;
                        text = UnityWebRequest.EscapeURL(text);
                        url = string.Format("{0}?p={1}", url.Trim(), text);
                    }
                    else
                    {
                        url = string.Format("{0}?{1}", url.Trim(), this.mParam);
                    }
                }
                float startTime = Time.realtimeSinceStartup;
                if (this.mCurRetryTimes > 0)
                {
                    Debug.LogFormat("Request url {0} retry {1}/{2}...", new object[]
                    {
                        url,
                        this.mCurRetryTimes,
                        this.mRetryTimes
                    });
                }
                else
                {
                    Debug.LogFormat("Request url {0} start...", new object[]
                    {
                        url
                    });
                }
                yield return this.Request(url);
                if (this.Response != null)
                {
                    Debug.LogFormat("Request url {0} complete. use {1} sec. response: {2}", new object[]
                    {
                        url,
                        Time.realtimeSinceStartup - startTime,
                        JsonUtility.ToJson(this.Response)
                    });
                    this.OnResponseCallback();
                    yield break;
                }
                Debug.LogFormat("Request url {0} complete. use {1} sec. response: null", new object[]
                {
                    url,
                    Time.realtimeSinceStartup - startTime
                });
            }
            this.OnResponseCallback();
			yield break;
		}

		private void LogFailed(string url, string errorInfo)
		{
			string eventName = string.Format("{0}Failed", this.mTag);
			string text = string.Format("{0} - {1}", errorInfo, Application.internetReachability);
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent(eventName, "url", url, "info", text);
			Debug.LogErrorFormat("Request url {0} error - {1}!", new object[]
			{
				url,
				text
			});
		}

		private void LogStart(string url, int curRetryTimes)
		{
			string eventName = string.Format("{0}Start", this.mTag);
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent(eventName, "url", url, "retry", curRetryTimes.ToString());
		}

		private IEnumerator Request(string url)
		{
			//this.Request();
            this.LogStart(url, this.mCurRetryTimes);
            using (UnityWebRequest rq = UnityWebRequest.Get(url))
            {
                yield return rq.SendWebRequest();
                if (!string.IsNullOrEmpty(rq.error))
                {
                    string errorInfo = string.Format("code: {0}, isNetworkError: {1}, isHttpError: {2}, error_info: {3}", new object[]
                    {
                        rq.responseCode,
                        rq.isNetworkError,
                        rq.isHttpError,
                        rq.error
                    });
                    this.LogFailed(url, errorInfo);
                    yield break;
                }
                try
                {
                    string text = rq.downloadHandler.text;
                    if (this.mEncrypt)
                    {
                        Debug.LogFormat("Request url response before decrypt: {0}", new object[]
                        {
                            text
                        });
                    }
                    this.Response = JsonUtility.FromJson<T>(text);
                    T response = this.Response;
                    if (response.HasError)
                    {
                        string errorInfo2 = string.Format("response_error: {0}", text);
                        this.LogFailed(url, errorInfo2);
                        this.mRetryTimes = 0;
                        this.Response = (T)((object)null);
                    }
                }
                catch (Exception ex)
                {
                    string errorInfo3 = string.Format("parse {0} from {1} failed! exception: {2}", typeof(T).FullName, rq.downloadHandler.text, ex.ToString());
                    this.LogFailed(url, errorInfo3);
                    this.Response = (T)((object)null);
                    this.mRetryTimes = 0;
                }
            }
            yield break;
		}

		private readonly Action mOnResponse;

		private string[] mUrls;

		private readonly string mParam;

		private readonly string mSubPath;

		private readonly bool mEncrypt;

		private int mRetryTimes;

		private int mCurRetryTimes;

		private readonly string mTag = string.Empty;
	}
}
