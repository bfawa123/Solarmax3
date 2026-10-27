using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ChapterWindow : BaseWindow
{
	private void Awake()
	{
		this.moveTrigger.onClick = new UIEventListener.VoidDelegate(this.OnTriggerClick);
		this.moveTrigger.onDragStart = new UIEventListener.VoidDelegate(this.OnTriggerDragStart);
		this.moveTrigger.onDrag = new UIEventListener.VectorDelegate(this.OnTriggerDrag);
		this.moveTrigger.onDragEnd = new UIEventListener.VoidDelegate(this.OnTriggerDragEnd);
		this.moveTrigger.onPress = new UIEventListener.BoolDelegate(this.OnTriggerPress);
		this.sensitive *= Solarmax.Singleton<UISystem>.Get().GetNGUIRoot().pixelSizeAdjustment;
		this.ScoreTable = this.friendRank.GetComponent<UITable>();
	}

	private void Update()
	{
		if (this.IsPause)
		{
			return;
		}
		if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_LEVELS)
		{
			this.UpdateLevelsScroll();
		}
	}

	private void UpdateLevelsScroll()
	{
		if (this.mouseDown)
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.5f;
		}
		else
		{
			this.deltaScroll.x = this.deltaScroll.x - this.deltaScroll.x * 0.025f;
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
		if (this.numParent.transform.localPosition.x > 0f)
		{
			this.localpos.x = 0f;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (this.numParent.transform.localPosition.x < (float)(-(float)this.MapIndexMax) * this.changePageLength)
		{
			this.localpos.x = (float)(-(float)this.MapIndexMax) * this.changePageLength;
			this.numParent.transform.localPosition = this.localpos;
		}
		if (!this.mouseDown && Math.Abs(this.deltaScroll.x) < 2f && !this.coverLayer.activeSelf)
		{
			int num4 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
			float num5 = (float)(-(float)num4) * this.changePageLength - this.numParent.transform.localPosition.x;
			if (Mathf.Abs(num5) > 0.002f)
			{
				this.bgSpeed = num5 * 0.0018f;
				BGManager.Inst.Scroll(this.bgSpeed);
			}
		}
		if (!this.bIsShowMap)
		{
			float num6 = this.changePageLength * (float)(-(float)ChapterWindow.selectMapIndex);
			this.currentAlpha = Mathf.Abs(this.numParent.transform.localPosition.x - num6) / num;
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
			this.SetSubNumAlpha();
			this.mapShow.ManualFade(this.currentAlpha);
		}
		int num7 = Math.Abs((int)((this.numParent.transform.localPosition.x - num) / this.changePageLength));
		if (num7 != ChapterWindow.selectMapIndex && num7 >= 0 && num7 <= this.MapIndexMax)
		{
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("moveClick");
			this.ShowMap(num7, true, false);
			this.mapShow.ManualFade(0f);
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

	private void SetSubNumAlpha(float numPositionX)
	{
		for (int i = 0; i < this.mapList.Count; i++)
		{
			GameObject gameObject = this.numParent.transform.Find("num" + i).gameObject;
			UILabel component = gameObject.GetComponent<UILabel>();
			float num = 0.6f - Mathf.Abs(numPositionX + gameObject.transform.localPosition.x) * (0.1f / this.numPositionInterval.x);
			if (i > this.MapIndexMax)
			{
				num -= 0.2f;
			}
			component.alpha = num;
		}
	}

	private void SetSubNumAlpha()
	{
		for (int i = 0; i < this.mapList.Count; i++)
		{
			if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS)
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
							component.alpha = 0.5f;
						}
					}
					else
					{
						component.alpha = 1f;
					}
				}
				else
				{
					component.alpha = 0.5f;
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

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateChapterWindow);
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnStartSingleBattle);
		base.RegisterEvent(EventId.OnSingleBattleEnd);
		base.RegisterEvent(EventId.OnHaveNewChapterUnlocked);
		base.RegisterEvent(EventId.OnNumSelectClicked);
		base.RegisterEvent(EventId.OnCloseFriendViewEvent);
		base.RegisterEvent(EventId.OnShowActiveChapters);
		base.RegisterEvent(EventId.OnRefrushPingFenBtn);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.IsPause = false;
		this.SetPlayerBaseInfo();
		if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_LEVELS)
		{
			this.friendRank.SetActive(true);
			this.levelInfo.SetActive(true);
			if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter != null)
			{
				ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id);
				if (data != null)
				{
					this.tileName.text = LanguageDataProvider.GetValue(data.name);
				}
			}
		}
		Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
		Solarmax.Singleton<LocalPlayer>.Get().LeaveBattle();
		if (!ChapterWindow.bFristOpenChapterWindow || this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS)
		{
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("PaidLevelBuyTOPWindow");
			ChapterWindow.bFristOpenChapterWindow = true;
		}
	}

	public override void OnHide()
	{
		this.chapterCells.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  OnUIEventHandler", new object[0]);
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		if (eventId == EventId.UpdateChapterWindow)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  OnUIEventHandler  UpdateChapterWindow", new object[0]);
			int num = (int)args[0];
			if (num == -1)
			{
				if (Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter == null)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			if (num == 1)
			{
				this.scrollView.gameObject.SetActive(false);
				this.deltaScroll = Vector2.zero;
				this.mapShow.MapFadeIn(0.5f);
				this.SwithViewContent(ChapterWindow.SHOW_TYPE.SHOW_LEVELS);
				this.Chapter22LevelAnimation(true);
				this.mapShow.StroceEnable(false);
				this.RefrushEvaluationStates();
				return;
			}
			if (num == 2)
			{
				this.mapShow.MapFadeIn(0.5f);
				this.mapShow.StroceEnable(false);
				return;
			}
			this.scrollView.gameObject.SetActive(true);
			this.SwithViewContent(ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS);
			return;
		}
		else
		{
			if (eventId == EventId.OnStartSingleBattle)
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("startBattle");
				this.OnStartSingleBattle();
				this.bShowFriendRanking = false;
				Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
				return;
			}
			if (eventId == EventId.OnSingleBattleEnd)
			{
				this.SingleBattleEndEventHandler();
				return;
			}
			if (eventId == EventId.UpdateMoney)
			{
				this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
				return;
			}
			if (eventId == EventId.OnHaveNewChapterUnlocked)
			{
				string chapter = (string)args[0];
				this.RefreshPayChapter(chapter);
				return;
			}
			if (eventId == EventId.OnNumSelectClicked)
			{
				int to = int.Parse(((GameObject)args[0]).name.Substring(3));
				this.ShowChoosedMap(ChapterWindow.selectMapIndex, to, 0.5f);
				return;
			}
			if (eventId == EventId.OnCloseFriendViewEvent)
			{
				this.OnClickFriendRanking();
				return;
			}
			if (eventId == EventId.OnShowActiveChapters)
			{
				this.RefrushChapterActive();
				return;
			}
			if (eventId == EventId.OnRefrushPingFenBtn)
			{
				this.RefrushEvaluationStates();
			}
			return;
		}
	}

	public void SingleBattleEndEventHandler()
	{
		LevelDataHandler levelDataHandler = Solarmax.Singleton<LevelDataHandler>.Get();
		if (levelDataHandler == null)
		{
			return;
		}
		this.isSingleBattleEndTransfer = false;
		this.isSingleBattleNextLevelTransfer = false;
		if (levelDataHandler.CanPlayPassedEvaluation() && !levelDataHandler.IsEvaluationed(Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id))
		{
			ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
			Solarmax.Singleton<UISystem>.Get().ShowWindow("PaidLeveScoreWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialogLayout, new object[]
			{
				0,
				currentChapter.id
			});
		}
		if (levelDataHandler.CanUnlockNextLevel())
		{
			this.coverLayer.SetActive(true);
			levelDataHandler.SaveNextLevelFirstUnlock();
			this.unLockView.SetActive(true);
			this.isSingleBattleEndTransfer = true;
			this.isSingleBattleNextLevelTransfer = true;
			this.SwithViewContent(ChapterWindow.SHOW_TYPE.SHOW_LEVELS);
			this.ShowNextMap();
		}
		else
		{
			this.isSingleBattleEndTransfer = true;
			this.SwithViewContent(ChapterWindow.SHOW_TYPE.SHOW_LEVELS);
		}
		this.RefreshStar();
	}

	private void CleanBtn()
	{
		if (this.numParent != null && this.numParent.transform.childCount > 0)
		{
			this.numParent.transform.DestroyChildren();
		}
	}

	private void ShowNextMap()
	{
		this.isUnlocking = true;
		this.ShowChoosedMap(ChapterWindow.selectMapIndex, ChapterWindow.selectMapIndex + 1, 0.5f);
	}

	private void ShowChoosedMap(int from, int to, float duration = 0.5f)
	{
		if (from == to)
		{
			return;
		}
		this.coverLayer.SetActive(true);
		this.bIsShowMap = false;
		float x = -this.changePageLength * (float)from;
		float x2 = -this.changePageLength * (float)to;
		Vector3 localPosition = this.numParent.transform.localPosition;
		Vector3 localPosition2 = this.numParent.transform.localPosition;
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
			this.unLockView.SetActive(true);
			this.isSingleBattleNextLevelTransfer = false;
			this.isSingleBattleEndTransfer = false;
			if (Solarmax.Singleton<LevelDataHandler>.Instance.GetNextLevelInfo() != null)
			{
				base.Invoke("ShowUnlockViewAnimation", 0.6f);
			}
		}
		base.Invoke("HideCoverBackground", 0.5f);
	}

	private void HideCoverBackground()
	{
		this.coverLayer.SetActive(false);
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Instance.HideWindow("ChapterWindow");
		Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("LobbyWindowView", EventId.UpdateChaptersWindow, new object[0]));
	}

	private void RefreshStar()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		ChapterLevelGroup nextLevelInfo = Solarmax.Singleton<LevelDataHandler>.Get().GetNextLevelInfo();
		if (nextLevelInfo != null)
		{
			int num = Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex + 1;
			if (nextLevelInfo.unLock)
			{
				float num2 = (float)(-(float)ChapterWindow.selectMapIndex) * this.numPositionInterval.x;
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
					component.nStar = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.star;
					component.MaxStar = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.maxStar;
					component.SetLevelCell(true);
				}
			}
		}
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
	}

	private string FormatPower()
	{
		int power = Solarmax.Singleton<LocalPlayer>.Get().playerData.power;
		string empty = string.Empty;
		return string.Format("{0} / 30", power);
	}

	private bool CheckBattleCondition()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow   CheckBattleCondition", new object[0]);
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter == null)
		{
			return false;
		}
		if (!Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(currentChapter.id))
		{
			Tips.Make(LanguageDataProvider.GetValue(203));
			return false;
		}
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

	private void OnTriggerClick(GameObject go)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  OnTriggerClick", new object[0]);
		if (this.pressTime.TotalMilliseconds > 300.0)
		{
			return;
		}
		if (ChapterWindow.selectMapIndex < 0 || ChapterWindow.selectMapIndex > this.MapIndexMax)
		{
			return;
		}
		if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS)
		{
			return;
		}
		if (!Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id))
		{
			this.OnBuyChapter();
			return;
		}
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter != null && !Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList[ChapterWindow.selectMapIndex].unLock)
		{
			this.PlayAnimation("PaidLevelWindow_UnlockLvun", 1f);
			Solarmax.Singleton<AudioManger>.Get().PlayEffect("click");
		}
		if (this.numSelect.transform.localScale.x < 0.9f)
		{
			return;
		}
		this.deltaScroll = Vector2.zero;
		if (ChapterWindow.selectMapIndex >= 0 && ChapterWindow.selectMapIndex < this.mapList.Count)
		{
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(this.mapList[ChapterWindow.selectMapIndex], 0);
		}
		if (this.CheckBattleCondition())
		{
			if (this.IsPause)
			{
				return;
			}
			this.IsPause = true;
			this.OnStartLevelResult();
			this.coverLayer.SetActive(true);
			global::Coroutine.DelayDo(1.8f, new EventDelegate(delegate()
			{
				this.HideCoverBackground();
			}));
		}
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

	private void OnTriggerDrag(GameObject go, Vector2 delta)
	{
		this.dragTotal += delta * this.sensitive;
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

	private void OnTriggerPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			this.pressTimeStart = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
			this.deltaScroll = Vector2.zero;
		}
		else
		{
			this.pressTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.pressTimeStart.Value;
		}
	}

	public void OnClickBack()
	{
		this.CleanBtn();
		this.deltaScroll.x = 0f;
		if (this.bShowFriendRanking)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			this.bShowFriendRanking = false;
		}
		this.friendRank.SetActive(false);
		this.levelInfo.SetActive(false);
		if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_LEVELS)
		{
			this.IsPause = true;
			this.numSelect.SetActive(false);
			this.unLockView.SetActive(false);
			this.mapShow.MapFadeOut(0.5f);
			float num = Math.Abs(this.numParent.transform.localPosition.x) / (this.changePageLength * (float)(this.mapList.Count - 1));
			base.Invoke("DelayShowChapter", 0.5f);
			this.Chapter22LevelAnimation(false);
			Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter = null;
		}
		else
		{
			this.OnClickBackMain();
		}
	}

	public void OnClickBackMain()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("HomeWindow", EventId.Undefine, new object[0]));
		Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter = null;
		Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel = null;
	}

	private void DelayShowChapter()
	{
		this.numSelect.SetActive(true);
		this.mapShow.StroceEnable(false);
		this.numParent.transform.localPosition = Vector3.zero;
		this.mapShow.gameObject.SetActive(false);
		this.scrollView.gameObject.SetActive(true);
		this.SwithViewContent(ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS);
		this.IsPause = false;
	}

	private void refreshMap()
	{
		if (this.MapIndexMax == 0)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("关卡数据不正确", new object[0]);
		}
		this.MapIndexMax = ((this.MapIndexMax <= this.mapList.Count - 1) ? this.MapIndexMax : (this.mapList.Count - 1));
		if (ChapterWindow.selectMapIndex > this.MapIndexMax)
		{
			ChapterWindow.selectMapIndex = -1;
		}
		if (ChapterWindow.selectMapIndex == -1)
		{
			ChapterWindow.selectMapIndex = 0;
		}
		this.RefreshRankScore();
	}

	private void SwithViewContent(ChapterWindow.SHOW_TYPE eType)
	{
		this.ShowType = eType;
		if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_CHAPTERS)
		{
			this.ShowChapterMap();
			this.numParent.SetActive(false);
			this.numSelect.SetActive(false);
			this.mapShow.gameObject.SetActive(false);
			this.unLockView.SetActive(false);
			this.backSelect.SetActive(true);
			this.playSelect.SetActive(true);
			this.friendRank.SetActive(false);
			this.levelInfo.SetActive(false);
		}
		else if (this.ShowType == ChapterWindow.SHOW_TYPE.SHOW_LEVELS)
		{
			this.ShowLevelMap();
			this.numSelect.SetActive(true);
			this.numParent.SetActive(true);
			this.mapShow.gameObject.SetActive(true);
			this.backSelect.SetActive(true);
			this.playSelect.SetActive(true);
			this.friendRank.SetActive(true);
			this.levelInfo.SetActive(true);
		}
	}

	private void ShowChapterMap()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info(" ChapterWindow   ShowChapterMap", new object[0]);
		this.Templateparent.transform.DestroyChildren();
		this.mapList.Clear();
		this.levelList.Clear();
		this.nameList.Clear();
		foreach (ChapterInfo chapterInfo in Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList)
		{
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(chapterInfo.id);
			if (data != null)
			{
				this.mapList.Add(data.id);
				this.levelList.Add(data);
			}
		}
		this.chapterCells.Clear();
		this.tileName.text = LanguageDataProvider.GetValue(1152);
		this.chapterCost.gameObject.SetActive(false);
		this.titleTabel.Reposition();
		for (int i = 0; i < this.mapList.Count; i++)
		{
			ChapterConfig chapterConfig = this.levelList[i];
			GameObject gameObject = this.Templateparent.AddChild(this.numTemplate);
			ChapterWindowCellEX component = gameObject.GetComponent<ChapterWindowCellEX>();
			gameObject.SetActive(true);
			gameObject.name = "chapter" + i;
			if (i < this.mapList.Count)
			{
				ChapterInfo info = Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList[i];
				component.cell01.GetComponent<ChapterWindowCell>().SetInfo(info);
				this.chapterCells.Add(component.cell01);
			}
			else
			{
				component.cell01.SetActive(false);
			}
			i++;
			if (i < this.mapList.Count)
			{
				ChapterInfo info2 = Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList[i];
				component.cell02.GetComponent<ChapterWindowCell>().SetInfo(info2);
				this.chapterCells.Add(component.cell02);
			}
			else
			{
				component.cell02.SetActive(false);
			}
			i++;
			if (i < this.mapList.Count)
			{
				ChapterInfo info3 = Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList[i];
				component.cell03.GetComponent<ChapterWindowCell>().SetInfo(info3);
				this.chapterCells.Add(component.cell03);
			}
			else
			{
				component.cell03.SetActive(false);
			}
		}
		this.scrollTable.Reposition();
		this.scrollView.ResetPosition();
	}

	private void ShowLevelMap()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  ShowLevelMap", new object[0]);
		this.numParent.transform.DestroyChildren();
		this.mapList.Clear();
		this.nameList.Clear();
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			foreach (ChapterLevelGroup chapterLevelGroup in currentChapter.levelList)
			{
				LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(chapterLevelGroup.displayID);
				if (data != null && Solarmax.Singleton<MapConfigProvider>.Instance.GetData(data.map) != null)
				{
					this.mapList.Add(chapterLevelGroup.groupID);
					this.nameList.Add(LanguageDataProvider.GetValue(data.levelName));
				}
			}
			this.starsCell.Clear();
			ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(currentChapter.id);
			this.tileName.text = LanguageDataProvider.GetValue(data2.name);
			if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(currentChapter.id))
			{
				this.chapterCost.gameObject.SetActive(false);
				this.needBuyTips.SetActive(false);
			}
			else
			{
				this.needBuyTips.SetActive(true);
			}
			this.titleTabel.Reposition();
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < this.mapList.Count; i++)
			{
				ChapterLevelGroup chapterLevelGroup2 = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[i]);
				GameObject gameObject = this.numParent.AddChild(this.numTemplate1);
				gameObject.name = "num" + i;
				gameObject.SetActive(true);
				zero.x = this.numPositionInterval.x * (float)i;
				zero.y = this.numTemplate1.transform.localPosition.y;
				gameObject.transform.localPosition = zero;
				gameObject.GetComponent<UILabel>().text = this.nameList[i];
				if (i == 0)
				{
					Transform transform = gameObject.transform.Find("line/line");
					if (transform != null)
					{
						transform.gameObject.SetActive(false);
					}
				}
				LevelStarCell component = gameObject.GetComponent<LevelStarCell>();
				if (component != null)
				{
					component.IsLevel = true;
					component.unLock = chapterLevelGroup2.unLock;
					component.nStar = chapterLevelGroup2.star;
					component.MaxStar = chapterLevelGroup2.maxStar;
					component.SetLevelCell(true);
				}
				this.starsCell.Add(gameObject);
			}
			this.MapIndexMax = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.levelList.Count;
			this.refreshMap();
			if (!this.isSingleBattleEndTransfer)
			{
				int chapterFirstNoStarsLevel = Solarmax.Singleton<LevelDataHandler>.Get().GetChapterFirstNoStarsLevel(currentChapter);
				int payChapterFirstNoFullStarsLevel = Solarmax.Singleton<LevelDataHandler>.Get().GetPayChapterFirstNoFullStarsLevel(currentChapter);
				if (currentChapter.unLock && chapterFirstNoStarsLevel != -1)
				{
					this.ShowMap(chapterFirstNoStarsLevel, false, true);
				}
				else if (payChapterFirstNoFullStarsLevel != -1)
				{
					this.ShowMap(payChapterFirstNoFullStarsLevel, false, true);
				}
			}
			else
			{
				this.isSingleBattleEndTransfer = false;
				this.ShowMap(ChapterWindow.selectMapIndex, false, true);
			}
			this.SetSubNumAlpha();
		}
	}

	private void ShowMap(int index, bool imme = false, bool bSetGB = false)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  ShowMap", new object[0]);
		if (this.MapIndexMax == -1)
		{
			return;
		}
		ChapterWindow.selectMapIndex = index;
		Solarmax.Singleton<LevelDataHandler>.Get().currentLevelIndex = ChapterWindow.selectMapIndex;
		this.mapShow.Switch(this.mapList[index], imme);
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList[index].unLock)
		{
			this.ShowLockSprite(false);
		}
		else
		{
			this.ShowLockSprite(true);
		}
		if (bSetGB)
		{
			this.numParent.transform.localPosition = -this.numPositionInterval * (float)index;
		}
	}

	private void ShowUnlockViewAnimation()
	{
		this.aniPlayer.onFinished.Clear();
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.UnLockAnimationEffectEnd)));
		this.PlayAnimation("PaidLevelWindow_UnlockLv", 1f);
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("unlock");
	}

	private void UnLockAnimationEffectEnd()
	{
		this.isUnlocking = false;
		this.unLockView.SetActive(false);
	}

	private void OnStartLevelResult()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterWindow  OnStartLevelResult", new object[0]);
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentLevel == null)
		{
			return;
		}
		string id = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.id;
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = (ChapterWindow.selectMapIndex == this.MapIndexMax);
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
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(id, GameType.PayLevel, true);
	}

	public void OnStartSingleBattle()
	{
		GuideManager.TriggerGuidecompleted(GuildEndEvent.startbattle);
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		this.mapShow.AlphaFadeOut(0.5f, null);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.8f;
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
	}

	public void OnClickAddPower()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(201),
			new EventDelegate(new EventDelegate.Callback(this.AddPower))
		});
	}

	public void AddPower()
	{
	}

	private void ShowLockSprite(bool bShow)
	{
		if (this.isUnlocking)
		{
			return;
		}
		if (this.IsShowLockView())
		{
			this.unLockView.SetActive(true);
		}
		else
		{
			this.unLockView.SetActive(bShow);
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

	private void PlayAnimation(string strAni, float speed = 1f)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public void OnClickFriendRanking()
	{
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.FindGroupLevel(this.mapList[ChapterWindow.selectMapIndex]);
		if (chapterLevelGroup == null || !chapterLevelGroup.unLock || Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter == null || !Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.unLock)
		{
			Tips.Make(LanguageDataProvider.GetValue(2185));
			return;
		}
		if (!this.bShowFriendRanking)
		{
			this.friendRank.SetActive(false);
			this.levelInfo.SetActive(false);
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(this.mapList[ChapterWindow.selectMapIndex], 0);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendRanking");
			this.bShowFriendRanking = true;
		}
		else
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("FriendRanking");
			this.bShowFriendRanking = false;
			this.friendRank.SetActive(true);
			this.levelInfo.SetActive(true);
		}
	}

	private void RefreshRankScore()
	{
		if (ChapterWindow.selectMapIndex >= this.mapList.Count)
		{
			return;
		}
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Instance.GetLevelInfo(currentChapter, this.mapList[ChapterWindow.selectMapIndex]);
		string format = "{0}";
		this.levelScore.text = string.Format(format, chapterLevelGroup.Score);
	}

	private void RefreshPayChapter(string chapter)
	{
		for (int i = 0; i < this.chapterCells.Count; i++)
		{
			GameObject gameObject = this.chapterCells[i];
			ChapterConfig chapterConfig = this.levelList[i];
			ChapterWindowCell component = gameObject.GetComponent<ChapterWindowCell>();
			ChapterInfo chapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList[i];
			if (chapterInfo.id.Equals(chapter))
			{
				component.SetInfo(chapterInfo);
			}
		}
		this.chapterCost.gameObject.SetActive(false);
		this.needBuyTips.SetActive(false);
		if (ChapterWindow.selectMapIndex >= 0 && ChapterWindow.selectMapIndex < Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList.Count)
		{
			bool unLock = Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.levelList[ChapterWindow.selectMapIndex].unLock;
			if (unLock && this.unLockView.activeSelf)
			{
				this.ShowUnlockViewAnimation();
			}
			Tips.Make(LanguageDataProvider.GetValue(1136));
		}
	}

	private void Chapter22LevelAnimation(bool into)
	{
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
			component.MainCameraBlur(into, 0.7f, 0.03f, (!into) ? 5.5f : 4.5f);
		}
	}

	public void OnClickSetting()
	{
		this.bShowFriendRanking = false;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SettingWindow");
	}

	public void OnClickAddMoney()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StoreWindow");
	}

	public void OnClickAvatar()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionWindow");
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
	}

	public void OnBuyChapter()
	{
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id))
		{
			return;
		}
		DateTime? dateTime = this.timerCheck;
		if (dateTime != null && (Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.timerCheck.Value).TotalSeconds < 5.0)
		{
			return;
		}
		this.timerCheck = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(currentChapter.id) == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("ActiveBuyWidnow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog1, new object[]
		{
			LanguageDataProvider.GetValue(1100),
			new EventDelegate(new EventDelegate.Callback(this.BuyChapter)),
			currentChapter.id
		});
	}

	private void BuyChapter()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(currentChapter.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		int num = data.costGold;
		num = ((currentChapter.nPromotionPrice <= 0) ? data.costGold : currentChapter.nPromotionPrice);
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < num)
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
			csbuyChapter.chapter = currentChapter.id;
			Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSBuyChapter>(219, csbuyChapter);
		}
	}

	private bool IsShowLockView()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		return currentChapter == null || !Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(currentChapter.id);
	}

	private void RefrushChapterActive()
	{
		for (int i = 0; i < this.chapterCells.Count; i++)
		{
			GameObject gameObject = this.chapterCells[i];
			ChapterConfig chapterConfig = this.levelList[i];
			ChapterWindowCell component = gameObject.GetComponent<ChapterWindowCell>();
			ChapterInfo info = Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList[i];
			component.SetInfo(info);
		}
	}

	public void OnEvaluationChapter()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			if (!Solarmax.Singleton<LevelDataHandler>.Instance.IsEvaluationed(currentChapter.id))
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("PaidLeveScoreWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialogLayout, new object[]
				{
					0,
					currentChapter.id
				});
			}
			else
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("ActiveBuyWidnow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog1, new object[]
				{
					LanguageDataProvider.GetValue(1100),
					null,
					currentChapter.id,
					1
				});
			}
		}
	}

	private void RefrushEvaluationStates()
	{
		ChapterInfo currentChapter = Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter;
		if (currentChapter != null)
		{
			bool flag = Solarmax.Singleton<LevelDataHandler>.Instance.CanPlayPassedEvaluation();
			bool flag2 = Solarmax.Singleton<LevelDataHandler>.Instance.IsEvaluationed(currentChapter.id);
			if (flag)
			{
				this.sorecBtn.SetActive(true);
				if (flag2)
				{
					this.redPoint.SetActive(false);
				}
				else
				{
					this.redPoint.SetActive(true);
				}
			}
			else
			{
				this.sorecBtn.SetActive(false);
			}
		}
	}

	public UILabel playerMoney;

	public UILabel tileName;

	public UITable titleTabel;

	public UILabel chapterName;

	public UILabel chapterCost;

	public UIEventListener moveTrigger;

	public GameObject numParent;

	public GameObject numTemplate;

	public GameObject Templateparent;

	public UIGrid scrollTable;

	public UIScrollView scrollView;

	public GameObject numTemplate1;

	private GameObject bgParent;

	public MapShow mapShow;

	public GameObject numSelect;

	public GameObject backSelect;

	public GameObject playSelect;

	public GameObject friendRank;

	private UITable ScoreTable;

	public UILabel levelScore;

	private List<GameObject> starsCell = new List<GameObject>();

	private List<GameObject> chapterCells = new List<GameObject>();

	public GameObject unLockView;

	public GameObject needBuyTips;

	public UISprite SpriteLock;

	public UISprite LockBg;

	private bool isUnlocking;

	public GameObject coverLayer;

	public UIPlayAnimation aniPlayer;

	public GameObject levelInfo;

	public GameObject sorecBtn;

	public GameObject redPoint;

	private List<ChapterConfig> levelList = new List<ChapterConfig>();

	private List<string> mapList = new List<string>();

	private List<string> nameList = new List<string>();

	private float sensitive = 1f;

	private Vector2 dragTotal = Vector2.zero;

	private DateTime? pressTimeStart;

	private TimeSpan pressTime;

	private bool IsPause;

	private float currentAlpha;

	private float changePageLength = 360f;

	private static int selectMapIndex = -1;

	private int MapIndexMax;

	private Vector3 numPositionInterval = new Vector3(360f, 0f, 0f);

	private bool mouseDown;

	private bool bIsShowMap;

	private Vector2 deltaScroll = Vector2.zero;

	private ChapterWindow.SHOW_TYPE ShowType;

	private Vector3 localpos = Vector3.zero;

	private float bgSpeed;

	private static bool bFristOpenChapterWindow;

	private bool isSingleBattleEndTransfer;

	private bool isSingleBattleNextLevelTransfer;

	private int difficultyLevel;

	private bool bShowFriendRanking;

	private DateTime? timerCheck;

	private enum SHOW_TYPE
	{
		SHOW_CHAPTERS,
		SHOW_LEVELS
	}
}
