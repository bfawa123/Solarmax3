using System;
using Solarmax;
using UnityEngine;

public class GameLoginBehaviour : MonoBehaviour
{
	private void Awake()
	{
		GameLoginBehaviour.m_LoginCallBack = LoginSDKCallback.getInstance();
	}

	private void Start()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Debug("GameLoginBehaviour start", new object[0]);
	}

	public void onInitResult(string strResult)
	{
		Debug.Log("onInitResult,code: " + strResult);
		GameLoginBehaviour.m_LoginCallBack.onInitResult(strResult);
	}

	public void onLoginResult(string loginResult)
	{
		Debug.Log("onLoginResult,loginResult: " + loginResult);
		GameLoginBehaviour.m_LoginCallBack.onLoginResult(loginResult);
	}

	public void onSwitchAccount(string switchResult)
	{
		Debug.Log("onSwitchAccount,switchResult: " + switchResult);
		GameLoginBehaviour.m_LoginCallBack.onSwitchAccount(switchResult);
	}

	public void OnGetProductInfo(string json)
	{
		Debug.LogFormat("OnGetProductInfo, json: {0}", new object[]
		{
			json
		});
		GameLoginBehaviour.m_LoginCallBack.OnGetProductInfo(json);
	}

	public void onSinglePayResult(string resuldCode)
	{
		Debug.Log("buy,resuldCode: " + resuldCode);
		GameLoginBehaviour.m_LoginCallBack.onSinglePayResult(resuldCode);
	}

	public void onPayResult(string resuldCode)
	{
		Debug.Log("onPayResult, resuldCode: " + resuldCode);
		GameLoginBehaviour.m_LoginCallBack.onPayResult(resuldCode);
	}

	public void onExitResult(string resuldCode)
	{
		this.mPressTimes++;
		if (this.mPressTimes == 1)
		{
			Tips.Make(LanguageDataProvider.GetValue(2160));
			base.Invoke("ResetMPressTimes", 1f);
		}
		if (this.mPressTimes == 2)
		{
			Solarmax.Singleton<LocalStorageSystem>.Instance.NeedSaveToDisk();
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveStorage();
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("LogOut");
			MonoSingleton<FlurryAnalytis>.Instance.EndFlurryAnalytis();
			Application.Quit();
		}
	}

	public void onPrivacyResult(string privacyResult)
	{
		Debug.LogFormat("Login -》 onPrivacyResult: {0}", new object[]
		{
			privacyResult
		});
		GameLoginBehaviour.m_LoginCallBack.onPrivacyResult(privacyResult);
	}

	private void ResetMPressTimes()
	{
		this.mPressTimes = 0;
	}

	public void onVideoComplete(string resuldCode)
	{
		Debug.Log("AdsSDK, resuldCode: " + resuldCode);
		GameLoginBehaviour.m_LoginCallBack.onVideoComplete(resuldCode);
	}

	private static LoginSDKCallback m_LoginCallBack;

	private int mPressTimes;
}
