using System;
using System.Collections.Generic;
using System.Linq;
using NetMessage;
using Solarmax;
using UnityEngine;

public class FriendRankingWindow : BaseWindow
{
	private void Awake()
	{
		this.selectTab = null;
		this.friendList = new List<FriendSimplePlayer>();
		this.allList = new List<FriendSimplePlayer>();
		UIEventListener component = this.Friend.gameObject.GetComponent<UIEventListener>();
		component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		UIEventListener component2 = this.All.gameObject.GetComponent<UIEventListener>();
		component2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component2.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		UIEventListener.Get(this.backBoxCollider).onClick = new UIEventListener.VoidDelegate(this.OnBackClicked);
		UIEventListener.Get(this.backBoxCollider).onPress = new UIEventListener.BoolDelegate(this.OnBgPressed);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.ReconnectResult);
		base.RegisterEvent(EventId.OnPveRankReportLoad);
		base.RegisterEvent(EventId.OnDownloadRankVideo);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.friendList.Clear();
		this.allList.Clear();
		this.grid.transform.DestroyChildren();
		string id = Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id;
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestPveRankReport(id);
		this.OnTabClick(this.All.gameObject);
		this.backBoxCollider.GetComponent<UISprite>().leftAnchor.absolute = 0;
		this.backBoxCollider.GetComponent<UISprite>().rightAnchor.absolute = 0;
	}

	private void PlayAnimation(string strAni, float speed = 1f)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, speed, false);
	}

	public override void OnHide()
	{
	}

	public void OnBgPressed(GameObject go, bool press)
	{
		this.OnBackClick();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.ReconnectResult)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel != null)
			{
				string id = Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id;
				Solarmax.Singleton<NetSystem>.Instance.helper.RequestPveRankReport(id);
			}
		}
		else if (eventId == EventId.OnPveRankReportLoad)
		{
			bool flag = (bool)args[0];
			List<FriendSimplePlayer> list = args[1] as List<FriendSimplePlayer>;
			this.ClearList(this.allList);
			this.allList = list;
			//from o in this.allList
			//orderby o.score descending
			//select o;
			if (this.allList != null)
			{
				this.allList = this.allList.OrderByDescending(x => x.score).ToList();
			}
			if (this.selectTab == this.All)
			{
				this.RefreshScrollView(this.allList);
			}
			List<FriendSimplePlayer> list2 = args[2] as List<FriendSimplePlayer>;
			this.ClearList(this.friendList);
			this.friendList = list2;
			//from o in this.friendList
			//orderby o.score descending
			//select o;
			if (this.friendList != null)
			{
				this.friendList = this.friendList.OrderByDescending(x => x.score).ToList();
			}
			if (this.selectTab == this.Friend)
			{
				this.RefreshScrollView(this.friendList);
			}
		}
		else if (eventId == EventId.OnDownloadRankVideo)
		{
			string text = (string)args[0];
			if (!string.IsNullOrEmpty(text))
			{
				PbSCFrames pbSCFrames = ReplayCollectManager.Get().LoadFileToReplayData(text, true);
				if (pbSCFrames != null)
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("UnTouchWindow");
					Solarmax.Singleton<BattleSystem>.Instance.replayManager.TryPlayRecord(pbSCFrames, true);
					base.Invoke("HideInvoke", 3f);
				}
			}
		}
	}

	private void HideInvoke()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("UnTouchWindow");
	}

	private void RefreshScrollView(List<FriendSimplePlayer> data)
	{
		this.grid.transform.DestroyChildren();
		int i = 0;
		int count = data.Count;
		while (i < count)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.template);
			gameObject.SetActive(true);
			FriendRankingCell component = gameObject.GetComponent<FriendRankingCell>();
			component.SetInfoLocal(data[i], i + 1, this.scrollView);
			i++;
		}
		this.grid.Reposition();
		this.scrollView.ResetPosition();
	}

	private void OnTabClick(GameObject go)
	{
		if (this.selectTab != null && go == this.selectTab.gameObject)
		{
			return;
		}
		if (go.name.Equals(this.Friend.gameObject.name))
		{
			this.selectTab = this.Friend;
			this.selectLine.transform.localPosition = new Vector3(-113f, 244f, 0f);
		}
		else
		{
			this.selectTab = this.All;
			this.selectLine.transform.localPosition = new Vector3(113f, 244f, 0f);
		}
		this.grid.transform.DestroyChildren();
		if (this.selectTab == this.All)
		{
			this.RefreshScrollView(this.allList);
		}
		else
		{
			this.RefreshScrollView(this.friendList);
		}
	}

	private void OnBackClicked(GameObject go)
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCloseFriendViewEvent, null);
	}

	public void OnBackClick()
	{
		this.OnBackClicked(null);
	}

	private void ClearList(List<FriendSimplePlayer> list)
	{
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = null;
			}
			list.Clear();
		}
	}

	public UILabel Friend;

	public UILabel All;

	public GameObject template;

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject selectLine;

	public GameObject backBoxCollider;

	private UILabel selectTab;

	private float selectHotTabTime;

	private List<FriendSimplePlayer> friendList;

	private List<FriendSimplePlayer> allList;

	public UIPlayAnimation aniPlayer;
}
