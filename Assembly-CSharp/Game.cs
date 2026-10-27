using System;
using System.Runtime.CompilerServices;
using System.Threading;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class Game : MonoBehaviour
{
	private void Awake()
	{
		try
		{
			Solarmax.Singleton<Framework>.Instance.SetWritableRootDir(Application.temporaryCachePath);
			LoggerSystem instance = Solarmax.Singleton<LoggerSystem>.Instance;
			if (Game.tmp0 == null)
			{
				Game.tmp0 = new Callback<string>(Debug.Log);
			}
			instance.SetConsoleLogger(new Solarmax.Logger(Game.tmp0));
			Solarmax.Singleton<LoadResManager>.Get().Init();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Awake exception {0}", new object[]
			{
				ex.ToString()
			});
		}
		this.initFinished = false;
		Game.game = this;
	}

	private void OnCheckVersionStatusResponse()
	{
		this.Init();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LogoWindow");
	}

	private void Start()
	{
		string text = null;
		try
		{
			MonoSingleton<FlurryAnalytis>.Instance.FlurryInit();
		}
		catch (Exception ex)
		{
			text = ex.ToString();
			Debug.LogErrorFormat("InitFlurry exception! {0}", new object[]
			{
				text
			});
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("InitFlurry");
		try
		{
			MonoSingleton<BuglyTools>.Instance.InitSDK();
		}
		catch (Exception ex2)
		{
			Debug.LogErrorFormat("InitBugly exception! {0}", new object[]
			{
				ex2.ToString()
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("InitBuglyException", "info", ex2.ToString());
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("InitBugly");
		if (!string.IsNullOrEmpty(text))
		{
			Debug.LogErrorFormat("InitFlurry error! {0}", new object[]
			{
				text
			});
		}
		try
		{
			UpgradeUtil.CheckPersistentResVersion(true);
		}
		catch (Exception ex3)
		{
			Debug.LogErrorFormat("CheckPersistentResVersion exception! {0}", new object[]
			{
				ex3.ToString()
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("CheckPersistentResVersionException", "info", ex3.ToString());
		}
		try
		{
			MiPlatformSDK.InitSDK();
			MiPlatformSDK.InitAds();
			AppsFlyerTool.InitSDK();
			MonoSingleton<AdsTools>.Instance.InitSDK();
		}
		catch (Exception ex4)
		{
			Debug.LogErrorFormat("InitSDK exception! {0}", new object[]
			{
				ex4.ToString()
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("InitSDKException", "info", ex4.ToString());
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("InitSDK");
		try
		{
			if (!Solarmax.Singleton<Framework>.Instance.InitLanguageAndPing())
			{
				Debug.LogError("Framework.Init_A failed!");
			}
		}
		catch (Exception ex5)
		{
			Debug.LogErrorFormat("Framework.Init_A exception! {0}", new object[]
			{
				ex5.ToString()
			});
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("Framework.Init_AException", "info", ex5.ToString());
			return;
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("Framework.Init_A");
		Solarmax.Singleton<BattleSystem>.Instance.battleData.root = this.battleRoot;
		base.gameObject.AddComponent<BGManager>();
		this.AsyncInitMsg();
		Solarmax.Singleton<AudioManger>.Get().Init();
		UISystem.DirectShowPrefab("ui/loadingwindow.prefab");
	}

	public void Init()
	{
		if (!Solarmax.Singleton<Framework>.Instance.Init())
		{
			Debug.LogError("Framework.Init_B failed!");
		}
		this.initFinished = true;
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("Framework.Init_B");
	}

	private void Update()
	{
		VoiceEngine.Poll();
	}

	private void ResetMPressTimes()
	{
		this.mPressTimes = 0;
	}

	private void AsyncInitMsg()
	{
		AsyncThread asyncThread = new AsyncThread(delegate(AsyncThread arg)
		{
			Thread.Sleep(100);
		});
		asyncThread.Start();
	}

	private void FixedUpdate()
	{
		if (this.initFinished)
		{
			Solarmax.Singleton<Framework>.Instance.Tick(Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
	}

	private void OnApplicationQuit()
	{
		VoiceEngine.Destroy();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		VoiceEngine.OnApplicationFocus(hasFocus);
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (!this.initFinished)
		{
			return;
		}
		if (!pauseStatus)
		{
			Debug.Log("Unity : enter server front!!!!");
			Solarmax.Singleton<UserSyncSysteam>.Get().StartThread();
		}
		else
		{
			Debug.Log("Unity : enter server back!!!!");
			Solarmax.Singleton<LocalStorageSystem>.Instance.NeedSaveToDisk();
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveStorage();
			Solarmax.Singleton<UserSyncSysteam>.Get().DestroyThread();
			if (pauseStatus)
			{
				this.appPauseBeginTime = Time.realtimeSinceStartup;
			}
			else
			{
				GameType gameType = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType;
				if ((gameType == GameType.PVP || gameType == GameType.League) && Time.realtimeSinceStartup - this.appPauseBeginTime >= 20f && Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
				{
					Debug.Log("在后台超过10s，主动断开连接");
					Solarmax.Singleton<NetSystem>.Instance.Close();
				}
			}
		}
	}

	public GameObject battleRoot;

	private float appPauseBeginTime;

	public bool initFinished;

	private int mPressTimes;

	public static Game game;

	[CompilerGenerated]
	private static Callback<string> tmp0;
}
