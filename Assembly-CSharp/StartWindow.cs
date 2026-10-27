using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class StartWindow : BaseWindow
{
	private void Awake()
	{
		UIEventListener uieventListener = this.globalMap;
		uieventListener.onDoubleClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onDoubleClick, new UIEventListener.VoidDelegate(this.OnSelectMapClick));
		this.csSignupScrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnChampionshipSignupScrollviewShowMore);
		this.csSignupLeagueList = new List<LeagueInfo>();
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.SyncUserData);
		base.RegisterEvent(EventId.OnGetLeagueInfoResult);
		base.RegisterEvent(EventId.OnLeagueListResult);
		base.RegisterEvent(EventId.OnLeagueSignUpResult);
		base.RegisterEvent(EventId.OnLeagueMatchResult);
		base.RegisterEvent(EventId.OnLeagueRankResult);
		base.RegisterEvent(EventId.OnLeagueRankBack);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.OnCombatClick();
		this.SetPlayerInfo();
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		this.PlayAnimation("StartWindow_in");
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.SyncUserData)
		{
			this.SetPlayerInfo();
		}
		else if (eventId == EventId.OnGetLeagueInfoResult)
		{
			this.championshipSignUpPage.SetActive(false);
			this.championshipPlayPage.SetActive(true);
			this.SetChampionshipPlayInfo((LeagueInfo)args[0], (MemberInfo)args[1]);
		}
		else if (eventId == EventId.OnLeagueListResult)
		{
			this.championshipSignUpPage.SetActive(true);
			this.championshipPlayPage.SetActive(false);
			this.SetChampionshipSignupInfo((int)args[0], (IList<LeagueInfo>)args[1]);
		}
		else if (eventId == EventId.OnLeagueSignUpResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			int num = (int)args[1];
			if (errCode == ErrCode.EC_Ok)
			{
				Transform transform = this.csSignupGrid.transform.Find(num.ToString());
				if (transform != null)
				{
					StartWindowChampionshipCell component = transform.gameObject.GetComponent<StartWindowChampionshipCell>();
					component.OnSignUpResult(errCode);
				}
			}
			else if (errCode == ErrCode.EC_LeagueIsFull)
			{
				Tips.Make(LanguageDataProvider.GetValue(210));
			}
			else if (errCode == ErrCode.EC_LeagueIn)
			{
				Tips.Make(LanguageDataProvider.GetValue(211));
			}
			else if (errCode == ErrCode.EC_LeagueNotExist)
			{
				Tips.Make(LanguageDataProvider.GetValue(212));
			}
			else if (errCode == ErrCode.EC_LeagueNotOpen)
			{
				Tips.Make(LanguageDataProvider.GetValue(213));
			}
			else
			{
				Tips.Make(LanguageDataProvider.GetValue(214) + errCode);
			}
		}
		else if (eventId == EventId.OnLeagueMatchResult)
		{
			ErrCode errCode2 = (ErrCode)args[0];
			int num2 = (int)args[1];
			if (errCode2 == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<UISystem>.Get().HideWindow("StartWindow");
				Solarmax.Singleton<UISystem>.Get().ShowWindow("MatchWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatch, new object[]
				{
					num2
				});
			}
			else if (errCode2 == ErrCode.EC_LeagueNotInMatchTime)
			{
				Tips.Make(LanguageDataProvider.GetValue(215));
			}
			else
			{
				Tips.Make(LanguageDataProvider.GetValue(216));
			}
		}
		else if (eventId == EventId.OnLeagueRankResult)
		{
			IList<MemberInfo> list = (IList<MemberInfo>)args[1];
			MemberInfo memberInfo = (MemberInfo)args[2];
			int num3 = (int)args[3];
			for (int i = 0; i < this.csPlayRanks.Length; i++)
			{
				if (i < list.Count)
				{
					this.csPlayRanks[i].text = list[i].name;
					this.csPlayRanks[i].transform.Find("r").GetComponent<UILabel>().text = (i + 1).ToString();
				}
				else
				{
					this.csPlayRanks[i].text = "--";
					this.csPlayRanks[i].transform.Find("r").GetComponent<UILabel>().text = "--";
				}
			}
			this.csPlayMyRank.text = memberInfo.name;
			this.csPlayMyRank.transform.Find("r").GetComponent<UILabel>().text = (num3 + 1).ToString();
		}
		else if (eventId == EventId.OnLeagueRankBack)
		{
			this.OnChampionshipClick();
		}
	}

	private void PlayAnimation(string state)
	{
		this.aniPlayer.clipName = state;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	public void OnPlayAnimationEnd()
	{
		if (this.isHide)
		{
			this.isHide = false;
			if (!this.matchTeam)
			{
				Solarmax.Singleton<UISystem>.Get().HideWindow("StartWindow");
				Solarmax.Singleton<UISystem>.Get().ShowWindow(this.waitShowWindowName);
			}
		}
		if (this.matchSingle)
		{
			this.matchSingle = false;
			this.matchTeam = false;
		}
		else if (this.matchTeam)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.TeamCreate(true, false);
			this.matchSingle = false;
			this.matchTeam = false;
		}
	}

	private void SetPlayerInfo()
	{
		this.userRewardLabel.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.score.ToString();
		this.userNameLabel.text = string.Format("Hi, {0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.name);
		this.userIconTexture.picUrl = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
	}

	private string GetDayStr(int day)
	{
		string result = string.Empty;
		if (day == 1)
		{
			result = "周一";
		}
		else if (day == 2)
		{
			result = "周二";
		}
		else if (day == 3)
		{
			result = "周三";
		}
		else if (day == 4)
		{
			result = "周四";
		}
		else if (day == 5)
		{
			result = "周五";
		}
		else if (day == 6)
		{
			result = "周六";
		}
		else if (day == 7)
		{
			result = "周日";
		}
		return result;
	}

	private void SetChampionshipPlayInfo(LeagueInfo data, MemberInfo member)
	{
	}

	private void UpdateChampionshipPlayTime()
	{
		DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		if (this.csPlayLeagueEndTime.CompareTo(serverTime) > 0)
		{
			TimeSpan timeSpan = this.csPlayLeagueEndTime - serverTime;
			if (timeSpan.Days > 0)
			{
				this.csPlayRemainTime.text = string.Format(LanguageDataProvider.GetValue(403), timeSpan.Days);
			}
			else if (timeSpan.Hours > 0)
			{
				this.csPlayRemainTime.text = string.Format(LanguageDataProvider.GetValue(404), timeSpan.Hours);
			}
			else if (timeSpan.Minutes > 0)
			{
				this.csPlayRemainTime.text = string.Format(LanguageDataProvider.GetValue(405), timeSpan.Minutes);
			}
			else
			{
				this.csPlayRemainTime.text = string.Format(LanguageDataProvider.GetValue(406), timeSpan.Seconds);
			}
		}
		else
		{
			this.csPlayRemainTime.text = LanguageDataProvider.GetValue(407);
		}
	}

	private void UpdateChampionshipPlaySmallRank()
	{
	}

	private void SetChampionshipSignupInfo(int start, IList<LeagueInfo> list)
	{
	}

	private void CloseToFight(bool single, bool team)
	{
		if (this.isHide)
		{
			return;
		}
		this.isHide = true;
		this.matchSingle = single;
		this.matchTeam = team;
		this.waitShowWindowName = "MatchWindow";
		this.PlayAnimation("StartWindow_out");
	}

	private void CloseToWindow(string windowName)
	{
		if (this.isHide)
		{
			return;
		}
		this.isHide = true;
		this.matchSingle = false;
		this.matchTeam = false;
		this.waitShowWindowName = windowName;
		this.PlayAnimation("StartWindow_out");
	}

	public void OnPlaySingle()
	{
		this.CloseToFight(true, false);
	}

	public void OnPlayTeam()
	{
		this.CloseToFight(false, true);
	}

	public void OnSelectMapClick(GameObject go)
	{
		this.CloseToWindow("SelectMapWindow");
	}

	public void OnIconClick()
	{
		this.CloseToWindow("SelectIconWindow");
	}

	public void OnRankClick()
	{
		this.CloseToWindow("RankWindow");
	}

	public void OnFriendClick()
	{
		this.CloseToWindow("FriendCareWindow");
	}

	public void OnVedioClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		this.CloseToWindow("ReplayWindow");
	}

	public void OnSettingClick()
	{
		this.CloseToWindow("SettingWindow");
	}

	private void SelectTab(bool combat, bool championship)
	{
		Color color = new Color(1f, 1f, 1f, 0.3f);
		Color white = Color.white;
		this.combatObj.SetActive(combat);
		this.combatTabLine.color = ((!combat) ? color : white);
		this.combatTabLabel.color = ((!combat) ? color : white);
		this.championshipObj.SetActive(championship);
		this.championshipTabLine.color = ((!championship) ? color : white);
		this.championshipTabLabel.color = ((!championship) ? color : white);
	}

	public void OnCombatClick()
	{
		this.SelectTab(true, false);
		this.isHide = false;
		this.matchSingle = false;
		this.matchTeam = false;
	}

	public void OnChampionshipClick()
	{
		this.SelectTab(false, true);
		this.csSignupLeagueList.Clear();
		this.csSignupGrid.transform.DestroyChildren();
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueInfo();
	}

	private void OnChampionshipSignupScrollviewShowMore()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueList(this.csSignupLeagueList.Count);
	}

	public void OnChampionshipPlayStartClick()
	{
	}

	public void OnChampionshipPlayRankClick()
	{
	}

	public UILabel userRewardLabel;

	public UILabel userNameLabel;

	public NetTexture userIconTexture;

	public UIPlayAnimation aniPlayer;

	public UIEventListener globalMap;

	private bool isHide;

	private string waitShowWindowName = string.Empty;

	private bool matchSingle;

	private bool matchTeam;

	public GameObject combatObj;

	public UISprite combatTabLine;

	public UILabel combatTabLabel;

	public GameObject championshipObj;

	public UISprite championshipTabLine;

	public UILabel championshipTabLabel;

	public GameObject championshipSignUpPage;

	public GameObject championshipPlayPage;

	public UIScrollView csSignupScrollView;

	public UIGrid csSignupGrid;

	public StartWindowChampionshipCell csSignupTemplate;

	private List<LeagueInfo> csSignupLeagueList;

	public UILabel csPlayName;

	public UILabel csPlayRemainTime;

	public UILabel csPlayStartTime;

	public UILabel csPlayWin;

	public UILabel csPlayNextAward;

	public UISprite[] csPlayLives;

	public UILabel[] csPlayRanks;

	public UILabel csPlayMyRank;

	private LeagueInfo csPlayLeagueData;

	private MemberInfo csPlayMemberData;

	private DateTime csPlayLeagueEndTime;
}
