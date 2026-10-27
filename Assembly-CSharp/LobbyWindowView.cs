using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LobbyWindowView : BaseWindow
{
	private void Awake()
	{
		this.moveTrigger.onClick = new UIEventListener.VoidDelegate(this.OnTriggerClick);
		this.moveTrigger.onDragStart = new UIEventListener.VoidDelegate(this.OnTriggerDragStart);
		this.moveTrigger.onDrag = new UIEventListener.VectorDelegate(this.OnTriggerDrag);
		this.moveTrigger.onDragEnd = new UIEventListener.VoidDelegate(this.OnTriggerDragEnd);
		this.moveTrigger.onPress = new UIEventListener.BoolDelegate(this.OnTriggerPress);
		this.sensitive *= Solarmax.Singleton<UISystem>.Get().GetNGUIRoot().pixelSizeAdjustment;
		this.mapShowChapters.transform.localScale = Vector3.one * 0.8f;
		this.mapShow.transform.localScale = Vector3.one * 0.8f;
		this.ScoreTable = this.friendRank.GetComponent<UITable>();
		this.payChapterTable = this.payChapter.transform.Find("backBtn").GetComponent<UITable>();
	}

	private void Update()
	{
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_NULL)
		{
			return;
		}
		if (this.unlockBGFade)
		{
			this.unlockBGAlpha += 0.1f;
			if (this.unlockBGAlpha <= 1f)
			{
				this.SetLockAlpha(this.unlockBGAlpha);
			}
			else
			{
				this.unlockBGFade = false;
				this.unlockBGAlpha = 0f;
			}
		}
		if (this.mouseDown)
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.5f;
		}
		else
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.05f;
		}
		float num = this.changePageLength / 2f;
		this.localpos = this.numParent.transform.localPosition;
		this.localpos.x = this.localpos.x + this.deltaScroll.x;
		this.numParent.transform.localPosition = this.localpos;
		if (!this.mouseDown && Math.Abs(this.deltaScroll.x) < 2f)
		{
			this.deltaScroll.x = 0f;
			int num2 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
			float num3 = (float)(-(float)num2) * this.changePageLength - this.numParent.transform.localPosition.x;
			this.localpos.x = this.localpos.x + num3 * 0.1f;
			this.numParent.transform.localPosition = this.localpos;
			if (!this.clickMoving && Mathf.Abs(num3) > 0.002f)
			{
				this.bgSpeed = num3 * 0.0018f;
				BGManager.Inst.Scroll(this.bgSpeed);
			}
		}
		bool flag = false;
		if (this.numParent.transform.localPosition.x > 0f)
		{
			flag = true;
			this.localpos.x = 0f;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (this.numParent.transform.localPosition.x < (float)(-(float)this.MapIndexMax) * this.changePageLength)
		{
			flag = true;
			this.localpos.x = (float)(-(float)this.MapIndexMax) * this.changePageLength;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (!this.bIsShowMap)
		{
			float num4 = this.changePageLength * (float)(-(float)LobbyWindowView.selectMapIndex);
			this.currentAlpha = Mathf.Abs(this.numParent.transform.localPosition.x - num4) / num;
			if (this.currentAlpha > 1f)
			{
				this.currentAlpha -= 1f;
			}
			else
			{
				this.currentAlpha = 1f - this.currentAlpha;
			}
			if (this.currentAlpha > 1f)
			{
				this.currentAlpha = 1f;
			}
			this.SetLockSpriteAlpha(this.currentAlpha);
			this.UpdateDiffuseAlpha(this.currentAlpha);
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
			{
				this.mapShowChapters.ManualFade(this.currentAlpha);
			}
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS && Solarmax.Singleton<BattleSystem>.Instance.canOperation)
			{
				this.mapShow.ManualFade(this.currentAlpha);
			}
		}
		int num5 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
		if (num5 != LobbyWindowView.selectMapIndex && num5 >= 0 && num5 <= this.MapIndexMax)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("moveClick");
			this.ShowMap(num5, true, this.Showtype, false);
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
			{
				this.mapShowChapters.ManualFade(0f);
			}
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
			{
				this.mapShow.ManualFade(0f);
			}
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			this.mapShowChapters.moveSelectEffectPoint(this.mapList[num5], this.currentAlpha);
		}
		if (this.deltaScroll.x > 0f)
		{
			this.bgSpeed = 0.1f;
		}
		else if (this.deltaScroll.x < 0f)
		{
			this.bgSpeed = -0.1f;
		}
		else if (this.deltaScroll.x == 0f)
		{
			this.bgSpeed = 0f;
			return;
		}
		if (!flag && Mathf.Abs(this.deltaScroll.x) > 0.25f)
		{
			BGManager.Inst.Scroll(this.bgSpeed);
		}
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateChaptersWindow);
		base.RegisterEvent(EventId.UpdateChapterWindow);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.UpdateStar);
		base.RegisterEvent(EventId.OnUnLockNextLevel);
		base.RegisterEvent(EventId.OnStartSingleBattle);
		base.RegisterEvent(EventId.OnSingleBattleEnd);
		base.RegisterEvent(EventId.AfterOccupiedEnd);
		base.RegisterEvent(EventId.AfterTransferChapter);
		base.RegisterEvent(EventId.ChapterUnLockFinish);
		base.RegisterEvent(EventId.StartChpateScaleAnimation);
		base.RegisterEvent(EventId.AfterUICameraBlurEffect);
		base.RegisterEvent(EventId.AfterMainCameraBlurEffect);
		base.RegisterEvent(EventId.OnNumSelectClicked);
		base.RegisterEvent(EventId.OnCloseFriendViewEvent);
		base.RegisterEvent(EventId.OnShowBuyChapterEvent);
		base.RegisterEvent(EventId.OnHaveNewChapterUnlocked);
		base.RegisterEvent(EventId.OnDifficultSelectClicked);
		base.RegisterEvent(EventId.OnAchieveBgClicked);
		base.RegisterEvent(EventId.OnGuiledEndStartGame);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.test.SetActive(false);
		this.addOneGold.SetActive(false);
		if (this.mapShowChapters != null)
		{
			this.mapShowChapters.ReflushLanguange();
		}
		Solarmax.Singleton<LevelDataHandler>.Instance.allStars = AchievementModel.GetALLCompletedStars();
		Solarmax.Singleton<LevelDataHandler>.Instance.ResetChapterStars();
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.SetPlayerBaseInfo();
		GuideManager.StartGuide(GuildCondition.GC_Ui, "LobbyWindowView", base.gameObject);
		this.titleLight.GetComponent<Animator>().Play("LobbyWindowView_light1 0");
		this.CoverBgEnable(false);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.winTEAM != Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
		{
			string[] array = base.gameObject.name.Split(new char[]
			{
				'('
			});
			if (!string.IsNullOrEmpty(array[0]))
			{
				GuideManager.StartGuide(GuildCondition.GC_BTFaild, array[0], base.gameObject);
			}
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.icon != string.Empty)
		{
			this.headBehavior.Load(Solarmax.Singleton<LocalPlayer>.Get().playerData.icon, null, null);
		}
		if (LobbyWindowView.isFirstShow)
		{
			LobbyWindowView.isFirstShow = false;
			if (PlayerPrefs.GetInt("Chapter_unlock_tips", -1) == 1)
			{
				PlayerPrefs.SetInt("Chapter_unlock_tips", -1);
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2054), 3f);
				this.unLockView.SetActive(true);
				base.Invoke("ShowNextMap", 1f);
			}
		}
		this.bIsShowMap = false;
		this.IsPause = false;
		this.mapShow.StroceEnable(false);
		this.lobbyAchieveView = this.achievementView.GetComponent<LobbyAchieveView>();
		this.functionOpenTips.Refresh();
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			this.chapterStar.text = string.Format("{0}", Solarmax.Singleton<LevelDataHandler>.Instance.allStars);
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			this.friendRank.SetActive(true);
			this.achievementView.SetActive(true);
			this.ShowDiffusPanel(true);
			this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
			string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
			this.chapterStar.text = string.Format("{0} / {1}", Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.star, Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.allstar);
		}
		this.SetSubNumAlpha();
		Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
		Solarmax.Singleton<LocalPlayer>.Get().LeaveBattle();
	}

	public override void OnHide()
	{
		UIEventListener.Get(this.headIcon.gameObject).onClick = null;
		GuideManager.ClearGuideData();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnUIEventHandler", new object[0]);
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (eventId == EventId.UpdateChaptersWindow)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnUIEventHandler  UpdateChaptersWindow", new object[0]);
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
			{
				this.mapShowChapters.galaxyTweenAlpa.enabled = false;
				this.aniPlayer.onFinished.Clear();
				this.mapShowChapters.EnableEffect(false);
				this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.FadeInChapterMapOver)));
				this.PlayAnimation("LobbyWindowView_MapShowMax", 1.5f);
				this.unLockView.SetActive(false);
				this.mapShow.MapFadeOut(0.1f);
				this.Chapter22LevelAnimation(false);
				this.ShowDiffusPanel(false);
				this.achieveAnchor.SetActive(false);
				this.achievementView.SetActive(false);
				this.ShowDiffusPanel(false);
				this.friendRank.SetActive(false);
				this.mapShowChapters.ShowChapterInfo(true);
			}
			else
			{
				this.SwithViewContent(LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS);
				this.SetPlayerBaseInfo();
			}
		}
		if (eventId == EventId.OnGuiledEndStartGame)
		{
			this.IsInitViewFromGuilde = true;
			this.SwithViewContent(LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS);
			this.mapShowChapters.gameObject.SetActive(false);
			this.PlayAnimation("LobbyWindowView_MapShowMin", 1f);
			this.FadeInLevelMapOver();
		}
		else if (eventId == EventId.UpdateChapterWindow)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnUIEventHandler  UpdateChapterWindow", new object[0]);
			int num = (int)args[0];
			if (num == 0)
			{
				this.isUnlocking = true;
			}
			this.CleanBtn();
			this.ShowChapters(false);
			this.mapShowChapters.galaxyTweenAlpa.enabled = false;
			this.mapShowChapters.solarEffect.SetActive(false);
			if (num == 0 || num == 3)
			{
				this.FadeInLevelMapOver();
			}
			else
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("LevelEntry");
				this.unLockView.SetActive(false);
				this.mapShow.MapFadeIn(0.35f);
				this.aniPlayer.onFinished.Clear();
				this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.FadeInLevelMapOver)));
				global::Coroutine.DelayDo(2f, new EventDelegate(delegate()
				{
					this.aniPlayer.onFinished.Clear();
				}));
				this.PlayAnimation("LobbyWindowView_MapShowMin", 1f);
			}
		}
		else if (eventId == EventId.OnStartSingleBattle)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("startBattle");
			if (this.bShowFriendRanking)
			{
				Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			}
			this.bShowFriendRanking = false;
			this.OnStartSingleBattle();
		}
		if (eventId == EventId.UpdateMoney)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			return;
		}
		if (eventId == EventId.UpdatePower)
		{
			this.playerPower.text = this.FormatPower();
			return;
		}
		if (eventId == EventId.UpdateStar)
		{
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
			{
				this.chapterStar.text = string.Format("{0}", Solarmax.Singleton<LevelDataHandler>.Instance.allStars);
			}
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
			{
				Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
				this.chapterStar.text = string.Format("{0} / {1}", Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.star, Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.allstar);
				return;
			}
		}
		else
		{
			if (eventId == EventId.OnSingleBattleEnd)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("--------战斗结束处理---------", new object[0]);
				this.RefreshRankScore();
				this.mapShowChapters.refreshLevelPointEffect();
				this.SingleBattleEndEventHandler();
				return;
			}
			if (eventId == EventId.AfterOccupiedEnd)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("--------传送门占领结束处理---------", new object[0]);
				TouchHandler.SetWarning(0);
				return;
			}
			if (eventId != EventId.OnUnLockNextLevel)
			{
				if (eventId == EventId.AfterTransferChapter)
				{
					this.AfterTransferAnimation();
					return;
				}
				if (eventId == EventId.ChapterUnLockFinish)
				{
					this.CoverBgEnable(false);
					return;
				}
				if (eventId == EventId.StartChpateScaleAnimation)
				{
					this.PlayAnimation("MapAnimation", 1f);
					return;
				}
				if (eventId == EventId.AfterUICameraBlurEffect)
				{
					UICamera.mainCamera.GetComponent<RapidBlurEffect>().enabled = false;
					return;
				}
				if (eventId == EventId.AfterMainCameraBlurEffect)
				{
					Camera.main.GetComponent<RapidBlurEffect>().enabled = false;
					return;
				}
				if (eventId == EventId.OnNumSelectClicked)
				{
					int to = int.Parse(((GameObject)args[0]).name.Substring(3));
					this.ShowChoosedMap(LobbyWindowView.selectMapIndex, to, 0.5f);
					return;
				}
				if (eventId == EventId.OnCloseFriendViewEvent)
				{
					this.OnClickFriendRanking();
					return;
				}
				if (eventId == EventId.OnShowBuyChapterEvent)
				{
					this.BuyChapter();
					return;
				}
				if (eventId == EventId.OnHaveNewChapterUnlocked)
				{
					string chapterId = (string)args[0];
					this.RefreshChapterAfterBuySuccess(chapterId);
					return;
				}
				if (eventId == EventId.OnDifficultSelectClicked)
				{
					bool flag = (bool)args[0];
					return;
				}
				if (eventId == EventId.OnAchieveBgClicked)
				{
					this.OnClickAchieveAnchor();
				}
			}
		}
	}

	private void UnlockChapterTips()
	{
		Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2054), 3f);
	}

	private bool CheckBattleCondition()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView   CheckBattleCondition", new object[0]);
		if (Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(203));
			return false;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(204));
			return false;
		}
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.FindGroupLevel(data.levelGroup);
		if (chapterLevelGroup != null && !chapterLevelGroup.unLock)
		{
			Tips.Make(LanguageDataProvider.GetValue(205));
			return false;
		}
		return true;
	}

	public void OnClickFriends()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendWindow");
	}

	public void OnClickRank()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("RankWindow");
	}

	public void OnClickReplay()
	{
	}

	public void OnClickSetting()
	{
		this.bShowFriendRanking = false;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickComment()
	{
	}

	public void OnClickAvatar()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnClickAddMoney1()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickAddMoney100()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickTestButton()
	{
		this.OnClickComment();
	}

	public void OnClickTest()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
	}

	public void OnClickPayChapter()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("ChapterWindow", EventId.UpdateChapterWindow, new object[]
		{
			0
		}));
	}

	public void OncClickTestLevel()
	{
	}

	public void OnClickRoom()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
	}

	private void EnsureClear()
	{
		this.deltaScroll = Vector2.zero;
	}

	public void OnClickBack()
	{
		if (this.isLevelUnlocking)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("正在解锁状态，不响应返回点击事件", new object[0]);
			return;
		}
		if (this.bShowFriendRanking)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			this.bShowFriendRanking = false;
		}
		this.EnsureClear();
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			GuideManager.ClearGuideData();
			GuideManager.StartGuide(GuildCondition.GC_Ui, "LobbyWindowView", base.gameObject);
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("LevelExit");
			this.mapShowChapters.galaxyTweenAlpa.enabled = false;
			this.aniPlayer.onFinished.Clear();
			this.mapShowChapters.EnableEffect(false);
			this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.FadeInChapterMapOver)));
			this.PlayAnimation("LobbyWindowView_MapShowMax", 1f);
			this.unLockView.SetActive(false);
			this.mapShow.MapFadeOut(0.5f);
			this.Chapter22LevelAnimation(false);
			this.achievementView.SetActive(false);
			this.ShowDiffusPanel(false);
			this.achieveAnchor.SetActive(false);
			this.friendRank.SetActive(false);
			this.mapShowChapters.ShowChapterInfo(true);
		}
		else if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			BGManager.Inst.AirShipFly(3, 0.9f, new ShowWindowParams("HomeWindow", EventId.Undefine, new object[0]));
		}
	}

	public void OnClickClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		BGManager.Inst.AirShipFly(3, 0.9f, new ShowWindowParams("HomeWindow", EventId.Undefine, new object[0]));
	}

	private void Chapter22LevelAnimation(bool into)
	{
		if (this.isSingleBattleEndTransfer && !into)
		{
			return;
		}
		this.CoverBgEnable(true);
		this.circleLight.SetActive(false);
		RapidBlurEffect component = UICamera.mainCamera.GetComponent<RapidBlurEffect>();
		if (component != null)
		{
			component.enabled = true;
			component.Blur();
		}
		component = Camera.main.GetComponent<RapidBlurEffect>();
		if (component != null)
		{
			component.enabled = true;
			component.MainCameraBlur(into, (!into) ? 1.1f : 1.8f, 0.02f, (!into) ? 5.5f : 4.5f);
		}
		this.PlayAnimation(this.topAnchorPlayer, "LobbyWindowView_EdgeFade", 1f);
		this.PlayAnimation(this.bottomAnchorPlayer, (!into) ? "LobbyWindowView_BottomFade1" : "LobbyWindowView_BottomFade", 1f);
		if (into)
		{
			base.Invoke("DisableBackbutton1", 1.1f);
			base.Invoke("DisableBackbutton", 1.2f);
			this.PlayAnimation(this.pkPlayer, "Pk", 1f);
			base.Invoke("PlayInToLevelAnimator", 1f);
		}
		else
		{
			base.Invoke("PlayOutLevelAnimator", 1f);
		}
		base.Invoke("DisableCoverBg", 1.8f);
	}

	private void DisableBackbutton()
	{
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[Solarmax.Singleton<LevelDataHandler>.Instance.currentChapterIndex];
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
		this.chapterName.text = LanguageDataProvider.GetValue(data.name);
		this.chapterStar.text = string.Format("{0} / {1}", chapterInfo.star, chapterInfo.allstar);
		this.chapterOject.SetActive(true);
	}

	private void DisableBackbutton1()
	{
		this.chapterOject.SetActive(false);
		this.Left.SetActive(false);
	}

	private void PlayInToLevelAnimator()
	{
	}

	private void PlayOutLevelAnimator()
	{
		this.PlayAnimation(this.pkPlayer, "PkFadeIn", 1f);
	}

	private void DisableCoverBg()
	{
		this.CoverBgEnable(false);
	}

	private void CoverBgEnable(bool enable)
	{
		if (this.coverBackground.activeSelf != enable)
		{
			this.coverBackground.SetActive(enable);
		}
	}

	public void OnClickFriendRanking()
	{
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.FindGroupLevel(this.mapList[LobbyWindowView.selectMapIndex]);
		if (chapterLevelGroup == null || !chapterLevelGroup.unLock)
		{
			Tips.Make(LanguageDataProvider.GetValue(2185));
			return;
		}
		if (!this.bShowFriendRanking)
		{
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
			{
				this.friendRank.SetActive(false);
				this.achieveAnchor.SetActive(false);
				this.achievementView.SetActive(false);
				this.ShowDiffusPanel(false);
				GuideManager.TriggerGuidecompleted(GuildEndEvent.clickedplayer);
				Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(this.mapList[LobbyWindowView.selectMapIndex], 0);
				Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendRanking");
				this.bShowFriendRanking = true;
			}
		}
		else
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			this.bShowFriendRanking = false;
			this.friendRank.SetActive(true);
			this.achievementView.SetActive(true);
			this.ShowDiffusPanel(true);
			this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
		}
	}

	public void OnClickAchieveAnchor()
	{
		if (!this.bShowFriendAchieveing)
		{
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
			{
				this.friendRank.SetActive(false);
				this.achieveAnchor.SetActive(false);
				this.bShowFriendAchieveing = true;
				this.achievementView.SetActive(true);
				this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
			}
		}
		else
		{
			this.bShowFriendAchieveing = false;
			this.ActiveAchievementView();
			this.friendRank.SetActive(true);
			this.achievementView.SetActive(true);
			this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
		}
	}

	public void SingleBattleEndEventHandler()
	{
		LevelDataHandler levelDataHandler = Solarmax.Singleton<LevelDataHandler>.Get();
		if (levelDataHandler == null)
		{
			return;
		}
		this.RefreshLevelBar();
		this.isSingleBattleEndTransfer = true;
		if (levelDataHandler.CanPlayPassedChapterAnimator())
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("----------单机战斗完成，进入下一个章节-----------", new object[0]);
			this.CoverBgEnable(true);
			this.UnlockNextChapter();
			this.achievementView.SetActive(false);
			this.ShowDiffusPanel(false);
		}
		else if (levelDataHandler.CanUnlockNextLevel())
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("----------单机战斗完成，进入本章节的下一个关卡{0}", new object[]
			{
				LobbyWindowView.selectLevelIndex
			});
			this.CoverBgEnable(true);
			this.isLevelUnlocking = true;
			this.moveTrigger.gameObject.SetActive(false);
			levelDataHandler.SaveNextLevelFirstUnlock();
			this.mapShowChapters.ChangeGalaxyPointColor(this.mapList[LobbyWindowView.selectLevelIndex]);
			LobbyWindowView.selectLevelIndex++;
			this.isSingleBattleNextLevelTransfer = true;
			this.ShowNextMapWithState();
			this.ActiveAchievementView();
		}
		else
		{
			this.ActiveAchievementView();
			Solarmax.Singleton<LoggerSystem>.Instance.Info("----------单机战斗完成，暂时不做处理-----------{0}", new object[]
			{
				LobbyWindowView.selectMapIndex
			});
			this.isSingleBattleEndTransfer = false;
		}
		this.SetDifficultLabel(-1);
	}

	private void ActiveAchievementView()
	{
		string groupID = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectMapIndex).groupID;
		this.achieveLabel.text = string.Format("{0}/{1}", AchievementModel.GetCompletedStars(groupID), AchievementModel.GetGroupStars(groupID));
		this.achieveRedDot.SetActive(AchievementModel.CheckUnRecievedTask(groupID));
	}

	private void RefreshAchievementView()
	{
		this.achievementView.SetActive(true);
		this.ShowDiffusPanel(true);
		this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
	}

	private void RefreshLevelBar()
	{
		ChapterLevelGroup nextLevelInfo = Solarmax.Singleton<LevelDataHandler>.Get().GetNextLevelInfo();
		if (nextLevelInfo != null)
		{
			int num = Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex + 1;
			if (nextLevelInfo.unLock)
			{
				this.SetSubNumAlpha();
			}
		}
		if (this.starsCell.Count > 0)
		{
			GameObject gameObject = this.starsCell[Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex];
			if (gameObject != null)
			{
				LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
				if (component != null)
				{
					component.IsLevel = true;
					component.unLock = true;
					AchievementModel.GetDiffcultStar(Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.groupId, out component.MaxStar, out component.nStar);
					component.SetLevelCell(false);
				}
			}
		}
	}

	private void UnlockNextChapter()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  UnlockNextChapter", new object[0]);
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.name == string.Empty && Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id == "1001")
		{
			this.PassChapterMapFadeOutOver();
			return;
		}
		this.mapShow.StroceEnable(true);
		this.mapShow.gameObject.SetActive(false);
		this.moveTrigger.gameObject.SetActive(false);
		Solarmax.Singleton<LevelDataHandler>.Get().SaveChapterAnimationStatus();
		this.mapShowChapters.galaxyTweenAlpa.enabled = false;
		this.PlayAnimation("MapShowMaxWithScale", 1f);
		this.mapShowChapters.EnableEffect(false);
		base.Invoke("StartTransferAnimation", 1f);
	}

	private void StartTransferAnimation()
	{
		this.topAnchors.SetActive(false);
		this.bottomAnchors.SetActive(false);
		this.down.SetActive(false);
		this.friendRank.SetActive(false);
		this.achievementView.SetActive(false);
		this.ShowDiffusPanel(false);
		this.mapShowChapters.SetTransferAnimation(true);
		this.mapShowChapters.StartScaleAnimation();
	}

	private void AfterTransferAnimation()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  AfterTransferAnimation", new object[0]);
		this.aniPlayer.onFinished.Clear();
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.PassChapterMapFadeOutOver)));
		this.PlayAnimation("LobbyWindowView_MapShowMin", 1f);
	}

	private void PassChapterMapFadeOutOver()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  PassChapterMapFadeOutOver", new object[0]);
		this.aniPlayer.onFinished.Clear();
		this.mapShowChapters.EnableEffect(true);
		this.mapShow.StroceEnable(false);
		this.mapShow.gameObject.SetActive(true);
		this.topAnchors.SetActive(true);
		this.bottomAnchors.SetActive(true);
		this.down.SetActive(true);
		this.friendRank.SetActive(true);
		this.achievementView.SetActive(true);
		this.ShowDiffusPanel(true);
		this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
		this.ActiveAchievementView();
		this.isSingleBattleEndTransfer = false;
		this.moveTrigger.gameObject.SetActive(true);
		this.mapShowChapters.SetTransferAnimation(false);
		if (Solarmax.Singleton<LevelDataHandler>.Get().CanUnlockNextLevel())
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2063), 3f);
			this.isLevelUnlocking = true;
			this.moveTrigger.gameObject.SetActive(false);
			Solarmax.Singleton<LevelDataHandler>.Get().SaveNextLevelFirstUnlock();
			LobbyWindowView.selectLevelIndex++;
			this.isSingleBattleNextLevelTransfer = true;
			base.Invoke("ShowNextMapWithState", 0.5f);
			return;
		}
		if (LobbyWindowView.selectChapterIndex == 0)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
			Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
			{
				1,
				LanguageDataProvider.GetValue(1118),
				new EventDelegate(new EventDelegate.Callback(this.OnCloseCommonWindow))
			});
		}
		this.CoverBgEnable(false);
	}

	private void OnCloseCommonWindow()
	{
		this.OnClickBack();
	}

	public void AudioBGVolumeGradualChange(float to, float delta)
	{
		float num = delta / 0.1f;
		float bgvolume = Solarmax.Singleton<AudioManger>.Get().GetBGVolume();
		if (bgvolume < 1E-45f)
		{
			return;
		}
		this.audioBGVolumeRatio = (to - bgvolume) / num;
		int num2 = 0;
		while ((float)num2 < num)
		{
			base.Invoke("SetBGVolume", 0.1f * (float)num2);
			num2++;
		}
	}

	private void SetBGVolume()
	{
		Solarmax.Singleton<AudioManger>.Get().ChangeBGVolume(this.audioBGVolumeRatio);
	}

	private void ShowUnlockChapter()
	{
		this.mapShowChapters.StartUnLockAnimation();
		this.unLockView.SetActive(true);
		this.aniPlayer.onFinished.Clear();
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.UnLockChapterEffectEnd)));
		this.lockBackground.SetActive(true);
		this.PlayAnimation("LobbyWindowView_lockOpen", 1f);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("unlock");
	}

	private void UnLockChapterEffectEnd()
	{
		this.lockBackground.SetActive(false);
		this.unLockView.SetActive(false);
		this.moveTrigger.gameObject.SetActive(true);
	}

	private void ShowLevel()
	{
		this.OnTriggerClick(base.gameObject);
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			if (string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().playerData.name))
			{
				this.playerName.text = "guest";
			}
			else
			{
				this.playerName.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
			}
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			int allStars = Solarmax.Singleton<LevelDataHandler>.Instance.allStars;
			this.playerScore.text = allStars.ToString();
			this.playerPower.text = this.FormatPower();
		}
	}

	private string FormatPower()
	{
		int power = Solarmax.Singleton<LocalPlayer>.Get().playerData.power;
		string empty = string.Empty;
		return string.Format("{0} / 30", power);
	}

	private void ChapterUnLockAnimation()
	{
		this.CoverBgEnable(true);
		this.mapShowChapters.StartUnLockAnimation();
		global::Coroutine.DelayDo(3.2f, new EventDelegate(delegate()
		{
			this.mapShowChapters.ShowBuyBtn();
			this.DisableCoverBg();
		}));
	}

	private void OnTriggerClick(GameObject go)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnTriggerClick", new object[0]);
		if (this.pressTime.TotalMilliseconds > 300.0)
		{
			return;
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS && !Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex].unLock)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("click");
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex].id);
			LanguageDataProvider.GetValue(Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.dependChapter).name);
			Tips.Make(LanguageDataProvider.GetValue(2220));
			this.PlayAnimation("LobbyWindowView_lockUN", 1f);
			return;
		}
		if (this.isUnlocking)
		{
			return;
		}
		if (LobbyWindowView.selectMapIndex < 0 || LobbyWindowView.selectMapIndex > this.MapIndexMax)
		{
			return;
		}
		if (this.numSelect.transform.localScale.x < 0.9f)
		{
			return;
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnTriggerClick --- Show Chapters", new object[0]);
			if (!Solarmax.Singleton<NetSystem>.Get().helper.CheckNetReachability())
			{
				return;
			}
			if (this.IsPause)
			{
				return;
			}
			ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Get().chapterList[LobbyWindowView.selectMapIndex];
			if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id).costGold > 0 && !Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(chapterInfo.id))
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("click");
				ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex].id);
				LanguageDataProvider.GetValue(Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data2.dependChapter).name);
				Tips.Make(LanguageDataProvider.GetValue(2220));
				this.PlayAnimation("LobbyWindowView_lockUN", 1f);
				return;
			}
			this.mapShowChapters.galaxyTweenAlpa.enabled = false;
			if (Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex].isWattingUnlock)
			{
				Solarmax.Singleton<LevelDataHandler>.Get().SaveChapterIsWattingUnlock(LobbyWindowView.selectChapterIndex, false);
				this.aniPlayer.onFinished.Clear();
				this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.UnLockAnimationEffectEnd)));
				this.waittingUnLockView.SetActive(false);
				this.CoverBgEnable(true);
				this.PlayAnimation("LobbyWindowView_lockOpen", 1f);
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("unlock");
				base.Invoke("ChapterUnLockAnimation", 1f);
				Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex].isWattingUnlock = false;
				return;
			}
			this.IsPause = true;
			this.Chapter22LevelAnimation(true);
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectChapter(this.mapList[LobbyWindowView.selectMapIndex]);
			this.mapShowChapters.ShowChapterInfo(false);
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnTriggerClick --- Show levels", new object[0]);
			if (!this.lockList[LobbyWindowView.selectMapIndex])
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  OnTriggerClick  SHOW_LEVELS  Locked", new object[0]);
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("click");
				this.PlayAnimation("LobbyWindowView_lockUN", 1f);
				Tips.Make(LanguageDataProvider.GetValue(205));
				return;
			}
			LobbyWindowView.selectLevelIndex = LobbyWindowView.selectMapIndex;
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(this.mapList[LobbyWindowView.selectMapIndex], LobbyWindowView.selectDifLevel);
			if (this.CheckBattleCondition())
			{
				if (this.IsPause)
				{
					return;
				}
				this.IsPause = true;
				this.OnStartLevelResult();
			}
			if (this.starsCell[LobbyWindowView.selectMapIndex] != null)
			{
				LevelStarCell component = go.GetComponent<LevelStarCell>();
				if (component != null)
				{
					component.SetShowDifficult(false);
				}
			}
			this.CoverBgEnable(true);
			base.Invoke("DisableCoverBg", 1.8f);
		}
	}

	public void BuyChapter()
	{
		DateTime? dateTime = this.buyChapterTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.buyChapterTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return;
			}
		}
		this.buyChapterTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		ChapterInfo chapter = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[LobbyWindowView.selectChapterIndex];
		ChapterConfig config = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapter.id);
		if (config == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < config.costGold)
		{
			global::Coroutine.DelayDo(0.2f, new EventDelegate(delegate()
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("GoldTipsWindow");
			}));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogVIPWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			1,
			LanguageDataProvider.GetValue(1100),
			new EventDelegate(delegate()
			{
				if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < config.costGold)
				{
					Tips.Make(LanguageDataProvider.GetValue(1102));
					global::Coroutine.DelayDo(0.2f, new EventDelegate(delegate()
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow("GoldTipsWindow");
					}));
				}
				else
				{
					CSBuyChapter csbuyChapter = new CSBuyChapter();
					csbuyChapter.chapter = chapter.id;
					Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSBuyChapter>(219, csbuyChapter);
				}
			}),
			config.costGold,
			LanguageDataProvider.GetValue(23),
			LanguageDataProvider.GetValue(config.name)
		});
	}

	public void RefreshChapterAfterBuySuccess(string chapterId)
	{
		Tips.Make(LanguageDataProvider.GetValue(1136));
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterId);
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(chapterId);
		if (data.costGold > 0 && Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(chapterId))
		{
			if (Solarmax.Singleton<LevelDataHandler>.Get().chapterList[LobbyWindowView.selectMapIndex].isWattingUnlock)
			{
				this.ShowLockSprite(true);
				this.waittingUnLockView.gameObject.SetActive(true);
			}
			else
			{
				this.waittingUnLockView.gameObject.SetActive(false);
				this.ShowLockSprite(false);
			}
		}
		if (chapterInfo != null && chapterInfo.id.Equals(chapterId))
		{
			this.mapShowChapters.RefreshChapterAfterBuySuccess();
		}
		this.SetSubNumAlpha();
	}

	private void OnTriggerDragStart(GameObject go)
	{
		this.dragTotal = Vector2.zero;
		this.mouseDown = true;
		this.bIsShowMap = false;
		if (this.bShowFriendRanking)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			this.bShowFriendRanking = false;
		}
	}

	private void OnTriggerPress(GameObject go, bool isPressed)
	{
		if (this.dragging && isPressed)
		{
			this.downIndex = true;
			this.deltaScroll = Vector2.zero;
		}
		else
		{
			this.downIndex = false;
		}
		if (isPressed)
		{
			this.pressTimeStart = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		}
		else
		{
			this.pressTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.pressTimeStart.Value;
		}
	}

	private void OnTriggerDrag(GameObject go, Vector2 delta)
	{
		this.dragTotal += delta * this.sensitive;
		if (Math.Abs(delta.x) > 2f)
		{
			this.dragging = true;
		}
		this.deltaScroll.x = this.deltaScroll.x + delta.x;
	}

	private void OnTriggerDragEnd(GameObject go)
	{
		this.mouseDown = false;
		if (Math.Abs(this.deltaScroll.x) > 150f)
		{
			if (this.deltaScroll.x > 0f)
			{
				this.deltaScroll.x = 100f + (float)UnityEngine.Random.Range(0, 50);
			}
			else
			{
				this.deltaScroll.x = -(100f + (float)UnityEngine.Random.Range(0, 50));
			}
		}
	}

	public void OnSettingClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	private void ShowMap(int index, bool imme = false, LobbyWindowView.SHOW_TYPE etype = LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS, bool bSetGB = false)
	{
		if (!imme)
		{
			this.bIsShowMap = true;
		}
		if (index < 0)
		{
			index = 0;
		}
		LobbyWindowView.selectMapIndex = index;
		if (etype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count <= index)
			{
				index = Solarmax.Singleton<LevelDataHandler>.Get().chapterList.Count - 1;
			}
			LobbyWindowView.selectChapterIndex = index;
			ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Get().chapterList[index];
			bool unLock = Solarmax.Singleton<LevelDataHandler>.Get().chapterList[index].unLock;
			this.mapShowChapters.Switch(this.mapList[index], imme);
			if (this.mapList[index] == "1001")
			{
				this.mapShowChapters.solarEffect.SetActive(true);
			}
			else
			{
				this.mapShowChapters.solarEffect.SetActive(false);
			}
			Solarmax.Singleton<LevelDataHandler>.Get().SaveCurrentChapterIndex(index);
			if (unLock)
			{
				ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
				if (data.costGold > 0 && Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(data.id))
				{
					if (Solarmax.Singleton<LevelDataHandler>.Get().chapterList[index].isWattingUnlock)
					{
						this.ShowLockSprite(true);
						this.waittingUnLockView.gameObject.SetActive(true);
					}
					else
					{
						this.waittingUnLockView.gameObject.SetActive(false);
						this.ShowLockSprite(false);
					}
				}
				else if (data.costGold > 0)
				{
					this.waittingUnLockView.gameObject.SetActive(false);
					this.ShowLockSprite(true);
				}
				else if (Solarmax.Singleton<LevelDataHandler>.Get().chapterList[index].isWattingUnlock)
				{
					this.waittingUnLockView.gameObject.SetActive(true);
					this.ShowLockSprite(true);
				}
				else
				{
					this.waittingUnLockView.gameObject.SetActive(false);
					this.ShowLockSprite(false);
				}
			}
			else
			{
				this.waittingUnLockView.gameObject.SetActive(false);
				this.ShowLockSprite(true);
			}
			this.achievementView.SetActive(false);
			this.ShowDiffusPanel(false);
		}
		if (etype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			if (this.levelList.Count <= index)
			{
				index = this.levelList.Count - 1;
			}
			LobbyWindowView.selectLevelIndex = index;
			LobbyWindowView.selectMapIndex = index;
			Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex = index;
			this.mapShow.Switch(this.levelList[index], imme);
			if (Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex) != null && (Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).unLock || (LobbyWindowView.selectLevelIndex >= 1 && Solarmax.Singleton<LevelDataHandler>.Instance.IsUnLock(this.mapList[LobbyWindowView.selectLevelIndex]))))
			{
				this.ShowLockSprite(false);
			}
			else
			{
				this.ShowLockSprite(true);
			}
			bool flag = this.secondList[index];
			if (flag)
			{
				this.ShowSecondEffect(true);
			}
			else
			{
				this.ShowSecondEffect(false);
			}
			this.SetDifficultLabel(-1);
		}
		if (bSetGB)
		{
			Vector3 vector = -this.numPositionInterval * (float)index;
			this.numParent.transform.localPosition = new Vector3(vector.x, -37f, vector.z);
		}
	}

	private void SetSubNumAlpha()
	{
		for (int i = 0; i < this.mapList.Count; i++)
		{
			if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
			{
				string text = this.mapList[i];
				ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.QueryChapterInfo(text);
				if (chapterInfo == null)
				{
					Solarmax.Singleton<LoggerSystem>.Instance.Error("刷新底部失败，chapterid: " + text, new object[0]);
					return;
				}
				GameObject gameObject = this.numParent.transform.Find("num" + i).gameObject;
				UILabel component = gameObject.GetComponent<UILabel>();
				if (chapterInfo.unLock)
				{
					ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(text);
					if (data.costGold > 0)
					{
						if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(text))
						{
							component.alpha = 1f;
						}
						else
						{
							component.alpha = 0.3f;
						}
					}
					else
					{
						component.alpha = 1f;
					}
				}
				else
				{
					component.alpha = 0.3f;
				}
			}
			else
			{
				string str = this.mapList[i];
				ChapterLevelGroup levelByIndex = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(i);
				if (levelByIndex == null)
				{
					Solarmax.Singleton<LoggerSystem>.Instance.Error("刷新底部失败，levelId: " + str, new object[0]);
					return;
				}
				GameObject gameObject2 = this.numParent.transform.Find("num" + i).gameObject;
				UILabel component2 = gameObject2.GetComponent<UILabel>();
				if (levelByIndex.unLock)
				{
					component2.alpha = 1f;
				}
				else
				{
					component2.alpha = 0.5f;
				}
			}
		}
	}

	private void ShowChapters(bool bCreateBtn = false)
	{
		this.mapList.Clear();
		this.levelList.Clear();
		this.nameList.Clear();
		this.lockList.Clear();
		this.secondList.Clear();
		this.mapAlpha.Clear();
		List<ChapterInfo> chapterList = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList;
		if (chapterList != null)
		{
			foreach (ChapterInfo chapterInfo in chapterList)
			{
				ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
				if (data != null)
				{
					this.mapList.Add(data.id);
					this.mapAlpha.Add(0.6f);
					this.levelList.Add(LanguageDataProvider.GetValue(data.name));
				}
			}
		}
		this.MapIndexMax = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList.Count;
		this.refreshMap(LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS);
		this.chapterName.text = LanguageDataProvider.GetValue(1151);
		this.chapterStar.text = string.Format("{0}", Solarmax.Singleton<LevelDataHandler>.Instance.allStars);
		for (int i = 0; i < this.mapList.Count; i++)
		{
			GameObject gameObject = this.numParent.AddChild(this.numTemplate);
			gameObject.name = "num" + i;
			gameObject.SetActive(true);
			gameObject.transform.localPosition = this.numPositionInterval * (float)i;
			UILabel component = gameObject.GetComponent<UILabel>();
			component.text = this.levelList[i];
			if (i == 0)
			{
				Transform transform = gameObject.transform.Find("line/line");
				if (transform != null)
				{
					transform.gameObject.SetActive(false);
				}
			}
			GameObject gameObject2 = gameObject.transform.Find("EFF_XJ_SaoGuang_3x").gameObject;
			GameObject gameObject3 = gameObject.transform.Find("EFF_XJ_SaoGuang_4x").gameObject;
			gameObject2.SetActive(false);
			gameObject3.SetActive(false);
			LevelStarCell component2 = gameObject.GetComponent<LevelStarCell>();
			if (component2 != null)
			{
				component2.IsLevel = false;
				component2.unLock = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[i].unLock;
				component2.nStar = 0;
				component2.achievedStars = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[i].star;
				component2.maxStars = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[i].allstar;
				component2.SetChapterStar();
				component2.number = i;
			}
		}
		this.SetSubNumAlpha();
		this.numTemplate.SetActive(false);
		this.ShowMap(LobbyWindowView.selectMapIndex, true, LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS, true);
	}

	private void ShowChapterLevels(bool IsShowMap = true)
	{
		this.mapList.Clear();
		this.levelList.Clear();
		this.nameList.Clear();
		this.lockList.Clear();
		this.secondList.Clear();
		this.mapAlpha.Clear();
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in currentChapter.levelList)
			{
				LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(chapterLevelGroup.displayID);
				if (data != null)
				{
					MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(data.map);
					if (data2 != null && (currentChapter.achieveMainLine || data.mainLine == 1))
					{
						this.mapList.Add(chapterLevelGroup.groupID);
						this.levelList.Add(chapterLevelGroup.displayID);
						this.nameList.Add(LanguageDataProvider.GetValue(data.levelName));
						this.mapAlpha.Add(0.6f);
						this.secondList.Add(data.mainLine != 1);
					}
				}
			}
			this.starsCell.Clear();
			for (int i = 0; i < this.mapList.Count; i++)
			{
				ChapterLevelGroup levelInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[i]);
				this.lockList.Add(levelInfo.unLock);
				GameObject gameObject = this.numParent.AddChild(this.numTemplate);
				gameObject.name = "num" + i;
				gameObject.SetActive(true);
				gameObject.transform.localPosition = this.numPositionInterval * (float)i;
				UILabel component = gameObject.GetComponent<UILabel>();
				component.text = this.nameList[i];
				if (i == 0)
				{
					Transform transform = gameObject.transform.Find("line/line");
					if (transform != null)
					{
						transform.gameObject.SetActive(false);
					}
				}
				UIEventListener component2 = gameObject.GetComponent<UIEventListener>();
				component2.onDragStart = new UIEventListener.VoidDelegate(this.OnTriggerDragStart);
				component2.onDrag = new UIEventListener.VectorDelegate(this.OnTriggerDrag);
				component2.onDragEnd = new UIEventListener.VoidDelegate(this.OnTriggerDragEnd);
				component2.onPress = new UIEventListener.BoolDelegate(this.OnTriggerPress);
				LevelStarCell component3 = gameObject.GetComponent<LevelStarCell>();
				if (component3 != null)
				{
					component3.IsLevel = true;
					component3.unLock = levelInfo.unLock;
					AchievementModel.GetDiffcultStar(levelInfo.groupID, out component3.MaxStar, out component3.nStar);
					component3.SetLevelCell(false);
				}
				this.starsCell.Add(gameObject);
			}
			this.SetSubNumAlpha();
		}
		ChapterConfig data3 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(currentChapter.id);
		this.chapterName.text = LanguageDataProvider.GetValue(data3.name);
		this.MapIndexMax = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.levelList.Count;
		this.refreshMap(LobbyWindowView.SHOW_TYPE.SHOW_LEVELS);
		this.numTemplate.SetActive(false);
		if (IsShowMap)
		{
			if (!this.isSingleBattleEndTransfer)
			{
				int num = Solarmax.Singleton<LevelDataHandler>.Get().GetChapterFirstNoStarsLevel(currentChapter);
				if (this.IsInitViewFromGuilde)
				{
					num = 0;
					this.IsInitViewFromGuilde = false;
				}
				int chapterFirstNoFullStarsLevel = Solarmax.Singleton<LevelDataHandler>.Get().GetChapterFirstNoFullStarsLevel(currentChapter);
				if (currentChapter.unLock && num != -1)
				{
					this.ShowMap(num, false, LobbyWindowView.SHOW_TYPE.SHOW_LEVELS, true);
				}
				else if (chapterFirstNoFullStarsLevel != -1)
				{
					this.ShowMap(chapterFirstNoFullStarsLevel, false, LobbyWindowView.SHOW_TYPE.SHOW_LEVELS, true);
				}
				else
				{
					this.ShowMap(0, false, LobbyWindowView.SHOW_TYPE.SHOW_LEVELS, true);
				}
			}
			else
			{
				this.isSingleBattleEndTransfer = false;
				this.ShowMap(LobbyWindowView.selectLevelIndex, false, LobbyWindowView.SHOW_TYPE.SHOW_LEVELS, true);
			}
		}
	}

	private void refreshMap(LobbyWindowView.SHOW_TYPE eType)
	{
		if (this.MapIndexMax == 0)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("关卡数据不正确", new object[0]);
		}
		this.MapIndexMax = ((this.MapIndexMax <= this.mapList.Count - 1) ? this.MapIndexMax : (this.mapList.Count - 1));
		if (eType == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().lobbyWindowType = 0;
			if (LobbyWindowView.selectChapterIndex > this.MapIndexMax)
			{
				LobbyWindowView.selectChapterIndex = -1;
			}
			if (LobbyWindowView.selectChapterIndex == -1)
			{
				LobbyWindowView.selectChapterIndex = Solarmax.Singleton<LevelDataHandler>.Get().InitCurrentChapterIndex();
				LobbyWindowView.selectMapIndex = LobbyWindowView.selectChapterIndex;
			}
			else
			{
				LobbyWindowView.selectMapIndex = LobbyWindowView.selectChapterIndex;
			}
			this.payChapterTable.Reposition();
		}
		else if (eType == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().lobbyWindowType = 1;
			if (LobbyWindowView.selectLevelIndex > this.MapIndexMax)
			{
				LobbyWindowView.selectLevelIndex = -1;
			}
			if (LobbyWindowView.selectLevelIndex == -1)
			{
				LobbyWindowView.selectLevelIndex = 0;
				LobbyWindowView.selectMapIndex = LobbyWindowView.selectLevelIndex;
			}
			else
			{
				LobbyWindowView.selectMapIndex = LobbyWindowView.selectLevelIndex;
			}
			this.RefreshRankScore();
		}
	}

	private void ShowNextMap()
	{
		this.ShowChoosedMap(LobbyWindowView.selectMapIndex, LobbyWindowView.selectMapIndex + 1, 0.5f);
	}

	private void ShowNextMapWithState()
	{
		this.isUnlocking = true;
		this.ShowChoosedMap(LobbyWindowView.selectMapIndex, LobbyWindowView.selectMapIndex + 1, 0.5f);
		this.SetDifficultLabel(-1);
	}

	private void ShowChoosedMap(int from, int to, float duration = 0.5f)
	{
		if (from == to)
		{
			return;
		}
		this.clickMoving = true;
		this.CoverBgEnable(true);
		this.bIsShowMap = false;
		float x = -this.changePageLength * (float)from;
		float x2 = -this.changePageLength * (float)to;
		Vector3 localPosition = this.numParent.transform.localPosition;
		Vector3 localPosition2 = this.numParent.transform.localPosition;
		if (this.isSingleBattleNextLevelTransfer)
		{
			this.CleanBtn();
			this.ShowChapterLevels(false);
			this.unLockView.SetActive(true);
			base.Invoke("UnlockBGFadeIn", 0.25f);
			base.Invoke("UnlockBGFadeIn", 0.5f);
			this.SetLockAlpha(0f);
		}
		localPosition2.x = x;
		localPosition.x = x2;
		TweenPosition tweenPosition = this.numParent.GetComponent<TweenPosition>();
		if (tweenPosition == null)
		{
			tweenPosition = this.numParent.AddComponent<TweenPosition>();
		}
		tweenPosition.ResetToBeginning();
		this.numParent.transform.localPosition = localPosition2;
		tweenPosition.from = localPosition2;
		tweenPosition.to = localPosition;
		tweenPosition.duration = duration;
		tweenPosition.Play(true);
		if (this.isSingleBattleNextLevelTransfer)
		{
			this.isSingleBattleNextLevelTransfer = false;
			if (Solarmax.Singleton<LevelDataHandler>.Instance.GetNextLevelInfo() != null)
			{
				base.Invoke("ShowUnlockViewAnimation", 0.6f);
			}
		}
		base.Invoke("HideCoverBackground", 0.5f);
	}

	private void HideCoverBackground()
	{
		this.CoverBgEnable(false);
		this.clickMoving = false;
		this.unlockBGFade = false;
	}

	private void UnlockBGFadeIn()
	{
		this.unlockBGFade = !this.unlockBGFade;
	}

	private void ShowUnlockViewAnimation()
	{
		this.aniPlayer.onFinished.Clear();
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.UnLockAnimationEffectEnd)));
		this.PlayAnimation("LobbyWindowView_lockOpen", 1f);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("unlock");
		this.lockBackground.SetActive(true);
		this.CoverBgEnable(false);
	}

	private void CleanBtn()
	{
		if (this.numParent != null && this.numParent.transform.childCount > 0)
		{
			this.numParent.transform.DestroyChildren();
		}
	}

	private void SwithViewContent(LobbyWindowView.SHOW_TYPE eType)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("LobbyWindowView  SwithViewContent", new object[0]);
		this.CleanBtn();
		this.Showtype = eType;
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			this.ShowDiffusPanel(false);
			this.ShowChapters(false);
			this.mapShowChapters.EnableEffect(false);
			this.mapShowChapters.gameObject.SetActive(true);
			this.mapShow.gameObject.SetActive(false);
			this.chapterOject.SetActive(false);
			this.effectObject.SetActive(false);
			this.friendRank.SetActive(false);
			this.achieveAnchor.SetActive(false);
			this.achievementView.SetActive(false);
			this.ShowDiffusPanel(false);
			this.payChapterTable.Reposition();
			this.Left.SetActive(false);
			this.back.SetActive(true);
			Solarmax.Singleton<UISystem>.Get().SetInitEvent(base.GetName(), EventId.UpdateChaptersWindow, new object[0]);
			return;
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			this.ShowChapterLevels(true);
			this.mapShowChapters.EnableEffect(true);
			this.mapShow.gameObject.SetActive(true);
			this.chapterOject.SetActive(true);
			this.friendRank.SetActive(true);
			this.achievementView.SetActive(true);
			this.ShowDiffusPanel(true);
			this.lobbyAchieveView.Show(Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectLevelIndex).groupID);
			this.ActiveAchievementView();
			this.payChapter.SetActive(false);
			this.pk.SetActive(false);
			this.Left.SetActive(false);
			this.back.SetActive(true);
			Solarmax.Singleton<UISystem>.Get().SetInitEvent(base.GetName(), EventId.UpdateChapterWindow, new object[]
			{
				3
			});
		}
	}

	public void OnStartSingleBattle()
	{
		BGManager.Inst.SetAirShipVisible(false);
		GuideManager.TriggerGuidecompleted(GuildEndEvent.startbattle);
		this.mapShow.AlphaFadeOut(0.5f, null);
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
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
			Solarmax.Singleton<BattleSystem>.Instance.canOperation = true;
		}));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleWindow_off");
		GuideManager.StartGuide(GuildCondition.GC_Level, Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId, null);
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
	}

	private void OnStartLevelResult()
	{
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentLevel == null)
		{
			return;
		}
		string id = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.id;
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = (LobbyWindowView.selectMapIndex == this.MapIndexMax);
		if (this.difficultyLevel == 0)
		{
			this.difficultyLevel = 1;
		}
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
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(id, GameType.SingleLevel, true);
	}

	private void FadeInLevelMapOver()
	{
		this.mapShow.StroceEnable(false);
		this.mapShowChapters.galaxyTweenAlpa.enabled = true;
		this.SwithViewContent(LobbyWindowView.SHOW_TYPE.SHOW_LEVELS);
		this.SetSubNumAlpha();
		this.IsPause = false;
	}

	private void FadeInChapterMapOver()
	{
		this.mapShowChapters.galaxyTweenAlpa.enabled = true;
		this.SwithViewContent(LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS);
		this.SetSubNumAlpha();
		this.IsPause = false;
		this.mapShow.StroceEnable(false);
		int @int = PlayerPrefs.GetInt("Chapter_unlock_tips", -1);
		if (@int == 1)
		{
			PlayerPrefs.SetInt("Chapter_unlock_tips", -1);
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2054), 3f);
			this.ShowChoosedMap(LobbyWindowView.selectMapIndex, LobbyWindowView.selectMapIndex + 1, 0.8f);
		}
	}

	private void PlayAnimation(string strAni, float speed = 1f)
	{
		this.PlayAnimation(this.aniPlayer, strAni, speed);
	}

	private void PlayAnimation(UIPlayAnimation player, string strAni, float speed = 1f)
	{
		player.clipName = strAni;
		player.resetOnPlay = true;
		player.Play(true, false, speed, false);
	}

	private void UnLockAnimationEffectEnd()
	{
		this.lockBackground.SetActive(false);
		this.isUnlocking = false;
		this.unLockView.SetActive(false);
		if (this.isSingleBattleEndTransfer)
		{
			this.isSingleBattleEndTransfer = false;
		}
		this.moveTrigger.gameObject.SetActive(true);
		this.isLevelUnlocking = false;
	}

	private void ShowLockSprite(bool bShow)
	{
		if (this.isUnlocking)
		{
			return;
		}
		if (bShow)
		{
			this.unLockView.SetActive(true);
		}
		else
		{
			this.unLockView.SetActive(false);
		}
	}

	private void ShowSecondEffect(bool bShow)
	{
		if (bShow)
		{
			this.effectObject.SetActive(true);
		}
		else
		{
			this.effectObject.SetActive(false);
		}
	}

	private void SetLockSpriteAlpha(float fAlpha)
	{
		if (this.isUnlocking)
		{
			return;
		}
		Color color = this.LockBg.color;
		color.a = fAlpha;
		this.LockBg.color = color;
		color = this.SpriteLock.color;
		color.a = fAlpha;
		this.SpriteLock.color = color;
	}

	private void SetLockAlpha(float fAlpha)
	{
		Color color = this.LockBg.color;
		color.a = fAlpha;
		this.LockBg.color = color;
		color = this.SpriteLock.color;
		color.a = fAlpha;
		this.SpriteLock.color = color;
	}

	private void RefreshRankScore()
	{
		if (LobbyWindowView.selectLevelIndex < 0 || LobbyWindowView.selectLevelIndex >= this.levelList.Count)
		{
			LobbyWindowView.selectLevelIndex = 0;
		}
		if (this.levelList.Count <= 0)
		{
			return;
		}
		ChapterLevelInfo level = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevel(this.levelList[LobbyWindowView.selectLevelIndex]);
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(level.id);
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.FindGroupLevel(data.levelGroup);
		string format = "{0}";
		this.levelScore.text = string.Format(format, chapterLevelGroup.Score);
	}

	public void OncClickSelectDif0()
	{
		LobbyWindowView.selectDifLevel = 0;
		this.ShowLevelEffect();
		this.SetDifficultLabel(0);
		GameObject gameObject = this.starsCell[LobbyWindowView.selectMapIndex];
		if (gameObject != null)
		{
			LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
			if (component != null)
			{
				component.SetShowDifficult(false);
			}
		}
	}

	public void OncClickSelectDif1()
	{
		if (this.IsCanSelectDiffuse(0) && !this.IsCanSelectDiffuse(1))
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2110), 1f);
			return;
		}
		if (!this.IsCanSelectDiffuse(1))
		{
			return;
		}
		LobbyWindowView.selectDifLevel = 1;
		this.ShowLevelEffect();
		this.SetDifficultLabel(1);
		GameObject gameObject = this.starsCell[LobbyWindowView.selectMapIndex];
		if (gameObject != null)
		{
			LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
			if (component != null)
			{
				component.SetShowDifficult(false);
			}
		}
	}

	public void OncClickSelectDif2()
	{
		if (this.IsCanSelectDiffuse(1) && !this.IsCanSelectDiffuse(2))
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2111), 1f);
			return;
		}
		if (!this.IsCanSelectDiffuse(2))
		{
			return;
		}
		LobbyWindowView.selectDifLevel = 2;
		this.ShowLevelEffect();
		this.SetDifficultLabel(2);
		GameObject gameObject = this.starsCell[LobbyWindowView.selectMapIndex];
		if (gameObject != null)
		{
			LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
			if (component != null)
			{
				component.SetShowDifficult(false);
			}
		}
	}

	private bool IsCanSelectDiffuse(int nDif)
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter == null)
		{
			return false;
		}
		ChapterLevelGroup levelInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[LobbyWindowView.selectMapIndex]);
		if (levelInfo == null)
		{
			return false;
		}
		ChapterLevelInfo level = levelInfo.GetLevel(nDif - 1);
		return level != null && (level.unLock || level.star > 0);
	}

	private void ShowLevelEffect()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter == null)
		{
			return;
		}
		ChapterLevelGroup levelInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[LobbyWindowView.selectMapIndex]);
		if (levelInfo == null)
		{
			return;
		}
		int num = levelInfo.mapList.Count - 1;
		if (num >= 0 && num < this.difList.Length)
		{
			this.difList[2].SetActive(true);
			LevelDiffuseCell component = this.difList[2].GetComponent<LevelDiffuseCell>();
			if (component)
			{
				component.SetInfoLocal(LobbyWindowView.selectDifLevel, levelInfo.groupID, num);
			}
		}
	}

	private void SetDifficultLabel(int select = -1)
	{
		if (this.achieveAnchor.activeSelf)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectMapIndex) == null)
			{
				return;
			}
			string groupID = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelByIndex(LobbyWindowView.selectMapIndex).groupID;
			this.achieveLabel.text = string.Format("{0}/{1}", AchievementModel.GetCompletedStars(groupID), AchievementModel.GetGroupStars(groupID));
			this.achieveRedDot.SetActive(AchievementModel.CheckUnRecievedTask(groupID));
		}
		int difficult = this.GetDifficult();
		if (select != -1)
		{
			difficult = select;
		}
		for (int i = 0; i < this.starsCell.Count; i++)
		{
			GameObject gameObject = this.starsCell[i];
			if (gameObject != null)
			{
				LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
				if (component != null)
				{
					component.SetDifficult(difficult, false);
				}
			}
		}
		LobbyWindowView.selectDifLevel = difficult;
		if (LobbyWindowView.lastSelectMapIndex != -1 && LobbyWindowView.lastSelectMapIndex != LobbyWindowView.selectMapIndex)
		{
			if (LobbyWindowView.lastSelectMapIndex < 0 || LobbyWindowView.lastSelectMapIndex >= this.starsCell.Count)
			{
				LobbyWindowView.lastSelectMapIndex = 0;
			}
			GameObject gameObject2 = this.starsCell[LobbyWindowView.lastSelectMapIndex];
			if (gameObject2 != null)
			{
				LevelStarCell component2 = gameObject2.GetComponent<LevelStarCell>();
				if (component2 != null)
				{
					component2.SetShowDifficult(false);
				}
			}
		}
		this.ShowDiffusPanel(true);
	}

	private int GetDifficult()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter == null)
		{
			return 0;
		}
		ChapterLevelGroup levelInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[LobbyWindowView.selectMapIndex]);
		if (levelInfo == null)
		{
			return 0;
		}
		int maxDiffuse = levelInfo.GetMaxDiffuse();
		LobbyWindowView.selectDifLevel = maxDiffuse;
		return maxDiffuse;
	}

	private void ShowDiffusPanel(bool bShow)
	{
		if (bShow)
		{
			LobbyWindowView.lastSelectMapIndex = LobbyWindowView.selectMapIndex;
			ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
			if (currentChapter == null)
			{
				return;
			}
			ChapterLevelGroup levelInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[LobbyWindowView.selectMapIndex]);
			if (levelInfo == null)
			{
				return;
			}
			this.difPanel.gameObject.SetActive(true);
			for (int i = 0; i < this.difList.Length; i++)
			{
				this.difList[i].SetActive(false);
			}
			int num = levelInfo.mapList.Count - 1;
			if (num >= 0 && num < this.difList.Length)
			{
				this.difList[2].SetActive(true);
				LevelDiffuseCell component = this.difList[2].GetComponent<LevelDiffuseCell>();
				if (component)
				{
					component.SetInfoLocal(LobbyWindowView.selectDifLevel, levelInfo.groupID, num);
				}
			}
		}
	}

	private void UpdateDiffuseAlpha(float fAlpha)
	{
		if (this.difPanel != null)
		{
			this.difPanel.alpha = fAlpha;
		}
	}

	public override void OnLanguageChanged()
	{
		base.OnLanguageChanged();
		Solarmax.Singleton<AchievementModel>.Get().onLanguageChanged();
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			this.ChangeTopLanguage();
			this.ChangeBottomLanguage();
			this.ChangeAchievementLanguage();
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_CHAPTERS)
		{
			this.ChangeUnlockContainerLanguage();
		}
	}

	private void ChangeTopLanguage()
	{
		ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.chapterList[Solarmax.Singleton<LevelDataHandler>.Instance.currentChapterIndex];
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
		this.chapterName.text = LanguageDataProvider.GetValue(data.name);
	}

	private void ChangeBottomLanguage()
	{
		this.nameList.Clear();
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in currentChapter.levelList)
			{
				LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(chapterLevelGroup.displayID);
				if (data != null)
				{
					MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(data.map);
					if (data2 != null && (currentChapter.achieveMainLine || data.mainLine == 1))
					{
						this.nameList.Add(LanguageDataProvider.GetValue(data.levelName));
					}
				}
			}
		}
		if (this.Showtype == LobbyWindowView.SHOW_TYPE.SHOW_LEVELS)
		{
			for (int i = 0; i < this.mapList.Count; i++)
			{
				GameObject gameObject = this.numParent.transform.GetChild(i).gameObject;
				UILabel component = gameObject.GetComponent<UILabel>();
				if (i < this.nameList.Count)
				{
					component.text = this.nameList[i];
				}
			}
		}
	}

	private void ChangeAchievementLanguage()
	{
		if (this.achievementView.activeSelf && this.lobbyAchieveView != null)
		{
			this.lobbyAchieveView.ChangeLanguage();
		}
	}

	private void ChangeUnlockContainerLanguage()
	{
		this.mapShowChapters.ChangeLanguage();
	}

	public void StartSingleGame()
	{
		Solarmax.Singleton<TaskModel>.Get().Init();
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestChapters();
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

	public UILabel playerName;

	public UILabel playerScore;

	public UILabel levelScore;

	public UILabel playerMoney;

	public UILabel playerPower;

	public UILabel chapterName;

	public UILabel chapterStar;

	public UILabel chapterStar1;

	public GameObject chapterOject;

	public GameObject Left;

	public GameObject back;

	public MapChapterShow mapShowChapters;

	public MapShow mapShow;

	public UIEventListener moveTrigger;

	public GameObject numParent;

	public GameObject numTemplate;

	public GameObject numSelect;

	public GameObject pk;

	public GameObject test;

	public GameObject unLockView;

	public GameObject waittingUnLockView;

	public UISprite headIcon;

	public PortraitTemplate headBehavior;

	public GameObject addOneGold;

	public GameObject achievementView;

	private LobbyAchieveView lobbyAchieveView;

	public FunctionOpenTips functionOpenTips;

	private const string SAVE_CHAPTER_UNLOCK_TIPS = "Chapter_unlock_tips";

	private int difficultyLevel;

	private List<string> levelList = new List<string>();

	private List<string> mapList = new List<string>();

	private List<string> nameList = new List<string>();

	private List<bool> lockList = new List<bool>();

	private List<bool> secondList = new List<bool>();

	private List<float> mapAlpha = new List<float>();

	private Vector3 numPositionInterval = new Vector3(360f, 0f, 0f);

	private bool mouseDown;

	private bool dragging;

	private bool downIndex;

	private bool bIsShowMap;

	private Vector2 deltaScroll = Vector2.zero;

	private DateTime? pressTimeStart;

	private TimeSpan pressTime;

	private float sensitive = 1f;

	private Vector2 dragTotal = Vector2.zero;

	private float currentAlpha;

	private float changePageLength = 360f;

	private static int selectChapterIndex = -1;

	private static int selectLevelIndex = -1;

	private static int selectMapIndex = -1;

	private static int lastSelectMapIndex = -1;

	private int MapIndexMax;

	private Vector3 localpos = Vector3.zero;

	public UIPlayAnimation aniPlayer;

	public UIPlayAnimation topAnchorPlayer;

	public UIPlayAnimation bottomAnchorPlayer;

	public UIPlayAnimation paidLevelPlayer;

	public UIPlayAnimation friendAnchorPlayer;

	public UIPlayAnimation pkPlayer;

	private bool IsPause;

	private bool isSingleBattleEndTransfer;

	private bool isSingleBattleNextLevelTransfer;

	public UISprite SpriteLock;

	public UISprite LockBg;

	public UIEventListener LockListener;

	public GameObject topAnchors;

	public GameObject bottomAnchors;

	public GameObject down;

	public GameObject effectObject;

	public GameObject friendRank;

	public GameObject achieveAnchor;

	public UILabel achieveLabel;

	public GameObject achieveRedDot;

	private UITable ScoreTable;

	public GameObject payChapter;

	private UITable payChapterTable;

	private bool isUnlocking;

	private List<GameObject> starsCell = new List<GameObject>();

	public GameObject titleLight;

	public GameObject circleLight;

	public GameObject lockBackground;

	public bool isLevelUnlocking;

	public GameObject coverBackground;

	public GameObject testButton;

	public UIWidget difPanel;

	public GameObject[] difList;

	private static int selectDifLevel;

	private static bool isFirstShow = true;

	private bool clickMoving;

	private bool unlockBGFade;

	private LobbyWindowView.SHOW_TYPE Showtype;

	private float unlockBGAlpha;

	private float bgSpeed;

	private bool IsInitViewFromGuilde;

	private bool bShowFriendRanking;

	private bool bShowFriendAchieveing;

	private float audioBGVolumeRatio;

	private DateTime? buyChapterTime;

	private enum SHOW_TYPE
	{
		SHOW_NULL,
		SHOW_CHAPTERS,
		SHOW_LEVELS
	}
}
