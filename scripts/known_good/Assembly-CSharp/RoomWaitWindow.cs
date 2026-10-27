using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class RoomWaitWindow : BaseWindow
{
	private static bool IsCoopPvE(CooperationType type)
	{
		return type == CooperationType.CT_2vPC || type == CooperationType.CT_3vPC || type == CooperationType.CT_4vPC;
	}

	private static int GetPlayerNumFromSubType(CooperationType type)
	{
		if (type == CooperationType.CT_1v1 || type == CooperationType.CT_2vPC)
		{
			return 2;
		}
		if (type == CooperationType.CT_1v1v1 || type == CooperationType.CT_3vPC)
		{
			return 3;
		}
		if (type == CooperationType.CT_1v1v1v1 || type == CooperationType.CT_2v2 || type == CooperationType.CT_3v1 || type == CooperationType.CT_4vPC)
		{
			return 4;
		}
		return 4;
	}

	private static int GetCoopPlayerNum(CooperationType type)
	{
		if (type == CooperationType.CT_2vPC)
		{
			return 2;
		}
		if (type == CooperationType.CT_3vPC)
		{
			return 3;
		}
		if (type == CooperationType.CT_4vPC)
		{
			return 4;
		}
		return 4;
	}

	private static CooperationType GetCoopSubType(int coopPlayerNum)
	{
		if (coopPlayerNum == 2)
		{
			return CooperationType.CT_2vPC;
		}
		if (coopPlayerNum == 3)
		{
			return CooperationType.CT_3vPC;
		}
		if (coopPlayerNum == 4)
		{
			return CooperationType.CT_4vPC;
		}
		return CooperationType.CT_4vPC;
	}

	private static int ResolveCoopPlayerNumFromLevelId(string levelId)
	{
		if (string.IsNullOrEmpty(levelId))
		{
			return 0;
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(levelId);
		if (data == null || string.IsNullOrEmpty(data.chapter))
		{
			return 0;
		}
		List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(data.chapter);
		if (allData == null || allData.Count == 0)
		{
			return 0;
		}
		for (int i = 0; i < allData.Count; i++)
		{
			ChapterAssistConfig chapterAssistConfig = allData[i];
			if (chapterAssistConfig != null && (levelId == chapterAssistConfig.level1 || levelId == chapterAssistConfig.level2 || levelId == chapterAssistConfig.level3 || levelId == chapterAssistConfig.level4))
			{
				return chapterAssistConfig.playerNum;
			}
		}
		return 0;
	}

	private void SyncCoopPlayerNumFromBattle()
	{
		CooperationType cooperationType = this.GetBattleSubTypeSafe();
		if (!RoomWaitWindow.IsCoopPvE(cooperationType))
		{
			return;
		}
		int coopPlayerNum = this.playerNum;
		if (coopPlayerNum != 2 && coopPlayerNum != 3 && coopPlayerNum != 4)
		{
			coopPlayerNum = RoomWaitWindow.GetCoopPlayerNum(cooperationType);
		}
		Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(coopPlayerNum);
	}

	private void EnsureUIReferences()
	{
		if (this.playerGos == null || this.playerGos.Length == 0)
		{
			this.playerGos = new GameObject[7];
		}
		if (this.page2Coop == null)
		{
			this.page2Coop = this.FindChildGameObjectContains("page2Coop");
			if (this.page2Coop == null)
			{
				this.page2Coop = this.FindChildGameObjectContains("2Coop");
			}
		}
		if (this.page3Coop == null)
		{
			this.page3Coop = this.FindChildGameObjectContains("page3Coop");
			if (this.page3Coop == null)
			{
				this.page3Coop = this.FindChildGameObjectContains("3Coop");
			}
		}
		if (this.page4H == null)
		{
			this.page4H = this.FindChildGameObjectContains("page4H");
			if (this.page4H == null)
			{
				this.page4H = this.FindChildGameObjectContains("4H");
			}
		}
		if (this.player2Coop == null || this.player2Coop.Length == 0)
		{
			GameObject gameObject = this.page2Coop;
			if (gameObject == null)
			{
				gameObject = this.page3Coop;
			}
			if (gameObject == null)
			{
				gameObject = this.page4H;
			}
			this.player2Coop = this.FindPlayersInPage(gameObject, 2);
		}
		if (this.player3Coop == null || this.player3Coop.Length == 0)
		{
			GameObject gameObject2 = this.page3Coop;
			if (gameObject2 == null)
			{
				gameObject2 = this.page4H;
			}
			this.player3Coop = this.FindPlayersInPage(gameObject2, 3);
		}
		if (this.player4H == null || this.player4H.Length == 0)
		{
			this.player4H = this.FindPlayersInPage(this.page4H, 4);
		}
		if (this.startBtn == null)
		{
			this.startBtn = this.FindChildButtonContains("start");
			if (this.startBtn == null)
			{
				this.startBtn = this.FindChildButtonContains("Start");
			}
		}
	}

	private UIButton FindChildButtonContains(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		UIButton[] componentsInChildren = base.gameObject.GetComponentsInChildren<UIButton>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UIButton uibutton = componentsInChildren[i];
			if (uibutton != null && uibutton.gameObject != null && uibutton.gameObject.name != null && uibutton.gameObject.name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return uibutton;
			}
		}
		return null;
	}

	private GameObject FindChildGameObjectContains(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = componentsInChildren[i];
			if (transform != null && transform.gameObject != null && transform.name != null && transform.name.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private GameObject[] FindPlayersInPage(GameObject page, int expectedCount)
	{
		if (page == null)
		{
			return new GameObject[0];
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < expectedCount + 3; i++)
		{
			Transform transform = page.transform.Find("player" + i);
			if (transform != null)
			{
				list.Add(transform.gameObject);
			}
		}
		if (list.Count >= expectedCount)
		{
			return list.ToArray();
		}
		list.Clear();
		Transform[] componentsInChildren = page.GetComponentsInChildren<Transform>(true);
		SortedDictionary<int, GameObject> sortedDictionary = new SortedDictionary<int, GameObject>();
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			Transform transform2 = componentsInChildren[j];
			if (transform2 == null || string.IsNullOrEmpty(transform2.name) || !transform2.name.StartsWith("player"))
			{
				continue;
			}
			Transform transform3 = transform2.Find("Portrait");
			if (transform3 == null)
			{
				continue;
			}
			int num;
			if (!int.TryParse(transform2.name.Substring(6), out num))
			{
				continue;
			}
			if (!sortedDictionary.ContainsKey(num))
			{
				sortedDictionary.Add(num, transform2.gameObject);
			}
		}
		foreach (KeyValuePair<int, GameObject> keyValuePair in sortedDictionary)
		{
			list.Add(keyValuePair.Value);
			if (list.Count >= expectedCount)
			{
				break;
			}
		}
		return list.ToArray();
	}

	private SortedDictionary<int, GameObject> FindAllPlayerSlots()
	{
		SortedDictionary<int, GameObject> sortedDictionary = new SortedDictionary<int, GameObject>();
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Transform transform = componentsInChildren[i];
			if (transform == null || string.IsNullOrEmpty(transform.name) || !transform.name.StartsWith("player"))
			{
				continue;
			}
			int key;
			if (!int.TryParse(transform.name.Substring(6), out key))
			{
				continue;
			}
			if (sortedDictionary.ContainsKey(key))
			{
				continue;
			}
			Transform transform2 = transform.Find("Portrait");
			if (transform2 == null)
			{
				continue;
			}
			sortedDictionary.Add(key, transform.gameObject);
		}
		int num = 0;
		while (sortedDictionary.ContainsKey(num))
		{
			num++;
		}
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			Transform transform3 = componentsInChildren[j];
			if (transform3 == null)
			{
				continue;
			}
			Transform transform4 = transform3.Find("Portrait");
			Transform transform5 = transform3.Find("name");
			Transform transform6 = transform3.Find("delete");
			if (transform4 == null || transform5 == null || transform6 == null)
			{
				continue;
			}
			if (sortedDictionary.ContainsValue(transform3.gameObject))
			{
				continue;
			}
			sortedDictionary[num] = transform3.gameObject;
			num++;
			if (num > 20)
			{
				break;
			}
		}
		return sortedDictionary;
	}

	private void SetActiveUpwards(GameObject go, bool active)
	{
		if (go == null)
		{
			return;
		}
		Transform transform = go.transform;
		while (transform != null)
		{
			GameObject gameObject = transform.gameObject;
			if (gameObject != null && gameObject.activeSelf != active)
			{
				gameObject.SetActive(active);
			}
			if (gameObject == base.gameObject)
			{
				break;
			}
			transform = transform.parent;
		}
	}

	private void ForceActivateLayout()
	{
		for (int i = 0; i < this.watchIndex; i++)
		{
			if (this.playerGos != null && i >= 0 && i < this.playerGos.Length)
			{
				this.SetActiveUpwards(this.playerGos[i], true);
			}
		}
		if (this.startBtn != null)
		{
			this.SetActiveUpwards(this.startBtn.gameObject, true);
		}
	}

	private void Awake()
	{
		this.roomLock.gameObject.GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnLockRoom);
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnMatchInit);
		base.RegisterEvent(EventId.OnMatchUpdate);
		base.RegisterEvent(EventId.OnMatchQuit);
		base.RegisterEvent(EventId.OnFriendLoadResultforMatch);
		base.RegisterEvent(EventId.OnModifyRoomNameResult);
		base.RegisterEvent(EventId.OnModifyRoomLockResult);
		base.RegisterEvent(EventId.OnFriendFollowResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.EnsureUIReferences();
		this.roomIdLabel.text = string.Empty;
		this.SetModelPage();
		this.SetUIPage();
		for (int i = 4; i < this.playerGos.Length; i++)
		{
			GameObject gameObject = this.playerGos[i];
			if (gameObject != null)
			{
				Transform transform = gameObject.transform.Find("Portrait");
				if (transform != null)
				{
					UIEventListener component = transform.GetComponent<UIEventListener>();
					if (component != null)
					{
						component.onClick = new UIEventListener.VoidDelegate(this.OnWatchPlayerClick);
					}
				}
			}
		}
		for (int j = 0; j < this.player1V1.Length; j++)
		{
			this.player1V1[j].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		for (int k = 0; k < this.player2V2.Length; k++)
		{
			this.player2V2[k].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		for (int l = 0; l < this.player3H.Length; l++)
		{
			this.player3H[l].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		for (int m = 0; m < this.player4H.Length; m++)
		{
			this.player4H[m].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		for (int n = 0; n < this.player2Coop.Length; n++)
		{
			this.player2Coop[n].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		for (int o = 0; o < this.player3Coop.Length; o++)
		{
			this.player3Coop[o].transform.Find("Portrait").GetComponent<UIEventListener>().onClick = new UIEventListener.VoidDelegate(this.OnNormalPlayerClick);
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendMembersWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchInviteOptype, new object[]
		{
			1
		});
		this.startBtn.gameObject.SetActive(false);
		this.showWatchPlayer = false;
		this.ShowWatch(true);
		if (!string.IsNullOrEmpty(this.matchId) || !string.IsNullOrEmpty(this.roomId))
		{
			this.SetPage();
			this.inputLabel.text = this.roomName;
		}
		base.InvokeRepeating("UpdateConnect", 0f, 5f);
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
			if (RoomWaitWindow.IsCoopPvE(Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType))
			{
				this.mapId = (string)args[1];
				this.roomId = this.matchId;
			}
			else
			{
				this.roomId = this.matchId;
			}
			IList<UserData> list = (IList<UserData>)args[2];
			IList<int> list2 = (IList<int>)args[3];
			this.hostId = (int)args[4];
			BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
			CooperationType cooperationType = (battleData != null) ? battleData.battleSubType : CooperationType.CT_Null;
			this.playerNum = RoomWaitWindow.GetPlayerNumFromSubType(cooperationType);
			int matchPlayerNum = (battleData != null) ? battleData.matchPlayerNum : 0;
			if (matchPlayerNum == 2 || matchPlayerNum == 3 || matchPlayerNum == 4)
			{
				this.playerNum = matchPlayerNum;
				if (RoomWaitWindow.IsCoopPvE(cooperationType) && battleData != null)
				{
					battleData.battleSubType = RoomWaitWindow.GetCoopSubType(matchPlayerNum);
				}
			}
			if (RoomWaitWindow.IsCoopPvE(cooperationType))
			{
				int num2 = RoomWaitWindow.ResolveCoopPlayerNumFromLevelId(this.mapId);
				if (num2 == 2 || num2 == 3 || num2 == 4)
				{
					this.playerNum = num2;
					if (battleData != null)
					{
						battleData.battleSubType = RoomWaitWindow.GetCoopSubType(num2);
					}
				}
			}
			this.SyncCoopPlayerNumFromBattle();
			bool flag = (bool)args[6];
			this.roomName = (string)args[7];
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
			this.SetModelPage();
			this.SetPage();
			this.inputLabel.text = this.roomName;
			if (flag)
			{
				this.roomLockOn.gameObject.SetActive(true);
			}
			else
			{
				this.roomLockOn.gameObject.SetActive(false);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
			MonoSingleton<FlurryAnalytis>.Instance.FlurryPVPBattleMatchEvent("1", this.matchId, "0", num.ToString(), this.roomId);
			MiGameAnalytics.MiAnalyticsPVPBattleMatchEvent("1", this.matchId, "0", num.ToString(), this.roomId);
			AppsFlyerTool.FlyerPVPBattleStartEvent();
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
					CooperationType cooperationType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
					if (RoomWaitWindow.IsCoopPvE(cooperationType))
					{
						Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(RoomWaitWindow.GetCoopPlayerNum(cooperationType));
						if (!string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow))
						{
							Solarmax.Singleton<UISystem>.Get().ShowWindow(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow);
						}
						else if (Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter != null)
						{
							Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationLevelWindow");
							Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
							{
								1,
								Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id
							});
						}
						else
						{
							Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
						}
					}
					else if (!string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow))
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow);
					}
					else
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
					}
					if (list6[j])
					{
						Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(909), 1f);
					}
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
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
		}
		else if (eventId == EventId.OnMatchQuit)
		{
			ErrCode errCode = (ErrCode)args[0];
			int num7 = (int)args[1];
			if (num7 > 0 && Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != num7)
			{
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(num7, false);
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
					CooperationType cooperationType2 = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
					if (RoomWaitWindow.IsCoopPvE(cooperationType2))
				{
						Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.SetCoopPlayerNum(RoomWaitWindow.GetCoopPlayerNum(cooperationType2));
					if (!string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow))
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow);
					}
					else if (Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter != null)
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationLevelWindow");
						Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
						{
							1,
							Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id
						});
					}
					else
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow("CooperationRoomWindow");
					}
				}
				else if (!string.IsNullOrEmpty(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow))
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow(Solarmax.Singleton<LocalPlayer>.Get().HomeWindow);
				}
				else
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
				}
			}
		}
		else if (eventId == EventId.OnFriendLoadResultforMatch)
		{
			for (int m = 0; m < this.allPlayers.Length; m++)
			{
				if (this.allPlayers[m] != null)
				{
					Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(this.allPlayers[m].userId, true);
				}
			}
			this.SetPage();
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
		}
		else if (eventId == EventId.OnFriendFollowResult)
		{
			this.SetPage();
		}
		else if (eventId == EventId.OnModifyRoomNameResult)
		{
			string text = (string)args[0];
			string text2 = (string)args[1];
			if (text.Equals(this.matchId))
			{
				this.roomName = text2;
				this.inputLabel.text = text2;
			}
		}
		else if (eventId == EventId.OnModifyRoomLockResult)
		{
			string text3 = (string)args[0];
			bool active = (bool)args[1];
			if (text3.Equals(this.matchId))
			{
				this.roomLockOn.gameObject.SetActive(active);
			}
		}
	}

	public void SetPage()
	{
		this.EnsureUIReferences();
		this.SetModelPage();
		this.SetUIPage();
		this.SetUITitle();
		this.roomIdLabel.text = this.roomId.ToString();
		if (this.roomLabel != null)
		{
			this.roomLabel.text = this.matchId;
		}
		this.watchCount = 0;
		this.playerCount = 0;
		int num = this.watchIndex;
		for (int i = 0; i < this.allPlayers.Length; i++)
		{
			if (i < this.watchIndex)
			{
				if (this.allPlayers[i] != null)
				{
					this.playerCount++;
				}
				this.SetPlayerInfo(this.playerGos[i], this.allPlayers[i], this.hostId);
			}
			else if (i >= this.watchIndex && num < this.playerGos.Length)
			{
				if (this.allPlayers[i] != null)
				{
					this.watchCount++;
				}
				this.SetPlayerInfo(this.playerGos[num], this.allPlayers[i], this.hostId);
				num++;
			}
		}
		this.startBtn.gameObject.SetActive(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == this.hostId);
		this.watchNumLabel.text = LanguageDataProvider.Format(910, new object[]
		{
			this.watchCount,
			3
		});
	}

	private void SetUITitle()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			this.roomTile.text = LanguageDataProvider.GetValue(2039);
		}
		else
		{
			this.roomTile.text = this.GetWindowTitle(this.mapId);
		}
	}

	private void SetUIPage()
	{
		CooperationType cooperationType = this.GetBattleSubTypeSafe();
		if (cooperationType == CooperationType.CT_1v1)
		{
			int num = this.player1V1.Length;
			for (int i = 0; i < num; i++)
			{
				this.playerGos[i] = this.player1V1[i];
			}
		}
		else if (cooperationType == CooperationType.CT_2vPC)
		{
			int num = this.player1V1.Length;
			for (int i = 0; i < num; i++)
			{
				this.playerGos[i] = this.player1V1[i];
			}
		}
		else if (cooperationType == CooperationType.CT_2v2)
		{
			int num2 = this.player2V2.Length;
			for (int j = 0; j < num2; j++)
			{
				this.playerGos[j] = this.player2V2[j];
			}
		}
		else if (cooperationType == CooperationType.CT_1v1v1)
		{
			int num3 = this.player3H.Length;
			for (int k = 0; k < num3; k++)
			{
				this.playerGos[k] = this.player3H[k];
			}
		}
		else if (cooperationType == CooperationType.CT_3vPC)
		{
			int num3 = this.player3H.Length;
			for (int k = 0; k < num3; k++)
			{
				this.playerGos[k] = this.player3H[k];
			}
		}
		else if (cooperationType == CooperationType.CT_1v1v1v1 || cooperationType == CooperationType.CT_4vPC || cooperationType == CooperationType.CT_3v1)
		{
			int num4 = this.player4H.Length;
			for (int l = 0; l < num4; l++)
			{
				this.playerGos[l] = this.player4H[l];
			}
		}
		else
		{
			int coopPlayerNum = this.playerNum;
			if (coopPlayerNum == 2 && this.player2Coop != null && this.player2Coop.Length > 0)
			{
				for (int m = 0; m < this.player2Coop.Length; m++)
				{
					this.playerGos[m] = this.player2Coop[m];
				}
			}
			else if (coopPlayerNum == 3 && this.player3Coop != null && this.player3Coop.Length > 0)
			{
				for (int n = 0; n < this.player3Coop.Length; n++)
				{
					this.playerGos[n] = this.player3Coop[n];
				}
			}
			else if (this.player4H != null && this.player4H.Length > 0)
			{
				for (int o = 0; o < this.player4H.Length; o++)
				{
					this.playerGos[o] = this.player4H[o];
				}
			}
		}
		this.ForceActivateLayout();
		if (this.hostId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			this.inputSearch.gameObject.GetComponent<BoxCollider>().enabled = false;
		}
		else
		{
			this.inputSearch.gameObject.GetComponent<BoxCollider>().enabled = true;
		}
	}

	private void SetModelPage()
	{
		if (this.page1V1 != null)
		{
			this.page1V1.SetActive(false);
		}
		if (this.page2V2 != null)
		{
			this.page2V2.SetActive(false);
		}
		if (this.page3H != null)
		{
			this.page3H.SetActive(false);
		}
		if (this.page4H != null)
		{
			this.page4H.SetActive(false);
		}
		if (this.page2Coop != null)
		{
			this.page2Coop.SetActive(false);
		}
		if (this.page3Coop != null)
		{
			this.page3Coop.SetActive(false);
		}
		CooperationType cooperationType = this.GetBattleSubTypeSafe();
		if (cooperationType == CooperationType.CT_1v1)
		{
			this.watchIndex = 2;
			if (this.page1V1 != null)
			{
				this.page1V1.SetActive(true);
			}
		}
		else if (cooperationType == CooperationType.CT_2vPC)
		{
			this.watchIndex = 2;
			if (this.page1V1 != null)
			{
				this.page1V1.SetActive(true);
			}
		}
		else if (cooperationType == CooperationType.CT_2v2)
		{
			this.watchIndex = 4;
			if (this.page2V2 != null)
			{
				this.page2V2.SetActive(true);
			}
		}
		else if (cooperationType == CooperationType.CT_1v1v1)
		{
			this.watchIndex = 3;
			if (this.page3H != null)
			{
				this.page3H.SetActive(true);
			}
		}
		else if (cooperationType == CooperationType.CT_3vPC)
		{
			this.watchIndex = 3;
			if (this.page3H != null)
			{
				this.page3H.SetActive(true);
			}
		}
		else if (cooperationType == CooperationType.CT_1v1v1v1 || cooperationType == CooperationType.CT_4vPC || cooperationType == CooperationType.CT_3v1)
		{
			this.watchIndex = 4;
			if (this.page4H != null)
			{
				this.page4H.SetActive(true);
			}
		}
		else
		{
			int coopPlayerNum = this.playerNum;
			if (coopPlayerNum == 2)
			{
				this.watchIndex = 2;
				if (this.page2Coop != null)
				{
					this.page2Coop.SetActive(true);
				}
			}
			else if (coopPlayerNum == 3)
			{
				this.watchIndex = 3;
				if (this.page3Coop != null)
				{
					this.page3Coop.SetActive(true);
				}
			}
			else
			{
				this.watchIndex = 4;
				if (this.page4H != null)
				{
					this.page4H.SetActive(true);
				}
			}
		}
		this.ForceActivateLayout();
	}

	private CooperationType GetBattleSubTypeSafe()
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		if (battleData != null)
		{
			return battleData.battleSubType;
		}
		if (this.playerNum == 2)
		{
			return CooperationType.CT_2vPC;
		}
		if (this.playerNum == 3)
		{
			return CooperationType.CT_3vPC;
		}
		if (this.playerNum == 4)
		{
			return CooperationType.CT_4vPC;
		}
		return CooperationType.CT_Null;
	}

	public void SetPlayerInfo(GameObject go, PlayerData pd, int hostId)
	{
		PortraitTemplate component = go.transform.Find("Portrait").GetComponent<PortraitTemplate>();
		NetTexture component2 = go.transform.Find("Portrait/IconB").GetComponent<NetTexture>();
		GameObject gameObject = go.transform.Find("delete").gameObject;
		UILabel component3 = go.transform.Find("name").GetComponent<UILabel>();
		if (pd == null)
		{
			component.Load("select_head_1.png", null, null);
			component2.gameObject.SetActive(false);
			gameObject.gameObject.SetActive(false);
			component3.gameObject.SetActive(false);
		}
		else
		{
			component.Load(pd.icon, null, null);
			component2.gameObject.SetActive(true);
			bool active = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == hostId && pd.userId != hostId;
			gameObject.SetActive(active);
			if (pd.userId == hostId)
			{
				component3.text = LanguageDataProvider.Format(908, new object[]
				{
					pd.name
				});
			}
			else
			{
				component3.text = pd.name;
			}
			component3.gameObject.SetActive(true);
		}
		Transform transform = go.transform.Find("addfriden");
		if (pd != null && transform != null)
		{
			GameObject gameObject2 = transform.gameObject;
			if (!Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(pd.userId) && pd.userId > 0 && pd.userId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				gameObject2.SetActive(true);
				UILabel component4 = gameObject2.transform.Find("useid").GetComponent<UILabel>();
				if (component4 != null)
				{
					component4.text = pd.userId.ToString();
				}
			}
			else
			{
				gameObject2.SetActive(false);
			}
		}
		if (pd == null && transform != null)
		{
			transform.gameObject.SetActive(false);
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
		if (num >= 4)
		{
			num -= 4 - this.watchIndex;
		}
		int userId = this.allPlayers[num].userId;
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(userId);
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

	public void OnStartClick()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.hostId)
		{
			return;
		}
		if (this.playerCount < 2)
		{
			Tips.Make(LanguageDataProvider.GetValue(912));
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.MatchComplete();
	}

	public void OnWatchClick()
	{
		this.ShowWatch(false);
	}

	public void OnLockRoom(GameObject go)
	{
		if (this.hostId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			Tips.Make(LanguageDataProvider.GetValue(2228));
			return;
		}
		if (!this.roomLockOn.gameObject.activeSelf)
		{
			this.roomLockOn.gameObject.SetActive(true);
			this.OnEditLockRoom(true);
		}
		else
		{
			this.roomLockOn.gameObject.SetActive(false);
			this.OnEditLockRoom(false);
		}
	}

	private void ShowWatch(bool first = false)
	{
		if (first)
		{
			for (int i = 0; i < this.watchPlayers.Length; i++)
			{
				TweenPosition tp = this.watchPlayers[i];
				tp.SetOnFinished(delegate()
				{
					tp.gameObject.SetActive(this.showWatchPlayer);
				});
				tp.gameObject.SetActive(false);
			}
		}
		else if (this.showWatchPlayer)
		{
			for (int j = 0; j < this.watchPlayers.Length; j++)
			{
				TweenPosition tweenPosition = this.watchPlayers[j];
				tweenPosition.ResetToBeginning();
				tweenPosition.from = new Vector3(this.firstWatchPlayerPosX + this.watchPlayerPosInterval * (float)j, 0f, 0f);
				tweenPosition.to = Vector3.zero;
				tweenPosition.PlayForward();
				tweenPosition.gameObject.SetActive(true);
			}
			this.showWatchPlayer = false;
		}
		else
		{
			for (int k = 0; k < this.watchPlayers.Length; k++)
			{
				TweenPosition tweenPosition2 = this.watchPlayers[k];
				tweenPosition2.ResetToBeginning();
				tweenPosition2.from = Vector3.zero;
				tweenPosition2.to = new Vector3(this.firstWatchPlayerPosX + this.watchPlayerPosInterval * (float)k, 0f, 0f);
				tweenPosition2.PlayForward();
				tweenPosition2.gameObject.SetActive(true);
			}
			this.showWatchPlayer = true;
		}
		if (this.showWatchPlayer)
		{
			this.watchAmmor.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			this.watchAmmor.transform.localScale = Vector3.one;
		}
	}

	private void OnWatchPlayerClick(GameObject go)
	{
		if (!this.IsCheckOnClick())
		{
			return;
		}
		int num = int.Parse(go.transform.parent.name.Substring(6));
		num = this.watchIndex + num - 4;
		if (num < this.allPlayers.Length && this.allPlayers[num] == null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestMatchMovePos(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId, num);
		}
	}

	private void OnNormalPlayerClick(GameObject go)
	{
		if (!this.IsCheckOnClick())
		{
			return;
		}
		Debug.Log("normal player click : " + go.transform.parent.name);
		int num = int.Parse(go.transform.parent.name.Substring(6));
		if (this.allPlayers[num] == null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestMatchMovePos(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId, num);
		}
	}

	public void OnInputClick()
	{
		string text = this.inputSearch.value.Trim();
		text = text.Replace('\r', ' ');
		text = text.Replace('\t', ' ');
		text = text.Replace('\n', ' ');
		while (this.EncodingTextLength(text) > 20)
		{
			text = text.Substring(0, text.Length - 1);
		}
		this.inputLabel.text = text;
	}

	public void OnModifyRoomName()
	{
		if (this.hostId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
		{
			Tips.Make(LanguageDataProvider.GetValue(2228));
			return;
		}
		string value = this.inputSearch.value;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		bool flag = Solarmax.Singleton<NameFilterConfigProvider>.Instance.nameCheck(value);
		if (flag)
		{
			Tips.Make(LanguageDataProvider.GetValue(1114));
			this.inputSearch.value = this.roomName;
			return;
		}
		this.ModifyRoomName(value);
	}

	private int EncodingTextLength(string s)
	{
		int num = 0;
		for (int i = 0; i < s.Length; i++)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(s.Substring(i, 1));
			num += ((bytes.Length <= 1) ? 1 : 2);
		}
		return num;
	}

	public void OnEditLockRoom(bool bLock)
	{
		CSEditWatchLock cseditWatchLock = new CSEditWatchLock();
		cseditWatchLock.match_id = this.roomId;
		cseditWatchLock.match_lock = bLock;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSEditWatchLock>(325, cseditWatchLock);
	}

	public void ModifyRoomName(string roomName)
	{
		CSEditWatchName cseditWatchName = new CSEditWatchName();
		cseditWatchName.match_id = this.roomId;
		cseditWatchName.match_name = roomName;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSEditWatchName>(323, cseditWatchName);
	}

	private bool IsCheckOnClick()
	{
		DateTime? dateTime = this.searchFriendTime;
		if (dateTime != null)
		{
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchFriendTime.Value;
			if (timeSpan.TotalSeconds < 5.0)
			{
				string message = string.Format(LanguageDataProvider.GetValue(1147), 5 - timeSpan.Seconds);
				Tips.Make(message);
				return false;
			}
		}
		this.searchFriendTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		return true;
	}

	private string GetWindowTitle(string matchid)
	{
		string text = string.Empty;
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchid);
		List<ChapterAssistConfig> allData = Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.GetAllData(data.chapter);
		if (allData != null && allData.Count > 0)
		{
			ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.chapter);
			if (data2 != null)
			{
				text = LanguageDataProvider.GetValue(data2.name);
			}
			for (int i = 0; i < allData.Count; i++)
			{
				if (matchid == allData[i].level1)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2104);
				}
				else if (matchid == allData[i].level2)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2105);
				}
				else if (matchid == allData[i].level3)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2106);
				}
				else if (matchid == allData[i].level4)
				{
					text = text + " - " + LanguageDataProvider.GetValue(allData[i].name);
					text = text + " - " + LanguageDataProvider.GetValue(2107);
				}
			}
		}
		return text;
	}

	public void OnAttentionClick(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		UILabel component = go.transform.Find("useid").GetComponent<UILabel>();
		if (component == null || string.IsNullOrEmpty(component.text))
		{
			return;
		}
		int userId = int.Parse(component.text);
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(userId, true);
	}

	public UILabel roomTile;

	public UILabel roomLabel;

	public UILabel roomIdLabel;

	public GameObject[] playerGos;

	public GameObject[] player1V1;

	public GameObject[] player2V2;

	public GameObject[] player3H;

	public GameObject[] player4H;

	public GameObject[] player2Coop;

	public GameObject[] player3Coop;

	public UIButton startBtn;

	public GameObject page1V1;

	public GameObject page2V2;

	public GameObject page3H;

	public GameObject page4H;

	public GameObject page2Coop;

	public GameObject page3Coop;

	public UISprite watchAmmor;

	public UILabel watchNumLabel;

	public TweenPosition[] watchPlayers;

	private string matchId;

	private string roomId;

	private string mapId;

	private int playerNum;

	private PlayerData[] allPlayers = new PlayerData[7];

	private int hostId;

	private Color enableColor = new Color(0.8784314f, 0.7647059f, 0.8117647f);

	private bool showWatchPlayer;

	private float firstWatchPlayerPosX = 190f;

	private float watchPlayerPosInterval = 140f;

	private int watchCount;

	private int watchIndex;

	private int playerCount;

	public UISprite roomLock;

	public UISprite roomLockOn;

	public UILabel inputLabel;

	public UIInput inputSearch;

	private DateTime? searchFriendTime;

	private string roomName = string.Empty;
}
