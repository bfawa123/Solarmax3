using System;
using System.Collections.Generic;
using MiGameSDK;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ResultWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFinished);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		base.RegisterEvent(EventId.OnLadderScoreChange);
		base.RegisterEvent(EventId.OnDoubleAdClicked);
		base.RegisterEvent(EventId.OnDoubleAdCanceled);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<EffectManager>.Instance.Destroy();
		this.IsShowMoneyChange = false;
		this.mapId = Solarmax.Singleton<LocalPlayer>.Get().battleMap;
		for (int i = 0; i < this.posRoots.Length; i++)
		{
		}
		this.SetPage();
		this.PlayAnimation("ResultWindow_h2");
		Solarmax.Singleton<BattleSystem>.Instance.battleData.root.SetActive(false);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
		{
			this.scoreCup.gameObject.SetActive(false);
			this.scoreChange.gameObject.SetActive(false);
			this.result.text = LanguageDataProvider.GetValue(106);
			this.leftTable.Reposition();
		}
		this.giveupAD = false;
	}

	public override void OnHide()
	{
		Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.root.SetActive(true);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFinished)
		{
			SCFinishBattle scfinishBattle = args[0] as SCFinishBattle;
			if (scfinishBattle == null)
			{
				return;
			}
			if (scfinishBattle.chest != null)
			{
			}
		}
		else if (eventId == EventId.OnFriendFollowResult)
		{
			int key = (int)args[0];
			bool follow = (bool)args[1];
			ErrCode errCode = (ErrCode)args[2];
			if (errCode == ErrCode.EC_Ok)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(815), 1f);
				if (this.dicResultBehavior.ContainsKey(key))
				{
					this.dicResultBehavior[key].OnAddFriendSuccess(follow);
				}
			}
		}
		else if (eventId == EventId.OnLadderScoreChange)
		{
			this.showAdBtn.GetComponent<UIButton>().enabled = false;
			int num = this.selfScoreChange / 2;
			this.score.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.score.ToString();
			this.scoreChange.text = string.Format("{0}[-][FF0000]{1}[-]", LanguageDataProvider.GetValue(2214), num);
			if (this.selfInfo != null)
			{
				this.selfInfo.ChangeScore();
			}
		}
		else if (eventId == EventId.OnDoubleAdClicked)
		{
			if (this.IsShowMoneyChange)
			{
				this.OnShowAdMoney();
			}
			else
			{
				this.OnShowAd();
			}
		}
		else if (eventId == EventId.OnDoubleAdCanceled)
		{
			this.OnGoHomeClick();
		}
	}

	public void OnPlayerFistAniamionEnd()
	{
		base.Invoke("PlayerFistAniamion", 0.5f);
	}

	private void PlayerFistAniamion()
	{
		this.PlayMvpEffect();
	}

	private void SetPage()
	{
		string lost = string.Empty;
		MapConfig data = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		if (data == null)
		{
			data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.mapId);
		}
		if (data == null)
		{
			return;
		}
		List<Team> list = new List<Team>();
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		Team team2 = null;
		bool flag = false;
		this.CalculateWinTeam(data, out team2, out flag);
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		bool flagFivePlayerCoop = battleData != null && battleData.matchPlayerNum == 5 && (battleData.battleSubType == CooperationType.CT_2vPC || battleData.battleSubType == CooperationType.CT_3vPC || battleData.battleSubType == CooperationType.CT_4vPC);
		for (int i = 0; i < data.player_count; i++)
		{
			int num = i + 1;
			if (flagFivePlayerCoop && num == 5)
			{
				continue;
			}
			TEAM team3 = (TEAM)num;
			Team team4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team3);
			if (team4 != team2 && team2.IsFriend(team4.groupID))
			{
				list.Add(team4);
			}
			else if (team4 == team2)
			{
				list.Add(team4);
			}
		}
		if (list.Count > 0)
		{
			list.Sort(delegate(Team arg0, Team arg1)
			{
				int num4 = arg0.resultOrder.CompareTo(arg1.resultOrder);
				if (num4 == 0)
				{
					num4 = arg0.scoreMod.CompareTo(arg1.scoreMod);
				}
				if (num4 == 0)
				{
					num4 = arg0.destory.CompareTo(arg1.destory);
				}
				return -num4;
			});
		}
		List<Team> list2 = new List<Team>();
		for (int j = 0; j < data.player_count; j++)
		{
			int num2 = j + 1;
			if (flagFivePlayerCoop && num2 == 5)
			{
				continue;
			}
			TEAM team5 = (TEAM)num2;
			Team team6 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team5);
			if (team6 != team2 && !team2.IsFriend(team6.groupID) && !list.Contains(team6))
			{
				list2.Add(team6);
			}
		}
		list2.Sort(delegate(Team arg0, Team arg1)
		{
			int num4 = arg0.resultOrder.CompareTo(arg1.resultOrder);
			if (num4 == 0)
			{
				num4 = arg0.scoreMod.CompareTo(arg1.scoreMod);
			}
			if (num4 == 0)
			{
				num4 = arg0.hitships.CompareTo(arg1.hitships);
			}
			return -num4;
		});
		list.AddRange(list2);
		if (list.Contains(team))
		{
			if (flag)
			{
				this.result.text = LanguageDataProvider.GetValue(110);
			}
			else if (team2.IsFriend(team.groupID))
			{
				this.result.text = LanguageDataProvider.GetValue(100);
			}
			else
			{
				this.result.text = LanguageDataProvider.GetValue(101);
			}
			if (team.scoreMod > 0)
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("onPVPvictory");
			}
			else
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("onPVPdefeated");
			}
		}
		else
		{
			this.result.text = LanguageDataProvider.GetValue(106);
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PVP)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
			{
				bool flag2 = Solarmax.Singleton<LocalPlayer>.Get().IsInSeason(Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType);
				if (flag2)
				{
					Solarmax.Singleton<LocalPlayer>.Get().playerData.battle_count++;
				}
				if (team2.IsFriend(team.groupID))
				{
					if (flag2)
					{
						Solarmax.Singleton<LocalPlayer>.Get().playerData.mvp_count++;
					}
					this.result.text = LanguageDataProvider.GetValue(100);
				}
				else
				{
					this.result.text = LanguageDataProvider.GetValue(101);
				}
			}
			else if (team.resultRank >= 0)
			{
				string[] array = LanguageDataProvider.GetValue(107).Split(new char[]
				{
					','
				});
				int num;
				if (data.player_count > 4)
				{
					num = 4 - team.resultRank - 1;
				}
				else
				{
					num = data.player_count - team.resultRank - 1;
				}
				this.result.text = LanguageDataProvider.Format(108, new object[]
				{
					array[num]
				});
				bool flag3 = Solarmax.Singleton<LocalPlayer>.Get().IsInSeason(Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType);
				if (flag3)
				{
					Solarmax.Singleton<LocalPlayer>.Get().playerData.battle_count++;
				}
				if (num == 0 && flag3)
				{
					Solarmax.Singleton<LocalPlayer>.Get().playerData.mvp_count++;
				}
			}
			else
			{
				this.result.text = LanguageDataProvider.Format(106, new object[0]);
			}
			lost = this.result.text;
		}
		else
		{
			if (team.resultEndtype == EndType.ET_Win)
			{
				this.result.text = LanguageDataProvider.Format(109, new object[]
				{
					team.leagueMvp
				});
			}
			else if (team.resultRank >= 0)
			{
				string[] array2 = LanguageDataProvider.GetValue(107).Split(new char[]
				{
					','
				});
				int num2 = data.player_count - team.resultRank - 1;
				this.result.text = LanguageDataProvider.Format(108, new object[]
				{
					array2[num2]
				});
			}
			else
			{
				this.result.text = LanguageDataProvider.Format(106, new object[0]);
			}
			lost = this.result.text;
		}
		int num3 = 0;
		this.dicResultBehavior.Clear();
		for (int k = 0; k < 4; k++)
		{
			if (k < list.Count)
			{
				Team team7 = list[k];
				if (team7.playerData.userId != -1)
				{
					bool isSeason = Solarmax.Singleton<LocalPlayer>.Get().IsInSeason(Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType) && Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType != MatchType.MT_Room;
					this.SetPosInfo(num3, team7, team7.scoreMod, team7.hitships, team2, isSeason, false);
					if (team7.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
					{
						this.selfScoreChange = team7.scoreMod;
						this.selfMoneyChange = team7.rewardMoney;
						this.selfMoneyMutily = team7.rewardMuitly;
					}
					num3++;
				}
			}
			else
			{
				this.SetPosInfo(k, null, 0, 0, null, false, true);
			}
		}
		this.IsShowMoneyChange = false;
		this.showAdBtn.SetActive(false);
		this.showAdMoney.SetActive(false);
		bool flag4 = Solarmax.Singleton<LocalPlayer>.Get().IsInSeason(Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType) && Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType != MatchType.MT_Room;
		if (flag4)
		{
			this.scoreCup.SetActive(true);
			this.scoreChange.gameObject.SetActive(true);
			Solarmax.Singleton<LocalPlayer>.Get().playerData.score = ((Solarmax.Singleton<LocalPlayer>.Get().playerData.score + this.selfScoreChange <= 0) ? 0 : (Solarmax.Singleton<LocalPlayer>.Get().playerData.score + this.selfScoreChange));
			this.score.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.score.ToString();
			this.showAdBtn.GetComponent<UIButton>().enabled = false;
			if (this.selfScoreChange < 0 && !Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
			{
				if (Solarmax.Singleton<LocalPlayer>.Get().playerData.score > 0)
				{
					this.showAdBtn.SetActive(true);
					this.showAdBtn.GetComponent<UIButton>().enabled = true;
					this.originScoreLable.text = this.selfScoreChange.ToString();
					this.adScoreLable.text = (this.selfScoreChange / 2).ToString();
				}
				this.scoreChange.text = string.Concat(new object[]
				{
					LanguageDataProvider.GetValue(2214),
					"[-][FF0000]",
					this.selfScoreChange,
					"[-]"
				});
			}
			else if (this.selfScoreChange > 0)
			{
				this.scoreChange.text = string.Concat(new object[]
				{
					LanguageDataProvider.GetValue(2214),
					"[-][00FF00]+",
					this.selfScoreChange,
					"[-]"
				});
			}
			else
			{
				this.scoreChange.text = LanguageDataProvider.GetValue(2214) + "+" + this.selfScoreChange;
			}
			if (this.selfMoneyChange > 0 && this.selfMoneyMutily >= 2)
			{
				this.IsShowMoneyChange = true;
				this.showAdMoney.SetActive(true);
				this.originMoney.text = LanguageDataProvider.GetValue(2263) + "X" + this.selfMoneyMutily;
			}
		}
		else
		{
			this.showAdBtn.SetActive(false);
			this.scoreCup.SetActive(false);
			this.scoreChange.gameObject.SetActive(false);
		}
		if (team2.team != TEAM.Neutral)
		{
			this.IsNeedPlayMvpEffect = true;
		}
		bool flag5 = team2 != null && team != null && team2.IsFriend(team.groupID);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
		{
			if (flag5)
			{
				string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
				if (Solarmax.Singleton<LevelDataHandler>.Get().IsNeedGiveRaward(matchId))
				{
					if (Solarmax.Singleton<LevelDataHandler>.Get().IsNeedSend(1, 1))
					{
						Solarmax.Singleton<NetSystem>.Instance.helper.SetLevelStar(Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id, matchId, 1, 1);
					}
					Solarmax.Singleton<LevelDataHandler>.Get().SetLevelStarToLocalStorage(1, 1);
					Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalAccount(true);
				}
				this.result.text = LanguageDataProvider.GetValue(100);
			}
			else
			{
				this.result.text = LanguageDataProvider.GetValue(101);
			}
		}
		float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
		string matchType = "1";
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType != GameType.PVP || Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType != GameType.League)
		{
			matchType = "0";
		}
		MonoSingleton<FlurryAnalytis>.Instance.FlurryPVPBattleEndEvent(matchType, this.mapId, team.scoreMod.ToString(), team.hitships.ToString(), team.destory.ToString(), battleTime.ToString());
		AppsFlyerTool.FlyerPVPBattleEndEvent(lost);
		MiGameAnalytics.MiAnalyticsPVPBattleEndEvent(matchType, this.mapId, team.scoreMod.ToString(), team.hitships.ToString(), team.destory.ToString(), battleTime.ToString());
		switch (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType)
		{
		case CooperationType.CT_1v1:
			this.image.spriteName = this.bigMarkNames[0];
			break;
		case CooperationType.CT_2v2:
			this.image.spriteName = this.bigMarkNames[1];
			break;
		case CooperationType.CT_1v1v1:
			this.image.spriteName = this.bigMarkNames[2];
			break;
		case CooperationType.CT_1v1v1v1:
			this.image.spriteName = this.bigMarkNames[3];
			break;
		default:
			this.image.spriteName = this.bigMarkNames[4];
			break;
		}
	}

	private void CalculateWinTeam(MapConfig map, out Team win, out bool timeout)
	{
		win = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(TEAM.Neutral);
		timeout = false;
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		bool flagFivePlayerCoop = battleData != null && battleData.matchPlayerNum == 5 && (battleData.battleSubType == CooperationType.CT_2vPC || battleData.battleSubType == CooperationType.CT_3vPC || battleData.battleSubType == CooperationType.CT_4vPC);
		for (int i = 1; i < LocalPlayer.MaxTeamNum; i++)
		{
			if (flagFivePlayerCoop && i == 5)
			{
				continue;
			}
			TEAM team = (TEAM)i;
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
			if (team2.resultEndtype == EndType.ET_Win)
			{
				win = team2;
			}
			else if (team2.resultEndtype == EndType.ET_Timeout)
			{
				timeout = true;
			}
		}
	}

	private GameObject GetPosRoot()
	{
		MapConfig data = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable;
		if (data == null)
		{
			data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(this.mapId);
		}
		GameObject gameObject;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight)
		{
			gameObject = this.posRoots[3];
		}
		else
		{
			gameObject = this.posRoots[data.player_count - 2];
		}
		return gameObject;
	}

	private void SetPosInfo(int pos, Team team, int score, int destroy, Team winTeam, bool isSeason, bool defaultTemplate = false)
	{
		if (pos >= this.posRoots.Length)
		{
			return;
		}
		if (!defaultTemplate)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.cellTemplate);
			gameObject.SetActive(true);
			ResultPlayerInfo component = gameObject.GetComponent<ResultPlayerInfo>();
			component.Init(team, team.playerData.icon, team.playerData.name, score, destroy.ToString(), team.rewardMoney, team.color, pos, isSeason);
			this.grid.Reposition();
			this.dicResultBehavior.Add(team.playerData.userId, component);
			if (team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				this.selfInfo = component;
			}
		}
		else
		{
			GameObject gameObject2 = this.grid.gameObject.AddChild(this.cellTemplate);
			gameObject2.SetActive(true);
			ResultPlayerInfo component2 = gameObject2.GetComponent<ResultPlayerInfo>();
			component2.FakeInit();
			this.grid.Reposition();
		}
	}

	private void PlayAnimation(string strAni)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	private void PlayMvpEffect()
	{
		if (!this.IsNeedPlayMvpEffect)
		{
			return;
		}
		this.PlayAnimation("ResultWindow_h2MVP");
		this.IsNeedPlayMvpEffect = false;
	}

	public void OnGoHomeClick()
	{
		if (this.showAdBtn.activeSelf && !this.giveupAD)
		{
			this.giveupAD = true;
			Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
			{
				RewardTipsWindow.ViewType.Integral
			}));
			return;
		}
		if (this.showAdMoney.activeSelf && !this.giveupAD)
		{
			this.giveupAD = true;
			Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
			{
				RewardTipsWindow.ViewType.Pvp,
				this.selfMoneyMutily
			}));
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		if (!string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow))
		{
			Solarmax.Singleton<BattleSystem>.Instance.Reset();
			Solarmax.Singleton<UISystem>.Get().ShowWindow(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow);
			return;
		}
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ReplayWindow");
		}
		else
		{
			bool flag = false;
			string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
			string text = string.Empty;
			if (!string.IsNullOrEmpty(matchId))
			{
				LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchId);
				if (data != null && !string.IsNullOrEmpty(data.chapter))
				{
					flag = Solarmax.Singleton<LevelDataHandler>.Instance.BeUnlockChapter(data.chapter);
					text = data.chapter;
				}
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
			{
				CooperationType battleSubType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
				int coopPlayerNum = 4;
				if (battleSubType == CooperationType.CT_2vPC)
				{
					coopPlayerNum = 2;
				}
				else if (battleSubType == CooperationType.CT_3vPC)
				{
					coopPlayerNum = 3;
				}
				Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(coopPlayerNum);
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationLevelWindow");
				if (!string.IsNullOrEmpty(text))
				{
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
					{
						1,
						text
					});
				}
			}
			else
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType == MatchType.MT_Room)
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
				}
				else
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("PvPRoomWindow");
				}
			}
		}
		if (this.IsShowMoneyChange)
		{
			Solarmax.Singleton<NetSystem>.Get().helper.ChangePvpReward(1);
		}
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.battleData.resumingFrame = -1;
	}

	public void OnShowAd()
	{
		this.giveupAD = true;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] args)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogAcclSroceDelLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.ChangeSeasonScore(this.selfScoreChange / 2 - this.selfScoreChange);
		});
	}

	public void OnShowAdMoney()
	{
		this.giveupAD = true;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] args)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogResultMoneyDelLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.ChangePvpReward(this.selfMoneyMutily);
			this.showAdMoney.GetComponent<UIButton>().enabled = false;
		});
	}

	public void OnShareClick()
	{
		Debug.LogWarning("You have click the share button!");
	}

	public void OnLeagueConfirmClick()
	{
	}

	public void OnAttentionClick(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		UILabel component = go.transform.Find("useid").GetComponent<UILabel>();
		if (component == null || string.IsNullOrEmpty(component.text))
		{
			return;
		}
		int userId = int.Parse(component.text);
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(userId, true);
	}

	public UILabel result;

	public GameObject infoTemplate;

	public GameObject[] posRoots;

	public UIPlayAnimation aniPlayer;

	public GameObject mvpSprite;

	public GameObject confirmBtn;

	public GameObject scoreCup;

	public UILabel score;

	public UILabel scoreChange;

	public UIGrid grid;

	public GameObject cellTemplate;

	public UITable leftTable;

	public UISprite image;

	public GameObject showAdBtn;

	public GameObject showAdMoney;

	public UILabel originScoreLable;

	public UILabel originMoney;

	public UILabel adScoreLable;

	private string mapId;

	private GameObject[] btnAttention;

	private string[] bigMarkNames = new string[]
	{
		"Signal1",
		"Signal2",
		"Signal3",
		"Signal4",
		"Signal6"
	};

	private Dictionary<int, ResultPlayerInfo> dicResultBehavior = new Dictionary<int, ResultPlayerInfo>();

	private bool IsNeedPlayMvpEffect;

	private ResultPlayerInfo selfInfo;

	private int selfScoreChange;

	private int selfMoneyChange;

	private int selfMoneyMutily = 1;

	private bool IsShowMoneyChange;

	private bool giveupAD;
}
