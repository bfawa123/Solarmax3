using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class FriendMembersWindow : BaseWindow
{
	private void Awake()
	{
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFriendLoadResult);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		base.RegisterEvent(EventId.OnMatchInviteOptype);
		base.RegisterEvent(EventId.OnRefreshFriendStats);
		base.RegisterEvent(EventId.OnFriendStateNotice);
		base.RegisterEvent(EventId.ReconnectResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.SetInfo();
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnFriendFollowResult)
		{
			if (eventId != EventId.OnFriendLoadResult)
			{
				switch (eventId)
				{
				case EventId.OnMatchInviteOptype:
					this.opType = (FriendMembersWindow.FRIENDOPTYPE)args[0];
					break;
				default:
					if (eventId != EventId.OnFriendStateNotice)
					{
						if (eventId == EventId.ReconnectResult)
						{
							Solarmax.Singleton<NetSystem>.Instance.helper.FriendLoad(0, false);
						}
					}
					else
					{
						List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
						if (myFriendList.Count > 0)
						{
							this.RefreshScrollView(myFriendList);
						}
					}
					break;
				case EventId.OnRefreshFriendStats:
					this.OnRefreshFriendStats();
					break;
				}
			}
			else
			{
				List<SimplePlayerData> myFriendList2 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
				if (myFriendList2.Count > 0)
				{
					this.RefreshScrollView(myFriendList2);
				}
				if (this.opType == FriendMembersWindow.FRIENDOPTYPE.Invite)
				{
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendLoadResultforMatch, new object[0]);
				}
			}
		}
		else
		{
			List<SimplePlayerData> myFriendList3 = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
			if (myFriendList3.Count > 0)
			{
				this.RefreshScrollView(myFriendList3);
			}
		}
	}

	private void SetInfo()
	{
	}

	private void OnRefreshFriendStats()
	{
		List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
		if (myFriendList.Count > 0)
		{
			this.RefreshScrollView(myFriendList);
		}
	}

	private void OnTabClick(GameObject go)
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("onClick");
		List<SimplePlayerData> myFriendList = Solarmax.Singleton<FriendDataHandler>.Get().myFriendList;
		if (myFriendList.Count > 0)
		{
			this.RefreshScrollView(myFriendList);
		}
	}

	private void RefreshScrollView(List<SimplePlayerData> data)
	{
		this.grid.transform.DestroyChildren();
		for (int i = 0; i < data.Count; i++)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.infoTemplate);
			gameObject.name = "infomation_" + data[i].userId;
			gameObject.SetActive(true);
			FriendRankingCell component = gameObject.GetComponent<FriendRankingCell>();
			if (this.opType == FriendMembersWindow.FRIENDOPTYPE.Play)
			{
				component.SetInfoLocal(data[i], i, this.scrollView);
			}
			if (this.opType == FriendMembersWindow.FRIENDOPTYPE.Invite)
			{
				component.SetInviteInfo(data[i], i, this.scrollView);
			}
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject infoTemplate;

	private FriendMembersWindow.FRIENDOPTYPE opType;

	private enum FRIENDOPTYPE
	{
		Play,
		Invite
	}
}
