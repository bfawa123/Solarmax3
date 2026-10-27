using System;

public abstract class LoginSDKCallback
{
	public static LoginSDKCallback getInstance()
	{
		if (LoginSDKCallback.instance == null)
		{
			LoginSDKCallback.instance = new MiGameSDKCallbackImpl();
		}
		return LoginSDKCallback.instance;
	}

	public abstract void onInitResult(string strResult);

	public abstract void onLoginResult(string loginResult);

	public abstract void onPrivacyResult(string privacyResult);

	public abstract void onSwitchAccount(string resuldCode);

	public abstract void OnGetProductInfo(string json);

	public abstract void onSinglePayResult(string resuldCode);

	public abstract void onPayResult(string resuldCode);

	public abstract void onExitResult(string resuldCode);

	public abstract void onVideoComplete(string resuldCode);

	private static LoginSDKCallback instance;
}
