using System;
using System.Collections.Generic;
using System.Text;
using NetMessage;
using Solarmax;
using UnityEngine;

public class DuelRoomWindow : BaseWindow
{
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
		base.RegisterEvent(EventId.OnBackWindowName);
		this.CreateUI();
		return true;
	}

	private void CreateUI()
	{
		UIRoot uiroot = this.GetUIRoot();
		int screenW = Screen.width;
		int screenH = Screen.height;
		float halfW = screenW * 0.5f;
		float halfH = screenH * 0.5f;

		GameObject panelGo = new GameObject("Panel");
		panelGo.layer = uiroot.gameObject.layer;
		panelGo.transform.SetParent(uiroot.transform, false);
		UIPanel panel = panelGo.AddComponent<UIPanel>();
		panel.depth = 100;

		GameObject bgGo = new GameObject("Bg");
		bgGo.layer = uiroot.gameObject.layer;
		bgGo.transform.SetParent(panelGo.transform, false);
		UITexture bgTex = bgGo.AddComponent<UITexture>();
		bgTex.width = screenW;
		bgTex.height = screenH;
		bgTex.depth = 0;
		bgTex.color = new Color(0.05f, 0.08f, 0.15f, 0.95f);

		GameObject backGo = new GameObject("BtnBack");
		backGo.layer = uiroot.gameObject.layer;
		backGo.transform.SetParent(panelGo.transform, false);
		backGo.transform.localPosition = new Vector3(-halfW + 60f, halfH - 60f, 0f);
		UISprite backSpr = backGo.AddComponent<UISprite>();
		backSpr.width = 50;
		backSpr.height = 50;
		backSpr.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
		UIButton backBtn = backGo.AddComponent<UIButton>();
		backBtn.tweenTarget = backGo;
		backGo.AddComponent<BoxCollider>().size = new Vector3(50f, 50f, 0f);
		EventDelegate.Add(backBtn.onClick, new EventDelegate(this.OnBackClick));

		GameObject backIconGo = new GameObject("BackIcon");
		backIconGo.layer = uiroot.gameObject.layer;
		backIconGo.transform.SetParent(backGo.transform, false);
		UILabel backIconLbl = backIconGo.AddComponent<UILabel>();
		backIconLbl.text = "\u2190";
		backIconLbl.fontSize = 32;
		backIconLbl.alignment = NGUIText.Alignment.Center;
		backIconLbl.color = Color.white;
		backIconLbl.width = 44;
		backIconLbl.height = 44;

		GameObject roomTitleGo = new GameObject("RoomTitle");
		roomTitleGo.layer = uiroot.gameObject.layer;
		roomTitleGo.transform.SetParent(panelGo.transform, false);
		roomTitleGo.transform.localPosition = new Vector3(-halfW + 140f, halfH - 60f, 0f);
		this.roomTile = roomTitleGo.AddComponent<UILabel>();
		this.roomTile.text = "\u623f\u95f4";
		this.roomTile.fontSize = 32;
		this.roomTile.alignment = NGUIText.Alignment.Left;
		this.roomTile.color = new Color(0.85f, 0.85f, 0.85f);
		this.roomTile.width = 120;
		this.roomTile.height = 50;

		GameObject roomNameGo = new GameObject("RoomName");
		roomNameGo.layer = uiroot.gameObject.layer;
		roomNameGo.transform.SetParent(panelGo.transform, false);
		roomNameGo.transform.localPosition = new Vector3(-halfW + 120f, halfH - 130f, 0f);
		this.inputLabel = roomNameGo.AddComponent<UILabel>();
		this.inputLabel.text = "match-";
		this.inputLabel.fontSize = 22;
		this.inputLabel.alignment = NGUIText.Alignment.Left;
		this.inputLabel.color = new Color(0.7f, 0.7f, 0.7f);
		this.inputLabel.width = 300;
		this.inputLabel.height = 36;

		GameObject roomIdGo = new GameObject("RoomId");
		roomIdGo.layer = uiroot.gameObject.layer;
		roomIdGo.transform.SetParent(panelGo.transform, false);
		roomIdGo.transform.localPosition = new Vector3(-halfW + 120f, halfH - 170f, 0f);
		this.roomIdLabel = roomIdGo.AddComponent<UILabel>();
		this.roomIdLabel.text = "\u623f\u95f4\u53f7: ";
		this.roomIdLabel.fontSize = 20;
		this.roomIdLabel.alignment = NGUIText.Alignment.Left;
		this.roomIdLabel.color = new Color(0.6f, 0.6f, 0.6f);
		this.roomIdLabel.width = 300;
		this.roomIdLabel.height = 32;

		GameObject lockGo = new GameObject("RoomLock");
		lockGo.layer = uiroot.gameObject.layer;
		lockGo.transform.SetParent(panelGo.transform, false);
		lockGo.transform.localPosition = new Vector3(halfW - 200f, halfH - 130f, 0f);
		this.roomLock = lockGo.AddComponent<UISprite>();
		this.roomLock.width = 30;
		this.roomLock.height = 30;
		this.roomLock.color = new Color(0.5f, 0.5f, 0.5f);
		UIEventListener lockListener = lockGo.AddComponent<UIEventListener>();
		lockListener.onClick = new UIEventListener.VoidDelegate(this.OnLockRoom);
		lockGo.AddComponent<BoxCollider>().size = new Vector3(30f, 30f, 0f);

		GameObject lockOnGo = new GameObject("RoomLockOn");
		lockOnGo.layer = uiroot.gameObject.layer;
		lockOnGo.transform.SetParent(lockGo.transform, false);
		this.roomLockOn = lockOnGo.AddComponent<UISprite>();
		this.roomLockOn.width = 30;
		this.roomLockOn.height = 30;
		this.roomLockOn.color = new Color(1f, 0.8f, 0.2f);
		lockOnGo.SetActive(false);

		GameObject lockTextGo = new GameObject("LockText");
		lockTextGo.layer = uiroot.gameObject.layer;
		lockTextGo.transform.SetParent(panelGo.transform, false);
		lockTextGo.transform.localPosition = new Vector3(halfW - 140f, halfH - 130f, 0f);
		UILabel lockText = lockTextGo.AddComponent<UILabel>();
		lockText.text = "\u4ec5\u9650\u9080\u8bf7";
		lockText.fontSize = 18;
		lockText.alignment = NGUIText.Alignment.Left;
		lockText.color = new Color(0.6f, 0.6f, 0.6f);
		lockText.width = 120;
		lockText.height = 28;

		float slotSpacing = screenW * 0.28f;
		float slotStartX = -slotSpacing * 0.5f;
		float slotY = 40f;
		this.playerGos = new GameObject[this.maxPlayers];
		for (int i = 0; i < this.maxPlayers; i++)
		{
			GameObject slotGo = new GameObject("player" + i);
			slotGo.layer = uiroot.gameObject.layer;
			slotGo.transform.SetParent(panelGo.transform, false);
			slotGo.transform.localPosition = new Vector3(slotStartX + i * slotSpacing, slotY, 0f);

			GameObject portraitGo = new GameObject("Portrait");
			portraitGo.layer = uiroot.gameObject.layer;
			portraitGo.transform.SetParent(slotGo.transform, false);
			UISprite hexagonSpr = portraitGo.AddComponent<UISprite>();
			hexagonSpr.width = 110;
			hexagonSpr.height = 110;
			hexagonSpr.color = new Color(0.2f, 0.3f, 0.4f, 0.6f);
			UIEventListener portraitListener = portraitGo.AddComponent<UIEventListener>();
			int idx = i;
			portraitListener.onClick = delegate(GameObject go)
			{
				this.OnNormalPlayerClick(go);
			};
			portraitGo.AddComponent<BoxCollider>().size = new Vector3(110f, 110f, 0f);

			GameObject iconGo = new GameObject("IconB");
			iconGo.layer = uiroot.gameObject.layer;
			iconGo.transform.SetParent(portraitGo.transform, false);
			NetTexture netTex = iconGo.AddComponent<NetTexture>();
			iconGo.SetActive(false);

			GameObject nameGo = new GameObject("name");
			nameGo.layer = uiroot.gameObject.layer;
			nameGo.transform.SetParent(slotGo.transform, false);
			nameGo.transform.localPosition = new Vector3(0f, -75f, 0f);
			UILabel nameLbl = nameGo.AddComponent<UILabel>();
			nameLbl.fontSize = 20;
			nameLbl.alignment = NGUIText.Alignment.Center;
			nameLbl.color = Color.white;
			nameLbl.width = 160;
			nameLbl.height = 30;
			nameGo.SetActive(false);

			GameObject deleteGo = new GameObject("delete");
			deleteGo.layer = uiroot.gameObject.layer;
			deleteGo.transform.SetParent(slotGo.transform, false);
			deleteGo.transform.localPosition = new Vector3(45f, 45f, 0f);
			UISprite deleteSpr = deleteGo.AddComponent<UISprite>();
			deleteSpr.width = 28;
			deleteSpr.height = 28;
			deleteSpr.color = new Color(0.8f, 0.2f, 0.2f);
			UIButton deleteBtn = deleteGo.AddComponent<UIButton>();
			deleteBtn.tweenTarget = deleteGo;
			deleteGo.AddComponent<BoxCollider>().size = new Vector3(28f, 28f, 0f);
			EventDelegate.Add(deleteBtn.onClick, new EventDelegate(this.OnDeleteClick));
			deleteGo.SetActive(false);

			GameObject addFriendGo = new GameObject("addfriden");
			addFriendGo.layer = uiroot.gameObject.layer;
			addFriendGo.transform.SetParent(slotGo.transform, false);
			addFriendGo.transform.localPosition = new Vector3(0f, -105f, 0f);
			UISprite addFriendSpr = addFriendGo.AddComponent<UISprite>();
			addFriendSpr.width = 80;
			addFriendSpr.height = 28;
			addFriendSpr.color = new Color(0.2f, 0.6f, 0.3f);
			UIButton addFriendBtn = addFriendGo.AddComponent<UIButton>();
			addFriendBtn.tweenTarget = addFriendGo;
			addFriendGo.AddComponent<BoxCollider>().size = new Vector3(80f, 28f, 0f);
			EventDelegate.Add(addFriendBtn.onClick, new EventDelegate(this.OnAttentionClick));

			GameObject addFriendLblGo = new GameObject("useid");
			addFriendLblGo.layer = uiroot.gameObject.layer;
			addFriendLblGo.transform.SetParent(addFriendGo.transform, false);
			UILabel addFriendLbl = addFriendLblGo.AddComponent<UILabel>();
			addFriendLbl.fontSize = 14;
			addFriendLbl.alignment = NGUIText.Alignment.Center;
			addFriendLbl.color = Color.white;
			addFriendLbl.width = 70;
			addFriendLbl.height = 22;
			addFriendGo.SetActive(false);

			this.playerGos[i] = slotGo;
		}

		GameObject watchGo = new GameObject("WatchPanel");
		watchGo.layer = uiroot.gameObject.layer;
		watchGo.transform.SetParent(panelGo.transform, false);
		watchGo.transform.localPosition = new Vector3(0f, -halfH + 220f, 0f);

		GameObject watchIconGo = new GameObject("WatchIcon");
		watchIconGo.layer = uiroot.gameObject.layer;
		watchIconGo.transform.SetParent(watchGo.transform, false);
		watchIconGo.transform.localPosition = new Vector3(-80f, 0f, 0f);
		UISprite watchIconSpr = watchIconGo.AddComponent<UISprite>();
		watchIconSpr.width = 36;
		watchIconSpr.height = 36;
		watchIconSpr.color = new Color(0.5f, 0.5f, 0.5f);

		GameObject watchNumGo = new GameObject("WatchNum");
		watchNumGo.layer = uiroot.gameObject.layer;
		watchNumGo.transform.SetParent(watchGo.transform, false);
		watchNumGo.transform.localPosition = new Vector3(-30f, 0f, 0f);
		this.watchNumLabel = watchNumGo.AddComponent<UILabel>();
		this.watchNumLabel.text = "\u89c2\u6218(0/3)";
		this.watchNumLabel.fontSize = 18;
		this.watchNumLabel.alignment = NGUIText.Alignment.Left;
		this.watchNumLabel.color = new Color(0.6f, 0.6f, 0.6f);
		this.watchNumLabel.width = 120;
		this.watchNumLabel.height = 28;

		GameObject startGo = new GameObject("BtnStart");
		startGo.layer = uiroot.gameObject.layer;
		startGo.transform.SetParent(panelGo.transform, false);
		startGo.transform.localPosition = new Vector3(0f, -halfH + 140f, 0f);
		UISprite startSpr = startGo.AddComponent<UISprite>();
		startSpr.width = 220;
		startSpr.height = 60;
		startSpr.color = new Color(0.15f, 0.55f, 0.75f, 0.9f);
		this.startBtn = startGo.AddComponent<UIButton>();
		this.startBtn.tweenTarget = startGo;
		startGo.AddComponent<BoxCollider>().size = new Vector3(220f, 60f, 0f);
		EventDelegate.Add(this.startBtn.onClick, new EventDelegate(this.OnStartClick));

		GameObject startLblGo = new GameObject("StartLabel");
		startLblGo.layer = uiroot.gameObject.layer;
		startLblGo.transform.SetParent(startGo.transform, false);
		UILabel startLbl = startLblGo.AddComponent<UILabel>();
		startLbl.text = "\u5f00\u59cb\u5bf9\u6218";
		startLbl.fontSize = 26;
		startLbl.alignment = NGUIText.Alignment.Center;
		startLbl.color = Color.white;
		startLbl.width = 200;
		startLbl.height = 50;

		this.tweenAlpha = TweenAlpha.Begin(panelGo, 0.3f, 0f);
		this.tweenAlpha.from = 0f;
		this.tweenAlpha.to = 1f;
		this.tweenAlpha.enabled = false;
	}

	private UIRoot GetUIRoot()
	{
		UIRoot uiroot = UnityEngine.Object.FindObjectOfType<UIRoot>();
		if (uiroot == null)
		{
			uiroot = base.gameObject.AddComponent<UIRoot>();
			uiroot.scalingStyle = UIRoot.Scaling.Constrained;
			uiroot.manualHeight = 720;
		}
		return uiroot;
	}

	public override void OnShow()
	{
		base.OnShow();
		if (this.tweenAlpha != null)
		{
			this.tweenAlpha.enabled = true;
			this.tweenAlpha.ResetToBeginning();
			this.tweenAlpha.PlayForward();
		}
		this.roomIdLabel.text = string.Empty;
		this.startBtn.gameObject.SetActive(false);
		Solarmax.Singleton<UISystem>.Get().ShowWindow("FriendMembersWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchInviteOptype, new object[] { 1 });
		base.InvokeRepeating("UpdateConnect", 0f, 5f);
	}

	public override void OnHide()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("FriendMembersWindow");
		base.CancelInvoke("UpdateConnect");
	}

	private void UpdateConnect()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnBackWindowName)
		{
			this.backWindow = (string)args[0];
		}
		else if (eventId == EventId.OnMatchInit)
		{
			this.matchId = (string)args[0];
			this.roomId = this.matchId;
			IList<UserData> list = (IList<UserData>)args[2];
			IList<int> list2 = (IList<int>)args[3];
			this.hostId = (int)args[4];
			bool flag = (bool)args[6];
			this.roomName = (string)args[7];
			int num = 0;
			for (int i = 0; i < list.Count; i++)
			{
				PlayerData playerData = new PlayerData();
				if (playerData.userId > 0) num++;
				playerData.Init(list[i]);
				int num2 = list2[i];
				this.allPlayers[num2] = playerData;
				Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(playerData.userId, true);
			}
			this.SetPage();
			this.inputLabel.text = this.roomName;
			if (flag) this.roomLockOn.gameObject.SetActive(true);
			else this.roomLockOn.gameObject.SetActive(false);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefreshFriendStats, new object[0]);
		}
		else if (eventId == EventId.OnMatchUpdate)
		{
			IList<UserData> list3 = (IList<UserData>)args[0];
			IList<int> list4 = (IList<int>)args[1];
			IList<int> list5 = (IList<int>)args[2];
			if (args.Length == 7) this.hostId = (int)args[6];
			for (int j = 0; j < list5.Count; j++)
			{
				int num3 = list5[j];
				if (this.allPlayers[num3] != null && this.allPlayers[num3].userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
				{
					Solarmax.Singleton<UISystem>.Get().HideAllWindow();
					Solarmax.Singleton<UISystem>.Get().ShowWindow(this.backWindow);
					return;
				}
				if (this.allPlayers[num3] != null)
				{
					Solarmax.Singleton<FriendDataHandler>.Instance.OnRefreshFriendStats(this.allPlayers[num3].userId, false);
				}
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
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.Format(901, new object[] { errCode }), 1f);
			}
			else if (errCode == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				string homeWindow = Solarmax.Singleton<LocalPlayer>.Get().HomeWindow;
				if (!string.IsNullOrEmpty(homeWindow))
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow(homeWindow);
				}
				else
				{
					Solarmax.Singleton<UISystem>.Get().ShowWindow(this.backWindow);
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
		this.roomIdLabel.text = "\u623f\u95f4\u53f7: " + this.roomId;
		this.watchCount = 0;
		this.playerCount = 0;
		for (int i = 0; i < this.allPlayers.Length; i++)
		{
			if (i < this.maxPlayers)
			{
				if (this.allPlayers[i] != null) this.playerCount++;
				this.SetPlayerInfo(this.playerGos[i], this.allPlayers[i], this.hostId);
			}
		}
		this.startBtn.gameObject.SetActive(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == this.hostId);
		this.watchNumLabel.text = LanguageDataProvider.Format(910, new object[] { this.watchCount, 3 });
	}

	public void SetPlayerInfo(GameObject go, PlayerData pd, int hostId)
	{
		Transform portraitTrans = go.transform.Find("Portrait");
		Transform iconTrans = go.transform.Find("Portrait/IconB");
		Transform deleteTrans = go.transform.Find("delete");
		Transform nameTrans = go.transform.Find("name");
		Transform addFriendTrans = go.transform.Find("addfriden");

		if (pd == null)
		{
			if (iconTrans != null) iconTrans.gameObject.SetActive(false);
			if (deleteTrans != null) deleteTrans.gameObject.SetActive(false);
			if (nameTrans != null) nameTrans.gameObject.SetActive(false);
			if (addFriendTrans != null) addFriendTrans.gameObject.SetActive(false);
			return;
		}

		if (iconTrans != null)
		{
			iconTrans.gameObject.SetActive(true);
			NetTexture netTex = iconTrans.GetComponent<NetTexture>();
			if (netTex != null)
			{
				string text = pd.icon;
				if (!text.EndsWith(".png")) text += ".png";
				netTex.picUrl = text;
			}
		}

		if (deleteTrans != null)
		{
			bool active = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == hostId && pd.userId != hostId;
			deleteTrans.gameObject.SetActive(active);
		}

		if (nameTrans != null)
		{
			nameTrans.gameObject.SetActive(true);
			UILabel nameLbl = nameTrans.GetComponent<UILabel>();
			if (nameLbl != null)
			{
				if (pd.userId == hostId)
				{
					nameLbl.text = LanguageDataProvider.Format(908, new object[] { pd.name });
				}
				else
				{
					nameLbl.text = pd.name;
				}
			}
		}

		if (addFriendTrans != null)
		{
			GameObject addFriendGo = addFriendTrans.gameObject;
			if (!Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(pd.userId) && pd.userId > 0 && pd.userId != Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
			{
				addFriendGo.SetActive(true);
				UILabel useidLbl = addFriendGo.transform.Find("useid")?.GetComponent<UILabel>();
				if (useidLbl != null) useidLbl.text = pd.userId.ToString();
			}
			else
			{
				addFriendGo.SetActive(false);
			}
		}
	}

	public void OnDeleteClick()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.hostId) return;
		string name = UIButton.current.gameObject.transform.parent.name;
		if (!name.StartsWith("player")) return;
		int num = int.Parse(name.Substring(6));
		if (num >= this.allPlayers.Length || this.allPlayers[num] == null) return;
		int userId = this.allPlayers[num].userId;
		Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(userId);
	}

	public void OnBackClick()
	{
		Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
		EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
		object[] array = new object[3];
		array[0] = 2;
		array[1] = LanguageDataProvider.GetValue(907);
		array[2] = new EventDelegate(delegate()
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.QuitMatch(-1);
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			string homeWindow = Solarmax.Singleton<LocalPlayer>.Get().HomeWindow;
			if (!string.IsNullOrEmpty(homeWindow))
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow(homeWindow);
			}
			else
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow(this.backWindow);
			}
		});
		instance.FireEvent(EventId.OnCommonDialog, array);
	}

	public void OnStartClick()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId != this.hostId) return;
		if (this.playerCount < 2)
		{
			Tips.Make(LanguageDataProvider.GetValue(912));
			return;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.MatchComplete();
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

	public void OnEditLockRoom(bool bLock)
	{
		CSEditWatchLock cseditWatchLock = new CSEditWatchLock();
		cseditWatchLock.match_id = this.roomId;
		cseditWatchLock.match_lock = bLock;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSEditWatchLock>(325, cseditWatchLock);
	}

	private void OnNormalPlayerClick(GameObject go)
	{
		if (!this.IsCheckOnClick()) return;
		string parentName = go.transform.parent.name;
		if (!parentName.StartsWith("player")) return;
		int num = int.Parse(parentName.Substring(6));
		if (num < this.allPlayers.Length && this.allPlayers[num] == null)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestMatchMovePos(Solarmax.Singleton<LocalPlayer>.Get().playerData.userId, num);
		}
	}

	public void OnAttentionClick()
	{
		GameObject currentGo = UIButton.current.gameObject;
		if (currentGo == null) return;
		Transform useidTrans = currentGo.transform.Find("useid");
		if (useidTrans == null) return;
		UILabel useidLbl = useidTrans.GetComponent<UILabel>();
		if (useidLbl == null || string.IsNullOrEmpty(useidLbl.text)) return;
		int userId = int.Parse(useidLbl.text);
		Solarmax.Singleton<NetSystem>.Instance.helper.FriendFollow(userId, true);
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

	private TweenAlpha tweenAlpha;

	public UILabel roomTile;

	public UILabel roomIdLabel;

	public UILabel inputLabel;

	public GameObject[] playerGos;

	public UIButton startBtn;

	public UISprite roomLock;

	public UISprite roomLockOn;

	public UILabel watchNumLabel;

	private string matchId;

	private string roomId;

	private string roomName = string.Empty;

	private int hostId;

	private int playerCount;

	private int watchCount;

	private int maxPlayers = 2;

	private PlayerData[] allPlayers = new PlayerData[7];

	private DateTime? searchFriendTime;

	public string backWindow = "HomeWindow";
}
