using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CRoom2V2Window : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.UpdateMoney);
		base.RegisterEvent(EventId.OnMatchInit);
		base.RegisterEvent(EventId.OnMatchUpdate);
		base.RegisterEvent(EventId.OnMatchQuit);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendMembersWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchInviteOptype, new object[]
		{
			1
		});
	}

	public override void OnHide()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendMembersWindow");
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.UpdateMoney)
		{
			this.playerMoney.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		}
		else if (eventId == EventId.OnMatchInit)
		{
			this.matchId = (string)args[0];
			this.roomId = (string)args[1];
			IList<UserData> list = (IList<UserData>)args[2];
			IList<int> list2 = (IList<int>)args[3];
			this.hostId = (int)args[4];
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				PlayerData playerData = new PlayerData();
				if (playerData.userId > 0)
				{
					num++;
				}
				playerData.Init(list[i]);
				int num2 = list2[i];
				this.allPlayers[num2] = playerData;
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(playerData.userId, true);
			}
			this.SetPage();
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
		}
		else if (eventId == EventId.OnMatchUpdate)
		{
			IList<UserData> list3 = (IList<UserData>)args[0];
			IList<int> list4 = (IList<int>)args[1];
			IList<int> list5 = (IList<int>)args[2];
			for (int j = 0; j < list5.Count; j++)
			{
				int num3 = list5[j];
				if (this.allPlayers[num3] != null && this.allPlayers[num3].userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
				{
					Solarmax.Singleton<UISystem>.Get().HideAllWindow();
					Solarmax.Singleton<UISystem>.Get().ShowWindow("PvPRoomWindow");
				}
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(this.allPlayers[num3].userId, false);
				this.allPlayers[num3] = null;
			}
			for (int k = 0; k < list3.Count; k++)
			{
				PlayerData playerData2 = new PlayerData();
				playerData2.Init(list3[k]);
				int num4 = list4[k];
				this.allPlayers[num4] = playerData2;
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(playerData2.userId, true);
			}
			this.SetPage();
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
		}
		else if (eventId == EventId.OnMatchQuit)
		{
			ErrCode errCode = (ErrCode)args[0];
			int num5 = (int)args[1];
			if (num5 > 0 && Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != num5)
			{
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(num5, false);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
				return;
			}
			if (errCode == ErrCode.EC_NotMaster)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
			}
			else if (errCode != ErrCode.EC_Ok)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, new object[]
				{
					errCode
				}), 1f);
			}
			else if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("PvPRoomWindow");
			}
		}
	}

	public void OnTwoClick()
	{
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			if (this.allPlayers[i] != null)
			{
				num++;
			}
		}
		if (num < 2)
		{
			Tips.Make(LanguageDataProvider.GetValue(912));
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.MatchComplete();
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
		EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
		EventId id = EventId.OnCommonDialog;
		object[] array = new object[3];
		array[0] = 2;
		array[1] = LanguageDataProvider.GetValue(907);
		array[2] = new EventDelegate(delegate()
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(-1);
		});
		instance.FireEvent(id, array);
	}

	private void SetPlayerInfo(GameObject go, PlayerData pd)
	{
		go.SetActive(true);
		NetTexture component = go.transform.Find("IconB").GetComponent<NetTexture>();
		UILabel component2 = go.transform.Find("Label").GetComponent<UILabel>();
		GameObject gameObject = go.transform.Find("Score").gameObject;
		UILabel component3 = go.transform.Find("Score/Score").GetComponent<UILabel>();
		GameObject gameObject2 = go.transform.Find("fangzhu").gameObject;
		if (pd == null)
		{
			component.gameObject.SetActive(false);
			component2.gameObject.SetActive(false);
			gameObject.SetActive(false);
			return;
		}
		component.gameObject.SetActive(true);
		component2.gameObject.SetActive(true);
		gameObject.SetActive(true);
		gameObject2.SetActive(true);   // ther is only one fangzhu flag, so always active
		//if (pd.userId == this.hostId)
		//{
		//	gameObject2.SetActive(true);
		//}
		//else
		//{
		//	gameObject2.SetActive(false);
		//}
        component.picUrl = pd.icon;
		component2.text = pd.name;
		component3.text = pd.score.ToString();
	}

	public void SetPage()
	{
		for (int i = 0; i < this.allPlayers.Length; i++)
		{
			this.SetPlayerInfo(this.playerGos[i], this.allPlayers[i]);
		}
	}

	public UILabel playerMoney;

	public GameObject[] playerGos;

	private string matchId;

	private string roomId;

	private int hostId;

	private PlayerData[] allPlayers = new PlayerData[2];
}
