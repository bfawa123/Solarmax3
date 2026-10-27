using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class FriendDataHandler : Solarmax.Singleton<FriendDataHandler>, IDataHandler, Lifecycle
{
	public bool Init()
	{
		this.myFriendList = new List<SimplePlayerData>();
		this.Release();
		Solarmax.Singleton<EventSystem>.Instance.RegisterEvent(EventId.OnMatchBeInviteRepose, this, null, new Callback<int, object, object[]>(this.OnEventHandler));
		return true;
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
		if (eventId == 139)
		{
			int srcID = (int)args[0];
			string icon = (string)args[1];
			string szName = (string)args[2];
			int nSroce = (int)args[3];
			this.OnMatchBeInviteHandle(srcID, icon, szName, nSroce);
		}
	}

	private void OnMatchBeInviteHandle(int srcID, string icon, string szName, int nSroce)
	{
		if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("CommonDialogTileWindow"))
		{
			return;
		}
		this.curInviteUserID = srcID;
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogTileWindow");
		string text = string.Format("{0} {1}", szName, LanguageDataProvider.GetValue(2119));
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnInviteDialog, new object[]
		{
			this.curInviteUserID,
			text,
			icon,
			szName,
			nSroce,
			10,
			new EventDelegate(new EventDelegate.Callback(this.OnMatchBeInviteAccpet)),
			new EventDelegate(new EventDelegate.Callback(this.OnMatchBeInviteRefuse))
		});
	}

	public void OnMatchBeInviteAccpet()
	{
		if (CollectionWindow.isShow || CollectionWindow.isShowBuyView)
		{
			BGManager.Inst.ApplyLastSkinConfig();
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.StartCSMatchInviteResp(true, this.curInviteUserID);
	}

	public void OnMatchBeInviteRefuse()
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.StartCSMatchInviteResp(false, this.curInviteUserID);
	}

	public void ChangeFriendState(int userID, int state)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == userID)
			{
				int onStats = this.myFriendList[i].onStats;
				if (state == this.myFriendList[i].onStats)
				{
					Debug.Log("-------好友前后状态相同不需要改变");
				}
				else
				{
					this.myFriendList[i].onStats = state;
					this.ReSort(i, onStats);
				}
				return;
			}
		}
	}

	public void OnRefreshFriendStats(int userID, bool bEnterRoom)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == userID)
			{
				int onStats = this.myFriendList[i].onStats;
				if (bEnterRoom)
				{
					this.myFriendList[i].onStats = 1;
				}
				else if (this.myFriendList[i].online)
				{
					this.myFriendList[i].onStats = 2;
				}
				else
				{
					this.myFriendList[i].onStats = 0;
				}
				this.ReSort(i, onStats);
				return;
			}
		}
	}

	private void ReSort(int index, int lastStatus)
	{
		if (lastStatus == this.myFriendList[index].onStats)
		{
			return;
		}
		if (lastStatus == 0 && this.myFriendList[index].onStats == 1)
		{
			this.leaveCount--;
			SimplePlayerData item = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.onlineCount + this.busyCount, item);
			this.busyCount++;
		}
		else if (lastStatus == 0 && this.myFriendList[index].onStats == 2)
		{
			this.leaveCount--;
			SimplePlayerData item2 = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.onlineCount, item2);
			this.onlineCount++;
		}
		else if (lastStatus == 1 && this.myFriendList[index].onStats == 0)
		{
			this.busyCount--;
			SimplePlayerData item3 = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.myFriendList.Count, item3);
			this.leaveCount++;
		}
		else if (lastStatus == 1 && this.myFriendList[index].onStats == 2)
		{
			this.busyCount--;
			SimplePlayerData item4 = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.onlineCount, item4);
			this.onlineCount++;
		}
		else if (lastStatus == 2 && this.myFriendList[index].onStats == 1)
		{
			this.onlineCount--;
			SimplePlayerData item5 = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.onlineCount + this.busyCount, item5);
			this.busyCount++;
		}
		else if (lastStatus == 2 && this.myFriendList[index].onStats == 0)
		{
			this.onlineCount--;
			SimplePlayerData item6 = this.myFriendList[index];
			this.myFriendList.RemoveAt(index);
			this.myFriendList.Insert(this.myFriendList.Count, item6);
			this.leaveCount++;
		}
		Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnFriendStateNotice, null);
	}

	public void Tick(float interval)
	{
	}

	public void Destroy()
	{
		this.Release();
	}

	public void Release()
	{
		this.onlineCount = 0;
		this.busyCount = 0;
		this.leaveCount = 0;
		this.myFriendList.Clear();
	}

	public void AddMyFollow(SimplePlayerData sPlayer)
	{
		if (!this.IsMyFriend(sPlayer))
		{
			if (sPlayer.onStats == 0)
			{
				this.leaveCount++;
			}
			if (sPlayer.onStats == 1)
			{
				this.busyCount++;
			}
			if (sPlayer.onStats == 2)
			{
				this.onlineCount++;
			}
			if (this.myFriendList.Count == 0)
			{
				this.myFriendList.Add(sPlayer);
				return;
			}
			int onStats = sPlayer.onStats;
			if (onStats != 0)
			{
				if (onStats != 1)
				{
					if (onStats == 2)
					{
						this.myFriendList.Insert(0, sPlayer);
					}
				}
				else
				{
					this.myFriendList.Insert(this.onlineCount, sPlayer);
				}
			}
			else
			{
				this.myFriendList.Insert(this.myFriendList.Count, sPlayer);
			}
		}
	}

	public void DelMyFollow(SimplePlayerData sPlayer)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == sPlayer.userId)
			{
				this.myFriendList.RemoveAt(i);
				return;
			}
		}
	}

	public bool IsMyFriend(SimplePlayerData player)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == player.userId)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsMyFriend(int playerID)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == playerID)
			{
				return true;
			}
		}
		return false;
	}

	public SimplePlayerData GetFriend(int userID)
	{
		for (int i = 0; i < this.myFriendList.Count; i++)
		{
			if (this.myFriendList[i].userId == userID)
			{
				return this.myFriendList[i];
			}
		}
		return null;
	}

	public List<SimplePlayerData> myFriendList;

	private int curInviteUserID = -1;

	private int onlineCount;

	private int busyCount;

	private int leaveCount;
}
