using System;
using System.Collections;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class BattleWindow_off : BaseWindow
{
	private void Awake()
	{
		for (int i = 0; i < this.unitdown.Length; i++)
		{
			UIEventListener component = this.unitdown[i].GetComponent<UIEventListener>();
			component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
		for (int j = 0; j < this.unitleft.Length; j++)
		{
			UIEventListener component2 = this.unitleft[j].GetComponent<UIEventListener>();
			component2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component2.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
		for (int k = 0; k < this.unitright.Length; k++)
		{
			UIEventListener component3 = this.unitright[k].GetComponent<UIEventListener>();
			component3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component3.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
	}

	private void Start()
	{
		this.percent = 1f;
		this.SetPercent(false);
		UIRoot nguiroot = Solarmax.Singleton<UISystem>.Get().GetNGUIRoot();
		this.sensitive *= nguiroot.pixelSizeAdjustment;
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnStartSingleBattle);
		base.RegisterEvent(EventId.NoticeSelfTeam);
		base.RegisterEvent(EventId.OnBattleDisconnect);
		base.RegisterEvent(EventId.RequestUserResult);
		base.RegisterEvent(EventId.OnTouchBegin);
		base.RegisterEvent(EventId.OnTouchPause);
		base.RegisterEvent(EventId.OnTouchEnd);
		base.RegisterEvent(EventId.OnPopulationUp);
		base.RegisterEvent(EventId.OnPopulationDown);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.percentPicWidth = (float)this.percetPic.width;
		this.InitFightProgressOption();
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		Color color = team.color;
		color.a = 0.7f;
		team.color = color;
		if (!this.IsShowButton())
		{
			GameObject[] array = this.buttonList;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
		}
		this.populationValueLabel.text = string.Format("{0}/{1}", team.current, team.currentMax);
		this.populationValueLabel.color = color;
		this.populationLabel.color = color;
		this.TimerProc();
		this.playSpeed = 2;
		this.SetPlaySpeedBtnStatus();
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentLevel != null)
		{
			this.levelConfig = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.id);
			if (this.levelConfig != null)
			{
				this.curStar = this.levelConfig.maxStar;
			}
			if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter != null)
			{
				this.chpaterConfig = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id);
			}
		}
		if (this.chpaterConfig != null && this.chpaterConfig.type == 0)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.trackChallenge)
			{
				this.achievementView.SetActive(true);
				this.achievementView.GetComponent<AchievementView>().Init();
			}
			else
			{
				this.achievementView.SetActive(false);
			}
		}
		else
		{
			this.achievementView.SetActive(false);
		}
		UIEventListener component = this.ProcessAram.GetComponent<UIEventListener>();
		component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnSelectProcessAram));
		this.SetLevelStarsAndDesc();
		this.UpdateSurvivalAim();
		if (!Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID()))
		{
			this.achievementView.SetActive(false);
			return;
		}
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID()].GetAchievementByDifficult((AchievementDifficult)Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentDiffcult(), false).Count == 0)
		{
			this.achievementView.SetActive(false);
			return;
		}
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.trackChallenge)
		{
			this.achievementView.SetActive(true);
			return;
		}
		this.achievementView.SetActive(false);
	}

	private void OnSelectProcessAram(GameObject go)
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		Vector2 vector = UICamera.currentCamera.WorldToScreenPoint(this.percentleft.gameObject.transform.position);
		Vector2 vector2 = UICamera.currentCamera.WorldToScreenPoint(this.percentright.gameObject.transform.position);
		Vector2 lastEventPosition = UICamera.lastEventPosition;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
		{
			this.percent = (lastEventPosition.x - vector.x) / (vector2.x - vector.x);
		}
		else
		{
			this.percent = (lastEventPosition.y - vector.y) / (vector2.y - vector.y);
		}
		if (this.percent < 0f)
		{
			this.percent = 0f;
		}
		if (this.percent > 1f)
		{
			this.percent = 1f;
		}
		this.percent = Mathf.Round(this.percent * 100f) / 100f;
		this.SetPercent(false);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.NoticeSelfTeam)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
			{
				return;
			}
			MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId);
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
			for (int i = 0; i < data.player_count; i++)
			{
				Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)(i + 1));
				for (int j = 0; j < data.mpcList.Count; j++)
				{
					MapPlayerConfig mapPlayerConfig = data.mpcList[j];
					if (mapPlayerConfig.camption == (int)team2.team)
					{
						if (team2 == team)
						{
							Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(mapPlayerConfig.tag);
							Solarmax.Singleton<EffectManager>.Get().ShowGuideEffect(node, true);
						}
						else if (team.IsFriend(team2.groupID))
						{
							Node node2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(mapPlayerConfig.tag);
							Solarmax.Singleton<EffectManager>.Get().ShowGuideEffect(node2, false);
						}
					}
				}
			}
			TouchHandler.ShowingGuidEffect = true;
		}
		else if (eventId == EventId.OnStartSingleBattle)
		{
			this.OnStartSingleBattle();
		}
		if (eventId == EventId.OnPopulationUp)
		{
			int num = (int)args[0];
			int num2 = (int)args[1];
			int num3 = (int)args[2];
			this.populationValueLabel.text = string.Format("{0} / {1}", num, num2);
			this.popLableValue1.text = this.populationValueLabel.text;
			this.popLable1.color = new Color(0f, 255f, 0f, 1f);
			this.popLableValue1.color = new Color(0f, 255f, 0f, 1f);
			this.popLableAdd.text = string.Format("+{0}", num3);
			this.popLableAdd.color = new Color(0.2f, 1f, 0.2f, 1f);
			this.popLableAdd.transform.localPosition = this.popLableValue1.transform.localPosition + new Vector3(this.popLableValue1.printedSize.x + 30f, 0f, 0f);
		}
		if (eventId == EventId.OnPopulationDown)
		{
			int num4 = (int)args[0];
			int num5 = (int)args[1];
			int num6 = (int)args[2];
			this.populationValueLabel.text = string.Format("{0} / {1}", num4, num5);
			this.popLableValue1.text = this.populationValueLabel.text;
			this.popLable1.color = new Color(255f, 0f, 0f, 1f);
			this.popLableValue1.color = new Color(255f, 0f, 0f, 1f);
			this.popLableAdd.text = string.Format("-{0}", num6);
			this.popLableAdd.color = new Color(1f, 0.2f, 0.2f, 1f);
			this.popLableAdd.transform.localPosition = this.popLableValue1.transform.localPosition + new Vector3(this.popLableValue1.printedSize.x + 30f, 0f, 0f);
		}
	}

	public override void OnHide()
	{
		this.isPlayingDropStarAnimation = false;
		this.dropStarAnimationDelta = 0f;
		this.dropStarAnimationQueue.Clear();
		GuideManager.ClearGuideData();
		if (this.chpaterConfig != null && this.chpaterConfig.type == 0)
		{
			AchievementView component = this.achievementView.GetComponent<AchievementView>();
			if (this.achievementView.activeSelf)
			{
				component.Destory();
			}
		}
	}

	private void Update()
	{
		if (this.popLable1.alpha > 0f)
		{
			UIRect uirect = this.popLableValue1;
			float alpha = this.popLable1.alpha - Time.deltaTime * 0.5f;
			this.popLable1.alpha = alpha;
			uirect.alpha = alpha;
			if (this.popLable1.alpha < 0f)
			{
				UIRect uirect2 = this.popLableValue1;
				alpha = 0f;
				this.popLable1.alpha = alpha;
				uirect2.alpha = alpha;
			}
		}
		if (this.popLableAdd.alpha > 0f)
		{
			this.popLableAdd.alpha -= Time.deltaTime * 0.5f;
			if (this.popLableAdd.alpha < 0f)
			{
				this.popLableAdd.alpha = 0f;
			}
		}
		this.UpdateStarProc();
		if (this.isPlayingDropStarAnimation)
		{
			this.dropStarAnimationDelta += Time.deltaTime;
			if (this.dropStarAnimationDelta > 1f)
			{
				this.isPlayingDropStarAnimation = false;
				this.dropStarAnimationDelta = 0f;
			}
		}
	}

	private void TimerProc()
	{
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		this.populationValueLabel.text = string.Format("{0} / {1}", team.current, team.currentMax);
		this.popLableValue1.text = this.populationValueLabel.text;
		this.UpdateSurvivalAim();
		base.Invoke("TimerProc", 0.5f);
	}

	private void UpdateStarProc()
	{
		if (this.levelConfig == null)
		{
			return;
		}
		if (this.isRetrying)
		{
			return;
		}
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		if (team != null)
		{
			string starType = this.levelConfig.starType;
			if (starType != null)
			{
				if (!(starType == "lost"))
				{
					if (!(starType == "time"))
					{
						if (starType == "kill")
						{
							float num = (float)team.hitships;
						}
					}
					else
					{
						float num = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
					}
				}
				else
				{
					float num = (float)team.destory;
				}
			}
		}
		if (!this.isPlayingDropStarAnimation && this.dropStarAnimationQueue.Count > 0)
		{
			if (this.levelConfig.starType == "kill")
			{
				this.PlayAnimation(this.dropStarAnimationQueue.Dequeue(), -1f);
			}
			else
			{
				this.PlayAnimation(this.dropStarAnimationQueue.Dequeue(), 1f);
			}
			this.isPlayingDropStarAnimation = true;
		}
	}

	private void OnProgressDragStart(GameObject go)
	{
		this.totalDrag = Vector2.zero;
	}

	private void OnProgressDrag(GameObject go, Vector2 delta)
	{
		this.OnSelectProcessAram(null);
	}

	public void OnCloseClick()
	{
	}

	private void ResetNumberSlider()
	{
		if (this.Processdown.activeSelf)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
			if (this.unitdown[this.unitdown.Length - 1].GetComponent<UIEventListener>().onClick != null)
			{
				this.OnSelectNumber(this.unitdown[this.unitdown.Length - 1]);
			}
		}
		if (this.Processleft.activeSelf)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
			if (this.unitleft[this.unitdown.Length - 1].GetComponent<UIEventListener>().onClick != null)
			{
				this.OnSelectNumber(this.unitleft[this.unitdown.Length - 1]);
			}
		}
		if (this.Processright.activeSelf)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
			if (this.unitright[this.unitdown.Length - 1].GetComponent<UIEventListener>().onClick != null)
			{
				this.OnSelectNumber(this.unitright[this.unitdown.Length - 1]);
			}
		}
	}

	private void SetPercent(bool bfouce = false)
	{
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption != 1)
		{
			float num = this.lineTotalLength * this.percent;
			Vector3 localPosition = this.percentZeroPos;
			localPosition.x += num;
			int width = (int)(this.lineTotalLength * this.percent + 2f - this.percentPicWidth / 2f);
			this.percentleft.width = width;
			width = (int)(this.lineTotalLength * (1f - this.percent) + 2f - this.percentPicWidth / 2f);
			this.percentright.width = width;
			this.percentleft.gameObject.SetActive(true);
			this.percentright.gameObject.SetActive(true);
			if (this.percent == 0f)
			{
				this.percentleft.gameObject.SetActive(false);
			}
			else if (this.percent == 1f)
			{
				this.percentright.gameObject.SetActive(false);
			}
			this.percetPic.transform.localPosition = localPosition;
			this.percentLabel.text = string.Format("{0}%", Mathf.RoundToInt(this.percent * 100f));
			if (this.percent != 1f)
			{
				GuideManager.TriggerGuidecompleted(GuildEndEvent.touch);
			}
			Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderRate = this.percent;
		}
	}

	private void InitFightProgressOption()
	{
		this.percentGo.SetActive(false);
		if (!this.IsShowProcessOp())
		{
			return;
		}
		int fightOption = Solarmax.Singleton<LocalSettingStorage>.Get().fightOption;
		if (fightOption == 1)
		{
			this.ShowNewProgress1(false, null);
		}
		else
		{
			UIEventListener component = this.percetPic.gameObject.GetComponent<UIEventListener>();
			component.onDrag = new UIEventListener.VectorDelegate(this.OnProgressDrag);
			component.onDragStart = new UIEventListener.VoidDelegate(this.OnProgressDragStart);
			this.lineTotalLength = 1200f;
			this.percentZeroPos = new Vector3(this.percetPic.transform.localPosition.x - this.lineTotalLength / 2f, this.percetPic.transform.localPosition.y, 0f);
			UIAnchor component2 = this.percentGo.GetComponent<UIAnchor>();
			component2.enabled = true;
			if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
			{
				component2.side = UIAnchor.Side.Bottom;
				component2.relativeOffset.x = 0f;
				component2.relativeOffset.y = 0.05f;
			}
			else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 1)
			{
				this.percentGo.transform.eulerAngles = new Vector3(0f, 0f, 90f);
				this.percentGo.transform.localScale = new Vector3(0.56f, 0.8f, 0f);
				component2.side = UIAnchor.Side.Left;
				component2.relativeOffset.x = 0.06f;
				component2.relativeOffset.y = -0.12f;
				this.percentLabel.transform.eulerAngles = new Vector3(0f, 0f, -90f);
				this.percentLabel.transform.localPosition = new Vector3(this.percentLabel.transform.localPosition.x, -this.percentLabel.transform.localPosition.y, this.percentLabel.transform.localPosition.z);
				this.ProcessLine.transform.localPosition = new Vector3(this.ProcessLine.transform.localPosition.x, -18f, this.ProcessLine.transform.localPosition.z);
			}
			else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 2)
			{
				this.percentGo.transform.eulerAngles = new Vector3(0f, 0f, 90f);
				this.percentGo.transform.localScale = new Vector3(0.56f, 0.8f, 0f);
				component2.side = UIAnchor.Side.Right;
				component2.relativeOffset.x = -0.06f;
				component2.relativeOffset.y = -0.04f;
			}
			this.percentGo.SetActive(true);
		}
	}

	private void ShowNewProgress1(bool show, Node node = null)
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
		{
			this.Processdown.SetActive(true);
			this.Processleft.SetActive(false);
			this.Processright.SetActive(false);
			this.SetSelectEffect(this.unitdown[7]);
		}
		else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 1)
		{
			this.Processdown.SetActive(false);
			this.Processleft.SetActive(true);
			this.Processright.SetActive(false);
			this.SetSelectEffect(this.unitleft[7]);
		}
		else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 2)
		{
			this.Processdown.SetActive(false);
			this.Processleft.SetActive(false);
			this.Processright.SetActive(true);
			this.SetSelectEffect(this.unitright[7]);
		}
	}

	private bool IsShowProcessOp()
	{
		string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		return (!matchId.Equals("1001010") && !matchId.Equals("1001020") && !matchId.Equals("1001030")) || Solarmax.Singleton<LevelDataHandler>.Get().IsUnLock(matchId);
	}

	private bool IsShowButton()
	{
		string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		return !matchId.Equals("1001010") || Solarmax.Singleton<LevelDataHandler>.Get().IsUnLock(matchId);
	}

	public void GiveUpOnClicked()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<UISystem>.Get().OnEventHandler(33, "CommonDialogWindow", new object[]
		{
			2,
			LanguageDataProvider.GetValue(1104),
			new EventDelegate(new EventDelegate.Callback(this.GiveUp)),
			new EventDelegate(new EventDelegate.Callback(this.PauseCancel))
		});
	}

	private void GiveUp()
	{
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.2f;
		tweenAlpha.SetOnFinished(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.PlayerGiveUp();
		});
		tweenAlpha.Play(true);
	}

	public void PauseCancel()
	{
		if (!this.pausePage.active && Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
		}
	}

	public void OnPauseClick()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
			this.pausePage.SetActive(false);
			this.pauseBtn.normalSprite = "level_pause";
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);
			this.pausePage.SetActive(true);
			this.pauseBtn.normalSprite = "button_sudix1";
		}
	}

	public void OnRetryClick()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null && (Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value).TotalSeconds < 5.0)
		{
			return;
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		this.isRetrying = true;
		Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.25f);
		Solarmax.Singleton<UISystem>.Get().FadeOutBattle(false, new EventDelegate(delegate()
		{
			this.ReStartLevelBattle();
			this.isRetrying = false;
		}));
		this.Start();
		this.ResetNumberSlider();
		this.playSpeed = 2;
		this.SetPlaySpeedBtnStatus();
		this.PlayAnimation("Stop", 1f);
		this.pausePage.SetActive(false);
		this.pauseBtn.normalSprite = "level_pause";
		this.isPlayingDropStarAnimation = false;
		this.dropStarAnimationDelta = 0f;
		this.dropStarAnimationQueue.Clear();
		this.SetLevelStarsAndDesc();
	}

	public void OnStar1Click()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleWindow_off OnStar1Click", new object[0]);
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(currentGroupID))
		{
			Solarmax.Singleton<AchievementManager>.Get().completeList.Clear();
			for (int i = 0; i < 1; i++)
			{
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] != AchievementType.Ads)
				{
					if (!Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success)
					{
						Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success = true;
						Solarmax.Singleton<AchievementManager>.Get().completeList.Add(Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i]);
					}
					if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] == AchievementType.PassDiffcult)
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievement(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					else
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievementWithoutReward(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAchievement();
				}
			}
		}
		Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM = Solarmax.Singleton<BattleSystem>.Get().battleData.currentTeam;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM).destory = this.levelConfig.star2Dead + 1;
		Solarmax.Singleton<BattleSystem>.Get().singleBattleController.QuitBattle(true);
	}

	public void OnStar2Click()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleWindow_off OnStar2Click", new object[0]);
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(currentGroupID))
		{
			Solarmax.Singleton<AchievementManager>.Get().completeList.Clear();
			for (int i = 0; i < Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements.Count; i++)
			{
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] != AchievementType.Ads && Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].diffcult < AchievementDifficult.Hell)
				{
					if (!Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success)
					{
						Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success = true;
						Solarmax.Singleton<AchievementManager>.Get().completeList.Add(Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i]);
					}
					if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] == AchievementType.PassDiffcult)
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievement(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					else
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievementWithoutReward(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAchievement();
				}
			}
		}
		Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM = Solarmax.Singleton<BattleSystem>.Get().battleData.currentTeam;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM).destory = this.levelConfig.star3Dead + 1;
		Solarmax.Singleton<BattleSystem>.Get().singleBattleController.QuitBattle(true);
	}

	public void OnStar3Click()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleWindow_off OnStar3Click", new object[0]);
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(currentGroupID))
		{
			Solarmax.Singleton<AchievementManager>.Get().completeList.Clear();
			for (int i = 0; i < Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements.Count; i++)
			{
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] != AchievementType.Ads && Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].diffcult < (AchievementDifficult)3)
				{
					if (!Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success)
					{
						Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success = true;
						Solarmax.Singleton<AchievementManager>.Get().completeList.Add(Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i]);
					}
					if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] == AchievementType.PassDiffcult)
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievement(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					else
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievementWithoutReward(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAchievement();
				}
			}
		}
		Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM = Solarmax.Singleton<BattleSystem>.Get().battleData.currentTeam;
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM);
		if (this.levelConfig.maxStar == 4)
		{
			team.destory = this.levelConfig.star4Dead + 1;
		}
		else
		{
			team.destory = 0;
		}
		Solarmax.Singleton<BattleSystem>.Get().singleBattleController.QuitBattle(true);
	}

	public void OnStar4Click()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleWindow_off OnStar4Click", new object[0]);
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(currentGroupID))
		{
			Solarmax.Singleton<AchievementManager>.Get().completeList.Clear();
			for (int i = 0; i < Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements.Count; i++)
			{
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] != AchievementType.Ads)
				{
					if (!Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success)
					{
						Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].success = true;
						Solarmax.Singleton<AchievementManager>.Get().completeList.Add(Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i]);
					}
					if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].types[0] == AchievementType.PassDiffcult)
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievement(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					else
					{
						Solarmax.Singleton<AchievementModel>.Get().SendAchievementWithoutReward(currentGroupID, Solarmax.Singleton<AchievementModel>.Get().achievementGroups[currentGroupID].achievements[i].id);
					}
					Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAchievement();
				}
			}
		}
		Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM = Solarmax.Singleton<BattleSystem>.Get().battleData.currentTeam;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Get().battleData.winTEAM).destory = 0;
		Solarmax.Singleton<BattleSystem>.Get().singleBattleController.QuitBattle(true);
	}

	public void OnSpeedClick1()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		if (this.playSpeed == 1)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 0.5f;
		this.playSpeed = 1;
		this.SetPlaySpeedBtnStatus();
	}

	public void OnSpeedClick2()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		if (this.playSpeed == 2)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 1f;
		this.playSpeed = 2;
		this.SetPlaySpeedBtnStatus();
	}

	public void OnSpeedClick3()
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		if (this.playSpeed == 3)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 2f;
		this.playSpeed = 3;
		this.SetPlaySpeedBtnStatus();
	}

	private void SetPlaySpeedBtnStatus()
	{
		for (int i = 0; i < this.playSpeedButton.Length; i++)
		{
			this.playSpeedButton[i].isEnabled = (i != this.playSpeed - 1);
		}
	}

	private IEnumerator ActiveLastStar()
	{
		this.playStars[this.levelConfig.maxStar - 1].gameObject.SetActive(false);
		yield return null;
		this.playStars[this.levelConfig.maxStar - 1].gameObject.SetActive(true);
		yield break;
	}

	private void SetLevelStarsAndDesc()
	{
		if (this.levelConfig == null)
		{
			return;
		}
		this.unlockplayStars[0].gameObject.SetActive(false);
		this.unlockplayStars[1].gameObject.SetActive(false);
		this.unlockplayStars[2].gameObject.SetActive(false);
		this.unlockplayStars[3].gameObject.SetActive(false);
		this.playStars[0].gameObject.SetActive(false);
		this.playStars[1].gameObject.SetActive(false);
		this.playStars[2].gameObject.SetActive(false);
		this.playStars[3].gameObject.SetActive(false);
		string winType = this.levelConfig.winType;
		if (winType != null)
		{
			if (!(winType == "killall"))
			{
				if (!(winType == "occupy"))
				{
					if (!(winType == "alive"))
					{
						if (winType == "killnum")
						{
							this.aims.text = string.Format(LanguageDataProvider.GetValue(1131), this.levelConfig.winTypeParam1);
						}
					}
					else
					{
						this.aims.text = string.Format(LanguageDataProvider.GetValue(1130), this.levelConfig.winTypeParam1);
					}
				}
				else
				{
					this.aims.text = LanguageDataProvider.GetValue(1129);
				}
			}
			else
			{
				this.aims.text = LanguageDataProvider.GetValue(1128);
				if (this.levelConfig.id == "1001010")
				{
					this.aims.text = LanguageDataProvider.GetValue(1160);
				}
			}
		}
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(currentGroupID))
		{
			if (Solarmax.Singleton<AchievementManager>.Get().difficult == AchievementDifficult.Simple)
			{
				this.aims.text = string.Format("[{0}]{1}{2}", LanguageDataProvider.GetValue(2104), LanguageDataProvider.GetValue(2186), this.aims.text);
			}
			else if (Solarmax.Singleton<AchievementManager>.Get().difficult == AchievementDifficult.Hard)
			{
				this.aims.text = string.Format("[{0}]{1}{2}", LanguageDataProvider.GetValue(2105), LanguageDataProvider.GetValue(2186), this.aims.text);
			}
			else if (Solarmax.Singleton<AchievementManager>.Get().difficult == AchievementDifficult.Hell)
			{
				this.aims.text = string.Format("[{0}]{1}{2}", LanguageDataProvider.GetValue(2106), LanguageDataProvider.GetValue(2186), this.aims.text);
			}
		}
		this.UpdateSurvivalPrefix();
	}

	private void UpdateSurvivalPrefix()
	{
		this.survivalAimsPrefix = string.Empty;
		string winType = Solarmax.Singleton<BattleSystem>.Instance.battleData.winType;
		if (winType != "alive" && winType != "surival" && winType != "survival")
		{
			return;
		}
		string winTypeParam = Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1;
		if (string.IsNullOrEmpty(winTypeParam) && this.levelConfig != null)
		{
			winTypeParam = this.levelConfig.winTypeParam1;
		}
		if (string.IsNullOrEmpty(winTypeParam))
		{
			return;
		}
		string value = string.Format(LanguageDataProvider.GetValue(1130), winTypeParam);
		if (this.aims == null || string.IsNullOrEmpty(this.aims.text))
		{
			return;
		}
		int num = this.aims.text.IndexOf(value, StringComparison.Ordinal);
		if (num > 0)
		{
			this.survivalAimsPrefix = this.aims.text.Substring(0, num);
		}
	}

	private void UpdateSurvivalAim()
	{
		if (this.aims == null)
		{
			return;
		}
		string winType = Solarmax.Singleton<BattleSystem>.Instance.battleData.winType;
		if (winType != "alive" && winType != "surival" && winType != "survival")
		{
			return;
		}
		string winTypeParam = Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1;
		if (string.IsNullOrEmpty(winTypeParam) && this.levelConfig != null)
		{
			winTypeParam = this.levelConfig.winTypeParam1;
		}
		float num;
		if (!float.TryParse(winTypeParam, out num) || num <= 0f)
		{
			return;
		}
		int num2 = Mathf.FloorToInt(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
		int num3 = Mathf.CeilToInt(num);
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 > num3)
		{
			num2 = num3;
		}
		if (this.survivalAimsPrefix == null)
		{
			this.survivalAimsPrefix = string.Empty;
		}
		this.aims.text = this.survivalAimsPrefix + string.Format(LanguageDataProvider.GetValue(9001000), num2, num3);
	}

	private void SetDescText(int star, float value)
	{
		string format = string.Empty;
		if (this.levelConfig.starType == "kill")
		{
			format = LanguageDataProvider.GetValue(1132);
		}
		else if (this.levelConfig.starType == "time")
		{
			format = LanguageDataProvider.GetValue(1133);
		}
		else if (this.levelConfig.starType == "lost")
		{
			format = LanguageDataProvider.GetValue(1110);
		}
		if (this.levelConfig.starType == "kill")
		{
			if (this.levelConfig.maxStar == 4)
			{
				switch (star)
				{
				case 1:
					this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star2Dead, value));
					break;
				case 2:
					this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star3Dead, value));
					break;
				case 3:
					this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star4Dead, value));
					break;
				case 4:
					this.desc.text = string.Format(format, string.Format(" ({0})", value));
					break;
				}
			}
			else if (star != 1)
			{
				if (star != 2)
				{
					if (star == 3)
					{
						this.desc.text = string.Format(format, string.Format(" ({0})", value));
					}
				}
				else
				{
					this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star3Dead, value));
				}
			}
			else
			{
				this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star2Dead, value));
			}
		}
		else
		{
			switch (star)
			{
			case 1:
				this.desc.text = string.Format(format, string.Format(" ({0})", value));
				break;
			case 2:
				this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star2Dead, value));
				break;
			case 3:
				this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star3Dead, value));
				break;
			case 4:
				this.desc.text = string.Format(format, string.Format(" ≤ {0} ({1})", this.levelConfig.star4Dead, value));
				break;
			}
		}
	}

	private void PlayAnimation(string strAni, float speed = 1f)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	private void ReStartLevelBattle()
	{
		if (this.difficultyLevel == 0)
		{
			this.difficultyLevel = 1;
		}
		GameType gameType = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType;
		string id = Solarmax.Singleton<LevelDataHandler>.Get().currentLevel.id;
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
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(id, GameType.SingleLevel, true);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = gameType;
	}

	public void OnStartSingleBattle()
	{
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 1f;
		tweenAlpha.duration = 0.5f;
		tweenAlpha.onFinished.Clear();
		tweenAlpha.SetOnFinished(delegate()
		{
			this.StartSingleBattle();
		});
		tweenAlpha.Play(true);
	}

	public void StartSingleBattle()
	{
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
		}));
	}

	public void OnSelectNumber(GameObject go)
	{
		UILabel component = go.transform.Find("num").GetComponent<UILabel>();
		if (component != null)
		{
			string text = component.text;
			if (text.Equals("ALL"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
			}
			else if (text.Equals("1"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 1f;
			}
			else if (text.Equals("5"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 5f;
			}
			else if (text.Equals("10"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 10f;
			}
			else if (text.Equals("30"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 30f;
			}
			else if (text.Equals("50"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 50f;
			}
			else if (text.Equals("80"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 80f;
			}
			else if (text.Equals("100"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 100f;
			}
			this.SetSelectEffect(go);
		}
	}

	private void SetSelectEffect(GameObject go)
	{
		GameObject gameObject = go.transform.Find("on").gameObject;
		UILabel component = go.transform.Find("num").GetComponent<UILabel>();
		if (gameObject == null)
		{
			return;
		}
		if (this.selectOn != null)
		{
			this.selectOn.SetActive(false);
			this.selectLabel.color = Vector4.one;
		}
		this.selectOn = gameObject;
		this.selectLabel = component;
		this.selectOn.SetActive(true);
		component.color = new Vector4(0f, 0f, 0f, 0.8f);
	}

	public UIButton pauseBtn;

	public GameObject pausePage;

	public UILabel populationLabel;

	public UILabel populationValueLabel;

	public GameObject percentGo;

	public GameObject ProcessAram;

	public GameObject ProcessLine;

	public UISprite percentleft;

	public UISprite percentright;

	public UISprite percetPic;

	public UILabel percentLabel;

	public GameObject Processdown;

	public GameObject[] unitdown;

	public GameObject Processleft;

	public GameObject[] unitleft;

	public GameObject Processright;

	public GameObject[] unitright;

	private GameObject selectOn;

	public GameObject star1Button;

	public GameObject star2Button;

	public GameObject star3Button;

	public GameObject star4Button;

	public GameObject[] buttonList;

	private float sensitive = 1f;

	private string newProgressTag = string.Empty;

	private float lineTotalLength;

	private Vector3 percentZeroPos = Vector3.zero;

	private float percent;

	private float percentPicWidth;

	public UILabel popLable1;

	public UILabel popLableValue1;

	public UILabel popLableAdd;

	private int playSpeed = 2;

	public UIButton[] playSpeedButton;

	public UISprite[] playStars;

	public UISprite[] unlockplayStars;

	public UILabel desc;

	public UILabel aims;

	private string survivalAimsPrefix;

	public GameObject achievementView;

	private LevelConfig levelConfig;

	private ChapterConfig chpaterConfig;

	private int curStar;

	private bool isRetrying;

	public UIPlayAnimation aniPlayer;

	private float dropStarAnimationDelta;

	private bool isPlayingDropStarAnimation;

	private Queue<string> dropStarAnimationQueue = new Queue<string>();

	private UILabel selectLabel;

	private Vector2 totalDrag;

	private DateTime? searchFriendTime;

	private int difficultyLevel;
}
