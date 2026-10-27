using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ChampionshipRankWindow : BaseWindow
{
	private void Awake()
	{
		this.scrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnScrollViewShowMore);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnLeagueShowRankData);
		base.RegisterEvent(EventId.OnLeagueRankResult);
		return true;
	}

	public override void OnShow()
	{
		this.rankList.Clear();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void OnScrollViewShowMore()
	{
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("ChampionshipRankWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLeagueRankBack, new object[0]);
	}

	public UILabel vsType;

	public UILabel hostName;

	public UILabel userNum;

	public UILabel type;

	public UILabel time;

	public UILabel mark;

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject template;

	private LeagueInfo leagueData;

	private List<MemberInfo> rankList = new List<MemberInfo>();
}
