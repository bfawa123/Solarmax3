using System;
using System.Collections;
using MiGameSDK;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LogoWindow : BaseWindow
{
	private void StartLoginTimeout()
	{
		this.StartLogin();
	}

	private bool IsCanShowPrivacy()
	{
		string lastLoginAccountId = Solarmax.Singleton<LocalStorageSystem>.Instance.GetLastLoginAccountId();
		return string.IsNullOrEmpty(lastLoginAccountId) && UpgradeUtil.GetGameConfig().Oversea && this.protolPanel.activeSelf && !this.protocolOn.activeSelf && false;
	}

	private void Awake()
	{
		this.progressChangeTimes = 100;
		this.progressChangeValue = 1f / (float)this.progressChangeTimes;
		this.progressValue = 0f;
		string value = LanguageDataProvider.GetValue(2180);
		this.progressLabel.text = value;
		this.progressLabel2.text = string.Format("{0}%", Mathf.RoundToInt(this.progressValue * 100f));
		this.protocolBG.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnPrivactyAuthorization);
	}

	private void Start()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LogoWindow Start", new object[0]);
		this.lblVersion.text = string.Format("Version: {0}", UpgradeUtil.GetAppVersion());
		this.StatrButton.SetActive(false);
		this.foreSprite.fillAmount = 0f;
		this.startLoginFired = false;
		
		if (!Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver)
		{
			base.InvokeRepeating("UpdateProgress", this.progressChangeValue, this.progressChangeValue);
			base.Invoke("DelayStart", 0.01f);
			base.Invoke("StartLoginTimeout", 3f);
		}
		else
		{
			base.InvokeRepeating("UpdateProgress", this.progressChangeValue, this.progressChangeValue);
			this.StartLogin();
		}
		this.logoutBtn.SetActive(false);
		this.protolPanel.SetActive(false);
		if (this.IsCanShowPrivacy())
		{
			this.protolPanel.SetActive(true);
			this.protocolOn.SetActive(false);
			return;
		}
		this.protolPanel.SetActive(false);
	}

	private void DelayStart()
	{
		NoticeRequest.GetNotice(delegate
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnHttpNotice, new object[0]);
		});
	}

	private void StartPCMode()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LogoWindow StartPCMode", new object[0]);
		this.SetProgress(1f);
		this.lblVersion.gameObject.SetActive(false);
		this.startLoginFired = true;
		this.StatrButton.SetActive(false);
		this.logoutBtn.SetActive(false);
		this.protolPanel.SetActive(false);
		this.enterHomeWindow = true;
		this.StartSingleGame();
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.RequestUserResult);
		base.RegisterEvent(EventId.CreateUserResult);
		base.RegisterEvent(EventId.OnHttpNotice);
		base.RegisterEvent(EventId.OnSDKLoginResult);
		base.RegisterEvent(EventId.OnGetPrivactyResult);
		base.RegisterEvent(EventId.UpdateChaptersView);
		base.RegisterEvent(EventId.OnSyncuserAndChaptersResult);
		base.RegisterEvent(EventId.OnUploadOldVersionData);
		base.RegisterEvent(EventId.OnStartSingleBattle);
		base.RegisterEvent(EventId.OnSelectServerList);
		base.RegisterEvent(EventId.OnHttpCreateAccountResponse);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Logo Window -- on show", new object[0]);
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.serverPanel.SetActive(false);
		this.selectPanel.SetActive(false);
		this.curSlider.value = 0f;
		this.enterHomeWindow = false;
		this.SetProgress(0f);
		this.logoutBtn.SetActive(false);
		Solarmax.Singleton<AssetManager>.Get().LoadShipAudio();
		Solarmax.Singleton<LocalStorageSystem>.Get().LoadStorage();
        //this.OnVisitorLoginClick();
        MonoSingleton<FlurryAnalytis>.Instance.LogEvent("LoginWindowShow");
	}

	public override void OnHide()
	{
		if (this.enterHomeWindow)
		{
			BGManager.Inst.UpdateAirShipFly(1f);
		}
		else
		{
			BGManager.Inst.SetAirShipVisible(false);
		}
	}

	private void afterCreateAccount()
	{
        Solarmax.Singleton<ThirdPartySystem>.Instance.OnSDKLogin(
            CreateAccountRequest.Account.AccountName,
            CreateAccountRequest.Account.Token
        );
    }

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler", new object[0]);
		if (eventId == EventId.OnHttpCreateAccountResponse)
		{
			string content = string.Format("服务器随机的账号为：{0}\ntoken为：{1}\n当前UUID为：{2}", CreateAccountRequest.Account.AccountName, CreateAccountRequest.Account.Token, Solarmax.Singleton<EngineSystem>.Instance.GetUUID());
            Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
            {
                1,
                content,
                new EventDelegate(this.afterCreateAccount)
            });
        }
		if (eventId == EventId.RequestUserResult)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- RequestUserResult", new object[0]);
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<AchievementModel>.Get().Init(true);
                this.StartSingleGame();
			}
			else if (errCode == ErrCode.EC_NoExist)
			{
				this.CreateDefaultUser();
			}
			else if (errCode == ErrCode.EC_NeedResume)
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
				{
					2,
					LanguageDataProvider.GetValue(20),
					new EventDelegate(new EventDelegate.Callback(this.ReconnectResume)),
					new EventDelegate(new EventDelegate.Callback(this.ReconnectGiveup))
				});
				Solarmax.Singleton<NetSystem>.Instance.helper.RequestUserInit();
				Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
				Solarmax.Singleton<AchievementModel>.Get().Init(true);
				Solarmax.Singleton<TaskModel>.Get().Init();
			}
			else if (errCode == ErrCode.EC_NeedUpdate)
			{
				this.isLoginGameServer = false;
				UpgradeUtil.ShowRestartAppWindow(2170);
			}
			else if (errCode == ErrCode.EC_NeedUpload)
			{
				Solarmax.Singleton<AchievementModel>.Get().Init(false);
				Solarmax.Singleton<TaskModel>.Get().Init();
				Solarmax.Singleton<OldLocalStorageSystem>.Instance.LoadUploadStorage();
				Solarmax.Singleton<NetSystem>.Instance.helper.UploadOldData();
			}
			else if (errCode == ErrCode.EC_AAOvertime)
			{
				Solarmax.Singleton<LocalPlayer>.Get().ShowFangChenMIInfo(LocalPlayer.FCM_TIP.Warring, true);
			}
			this.SetProgress(1f);
			return;
		}
		if (eventId == EventId.CreateUserResult)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- CreateUserResult", new object[0]);
			ErrCode errCode2 = (ErrCode)args[0];
			if (errCode2 == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<TaskModel>.Get().Init();
				Solarmax.Singleton<AchievementModel>.Get().Init(true);
				this.StartGuideBattleGame();
			}
			else if (errCode2 == ErrCode.EC_NameExist)
			{
				Tips.Make(LanguageDataProvider.GetValue(4));
			}
			else if (errCode2 == ErrCode.EC_InvalidName)
			{
				Tips.Make(LanguageDataProvider.GetValue(5));
			}
			else if (errCode2 == ErrCode.EC_AccountExist)
			{
				Tips.Make(LanguageDataProvider.GetValue(6));
			}
			else
			{
				Tips.Make(LanguageDataProvider.GetValue(7));
			}
			this.SetProgress(1f);
			return;
		}
		if (eventId == EventId.OnHttpNotice)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnHttpNotice", new object[0]);
			if (NoticeRequest.Notice == null)
			{
				this.StartLogin();
				return;
			}
			NoticeConfig notice = NoticeRequest.Notice;
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonNoticeWindow");
			MonoSingleton<FlurryAnalytis>.Instance.FlurryNoticeEffect();
			MiGameAnalytics.MiAnalyticsNoticeEffect();
			if (notice.Type == 0)
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
				{
					1,
					notice.Content,
					new EventDelegate(new EventDelegate.Callback(this.StartLogin))
				});
				return;
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				4,
				notice.Content,
				new EventDelegate(new EventDelegate.Callback(this.StartLogin))
			});
			return;
		}
		else if (eventId == EventId.OnSDKLoginResult)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnSDKLoginResult", new object[0]);
			if ((int)args[0] != 1)
			{
				this.StatrButton.SetActive(true);
				this.isLogining = false;
				this.IsCanLogin = false;
				this.serverPanel.SetActive(false);
				this.StartLable.alpha = 1f;
				this.PlayAnimationLoop("LogoWindow_in");
				return;
			}
			if (this.IsCanShowPrivacy())
			{
				MiGameLoginSDK.OpenPrivacty();
				return;
			}
			this.StatrButton.SetActive(true);
			this.IsCanLogin = true;
			this.isLogining = false;
			this.HandleSdkLoginCallBack();
			this.serverPanel.SetActive(true);
			this.visiterLoginPanel.SetActive(false);
			return;
		}
		else if (eventId == EventId.OnGetPrivactyResult)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnGetPrivactyResult", new object[0]);
			Debug.Log("Login -》 OnGetPrivactyResult ");
			if ((int)args[0] == 1)
			{
				this.IsCanLogin = true;
				this.isLogining = false;
				this.protolPanel.SetActive(true);
				this.protocolOn.SetActive(true);
				this.StatrButton.SetActive(true);
				this.HandleSdkLoginCallBack();
				this.serverPanel.SetActive(true);
				this.visiterLoginPanel.SetActive(false);
				return;
			}
			this.IsCanLogin = false;
			this.isLogining = false;
			this.StatrButton.SetActive(true);
			this.serverPanel.SetActive(false);
			this.StartLable.alpha = 1f;
			this.PlayAnimationLoop("LogoWindow_in");
			return;
		}
		else
		{
			if (eventId == EventId.UpdateChaptersView)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- UpdateChaptersView", new object[0]);
				this.SetProgress(1f);
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				MonoSingleton<FlurryAnalytis>.Instance.LogEvent("EnterHomeWindow");
				this.enterHomeWindow = true;
				BGManager.Inst.AirShipFly(1, 0.9f, new ShowWindowParams("HomeWindow", EventId.UpdateChaptersWindow, new object[]
				{
					1
				}));
				return;
			}
			if (eventId == EventId.OnUploadOldVersionData)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnUploadOldVersionData", new object[0]);
				Solarmax.Singleton<AchievementModel>.Get().Init(true);
				this.StartSingleGame();
				return;
			}
			if (eventId == EventId.OnStartSingleBattle)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnStartSingleBattle", new object[0]);
				this.OnStartSingleBattle();
				return;
			}
			if (eventId == EventId.OnSelectServerList)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("OnUIEventHandler -- OnSelectServerList", new object[0]);
				this.selectedServer = (ServerListItemConfig)args[0];
				this.serverName.text = this.selectedServer.Name;
			}
			return;
		}
	}

	private void StartLogin()
	{
		if (this.startLoginFired)
		{
			return;
		}
		this.startLoginFired = true;
		base.CancelInvoke("StartLoginTimeout");
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("ShowLoginButton");
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Start Login", new object[0]);
		this.StatrButton.SetActive(true);
		this.PlayAnimationLoop("LogoWindow_in");
        //this.OnLoginClick();
    }

    public void ReconnectResume()
	{
		BGManager.Inst.AirShipFly(1, 0f, ShowWindowParams.None);
		global::Coroutine.DelayDo(1.5f, new EventDelegate(delegate()
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.ReconnectResume(-2);
			BGManager.Inst.SetAirShipVisible(false);
		}));
	}

	public void ReconnectGiveup()
	{
		int userId = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId;
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(userId);
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitBattle(-1);
		Solarmax.Singleton<AchievementModel>.Get().Init(true);
		this.StartSingleGame();
	}

	public void OnLoginClick()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("On Login Click", new object[0]);
		//if (this.isLoginGameServer)
		//{
		//	Solarmax.Singleton<LoggerSystem>.Instance.Info("正在登陆游戏服务器", new object[0]);
		//	return;
		//}
		//this.isLoginGameServer = true;
		//this.StatrButton.SetActive(false);
		//GatewayRequest.GetGateway(this.selectedServer, new Action(this.LoginGate));

        if (this.IsCanLogin)
        {
            if (this.isLoginGameServer)
            {
                Solarmax.Singleton<LoggerSystem>.Instance.Info("正在登陆游戏服务器", new object[0]);
                return;
            }
            if (this.selectedServer == null)
            {
                Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2134), 1f);
                return;
            }
            this.isLoginGameServer = true;
            this.StatrButton.SetActive(false);
            GatewayRequest.GetGateway(this.selectedServer, new Action(this.LoginGate));
            return;
        }
        else
        {
            if (this.isLogining)
            {
                return;
            }
            this.isLogining = true;
            Debug.Log("call MiGameSDK OnLoginClick...66");
            //MiGameLoginSDK.loginAccount();
			// 使用请求代替 migame 的登录，根据 uuid 返回 account_name 和 token，起到相同的效果
			// 可以在服务端做映射，可以让不同uuid获得相同的账号，但是目前只能开设后门实现，正常还是仅支持一个设备一个账号
            CreateAccountRequest.CreateAccount(delegate
            {
                Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnHttpCreateAccountResponse);
            });
            return;
        }
    }

	public void OnVisitorLoginClick()
	{
		if (this.isLogining)
		{
			return;
		}
		this.isLogining = true;
		string text = LoadResManager.LoadCustomTxt("data/notice.txt");
		if (!string.IsNullOrEmpty(text))
		{
			LocalPlayer.LocalNotice = text;
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonNoticeWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				1,
				text,
				new EventDelegate(new EventDelegate.Callback(this.StartLogin))
			});
			return;
		}
		this.StartLogin();
	}

	private void SetProgress(float progress)
	{
		this.targetProgress = progress;
	}

	private void UpdateProgress()
	{
		if (this.progressValue >= this.targetProgress)
		{
			return;
		}
		this.progressValue += this.progressChangeValue;
		this.progressValue = Mathf.Clamp01(this.progressValue);
		this.foreSprite.fillAmount = this.progressValue;
		string value = LanguageDataProvider.GetValue(2180);
		this.progressLabel.text = value;
		this.progressLabel2.text = string.Format("{0}%", Mathf.RoundToInt(this.progressValue * 100f));
		if (this.curSlider != null)
		{
			this.curSlider.value = this.progressValue;
		}
		BGManager.Inst.UpdateAirShipFly(this.progressValue);
	}

	private void LoginGate()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Login Gate.", new object[0]);
        if (GatewayRequest.Response == null)
        {
            return;
        }
        if (string.IsNullOrEmpty(GatewayRequest.Response.Host))
        {
            string text = string.Format("EmptyHost - {0}:{1}", this.selectedServer.Name, this.selectedServer.Url);
            Debug.LogErrorFormat("GetGatewayFailed: {0}", new object[]
            {
                text
            });
            MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetGatewayFailed", "info", text);
            return;
        }
        if (Solarmax.Singleton<LocalSettingStorage>.Get().serverUrl != this.selectedServer.Url)
        {
            Solarmax.Singleton<LocalSettingStorage>.Get().serverUrl = this.selectedServer.Url;
            Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
        }
        this.serverPanel.SetActive(false);
		this.progressGo.SetActive(true);
		Solarmax.Singleton<ThirdPartySystem>.Instance.Login();
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetGatewaySuccess", "host", GatewayRequest.Response.Host);
        global::Coroutine.Start(this.ConnectGate());
	}

	private IEnumerator ConnectGate()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Connect Gate.", new object[0]);
		this.SetProgress(0.2f);
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Connect Gate --- Progress 20%", new object[0]);
		yield return new WaitForSeconds(0.2f);
		UILabel uilabel = this.loadingTip;
		uilabel.text += LanguageDataProvider.GetValue(310);
		Solarmax.Singleton<AssetManager>.Get().LoadBattleResources();
		this.SetProgress(0.6f);
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Connect Gate --- Progress 60%", new object[0]);
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("LoadBattleResources");
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.shipManager.PreloadEntity();
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("PreloadEntity");
		yield return new WaitForSeconds(0.4f);
		yield return Solarmax.Singleton<NetSystem>.Instance.helper.ConnectServer(true);
        if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            MonoSingleton<FlurryAnalytis>.Instance.LogEvent("ConnectGateSuccess", "host", GatewayRequest.Response.Host);
            this.SetProgress(0.8f);
            yield return new WaitForSeconds(0.2f);
            UILabel uilabel2 = this.loadingTip;
            uilabel2.text += LanguageDataProvider.GetValue(311);
            yield return new WaitForSeconds(0.2f);
            this.SetProgress(1f);
            yield return new WaitForSeconds(0.3f);
            this.CheckUser();
        }
        else
        {
            MonoSingleton<FlurryAnalytis>.Instance.LogEvent("ConnectGateFailed", "host", GatewayRequest.Response.Host);
        }
  //      this.SetProgress(0.8f);
		//Solarmax.Singleton<LoggerSystem>.Instance.Info("Connect Gate --- Progress 80%", new object[0]);
		//yield return new WaitForSeconds(0.2f);
		//UILabel uilabel2 = this.loadingTip;
		//uilabel2.text += LanguageDataProvider.GetValue(311);
		//yield return new WaitForSeconds(0.2f);
		//this.SetProgress(1f);
		//Solarmax.Singleton<LoggerSystem>.Instance.Info("Connect Gate --- Progress 100%", new object[0]);
		//yield return new WaitForSeconds(0.3f);
		//this.CheckUser();
		yield break;
	}

	private void CheckUser()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestUser();
	}

	private void CreateDefaultUser()
	{
		int index = 0;
		string icon = SelectIconWindow.GetIcon(index);
		Solarmax.Singleton<NetSystem>.Instance.helper.CreateUser(string.Empty, icon);
	}

	public void StartSingleGame()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LogoWindow  StartSingleGame ", new object[0]);
		Solarmax.Singleton<RankModel>.Get().Init();
		Solarmax.Singleton<TaskModel>.Get().Init();
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestUserInit();
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestChapters();
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0.5f;
		tweenAlpha.duration = 0.5f;
	}

	private void RequestAccountAndChapters()
	{
		string text = Solarmax.Singleton<LocalAccountStorage>.Get().account + ".txt";
		string file = MonoSingleton<UpdateSystem>.Instance.saveRoot + text;
		Solarmax.Singleton<NetSystem>.Instance.helper.GenPresignedUrl(text, "GET", "text/plain", file, 117);
	}

	public void HandleSdkLoginCallBack()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("HandleSdkLoginCallBack", new object[0]);
		string account = Solarmax.Singleton<LocalAccountStorage>.Get().account;
        Solarmax.Singleton<LoggerSystem>.Instance.Info("HandleSdkLoginCallBack account: {0}", account);
        if (string.IsNullOrEmpty(account))
		{
            return;
		}
		Solarmax.Singleton<LocalStorageSystem>.Instance.SetLastLoginAccountId(account, MiPlatformSDK.IsVisitor);
		Solarmax.Singleton<LocalStorageSystem>.Instance.LoadAccountRelated(account);
		Solarmax.Singleton<LocalAccountStorage>.Get().account = account;
		Solarmax.Singleton<LanguageDataProvider>.Get().Load();
		BGManager.Inst.ApplySkinConfig(null, false);
		this.StartLable.text = LanguageDataProvider.GetValue(2135);
		Solarmax.Singleton<LevelDataHandler>.Get().AfterAccountLogin();
		ServerListRequest.GetServerList(new Action(this.DisplayServerList));
	}

	private void DisplayServerList()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("DisplayServerList", new object[0]);
        if (ServerListRequest.Response == null)
		{
            return;
		}
        if (ServerListRequest.Response.Servers == null || ServerListRequest.Response.Servers.Length < 1)
		{
            Debug.LogError("GetServerListFailed: EmtpyServerList");
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetServerListFailed", "info", "EmptyServerList");
			return;
		}
		this.grid.gameObject.transform.DestroyChildren();
		this.serverName.text = LanguageDataProvider.GetValue(2134);
		ServerListItemConfig[] servers = ServerListRequest.Response.Servers;
		string serverUrl = Solarmax.Singleton<LocalSettingStorage>.Get().serverUrl;
		int num = -1;
		for (int i = 0; i < servers.Length; i++)
		{
			ServerListItemConfig serverListItemConfig = servers[i];
			GameObject gameObject = this.grid.gameObject.AddChild(this.templete);
			gameObject.name = "serverItem" + i;
			gameObject.SetActive(true);
			SelectServerCell component = gameObject.GetComponent<SelectServerCell>();
			if (component != null)
			{
				component.SetInfoLocal(servers[i], this.selectPanel);
			}
			if (serverUrl == serverListItemConfig.Url)
			{
				num = i;
			}
        }
        this.grid.Reposition();
		this.scrollview.ResetPosition();
		if (num < 0 && servers.Length == 1)
		{
			num = 0;
		}
		if (num >= 0)
		{
			this.selectedServer = servers[num];
			this.serverName.text = this.selectedServer.Name;
		}
		BGManager.Inst.AirShipFly(0);
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("GetServerListSuccess", "server", string.Format("{0}:{1}", servers[0].Name, servers[0].Url));
	}

	private void PlayAnimationLoop(string strAni)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = false;
		this.aniPlayer.Play(true, false, 1f, true);
	}

	private void PlayAnimation(string strAni)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = false;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public void OnServerPanelClick()
	{
        if (ServerListRequest.Response == null)
        {
            return;
        }
        this.selectPanel.SetActive(true);
	}

	public void OnCloseServerPanelClick()
	{
		this.selectPanel.SetActive(false);
	}

	private void StartGuideBattleGame()
	{
		Solarmax.Singleton<LevelDataHandler>.Get().SetSelectChapter("1001");
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter == null)
		{
			return;
		}
		Solarmax.Singleton<LevelDataHandler>.Get().SetSelectLevel("100101", 0);
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentLevel == null)
		{
			return;
		}
		this.lblVersion.gameObject.SetActive(false);
		BGManager.Inst.SetAirShipVisible(false);
		string id = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.id;
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = false;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.difficultyLevel = this.difficultyLevel;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = 1;
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(id);
		if (data != null)
		{
			if (this.difficultyLevel == 1)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.easyAI;
			}
			else if (this.difficultyLevel == 2)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.generalAI;
			}
			else if (this.difficultyLevel == 3)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.hardAI;
			}
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiParam = data.aiParam;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.dyncDiffType = data.dyncDiffType;
		}
		Solarmax.Singleton<BattleSystem>.Instance.canOperation = false;
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(id, GameType.GuildeLevel, true);
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("RequestGuideBattle");
	}

	public void OnStartSingleBattle()
	{
		GuideManager.TriggerGuidecompleted(GuildEndEvent.startbattle);
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 1f;
		tweenAlpha.SetOnFinished(delegate()
		{
			this.StartSingleBattle();
		});
		tweenAlpha.Play(true);
	}

	public void StartSingleBattle()
	{
		base.gameObject.SetActive(true);
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.canOperation = true;
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
			GuideManager.StartGuide(GuildCondition.GC_Level, Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId, null);
		}));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleWindow_off");
		RapidBlurEffect behavior = Camera.main.GetComponent<RapidBlurEffect>();
		if (behavior != null)
		{
			behavior.enabled = true;
			behavior.MainBgScale(false, 5.5f, 0.035f);
			global::Coroutine.DelayDo(0.55f, new EventDelegate(delegate()
			{
				behavior.enabled = false;
			}));
		}
		MonoSingleton<FlurryAnalytis>.Instance.LogEvent("StartGuideBattle");
	}

	public void OnPrivactyAuthorization(GameObject go)
	{
		if (this.protocolOn.activeSelf)
		{
			this.protocolOn.SetActive(false);
		}
		else
		{
			this.protocolOn.SetActive(true);
		}
	}

	public void OnLogoutAccount()
	{
		MiGameLoginSDK.OpenUserCenter();
		this.StatrButton.SetActive(true);
		this.IsCanLogin = true;
		this.isLogining = false;
		this.serverPanel.SetActive(true);
		this.visiterLoginPanel.SetActive(false);
		Solarmax.Singleton<LevelDataHandler>.Instance.Reset();
	}

	public GameObject progressGo;

	public UILabel progressLabel;

	public UILabel progressLabel2;

	public UILabel loadingTip;

	public UISprite foreSprite;

	public GameObject StatrButton;

	public UILabel StartLable;

	public UIPlayAnimation aniPlayer;

	public UISlider curSlider;

	private int progressChangeTimes;

	private float progressChangeValue;

	private float progressValue;

	private float targetProgress;

	private bool isLogining;

	private bool IsCanLogin;

	public GameObject serverPanel;

	public GameObject templete;

	public GameObject selectPanel;

	public UIScrollView scrollview;

	public UIGrid grid;

	public UILabel serverName;

	public GameObject visiterLoginPanel;

	private bool enterHomeWindow;

	public UILabel lblVersion;

	public GameObject protolPanel;

	public GameObject protocolBG;

	public GameObject protocolOn;

	public GameObject logoutBtn;

	private bool startLoginFired;

	private ServerListItemConfig selectedServer;

	private bool isLoginGameServer;

	private int difficultyLevel = 1;
}
