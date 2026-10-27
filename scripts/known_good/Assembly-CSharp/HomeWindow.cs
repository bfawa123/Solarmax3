using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class HomeWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnUpdateName);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnPullAchievementData);
		base.RegisterEvent(EventId.OnMonthCheckSuccess);
		base.RegisterEvent(EventId.OnLotteryNotesDone);
		base.RegisterEvent(EventId.OnMailListResponse);
		if (null != this.PlanetPrefab)
		{
			this.mPlanetGo = UnityEngine.Object.Instantiate<GameObject>(this.PlanetPrefab);
			this.mPlanetGo.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		}
		Solarmax.Singleton<MonthCheckModel>.Get().Init();
		return true;
	}

	public override void Release()
	{
		if (null != this.mPlanetGo)
		{
			UnityEngine.Object.Destroy(this.mPlanetGo);
		}
	}

	public override void OnShow()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("HomeWindow   OnShow", new object[0]);
		base.OnShow();
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().SetConnectStatus(ConnectionStatus.CONNECTED);
		this.mShowAirShipWhenHide = false;
		BGManager.Inst.SetAirShipVisible(true);
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = string.Empty;
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.functionOpenTips.Refresh();
		this.RefeshFunctionButtons();
		Solarmax.Singleton<NetSystem>.Get().helper.LotteryNotesFromServer();
		Solarmax.Singleton<NetSystem>.Get().helper.GetMailList(0);
		this.showReNameWindow = false;
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED && Solarmax.Singleton<LocalPlayer>.Get().playerData.name == string.Empty)
		{
			this.showReNameWindow = true;
			Solarmax.Singleton<UISystem>.Get().ShowWindow("SingleClearWindow");
		}
		string[] array = base.gameObject.name.Split(new char[]
		{
			'('
		});
		if (!string.IsNullOrEmpty(array[0]))
		{
			GuideManager.StartGuide(GuildCondition.GC_Ui, array[0], base.gameObject);
		}
		Solarmax.Singleton<LevelDataHandler>.Instance.allStars = AchievementModel.GetALLCompletedStars();
		this.RefreshPlayerInfo();
		if (null != this.mPlanetGo)
		{
			this.mPlanetGo.SetActive(true);
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.icon != string.Empty)
		{
			this.headBehavior.Load(Solarmax.Singleton<LocalPlayer>.Get().playerData.icon, null, null);
		}
		RapidBlurEffect component = Camera.main.GetComponent<RapidBlurEffect>();
		if (component != null && Camera.main.orthographicSize != 5.5f)
		{
			component.enabled = true;
			component.MainBgScale(false, 5.5f, 5.5f);
			global::Coroutine.DelayDo(0.1f, new EventDelegate(delegate()
			{
				Camera.main.GetComponent<RapidBlurEffect>().enabled = false;
			}));
		}
		if (!this.showReNameWindow)
		{
			this.NeedShowRechareMonthCardWindow();
		}
		if (UpgradeUtil.GetGameConfig().Oversea)
		{
			this.forum.SetActive(true);
			return;
		}
		this.forum.SetActive(false);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnUpdateName)
		{
			this.RefreshPlayerInfo();
			if (this.showReNameWindow)
			{
				this.NeedShowRechareMonthCardWindow();
			}
		}
		else if (eventId == EventId.OnPullAchievementData)
		{
			this.RefeshFunctionButtons();
		}
		else if (eventId == EventId.UpdateMoney)
		{
			this.lblMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
		else if (eventId == EventId.OnMonthCheckSuccess)
		{
			this.RefeshFunctionButtons();
		}
		else if (eventId == EventId.OnLotteryNotesDone)
		{
			this.RefeshFunctionButtons();
		}
		else if (eventId == EventId.OnMailListResponse)
		{
			this.UpdateMailReddot();
		}
	}

	public override void OnHide()
	{
		if (null != this.mPlanetGo)
		{
			this.mPlanetGo.SetActive(false);
		}
		GuideManager.ClearGuideData();
		if (!this.mShowAirShipWhenHide)
		{
			BGManager.Inst.SetAirShipVisible(false);
		}
	}

	private void RefeshFunctionButtons()
	{
		for (int i = 0; i < this.functionButtons.Length; i++)
		{
			FuctionButton fuctionButton = this.functionButtons[i];
			if (fuctionButton.GameObjects.Length != 0 && !(fuctionButton.GameObjects[0] == null))
			{
				if (fuctionButton.GameObjects.Length == 1)
				{
					fuctionButton.GameObjects[0].SetActive(false);
				}
				else
				{
					bool flag = Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(fuctionButton.FuctionType);
					flag = true;
					if (fuctionButton.GameObjects.Length != 0 && !(fuctionButton.GameObjects[0] == null))
					{
						fuctionButton.GameObjects[0].SetActive(true);
						UISprite component = fuctionButton.GameObjects[0].GetComponent<UISprite>();
						if (!(component == null) && fuctionButton.GameObjects.Length >= 2 && !(fuctionButton.GameObjects[1] == null))
						{
							if (flag)
							{
								component.color = new Color(1f, 1f, 1f, 1f);
								fuctionButton.GameObjects[1].SetActive(false);
								UIButton component2 = component.GetComponent<UIButton>();
								if (component2 != null)
								{
									component2.ResetDefaultColor(new Color(1f, 1f, 1f, 1f));
								}
							}
							else
							{
								fuctionButton.GameObjects[1].SetActive(true);
								component.color = new Color(1f, 1f, 1f, 0.5f);
							}
							FunctionType fuctionType = fuctionButton.FuctionType;
							switch (fuctionType)
							{
							case FunctionType.Challenge:
								fuctionButton.GameObjects[2].SetActive(this.NeedShowChallengeReddot());
								break;
							case FunctionType.MonthCheck:
								fuctionButton.GameObjects[2].SetActive(this.NeedShowMothCheckReddot());
								break;
							case FunctionType.MonthRecharge:
								fuctionButton.GameObjects[0].SetActive(UpgradeUtil.GetGameConfig().EnablePay);
								fuctionButton.GameObjects[2].SetActive(this.NeedRechargeReddot());
								break;
							case FunctionType.Bag:
								fuctionButton.GameObjects[2].SetActive(this.NeedShowBagReddot());
								break;
							case FunctionType.Lottery:
								fuctionButton.GameObjects[2].SetActive(this.NeedLotteryReddot());
								break;
							default:
								if (fuctionType == FunctionType.Task)
								{
									fuctionButton.GameObjects[2].SetActive(this.NeedShowTaskReddot());
								}
								break;
							}
						}
					}
				}
			}
		}
	}

	private void RefreshPlayerInfo()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		if (playerData == null)
		{
			return;
		}
		this.lblName.text = ((!string.IsNullOrEmpty(playerData.name)) ? playerData.name : "无名喵喵");
		this.lblMoney.text = playerData.money.ToString();
		this.lblScore.text = playerData.score.ToString();
		this.chapterStar.text = string.Format("{0}", Solarmax.Singleton<LevelDataHandler>.Instance.allStars);
	}

	private bool NeedShowTaskReddot()
	{
		return Solarmax.Singleton<TaskConfigProvider>.Get().HaveDailyTaskCompleted();
	}

	private bool NeedShowChallengeReddot()
	{
		this.challengeReddot.SetActive(false);
		foreach (ChapterInfo chapterInfo in Solarmax.Singleton<LevelDataHandler>.Get().chapterList)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in chapterInfo.levelList)
			{
				List<Achievement> challenges = AchievementModel.GetChallenges(chapterLevelGroup.groupID);
				foreach (Achievement achievement in challenges)
				{
					if (!Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(achievement.id))
					{
						Solarmax.Singleton<LoggerSystem>.Instance.Error("challenge window 任务为空: " + achievement.id, new object[0]);
					}
					else
					{
						TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Get().dataList[achievement.taskId];
						if (taskConfig.status == TaskStatus.Completed)
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	private bool NeedShowMothCheckReddot()
	{
		return Solarmax.Singleton<MonthCheckModel>.Get().needCheck;
	}

	private bool NeedRechargeReddot()
	{
		return Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive || (!Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive && !StoreWindow.hasOpened);
	}

	private void NeedShowRechareMonthCardWindow()
	{
		if (!Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive)
		{
			return;
		}
		if (!Solarmax.Singleton<MonthCheckModel>.Get().firstOpen)
		{
			return;
		}
		Solarmax.Singleton<MonthCheckModel>.Get().firstOpen = false;
		DateTime d = new DateTime(1970, 1, 1);
		long num = (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds;
		long num2 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) / 86400L;
		long num3 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) % 86400L;
		if (num3 > 0L)
		{
			num2 += 1L;
		}
		if (num2 >= 1L && num2 <= 3L)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("MonthRechargeTipsWindow");
		}
	}

	private bool NeedLotteryReddot()
	{
		return Solarmax.Singleton<LuckModel>.Get().needRedDot();
	}

	private void UpdateMailReddot()
	{
		bool active = Solarmax.Singleton<MailModel>.Get().HasUnreadMails();
		this.mailReddot.SetActive(active);
	}

	private bool NeedShowBagReddot()
	{
		return false;
	}

	public void OnBnSettingsClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnBnMailClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("MailWindow");
	}

	public void OnBnStoreClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HallWindow");
	}

	public void OnBnPvpLadderClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvpLadder))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvpLadder));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("PvPRoomWindow");
	}

	public void OnBnPvpCooperationClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvpCooperation))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvpLadder));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		EventDelegate eventDelegate = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation2));
		EventDelegate eventDelegate2 = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation3));
		EventDelegate eventDelegate3 = new EventDelegate(new EventDelegate.Callback(this.OpenCooperation4));
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			6,
			LanguageDataProvider.GetValue(7018),
			eventDelegate,
			eventDelegate2,
			eventDelegate3,
			LanguageDataProvider.GetValue(7019),
			LanguageDataProvider.GetValue(7020),
			LanguageDataProvider.GetValue(7021)
		});
	}

	private void OpenCooperationInternal(int playerNum)
	{
		Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(playerNum);
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HomeWindow";
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBackWindowName, new object[]
		{
			"HomeWindow"
		});
	}

	private void OpenCooperation2()
	{
		this.OpenCooperationInternal(2);
	}

	private void OpenCooperation3()
	{
		this.OpenCooperationInternal(3);
	}

	private void OpenCooperation4()
	{
		this.OpenCooperationInternal(4);
	}

	public void OnBnPvpDuelClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvpLadder))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvpLadder));
			return;
		}
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HomeWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1, 2, false, string.Empty, -1, string.Empty, false);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBnPvpMeleeClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvpLadder))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvpLadder));
			return;
		}
		Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HomeWindow";
		Solarmax.Singleton<NetSystem>.Instance.helper.StartMatchReq(MatchType.MT_Room, string.Empty, string.Empty, CooperationType.CT_1v1v1, 3, false, string.Empty, -1, string.Empty, false);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBnPvpRoomClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvpRoom))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvpLadder));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HallWindow");
	}

	public void OnBnPveFreeClick()
	{
		this.mShowAirShipWhenHide = true;
		GuideManager.TriggerGuidecompleted(GuildEndEvent.startbattle);
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		BGManager.Inst.AirShipFly(2, 0.9f, new ShowWindowParams("LobbyWindowView", EventId.UpdateChaptersWindow, new object[]
		{
			1
		}));
	}

	public void OnBnPvePayClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.PvePay))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.PvePay));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter = null;
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("ChapterWindow", EventId.UpdateChapterWindow, new object[]
		{
			-1
		}));
	}

	public void OnBnTaskClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.Task))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.Task));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("TaskWindow");
	}

	public void OnBnSocialClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.Social))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.Social));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendWindow");
	}

	public void OnBnRankingClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.Ranking))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.Ranking));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RankWindow");
	}

	public void OnBnPlayBackClick()
	{
	}

	public void OnBnChallengeClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("ChallengeWindow");
	}

	public void OnBnMonthCheckClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.MonthCheck))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.MonthCheck));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("MonthCheckWindow");
	}

	public void OnBnMonthRechargeClick()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.MonthRecharge))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.MonthRecharge));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("StoreWindow", EventId.OnShowMonthCardEffect, null));
		this.RefeshFunctionButtons();
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickAvatar()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionWindow");
	}

	public void OnClickForum()
	{
		if (NoticeRequest.Notice != null && !string.IsNullOrEmpty(NoticeRequest.Notice.forum))
		{
			Application.OpenURL(NoticeRequest.Notice.forum);
		}
	}

	public void OnClickBag()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BagWindow");
	}

	public void OnClickLottery()
	{
		if (!Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.CheckFunctionOpen(FunctionType.Lottery))
		{
			Tips.Make(Solarmax.Singleton<FunctionOpenConfigProvider>.Instance.GetFunctionUnlockDesc(FunctionType.Lottery));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LuckWindow");
	}

	public UILabel lblName;

	public PortraitTemplate headBehavior;

	public UILabel lblScore;

	public UILabel lblMoney;

	public UILabel chapterStar;

	public global::Ping PingView;

	public GameObject PlanetPrefab;

	private GameObject mPlanetGo;

	public GameObject payRed;

	public GameObject taskReddot;

	public FuctionButton[] functionButtons;

	public FunctionOpenTips functionOpenTips;

	public GameObject bg;

	public GameObject challengeReddot;

	public GameObject forum;

	private bool mShowAirShipWhenHide;

	private bool showReNameWindow;

	public GameObject mailReddot;
}
