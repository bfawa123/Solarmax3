using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class TeamWindow : BaseWindow
{
	private void Awake()
	{
		this.selectTab = null;
		this.inviteList = new List<SimplePlayerData>();
		UIEventListener component = this.friendTab.gameObject.GetComponent<UIEventListener>();
		component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		UIEventListener component2 = this.aroundTab.gameObject.GetComponent<UIEventListener>();
		component2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component2.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		this.scrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnInviteShowMore);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnTeamUpdate);
		base.RegisterEvent(EventId.OnFriendLoadResult);
		base.RegisterEvent(EventId.OnTeamInviteResult);
		base.RegisterEvent(EventId.OnTeamInviteResponse);
		base.RegisterEvent(EventId.OnTeamStart);
		base.RegisterEvent(EventId.OnTeamDelete);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (Solarmax.Singleton<TeamInviteData>.Get().isLeader)
		{
			this.inviteList.Clear();
			this.OnTabClick(this.friendTab.gameObject);
		}
		else
		{
			this.friendTab.gameObject.SetActive(false);
			this.aroundTab.gameObject.SetActive(false);
			this.scrollView.gameObject.SetActive(false);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnTeamUpdate)
		{
			this.SetPage();
		}
		else if (eventId == EventId.OnFriendLoadResult)
		{
			List<SimplePlayerData> list = args[2] as List<SimplePlayerData>;
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = this.grid.gameObject.AddChild(this.infoTemplate);
				gameObject.name = "infomation_" + list[i].userId;
				gameObject.SetActive(true);
				TeamWindowCell component = gameObject.GetComponent<TeamWindowCell>();
				component.SetInfo(list[i]);
			}
			this.grid.Reposition();
			if (this.inviteList.Count == 0)
			{
				this.scrollView.ResetPosition();
			}
			this.inviteList.AddRange(list);
		}
		else if (eventId == EventId.OnTeamInviteResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			int num = (int)args[1];
			if (errCode == ErrCode.EC_Ok)
			{
				Transform transform = this.grid.transform.Find("infomation_" + num);
				if (transform != null)
				{
					TeamWindowCell component2 = transform.gameObject.GetComponent<TeamWindowCell>();
					component2.OnInviteSend();
				}
				else
				{
					Debug.LogError("OnTeamInviteResult   找不到子节点");
				}
			}
			else if (errCode == ErrCode.EC_Offline)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(917), 1f);
			}
			else if (errCode == ErrCode.EC_OnBattle)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(918), 1f);
			}
		}
		else if (eventId == EventId.OnTeamInviteResponse)
		{
			ErrCode code = (ErrCode)args[0];
			int num2 = (int)args[1];
			Transform transform2 = this.grid.transform.Find("infomation_" + num2);
			if (transform2 != null)
			{
				TeamWindowCell component3 = transform2.gameObject.GetComponent<TeamWindowCell>();
				component3.OnInviteResponse(code);
			}
			else
			{
				Debug.LogError("OnTeamInviteResponse   找不到子节点");
			}
		}
		else if (eventId == EventId.OnTeamStart)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("TeamWindow");
			Solarmax.Singleton<UISystem>.Get().ShowWindow("MatchWindow");
			Debug.Log("组队已开始");
		}
		else if (eventId == EventId.OnTeamDelete)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("TeamWindow");
			Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
		}
	}

	private void SetPage()
	{
		List<SimplePlayerData> teamPlayers = Solarmax.Singleton<TeamInviteData>.Get().teamPlayers;
		for (int i = 0; i < this.players.Length; i++)
		{
			GameObject gameObject = this.players[i].transform.Find("icon").gameObject;
			if (i < teamPlayers.Count)
			{
				gameObject.SetActive(true);
				gameObject.GetComponent<NetTexture>().picUrl = teamPlayers[i].icon;
				this.players[i].transform.Find("name").GetComponent<UILabel>().text = teamPlayers[i].name;
				this.players[i].transform.Find("emptyicon").gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(false);
				this.players[i].transform.Find("name").GetComponent<UILabel>().text = "待邀请";
				this.players[i].transform.Find("emptyicon").gameObject.SetActive(true);
			}
		}
		if (Solarmax.Singleton<TeamInviteData>.Get().isLeader)
		{
			this.startGameBtn.gameObject.SetActive(true);
			this.quitGameBtn.gameObject.SetActive(false);
		}
		else
		{
			this.startGameBtn.gameObject.SetActive(false);
			this.quitGameBtn.gameObject.SetActive(true);
		}
	}

	private void OnTabClick(GameObject go)
	{
		if (this.selectTab != null && go == this.selectTab.gameObject)
		{
			return;
		}
		Color color = new Color(1f, 1f, 1f, 0.3f);
		this.friendTab.color = color;
		this.aroundTab.color = color;
		this.selectTab = go.GetComponent<UILabel>();
		this.selectTab.color = Color.white;
		if (this.selectTab == this.friendTab)
		{
			this.friendTabSubPic.gameObject.SetActive(true);
			this.aroundTabSubPic.gameObject.SetActive(false);
		}
		else if (this.selectTab == this.aroundTab)
		{
			this.friendTabSubPic.gameObject.SetActive(false);
			this.aroundTabSubPic.gameObject.SetActive(true);
		}
		this.inviteList.Clear();
		this.OnInviteShowMore();
		this.grid.transform.DestroyChildren();
	}

	private void OnInviteShowMore()
	{
	}

	public void OnStartGameClick()
	{
		if (Solarmax.Singleton<TeamInviteData>.Get().isLeader)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.TeamStart();
		}
		else
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(919), 1f);
		}
	}

	public void OnQuitGameClick()
	{
		if (!Solarmax.Singleton<TeamInviteData>.Get().isLeader)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.TeamLeave(Solarmax.Singleton<TeamInviteData>.Get().leaderId);
		}
		else
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(920), 1f);
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.TeamLeave(Solarmax.Singleton<TeamInviteData>.Get().leaderId);
	}

	public UILabel friendTab;

	public UISprite friendTabSubPic;

	public UILabel aroundTab;

	public UISprite aroundTabSubPic;

	public GameObject[] players;

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject infoTemplate;

	public UIButton startGameBtn;

	public UIButton quitGameBtn;

	private UILabel selectTab;

	private List<SimplePlayerData> inviteList;
}
