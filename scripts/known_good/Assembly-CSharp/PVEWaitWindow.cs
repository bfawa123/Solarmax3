using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class PVEWaitWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMatchInit);
		base.RegisterEvent(EventId.OnMatchUpdate);
		base.RegisterEvent(EventId.OnMatchQuit);
		base.RegisterEvent(EventId.OnPveWaitWindow);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendMembersWindow");
		this.startBtn.gameObject.SetActive(false);
		if (!string.IsNullOrEmpty(this.matchId) || !string.IsNullOrEmpty(this.roomId))
		{
			this.SetModelPage();
			this.SetPage();
		}
	}

	public override void OnHide()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendMembersWindow");
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnMatchInit)
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
			}
			this.CooperaitonType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
			this.SetModelPage();
			this.SetPage();
		}
		else if (eventId == EventId.OnMatchUpdate)
		{
			IList<UserData> list3 = (IList<UserData>)args[0];
			IList<int> list4 = (IList<int>)args[1];
			IList<int> list5 = (IList<int>)args[2];
			IList<bool> list6 = (IList<bool>)args[3];
			IList<int> list7 = (IList<int>)args[4];
			IList<int> list8 = (IList<int>)args[5];
			if (args.Length == 7)
			{
				this.hostId = (int)args[6];
			}
			for (int j = 0; j < list5.Count; j++)
			{
				int num3 = list5[j];
				if (this.allPlayers[num3] != null && this.allPlayers[num3].userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
				{
					Solarmax.Singleton<UISystem>.Instance.HideWindow("RoomWaitWindow");
					Solarmax.Singleton<UISystem>.Instance.ShowWindow("CreateRoomWindow");
					if (list6[j])
					{
						Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(909), 1f);
					}
				}
				this.allPlayers[num3] = null;
			}
			for (int k = 0; k < list3.Count; k++)
			{
				PlayerData playerData2 = new PlayerData();
				playerData2.Init(list3[k]);
				int num4 = list4[k];
				this.allPlayers[num4] = playerData2;
			}
			for (int l = 0; l < list7.Count; l++)
			{
				Solarmax.Singleton<AudioManger>.Get().PlayEffect("onOpen");
				int num5 = list7[l];
				int num6 = list8[l];
				PlayerData playerData3 = this.allPlayers[num5];
				this.allPlayers[num5] = this.allPlayers[num6];
				this.allPlayers[num6] = playerData3;
			}
			this.SetPage();
		}
		else if (eventId == EventId.OnMatchQuit)
		{
			ErrCode errCode = (ErrCode)args[0];
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
				if (this.playerNum == 2 || this.playerNum == 3 || this.playerNum == 4)
				{
					Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(this.playerNum);
				}
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationLevelWindow");
			}
		}
		else if (eventId == EventId.OnPveWaitWindow)
		{
			this.playerNum = (int)args[0];
			this.matchId = (string)args[1];
			this.hostId = (int)args[2];
			if (this.playerNum == 2 || this.playerNum == 3 || this.playerNum == 4)
			{
				Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(this.playerNum);
			}
			if (this.playerNum == 2)
			{
				this.CooperaitonType = CooperationType.CT_2vPC;
			}
			else if (this.playerNum == 3)
			{
				this.CooperaitonType = CooperationType.CT_3vPC;
			}
			else if (this.playerNum == 4)
			{
				this.CooperaitonType = CooperationType.CT_4vPC;
			}
			this.SetModelPage();
			this.SetPage();
		}
	}

	public void SetPage()
	{
		this.SetUIPage();
		for (int i = 0; i < this.allPlayers.Length; i++)
		{
			this.SetPlayerInfo(this.playerGos[i], this.allPlayers[i], this.hostId);
		}
		this.startBtn.gameObject.SetActive(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == this.hostId);
	}

	private void SetUIPage()
	{
		if (this.CooperaitonType == CooperationType.CT_2vPC)
		{
			int num = this.player2H.Length;
			for (int i = 0; i < num; i++)
			{
				this.playerGos[i] = this.player2H[i];
			}
		}
		else if (this.CooperaitonType == CooperationType.CT_3vPC)
		{
			int num2 = this.player3H.Length;
			for (int j = 0; j < num2; j++)
			{
				this.playerGos[j] = this.player3H[j];
			}
		}
		else if (this.CooperaitonType == CooperationType.CT_4vPC)
		{
			int num3 = this.player4H.Length;
			for (int k = 0; k < num3; k++)
			{
				this.playerGos[k] = this.player4H[k];
			}
		}
	}

	private void SetModelPage()
	{
		if (this.page2H != null)
		{
			this.page2H.SetActive(false);
		}
		if (this.page3H != null)
		{
			this.page3H.SetActive(false);
		}
		if (this.page4H != null)
		{
			this.page4H.SetActive(false);
		}
		if (this.CooperaitonType == CooperationType.CT_2vPC)
		{
			if (this.page2H != null)
			{
				this.page2H.SetActive(true);
			}
		}
		else if (this.CooperaitonType == CooperationType.CT_3vPC)
		{
			if (this.page3H != null)
			{
				this.page3H.SetActive(true);
			}
		}
		else if (this.CooperaitonType == CooperationType.CT_4vPC)
		{
			if (this.page4H != null)
			{
				this.page4H.SetActive(true);
			}
		}
	}

	public void SetPlayerInfo(GameObject go, PlayerData pd, int hostId)
	{
		NetTexture component = go.transform.Find("Portrait/IconB").GetComponent<NetTexture>();
		GameObject gameObject = go.transform.Find("delete").gameObject;
		UILabel component2 = go.transform.Find("name").GetComponent<UILabel>();
		if (pd == null)
		{
			component.gameObject.SetActive(false);
			gameObject.gameObject.SetActive(false);
			component2.gameObject.SetActive(false);
		}
		else
		{
			component.picUrl = pd.icon;
			component.gameObject.SetActive(true);
			bool active = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == hostId && pd.userId != hostId;
			gameObject.SetActive(active);
			if (pd.userId == hostId)
			{
				component2.text = LanguageDataProvider.Format(908, new object[]
				{
					pd.name
				});
			}
			else
			{
				component2.text = pd.name;
			}
			component2.gameObject.SetActive(true);
		}
	}

	public void OnDeleteClick()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.hostId)
		{
			return;
		}
		string name = UIButton.current.gameObject.transform.parent.name;
		if (!name.StartsWith("player"))
		{
			return;
		}
		int num = int.Parse(name.Substring(6));
		int userId = this.allPlayers[num].userId;
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(userId);
	}

	public void OnBackClick()
	{
	}

	public void OnStartClick()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.hostId)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 4; i++)
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

	public GameObject[] playerGos;

	public GameObject[] player2H;

	public GameObject[] player3H;

	public GameObject[] player4H;

	public UIButton startBtn;

	public GameObject page2H;

	public GameObject page3H;

	public GameObject page4H;

	private string roomId;

	private string matchId;

	private int playerNum;

	private PlayerData[] allPlayers = new PlayerData[4];

	private int hostId;

	private Color enableColor = new Color(0.8784314f, 0.7647059f, 0.8117647f);

	private CooperationType CooperaitonType = CooperationType.CT_Null;
}
