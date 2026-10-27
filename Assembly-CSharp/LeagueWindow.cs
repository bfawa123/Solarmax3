using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class LeagueWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		this.rankList = new List<MemberInfo>();
		base.RegisterEvent(EventId.OnGetLeagueInfoResult);
		base.RegisterEvent(EventId.OnLeagueListResult);
		base.RegisterEvent(EventId.OnLeagueSignUpResult);
		base.RegisterEvent(EventId.OnLeagueRankResult);
		base.RegisterEvent(EventId.OnLeagueMatchResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.ShowPage(false, false, false, false, false);
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueInfo();
		base.InvokeRepeating("UpdateTime", 1f, 1f);
	}

	public override void OnHide()
	{
		this.rankList.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnGetLeagueInfoResult)
		{
			this.leagueInfo = (LeagueInfo)args[0];
			this.selfInfo = (MemberInfo)args[1];
			this.start = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_start);
			this.end = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_finish);
			DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
			if (serverTime.CompareTo(this.start) < 0)
			{
				this.SetWaitPage();
			}
			else if (serverTime.CompareTo(this.end) < 0)
			{
				if (this.selfInfo.battle_num == 0)
				{
					this.SetWaitPage();
				}
				else
				{
					this.SetBattlePage();
					Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueRank(this.leagueInfo.id, 0);
				}
			}
			else
			{
				this.SetResultPage();
				Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueRank(this.leagueInfo.id, 0);
			}
		}
		else if (eventId == EventId.OnLeagueListResult)
		{
			int num = (int)args[0];
			IList<LeagueInfo> list = (IList<LeagueInfo>)args[1];
			if (list == null || list.Count == 0)
			{
				this.SetNotOpenPage();
			}
			else
			{
				this.SetSignUpPage(list[0]);
			}
		}
		else if (eventId == EventId.OnLeagueSignUpResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			string text = (string)args[1];
			if (errCode != ErrCode.EC_Ok)
			{
				if (errCode == ErrCode.EC_LeagueIsFull)
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
		}
		else if (eventId == EventId.OnLeagueRankResult)
		{
			IList<MemberInfo> collection = (IList<MemberInfo>)args[1];
			this.rankList.AddRange(collection);
			if (this.battlePage.activeSelf)
			{
				this.SetRankScrollView(this.battleRankScrollView, this.battleRankGrid, this.battleStaticRanks);
			}
			else if (this.resultPage.activeSelf)
			{
				this.SetRankScrollView(this.resultRankScrollView, this.resultRankGrid, this.resultStaticRanks);
				this.SetResultSelfRank();
			}
		}
		else if (eventId == EventId.OnLeagueMatchResult)
		{
			ErrCode errCode2 = (ErrCode)args[0];
			if (errCode2 != ErrCode.EC_Ok)
			{
				if (errCode2 == ErrCode.EC_LeagueNotInMatchTime)
				{
					Tips.Make(LanguageDataProvider.GetValue(215));
				}
				else
				{
					Tips.Make(LanguageDataProvider.GetValue(216));
				}
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void UpdateTime()
	{
		if (this.signupPage.activeSelf)
		{
			this.UpdateTimeSignUp();
		}
		else if (this.waitPage.activeSelf)
		{
			this.UpdateTimeWait();
		}
		else if (this.battlePage.activeSelf)
		{
			this.UpdateTimeBattle();
		}
	}

	private void ShowPage(bool notOpen, bool signUp, bool wait, bool battle, bool result)
	{
		this.notopenPage.gameObject.SetActive(notOpen);
		this.signupPage.gameObject.SetActive(signUp);
		this.waitPage.gameObject.SetActive(wait);
		this.battlePage.gameObject.SetActive(battle);
		this.resultPage.gameObject.SetActive(result);
	}

	private void SetNotOpenPage()
	{
		this.ShowPage(true, false, false, false, false);
	}

	private void SetSignUpPage(LeagueInfo leagueInfo)
	{
		this.leagueInfo = leagueInfo;
		this.ShowPage(false, true, false, false, false);
		this.start = Solarmax.Singleton<TimeSystem>.Instance.GetTime(leagueInfo.signup_start);
		this.end = Solarmax.Singleton<TimeSystem>.Instance.GetTime(leagueInfo.signup_finish);
		this.UpdateTimeSignUp();
		DateTime timeCST = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(leagueInfo.combat_start);
		DateTime timeCST2 = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(leagueInfo.combat_finish);
		if (timeCST.Year == timeCST2.Year && timeCST.Month == timeCST2.Month && timeCST.Day == timeCST2.Day)
		{
			this.signLeagueTime.text = LanguageDataProvider.Format(703, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
		else
		{
			this.signLeagueTime.text = LanguageDataProvider.Format(704, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Year,
				timeCST2.Month,
				timeCST2.Day,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
		this.signLeagueDesc.text = leagueInfo.desc;
	}

	public void OnSignupClick()
	{
		DateTime timeCST = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(this.leagueInfo.combat_start);
		DateTime timeCST2 = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(this.leagueInfo.combat_finish);
		string text = string.Empty;
		if (timeCST.Year == timeCST2.Year && timeCST.Month == timeCST2.Month && timeCST.Day == timeCST2.Day)
		{
			text = LanguageDataProvider.Format(717, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
		else
		{
			text = LanguageDataProvider.Format(718, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Year,
				timeCST2.Month,
				timeCST2.Day,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
		{
			2,
			text,
			new EventDelegate(delegate()
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.RequestLeagueSignUp(this.leagueInfo.id);
			})
		});
	}

	private void UpdateTimeSignUp()
	{
		DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		if (this.start.CompareTo(serverTime) > 0)
		{
			this.signLastTime.text = LanguageDataProvider.GetValue(707);
			this.signBtn.enabled = false;
		}
		else if (this.end.CompareTo(serverTime) < 0)
		{
			this.signLastTime.text = LanguageDataProvider.GetValue(708);
			this.signBtn.enabled = false;
		}
		else
		{
			TimeSpan timeSpan = this.end - serverTime;
			if (timeSpan.TotalDays > 1.0)
			{
				this.signLastTime.text = LanguageDataProvider.Format(701, new object[]
				{
					timeSpan.Days,
					timeSpan.Hours
				});
			}
			else
			{
				this.signLastTime.text = LanguageDataProvider.Format(702, new object[]
				{
					timeSpan.Hours,
					timeSpan.Minutes,
					timeSpan.Seconds
				});
			}
			this.signBtn.enabled = true;
		}
	}

	public void OnSinupHelpBtnClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowDialogWindow(1, LanguageDataProvider.GetValue(750));
	}

	private void SetWaitPage()
	{
		this.ShowPage(false, false, true, false, false);
		this.start = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_start);
		this.end = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_finish);
		this.UpdateTimeWait();
		TimeSpan timeSpan = this.end - this.start;
		if (timeSpan.Hours == 0)
		{
			this.waitLeagueTotalTime.text = LanguageDataProvider.Format(719, new object[]
			{
				timeSpan.Minutes
			});
		}
		else if (timeSpan.Hours > 0 && timeSpan.Minutes == 0)
		{
			this.waitLeagueTotalTime.text = LanguageDataProvider.Format(715, new object[]
			{
				timeSpan.Hours
			});
		}
		else
		{
			this.waitLeagueTotalTime.text = LanguageDataProvider.Format(720, new object[]
			{
				timeSpan.Hours,
				timeSpan.Minutes
			});
		}
		this.waitLeaguePlayer.text = this.leagueInfo.cur_num.ToString();
		DateTime timeCST = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(this.start);
		DateTime timeCST2 = Solarmax.Singleton<TimeSystem>.Instance.GetTimeCST(this.end);
		if (timeCST.Year == timeCST2.Year && timeCST.Month == timeCST2.Month && timeCST.Day == timeCST2.Day)
		{
			this.waitLeagutTime.text = LanguageDataProvider.Format(703, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
		else
		{
			this.waitLeagutTime.text = LanguageDataProvider.Format(704, new object[]
			{
				timeCST.Year,
				timeCST.Month,
				timeCST.Day,
				timeCST.Hour,
				timeCST.Minute,
				timeCST2.Year,
				timeCST2.Month,
				timeCST2.Day,
				timeCST2.Hour,
				timeCST2.Minute
			});
		}
	}

	public void OnCombatClick()
	{
	}

	private void UpdateTimeWait()
	{
		DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		if (this.start.CompareTo(serverTime) > 0)
		{
			TimeSpan timeSpan = this.start - serverTime;
			this.waitAreadyTime.gameObject.SetActive(false);
			this.waitLastTime.transform.parent.gameObject.SetActive(true);
			this.waitLastTime.text = LanguageDataProvider.Format(702, new object[]
			{
				(int)Math.Floor(timeSpan.TotalHours),
				timeSpan.Minutes,
				timeSpan.Seconds
			});
		}
		else if (this.end.CompareTo(serverTime) < 0)
		{
			this.waitAreadyTime.gameObject.SetActive(false);
			this.waitLastTime.transform.parent.gameObject.SetActive(true);
			this.waitLastTime.text = LanguageDataProvider.GetValue(710);
		}
		else
		{
			TimeSpan timeSpan2 = serverTime - this.start;
			this.waitAreadyTime.gameObject.SetActive(true);
			if (timeSpan2.TotalHours > 1.0)
			{
				this.waitAreadyTime.text = LanguageDataProvider.Format(714, new object[]
				{
					(int)Math.Floor(timeSpan2.TotalHours),
					timeSpan2.Minutes,
					timeSpan2.Seconds
				});
			}
			else if (timeSpan2.TotalMinutes > 1.0)
			{
				this.waitAreadyTime.text = LanguageDataProvider.Format(713, new object[]
				{
					timeSpan2.Minutes,
					timeSpan2.Seconds
				});
			}
			else
			{
				this.waitAreadyTime.text = LanguageDataProvider.Format(712, new object[]
				{
					timeSpan2.Seconds
				});
			}
			this.waitLastTime.transform.parent.gameObject.SetActive(false);
		}
	}

	public void OnWaitHelpBtnClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowDialogWindow(1, LanguageDataProvider.GetValue(751));
	}

	private void SetBattlePage()
	{
		this.ShowPage(false, false, false, true, false);
		this.start = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_start);
		this.end = Solarmax.Singleton<TimeSystem>.Instance.GetTime(this.leagueInfo.combat_finish);
		DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		TimeSpan timeSpan = this.end - serverTime;
		this.battleLastTime.text = LanguageDataProvider.Format(702, new object[]
		{
			(int)Math.Floor(timeSpan.TotalHours),
			timeSpan.Minutes,
			timeSpan.Seconds
		});
		this.battleScore.text = this.selfInfo.score.ToString();
		this.battleMVP.text = this.selfInfo.mvp.ToString();
		for (int i = 0; i < this.battleStaticRanks.Length; i++)
		{
			this.SetBattleRankItem(this.battleStaticRanks[i], null, i + 1);
		}
	}

	private void UpdateTimeBattle()
	{
		DateTime serverTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		TimeSpan timeSpan = this.end - serverTime;
		this.battleLastTime.text = LanguageDataProvider.Format(702, new object[]
		{
			(int)Math.Floor(timeSpan.TotalHours),
			timeSpan.Minutes,
			timeSpan.Seconds
		});
	}

	private void SetBattleRankItem(GameObject go, MemberInfo memberInfo, int rank)
	{
		string spriteName = string.Empty;
		string text = "--";
		int num = 0;
		int num2 = 0;
		bool active = false;
		if (memberInfo != null)
		{
			spriteName = memberInfo.icon;
			text = memberInfo.name;
			num = memberInfo.score;
			num2 = memberInfo.mvp;
			active = (memberInfo.id == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId);
		}
		go.transform.Find("icon").GetComponent<UISprite>().spriteName = spriteName;
		go.transform.Find("name").GetComponent<UILabel>().text = text;
		go.transform.Find("score").GetComponent<UILabel>().text = num.ToString();
		go.transform.Find("mvp").GetComponent<UILabel>().text = num2.ToString();
		go.transform.Find("rank").GetComponent<UILabel>().text = rank.ToString();
		if (rank > 3)
		{
			go.transform.Find("rank/bg").gameObject.SetActive(false);
		}
		go.transform.Find("bg").gameObject.SetActive(active);
		go.SetActive(memberInfo != null);
	}

	private void SetRankScrollView(UIScrollView scrollview, UIGrid grid, GameObject[] staticRanks)
	{
		this.grid = grid;
		this.template = staticRanks[0];
		grid.onCustomSort = delegate(Transform arg0, Transform arg1)
		{
			int num2 = int.Parse(arg0.name);
			int value = int.Parse(arg1.name);
			return num2.CompareTo(value);
		};
		scrollview.onShowMore = new UIScrollView.OnDragNotification(this.OnScrollViewShowMore);
		scrollview.onShowLess = new UIScrollView.OnDragNotification(this.OnScrollViewShowLess);
		for (int i = 0; i < staticRanks.Length; i++)
		{
			this.SetBattleRankItem(staticRanks[i], (i < this.rankList.Count) ? this.rankList[i] : null, i + 1);
		}
		this.selfRankIndex = -1;
		int j = 0;
		int count = this.rankList.Count;
		while (j < count)
		{
			if (this.rankList[j].id == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				this.selfRankIndex = j;
				break;
			}
			j++;
		}
		if (this.rankList.Count > 3)
		{
			grid.transform.DestroyChildren();
			if (this.selfRankIndex >= 3)
			{
				this.showIndexMax = this.rankList.Count - 1;
				this.showIndexMin = this.showIndexMax - 9;
				if (this.selfRankIndex < this.showIndexMin + 2 || this.selfRankIndex > this.showIndexMax)
				{
					this.showIndexMin = this.selfRankIndex - 2;
					this.showIndexMax = this.showIndexMin + 9;
				}
				if (this.showIndexMin < 3)
				{
					this.showIndexMin = 3;
				}
				if (this.showIndexMax > this.rankList.Count - 1)
				{
					this.showIndexMax = this.rankList.Count - 1;
				}
			}
			else
			{
				this.showIndexMin = 3;
				this.showIndexMax = this.showIndexMin + 9;
				if (this.showIndexMax > this.rankList.Count - 1)
				{
					this.showIndexMax = this.rankList.Count - 1;
				}
			}
			for (int k = this.showIndexMin; k <= this.showIndexMax; k++)
			{
				GameObject gameObject = grid.gameObject.AddChild(staticRanks[0]);
				this.SetBattleRankItem(gameObject, this.rankList[k], k + 1);
				gameObject.SetActive(true);
				gameObject.name = (k + 1).ToString();
			}
			grid.Reposition();
			scrollview.ResetPosition();
			int num = 0;
			if (this.selfRankIndex - this.showIndexMin + 1 > 3)
			{
				num = this.selfRankIndex - this.showIndexMin + 1 - 3;
			}
			if (this.showIndexMax - this.showIndexMin + 1 - num < 4)
			{
				num = this.showIndexMax - this.showIndexMin - 3;
			}
			if (num > 0)
			{
				scrollview.MoveRelative(new Vector3(0f, (float)num * grid.cellHeight + 10f, 0f));
			}
		}
	}

	private void FillScrollView(int start, bool more, int count)
	{
		if (more)
		{
			for (int i = start; i <= start + count; i++)
			{
				if (i > this.rankList.Count - 1)
				{
					break;
				}
				GameObject gameObject = this.grid.gameObject.AddChild(this.template);
				this.SetBattleRankItem(gameObject, this.rankList[i], i + 1);
				gameObject.SetActive(true);
				gameObject.name = (i + 1).ToString();
				this.showIndexMax++;
			}
		}
		else
		{
			for (int j = start; j >= start - count; j--)
			{
				if (j < 3)
				{
					break;
				}
				GameObject gameObject2 = this.grid.gameObject.AddChild(this.template);
				this.SetBattleRankItem(gameObject2, this.rankList[j], j + 1);
				gameObject2.SetActive(true);
				gameObject2.name = (j + 1).ToString();
				this.showIndexMin--;
			}
		}
		this.grid.Reposition();
	}

	private void OnScrollViewShowLess()
	{
		if (this.showIndexMin == 3)
		{
			return;
		}
		this.FillScrollView(this.showIndexMin - 1, false, 10);
	}

	private void OnScrollViewShowMore()
	{
		if (this.showIndexMax == this.rankList.Count - 1)
		{
			return;
		}
		this.FillScrollView(this.showIndexMax + 1, true, 10);
	}

	private void SetResultPage()
	{
		this.ShowPage(false, false, false, false, true);
		this.resultRank.text = LanguageDataProvider.Format(716, new object[]
		{
			"?"
		});
		this.resultScore.text = this.selfInfo.score.ToString();
		this.resultMVP.text = this.selfInfo.mvp.ToString();
		for (int i = 0; i < this.battleStaticRanks.Length; i++)
		{
			this.SetBattleRankItem(this.battleStaticRanks[i], null, i + 1);
		}
	}

	private void SetResultSelfRank()
	{
		this.resultRank.text = LanguageDataProvider.Format(716, new object[]
		{
			this.selfRankIndex + 1
		});
	}

	public void OnResultConfirmClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitLeague();
	}

	public GameObject notopenPage;

	public GameObject signupPage;

	public UILabel signLastTime;

	public UILabel signLeagueTime;

	public UILabel signLeagueDesc;

	public UIButton signBtn;

	public GameObject waitPage;

	public UILabel waitLastTime;

	public UILabel waitAreadyTime;

	public UILabel waitLeagueTotalTime;

	public UILabel waitLeaguePlayer;

	public UILabel waitLeagutTime;

	public UIButton waitBattleBtn;

	public GameObject battlePage;

	public UILabel battleLastTime;

	public UILabel battleScore;

	public UILabel battleMVP;

	public GameObject[] battleStaticRanks;

	public UIScrollView battleRankScrollView;

	public UIGrid battleRankGrid;

	public GameObject resultPage;

	public UILabel resultRank;

	public UILabel resultScore;

	public UILabel resultMVP;

	public GameObject[] resultStaticRanks;

	public UIScrollView resultRankScrollView;

	public UIGrid resultRankGrid;

	private DateTime start;

	private DateTime end;

	private LeagueInfo leagueInfo;

	private MemberInfo selfInfo;

	private List<MemberInfo> rankList;

	private int selfRankIndex;

	private int showIndexMin;

	private int showIndexMax;

	private UIGrid grid;

	private GameObject template;
}
