using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class FriendCareWindow : BaseWindow
{
	private void Awake()
	{
		this.selectTab = null;
		this.myFollowList = new List<SimplePlayerData>();
		this.followerList = new List<SimplePlayerData>();
		this.myFollowStatus = new List<bool>();
		this.followerStatus = new List<bool>();
		UIEventListener component = this.followerTab.gameObject.GetComponent<UIEventListener>();
		component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		UIEventListener component2 = this.myfollowTab.gameObject.GetComponent<UIEventListener>();
		component2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component2.onClick, new UIEventListener.VoidDelegate(this.OnTabClick));
		this.scrollView.onShowMore = new UIScrollView.OnDragNotification(this.OnScrollViewShowMore);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFriendLoadResult);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.myFollowList.Clear();
		this.followerList.Clear();
		this.myFollowStatus.Clear();
		this.followerStatus.Clear();
		this.OnTabClick(this.followerTab.gameObject);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnFriendLoadResult)
		{
			if (eventId == EventId.OnFriendFollowResult)
			{
				int num = (int)args[0];
				bool flag = (bool)args[1];
				ErrCode errCode = (ErrCode)args[2];
				if (errCode == ErrCode.EC_Ok)
				{
					Transform transform = this.grid.transform.Find("infomation_" + num);
					if (transform != null)
					{
						FriendWindowCell component = transform.gameObject.GetComponent<FriendWindowCell>();
						component.OnCareResult(flag);
					}
					else
					{
						Debug.Log("OnFriendFollowResult   找不到子节点");
					}
					if (this.selectTab == this.myfollowTab && !flag)
					{
						SimplePlayerData simplePlayerData = null;
						for (int i = 0; i < this.myFollowList.Count; i++)
						{
							if (this.myFollowList[i].userId == num)
							{
								simplePlayerData = this.myFollowList[i];
							}
						}
						if (simplePlayerData != null)
						{
							this.myFollowList.Remove(simplePlayerData);
						}
						if (transform != null)
						{
							this.grid.RemoveChild(transform);
							transform.transform.SetParent(null);
							UnityEngine.Object.Destroy(transform.gameObject);
							this.grid.Reposition();
						}
					}
				}
				else
				{
					string text = (!flag) ? "取消关注" : "关注好友";
					text = string.Format("{0} userId: {1} 失败！", text, num);
					Tips.Make(Tips.TipsType.FlowUp, text, 1f);
				}
			}
		}
		else
		{
			bool flag2 = true;
			bool flag3;
			if (flag2)
			{
				flag3 = (this.selectTab == this.myfollowTab);
			}
			else
			{
				flag3 = (this.selectTab == this.followerTab);
			}
			if (!flag3)
			{
				return;
			}
			List<SimplePlayerData> data = args[2] as List<SimplePlayerData>;
			List<bool> status = args[3] as List<bool>;
			this.RefreshScrollView(data, status, false);
		}
	}

	private void RefreshScrollView(List<SimplePlayerData> data, List<bool> status, bool useOldData)
	{
		for (int i = 0; i < data.Count; i++)
		{
			GameObject gameObject = this.grid.gameObject.AddChild(this.infoTemplate);
			gameObject.name = "infomation_" + data[i].userId;
			gameObject.SetActive(true);
			FriendWindowCell component = gameObject.GetComponent<FriendWindowCell>();
			component.SetFollowerInfo(data[i], this.scrollView);
		}
		this.grid.Reposition();
		if (this.selectTab == this.followerTab)
		{
			if (this.followerList.Count == 0 || useOldData)
			{
				this.scrollView.ResetPosition();
			}
			if (!useOldData)
			{
				this.followerList.AddRange(data);
				this.followerStatus.AddRange(status);
			}
		}
		else
		{
			if (this.myFollowList.Count == 0 || useOldData)
			{
				this.scrollView.ResetPosition();
			}
			if (!useOldData)
			{
				this.myFollowList.AddRange(data);
				this.myFollowStatus.AddRange(status);
			}
		}
	}

	private void OnTabClick(GameObject go)
	{
		if (this.selectTab != null && go == this.selectTab.gameObject)
		{
			return;
		}
		Color color = new Color(1f, 1f, 1f, 0.3f);
		this.followerTab.color = color;
		this.myfollowTab.color = color;
		this.selectTab = go.GetComponent<UILabel>();
		this.selectTab.color = Color.white;
		this.followerTabSubPic.gameObject.SetActive(this.selectTab == this.followerTab);
		this.myfollowTabSubPic.gameObject.SetActive(this.selectTab == this.myfollowTab);
		this.grid.transform.DestroyChildren();
		if (this.selectTab == this.followerTab)
		{
			if (Time.realtimeSinceStartup > this.selectFollowerTabTime + 3f)
			{
				this.selectFollowerTabTime = Time.realtimeSinceStartup;
				this.followerList.Clear();
				this.followerStatus.Clear();
				this.OnScrollViewShowMore();
			}
			else
			{
				this.RefreshScrollView(this.followerList, this.followerStatus, true);
			}
		}
		else if (this.myFollowList.Count == 0)
		{
			this.OnScrollViewShowMore();
		}
		else
		{
			this.RefreshScrollView(this.myFollowList, this.myFollowStatus, true);
		}
	}

	private void OnScrollViewShowMore()
	{
	}

	public void OnAddFriendClick()
	{
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendWindow");
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendCareWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("StartWindow");
	}

	public UILabel followerTab;

	public UISprite followerTabSubPic;

	public UILabel myfollowTab;

	public UISprite myfollowTabSubPic;

	public UIScrollView scrollView;

	public UIGrid grid;

	public GameObject infoTemplate;

	private UILabel selectTab;

	private float selectFollowerTabTime;

	private List<SimplePlayerData> myFollowList;

	private List<bool> myFollowStatus;

	private List<SimplePlayerData> followerList;

	private List<bool> followerStatus;
}
