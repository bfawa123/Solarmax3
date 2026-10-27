using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class BattleWindow : BaseWindow
{
	private MapConfig GetRuntimeMapConfig()
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		MapConfig mapConfig = (battleData != null) ? battleData.currentTable : null;
		if (mapConfig == null)
		{
			mapConfig = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId.ToString());
		}
		return mapConfig;
	}

	private void Start()
	{
		this.battleCoundown.transform.parent.gameObject.SetActive(false);
		this.percent = 1f;
		this.SetPercent();
		UIRoot nguiroot = Solarmax.Singleton<UISystem>.Get().GetNGUIRoot();
		this.sensitive *= nguiroot.pixelSizeAdjustment;
		this.curAddSpeedSegment = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.curProcduceIndex;
		this.battleTimeMax = this.GetBattleTimeMaxSec();
		this.FindAimsLabel();
		for (int i = 0; i < this.unitdown.Length; i++)
		{
			UIEventListener component = this.unitdown[i].GetComponent<UIEventListener>();
			component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
		for (int j = 0; j < this.unitleft.Length; j++)
		{
			UIEventListener component2 = this.unitleft[j].GetComponent<UIEventListener>();
			component2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component2.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
		for (int k = 0; k < this.unitright.Length; k++)
		{
			UIEventListener component3 = this.unitright[k].GetComponent<UIEventListener>();
			component3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component3.onClick, new UIEventListener.VoidDelegate(this.OnSelectNumber));
		}
	}

	private int GetBattleTimeMaxSec()
	{
		int num = int.Parse(Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(4)) * 60;
		CooperationType battleSubType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
		if (battleSubType == CooperationType.CT_4vPC || battleSubType == CooperationType.CT_3vPC || battleSubType == CooperationType.CT_2vPC)
		{
			MapConfig data = this.GetRuntimeMapConfig();
			if (data != null && data.coopTimeLimitMin > 0)
			{
				num = data.coopTimeLimitMin * 60;
			}
		}
		if (battleSubType == CooperationType.CT_3v1)
		{
			MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId.ToString());
			if (data2 != null && data2.pvpTimeLimitMin > 0)
			{
				num = data2.pvpTimeLimitMin * 60;
			}
		}
		return num;
	}

	/// <summary>
	/// 地图切换后刷新时间上限（合作模式按当前 mapconfig 重新计算）
	/// </summary>
	public void RefreshBattleTime()
	{
		this.battleTimeMax = this.GetBattleTimeMaxSec();
	}

	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.NoticeSelfTeam);
		base.RegisterEvent(EventId.OnBattleDisconnect);
		base.RegisterEvent(EventId.RequestUserResult);
		base.RegisterEvent(EventId.OnSelfDied);
		base.RegisterEvent(EventId.OnPopulationUp);
		base.RegisterEvent(EventId.OnPopulationDown);
		base.RegisterEvent(EventId.PlayerGiveUp);
		base.RegisterEvent(EventId.PlayerDead);
		base.RegisterEvent(EventId.OnFinishedBattle);
		base.RegisterEvent(EventId.OnPVPBattleAccelerate);
		base.RegisterEvent(EventId.OnPVPBattleBoom);
		base.RegisterEvent(EventId.PlayerSpeekStart);
		base.RegisterEvent(EventId.PlayerSpeekEnd);
		base.RegisterEvent(EventId.DoubleTouch);
		base.RegisterEvent(EventId.BattleMarkTarget);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.FindAimsLabel();
		this.ShowAllAliveTip();
		this.percentPicWidth = (float)this.percetPic.width;
		this.giveupForQuit = false;
		this.giveupLabel.text = LanguageDataProvider.GetValue(418);
		string data = Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(2);
		this.giveupCountTime = (float)int.Parse(data);
		this.enableVoiceChat = false;
		GameType gameType = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType;
		if (gameType == GameType.PVP || gameType == GameType.League)
		{
			this.enableVoiceChat = true;
		}
		this.InitFightProgressOption();
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		Color color = team.color;
		color.a = 0.7f;
		this.populationValueLabel.text = string.Format("{0}/{1}", team.current, team.currentMax);
        this.populationValueLabel.color = color;
        this.populationLabel.color = color;
        this.ShowModeUI();
		this.ShowPlayerInfo();
		this.TimerProc();
		UIEventListener component = this.ProcessAram.GetComponent<UIEventListener>();
		component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(this.OnSelectProcessAram));
		this.clockAniStr = "Battle_battleframe_clock1";
		this.EnterVoiceChat(team.groupID);
		this.RefreshVoiceChatPanel();
		this.RefreshSpeakers();
	}

	private bool EnterVoiceChat(int voiceTeamId)
	{
		if (!this.enableVoiceChat)
		{
			return false;
		}
		string voiceRoomId = Solarmax.Singleton<BattleSystem>.Instance.battleData.voiceRoomId;
		if (string.IsNullOrEmpty(voiceRoomId))
		{
			return false;
		}
		int[] voiceRoomToken = Solarmax.Singleton<BattleSystem>.Instance.battleData.voiceRoomToken;
		if (voiceRoomToken == null || voiceRoomToken.Length == 0)
		{
			return false;
		}
		byte[] array = new byte[voiceRoomToken.Length];
		for (int i = 0; i < voiceRoomToken.Length; i++)
		{
			array[i] = (byte)voiceRoomToken[i];
		}
		VoiceEngine.EnterRoom(voiceRoomId, voiceTeamId, false, array);
		Solarmax.Singleton<LocalSettingStorage>.Get().enableSpeaker = VoiceEngine.IsSpeakerEnable();
		Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone = VoiceEngine.IsMicrophoneEnable();
		if (!VoiceEngine.IsSpeakerEnable())
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel = false;
			Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel = false;
		}
		else
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel = VoiceEngine.IsRoomChannelEnable();
			Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel = !Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel;
		}
		this.LblRoomChannel.SetActive(false);
		this.LblTeamChannel.SetActive(false);
		return true;
	}

	private void ShowModeUI()
	{
		this.player.SetActive(false);
		this.player2V2.SetActive(false);
		for (int i = 0; i < this.playerFrames.Length; i++)
		{
			this.playerFrames[i] = null;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			int num = this.pvps.Length;
			for (int j = 0; j < num; j++)
			{
				this.playerFrames[j] = this.pvps[j];
			}
			this.player2V2.SetActive(true);
		}
		else
		{
			int num2 = this.players.Length;
			for (int k = 0; k < num2; k++)
			{
				this.playerFrames[k] = this.players[k];
			}
			this.player.SetActive(true);
		}
	}

	private void ShowPlayerInfo()
	{
		for (int i = 0; i < this.playerFrames.Length; i++)
		{
			if (this.playerFrames[i] != null)
			{
				this.playerFrames[i].SetActive(false);
			}
		}
		List<Team> list = new List<Team>();
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
		{
			int num = -1;
			int num2 = 0;
			for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
			{
				Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)j);
				if (team != null && team.playerData.userId != -1)
				{
					if (num2 >= 4)
					{
						break;
					}
					if (team.playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
					{
						num = team.groupID;
					}
					list.Add(team);
					num2++;
				}
			}
			list.Sort((Team a, Team b) => num == 0 ? a.groupID.CompareTo(b.groupID) : b.groupID.CompareTo(a.groupID));
			int num3 = -1;
			int num4 = -1;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].playerData.userId == Solarmax.Singleton<LocalPlayer>.Get().playerData.userId)
				{
					num3 = k;
				}
				else if (list[k].groupID == num)
				{
					num4 = k;
				}
			}
			if (num3 != -1 && num4 != -1 && num3 != num4 && num3 < num4)
			{
				Team value = list[num3];
				list[num3] = list[num4];
				list[num4] = value;
			}
		}
		else
		{
			int num5 = 0;
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
			if (team2 == null)
			{
				return;
			}
			for (int l = 1; l < LocalPlayer.MaxTeamNum; l++)
			{
				Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)l);
				if (team3 != null && team3.team != team2.team)
				{
					if (team3.playerData.userId != -1)
					{
						if (num5 >= 3)
						{
							break;
						}
						list.Add(team3);
						num5++;
					}
				}
			}
		}
		this.mapTeam2Index.Clear();
		for (int m = 0; m < list.Count; m++)
		{
			Team team4 = list[m];
			GameObject gameObject = this.playerFrames[m];
			gameObject.SetActive(true);
			this.SetPlayerInfo(gameObject, team4);
			this.mapTeam2Index.Add((int)team4.team, m);
			Transform transform = gameObject.transform.Find("speaker");
			this.speakers.Add(team4.playerData.userId, transform.GetComponent<SequenceFrame>());
			transform.GetComponent<UISprite>().color = new Color(team4.color.r, team4.color.g, team4.color.b, 1f);
		}
		int num6 = (list.Count <= 3) ? list.Count : 3;
		Vector3 localPosition = this.VoiceChatPanel.transform.localPosition;
		this.VoiceChatPanel.transform.localPosition = new Vector3(localPosition.x + (float)((3 - num6) * 220), localPosition.y, localPosition.z);
	}

	private void SetPlayerInfo(GameObject go, Team teamTmp)
	{
		Color color = teamTmp.color;
		color.a = 1f;
		UISprite component = go.GetComponent<UISprite>();
		NetTexture component2 = go.transform.Find("icon").GetComponent<NetTexture>();
		UILabel component3 = go.transform.Find("name").GetComponent<UILabel>();
		UILabel component4 = go.transform.Find("src").GetComponent<UILabel>();
		UISprite component5 = go.transform.Find("bei").GetComponent<UISprite>();
		component.color = color;
		component3.color = color;
		component4.color = color;
		component5.color = color;
		component4.text = string.Format("{0}", teamTmp.playerData.score);
		component3.text = teamTmp.playerData.name;
		if (!teamTmp.playerData.icon.EndsWith(".png"))
		{
			PlayerData playerData = teamTmp.playerData;
			playerData.icon += ".png";
		}
		component2.picUrl = teamTmp.playerData.icon;
	}

	private void OnSelectProcessAram(GameObject go)
	{
		if (!Solarmax.Singleton<BattleSystem>.Instance.canOperation)
		{
			return;
		}
		Vector2 vector = UICamera.currentCamera.WorldToScreenPoint(this.percentleft.gameObject.transform.position);
		Vector2 vector2 = UICamera.currentCamera.WorldToScreenPoint(this.percentright.gameObject.transform.position);
		Vector2 lastEventPosition = UICamera.lastEventPosition;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
		{
			this.percent = (lastEventPosition.x - vector.x) / (vector2.x - vector.x);
		}
		else
		{
			this.percent = (lastEventPosition.y - vector.y) / (vector2.y - vector.y);
		}
		if (this.percent < 0f)
		{
			this.percent = 0f;
		}
		if (this.percent > 1f)
		{
			this.percent = 1f;
		}
		this.percent = Mathf.Round(this.percent * 100f) / 100f;
		this.SetPercent();
	}

	public override void OnHide()
	{
		GuideManager.ClearGuideData();
		VoiceEngine.ExitRoom();
		foreach (KeyValuePair<int, PrefabInstance> keyValuePair in this.targetMarkers)
		{
			PrefabInstance value = keyValuePair.Value;
			PrefabPool.FreeInstance(ref value);
		}
		this.targetMarkers.Clear();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.NoticeSelfTeam)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay)
			{
				return;
			}
			MapConfig data = this.GetRuntimeMapConfig();
			if (data == null)
			{
				return;
			}
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
			if (team == null)
			{
				return;
			}
			for (int i = 0; i < data.mpcList.Count; i++)
			{
				MapPlayerConfig mapPlayerConfig = data.mpcList[i];
				if (mapPlayerConfig == null || mapPlayerConfig.camption <= 0 || mapPlayerConfig.camption >= LocalPlayer.MaxTeamNum)
				{
					continue;
				}
				Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)mapPlayerConfig.camption);
				if (team2 == null)
				{
					continue;
				}
				if (team2 == team)
				{
					Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(mapPlayerConfig.tag);
					Solarmax.Singleton<EffectManager>.Get().ShowGuideEffect(node, true);
				}
				else if (team.IsFriend(team2.groupID))
				{
					Node node2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(mapPlayerConfig.tag);
					Solarmax.Singleton<EffectManager>.Get().ShowGuideEffect(node2, false);
				}
			}
			TouchHandler.ShowingGuidEffect = true;
		}
		else if (eventId == EventId.OnBattleDisconnect)
		{
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("ReconnectWindow");
		}
		else if (eventId == EventId.RequestUserResult)
		{
			ErrCode errCode = (ErrCode)args[0];
			if (errCode == ErrCode.EC_NeedResume)
			{
				Solarmax.Singleton<NetSystem>.Instance.helper.ReconnectResume(Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame() / 5 + 1);
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.Reset();
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
			}
		}
		else if (eventId == EventId.OnSelfDied)
		{
			this.giveupLabel.text = LanguageDataProvider.GetValue(419);
			this.giveupForQuit = true;
		}
		else if (eventId == EventId.OnFinishedBattle)
		{
			this.battleCoundown.gameObject.SetActive(false);
		}
		else if (eventId == EventId.OnPopulationUp)
		{
			int num = (int)args[0];
			int num2 = (int)args[1];
			int num3 = (int)args[2];
			this.populationValueLabel.text = string.Format("{0} / {1}", num, num2);
			this.popLableValue1.text = this.populationValueLabel.text;
			this.popLable1.color = new Color(0f, 255f, 0f, 1f);
			this.popLableValue1.color = new Color(0f, 255f, 0f, 1f);
			this.popLableAdd.text = string.Format("+{0}", num3);
			this.popLableAdd.color = new Color(0.2f, 1f, 0.2f, 1f);
			this.popLableAdd.transform.localPosition = this.popLableValue1.transform.localPosition + new Vector3(this.popLableValue1.printedSize.x + 30f, 0f, 0f);
		}
		else if (eventId == EventId.OnPopulationDown)
		{
			int num4 = (int)args[0];
			int num5 = (int)args[1];
			int num6 = (int)args[2];
			this.populationValueLabel.text = string.Format("{0} / {1}", num4, num5);
			this.popLableValue1.text = this.populationValueLabel.text;
			this.popLable1.color = new Color(255f, 0f, 0f, 1f);
			this.popLableValue1.color = new Color(255f, 0f, 0f, 1f);
			this.popLableAdd.text = string.Format("-{0}", num6);
			this.popLableAdd.color = new Color(1f, 0.2f, 0.2f, 1f);
			this.popLableAdd.transform.localPosition = this.popLableValue1.transform.localPosition + new Vector3(this.popLableValue1.printedSize.x + 30f, 0f, 0f);
		}
		else if (eventId == EventId.PlayerGiveUp)
		{
			TEAM team3 = (TEAM)args[0];
			this.OperateTeamGiveUP((int)team3);
		}
		else if (eventId == EventId.PlayerDead)
		{
			TEAM team4 = (TEAM)args[0];
			this.OperateTeamDead((int)team4);
		}
		else if (eventId == EventId.OnPVPBattleAccelerate)
		{
			Tips.Make(LanguageDataProvider.GetValue(1116), 3f);
			this.clockAniStr = "Battle_battleframe_clock2";
			this.clockAni.Play("Battle_battleframe_clock2");
		}
		else if (eventId == EventId.OnPVPBattleBoom)
		{
			Tips.Make(LanguageDataProvider.GetValue(1117), 3f);
			this.clockAniStr = "Battle_battleframe_clock3";
			this.clockAni.Play("Battle_battleframe_clock3");
		}
		else if (eventId == EventId.PlayerSpeekStart)
		{
			int key = (int)args[0];
			SequenceFrame sequenceFrame = null;
			if (this.speakers.TryGetValue(key, out sequenceFrame))
			{
				sequenceFrame.Play(0f);
			}
		}
		else if (eventId == EventId.PlayerSpeekEnd)
		{
			int key2 = (int)args[0];
			SequenceFrame sequenceFrame2 = null;
			if (this.speakers.TryGetValue(key2, out sequenceFrame2))
			{
				sequenceFrame2.Stop();
			}
		}
		else if (eventId == EventId.DoubleTouch)
		{
			Node node3 = (Node)args[0];
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestBattleMarkTarget(node3.tag);
		}
		else if (eventId == EventId.BattleMarkTarget)
		{
			int userId = (int)args[0];
			string nodeTag = (string)args[1];
			this.ShowOrHideTargetMarker(userId, nodeTag);
		}
	}

	private void ShowOrHideTargetMarker(int userId, string nodeTag)
	{
		Node node = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.nodeManager.GetNode(nodeTag);
		if (node == null)
		{
			return;
		}
		PrefabInstance prefabInstance = null;
		if (!this.targetMarkers.TryGetValue(userId, out prefabInstance))
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(userId);
			if (team == null)
			{
				return;
			}
			prefabInstance = PrefabPool.NewInstane("TargetMarker");
			prefabInstance.SetColor(new Color(team.color.r, team.color.g, team.color.b, 1f));
			prefabInstance.GetOrAddComponent<FollowNode>().Follow(node.tag, new Vector3(0f, node.GetHalfNodeSize(), 0f));
			this.targetMarkers.Add(userId, prefabInstance);
			return;
		}
		else
		{
			FollowNode orAddComponent = prefabInstance.GetOrAddComponent<FollowNode>();
			if (orAddComponent.IsTarget(node.tag))
			{
				orAddComponent.ClearFollow();
				this.targetMarkers.Remove(userId);
				PrefabPool.FreeInstance(ref prefabInstance);
				return;
			}
			orAddComponent.Follow(node.tag, new Vector3(0f, node.GetHalfNodeSize(), 0f));
			return;
		}
	}

	private void Update()
	{
		if (this.popLable1.alpha > 0f)
		{
			UIRect uirect = this.popLableValue1;
			float alpha = this.popLable1.alpha - Time.deltaTime * 0.5f;
			this.popLable1.alpha = alpha;
			uirect.alpha = alpha;
			if (this.popLable1.alpha < 0f)
			{
				UIRect uirect2 = this.popLableValue1;
				alpha = 0f;
				this.popLable1.alpha = alpha;
				uirect2.alpha = alpha;
			}
		}
		if (this.popLableAdd.alpha > 0f)
		{
			this.popLableAdd.alpha -= Time.deltaTime * 0.5f;
			if (this.popLableAdd.alpha < 0f)
			{
				this.popLableAdd.alpha = 0f;
			}
		}
		this.fUpdateBattleTime += Time.deltaTime;
		if (this.fUpdateBattleTime > 0.02f)
		{
			this.fUpdateBattleTime = 0f;
			this.UpdateBattleTime();
			this.UpdateCountdownTime();
			this.UpdateBattleTimeColor();
			this.UpdateSurvivalAim();
		}
	}

	private void FindAimsLabel()
	{
		if (this.aimsLabel != null)
		{
			return;
		}
		UILabel[] componentsInChildren = base.GetComponentsInChildren<UILabel>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			if (uilabel != null && string.Equals(uilabel.gameObject.name, "aims", StringComparison.OrdinalIgnoreCase))
			{
				this.aimsLabel = uilabel;
				return;
			}
		}
		string value = LanguageDataProvider.GetValue(1128);
		string value2 = LanguageDataProvider.GetValue(1129);
		string value3 = LanguageDataProvider.GetValue(1130);
		string value4 = LanguageDataProvider.GetValue(1131);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UILabel uilabel = componentsInChildren[i];
			string text = (uilabel != null) ? uilabel.text : null;
			if (!string.IsNullOrEmpty(text) && (text == value || text.IndexOf(value, StringComparison.Ordinal) >= 0 || text == value2 || text.IndexOf(value2, StringComparison.Ordinal) >= 0 || this.MatchFormatText(text, value3) || this.MatchFormatText(text, value4)))
			{
				this.aimsLabel = uilabel;
				return;
			}
		}
	}

	private bool MatchFormatText(string text, string template)
	{
		if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(template))
		{
			return false;
		}
		int num = template.IndexOf("{0}", StringComparison.Ordinal);
		if (num < 0)
		{
			return text.IndexOf(template, StringComparison.Ordinal) >= 0;
		}
		string value = template.Substring(0, num);
		string value2 = template.Substring(num + 3);
		int num2 = string.IsNullOrEmpty(value) ? 0 : text.IndexOf(value, StringComparison.Ordinal);
		if (num2 < 0)
		{
			return false;
		}
		int startIndex = num2 + value.Length;
		if (string.IsNullOrEmpty(value2))
		{
			return true;
		}
		return text.IndexOf(value2, startIndex, StringComparison.Ordinal) >= 0;
	}

	private void UpdateSurvivalAim()
	{
		if (this.aimsLabel == null)
		{
			this.FindAimsLabel();
			if (this.aimsLabel == null)
			{
				return;
			}
		}
		string winType = Solarmax.Singleton<BattleSystem>.Instance.battleData.winType;
		if (winType != "alive" && winType != "surival" && winType != "survival")
		{
			return;
		}
		float num;
		if (!float.TryParse(Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1, out num) || num <= 0f)
		{
			return;
		}
		int num2 = Mathf.FloorToInt(Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
		int num3 = Mathf.CeilToInt(num);
		if (num2 < 0)
		{
			num2 = 0;
		}
		if (num2 > num3)
		{
			num2 = num3;
		}
		this.aimsLabel.text = string.Format(LanguageDataProvider.GetValue(9001000), num2, num3);
	}

	private bool IsCoopAllAlive()
	{
		CooperationType battleSubType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
		if (battleSubType != CooperationType.CT_4vPC && battleSubType != CooperationType.CT_3vPC && battleSubType != CooperationType.CT_2vPC)
		{
			return false;
		}
		MapConfig mapConfig = this.GetRuntimeMapConfig();
		return mapConfig != null && mapConfig.allAlive == 1;
	}

	private void ShowAllAliveTip()
	{
		bool flag = this.IsCoopAllAlive();
		if (!flag)
		{
			if (this.allAliveLabel != null)
			{
				this.allAliveLabel.gameObject.SetActive(false);
			}
			return;
		}
		if (this.allAliveLabel == null)
		{
			this.allAliveLabel = this.CreateAllAliveLabel();
		}
		if (this.allAliveLabel != null)
		{
			this.allAliveLabel.gameObject.SetActive(true);
			string value = LanguageDataProvider.GetValue(7024);
			this.allAliveLabel.text = (string.IsNullOrEmpty(value) ? "所有玩家必须存活" : value);
		}
	}

	private UILabel CreateAllAliveLabel()
	{
		if (this.battleTime == null)
		{
			return null;
		}
		Transform parent = this.battleTime.transform.parent;
		if (parent == null)
		{
			parent = this.battleTime.transform;
		}
		GameObject gameObject = new GameObject("AllAliveTip");
		gameObject.layer = parent.gameObject.layer;
		gameObject.transform.SetParent(parent, false);
		gameObject.transform.localScale = Vector3.one;
		// 放在合作倒计时（battleTime）正下方，偏移量可按实际界面微调
		gameObject.transform.localPosition = this.battleTime.transform.localPosition + new Vector3(0f, -50f, 0f);

		UILabel uilabel = gameObject.AddComponent<UILabel>();
		this.ApplyAllAliveFont(uilabel);
		uilabel.supportEncoding = true;
		uilabel.alignment = NGUIText.Alignment.Center;
		uilabel.pivot = UIWidget.Pivot.Top;
		uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
		uilabel.color = new Color(1f, 1f, 1f, 1f);
		uilabel.fontSize = 24;
		uilabel.width = 400;
		uilabel.height = 40;
		uilabel.depth = this.battleTime.depth + 1;
		return uilabel;
	}

	private void ApplyAllAliveFont(UILabel target)
	{
		UILabel[] allLabels = Resources.FindObjectsOfTypeAll<UILabel>();
		// 优先 MFYueHei（猫饭悦黑）
		for (int i = 0; i < allLabels.Length; i++)
		{
			UILabel uilabel = allLabels[i];
			if (uilabel != null && this.GetLabelFontName(uilabel).ToLower().IndexOf("mfyuehei") >= 0)
			{
				target.bitmapFont = uilabel.bitmapFont;
				target.trueTypeFont = uilabel.trueTypeFont;
				return;
			}
		}
		// 其次用战斗界面已有的中文 aims 标签字体
		if (this.aimsLabel != null)
		{
			target.bitmapFont = this.aimsLabel.bitmapFont;
			target.trueTypeFont = this.aimsLabel.trueTypeFont;
			return;
		}
		// 兜底：方正大标宋（fzdbsjw）
		for (int j = 0; j < allLabels.Length; j++)
		{
			UILabel uilabel2 = allLabels[j];
			if (uilabel2 != null && this.GetLabelFontName(uilabel2).ToLower().IndexOf("fzdbsjw") >= 0)
			{
				target.bitmapFont = uilabel2.bitmapFont;
				target.trueTypeFont = uilabel2.trueTypeFont;
				return;
			}
		}
	}

	private string GetLabelFontName(UILabel label)
	{
		if (label == null)
		{
			return string.Empty;
		}
		if (label.bitmapFont != null)
		{
			return label.bitmapFont.name;
		}
		if (label.trueTypeFont != null)
		{
			return label.trueTypeFont.name;
		}
		return string.Empty;
	}

	private void UpdateBattleTime()
	{
		int num = Mathf.RoundToInt((float)this.battleTimeMax - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
		if (num == this.lastTimeSeconds)
		{
			if (this.lastTimeSeconds <= 0)
			{
				this.PlayTimeWillEnd(false);
			}
			return;
		}
		this.lastTimeSeconds = num;
		if (this.lastTimeSeconds < 0)
		{
			this.lastTimeSeconds = 0;
		}
		if (this.lastTimeSeconds > 10)
		{
			this.battleTime.text = string.Format("{0:D2}:{1:D2}", this.lastTimeSeconds / 60, this.lastTimeSeconds % 60);
			this.clockAni.Play(this.clockAniStr);
		}
		else
		{
			this.battleTime.text = this.lastTimeSeconds.ToString();
			if (!this.battleTime.gameObject.activeSelf)
			{
			}
			this.PlayTimeWillEnd(true);
		}
	}

	private void UpdateCountdownTime()
	{
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		float loseAllMatrixTime = team.GetLoseAllMatrixTime(0f);
		if (loseAllMatrixTime > 0f && team.current > 0)
		{
			int num = int.Parse(Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(6));
			int num2 = num - (int)loseAllMatrixTime;
			if (num2 < 0)
			{
				if (this.battleCoundown.transform.parent.gameObject.activeSelf)
				{
					this.battleCoundown.transform.parent.gameObject.SetActive(false);
				}
			}
			else if (!this.battleCoundown.transform.parent.gameObject.activeSelf)
			{
				this.battleCoundown.transform.parent.gameObject.SetActive(true);
			}
			this.battleCoundown.text = num2.ToString();
			this.lastTicksNum = 0;
		}
		else
		{
			if (this.battleCoundown.transform.parent.gameObject.activeSelf && this.lastTicksNum == 0)
			{
				this.lastTicksNum = 2;
			}
			if (this.lastTicksNum > 0)
			{
				this.lastTicksNum--;
				if (this.lastTicksNum == 0)
				{
					this.battleCoundown.transform.parent.gameObject.SetActive(false);
				}
			}
		}
	}

	private void TimerProc()
	{
		Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
		this.populationValueLabel.text = string.Format("{0} / {1}", team.current, team.currentMax);
		this.popLableValue1.text = this.populationValueLabel.text;
		base.Invoke("TimerProc", 0.5f);
	}

	private void OnProgressDragStart(GameObject go)
	{
		this.totalDrag = Vector2.zero;
	}

	private void OnProgressDrag(GameObject go, Vector2 delta)
	{
		this.OnSelectProcessAram(null);
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("CustomSelectWindowNew");
	}

	private void SetPercent()
	{
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption != 1)
		{
			float num = this.lineTotalLength * this.percent;
			Vector3 localPosition = this.percentZeroPos;
			localPosition.x += num;
			int width = (int)(this.lineTotalLength * this.percent + 2f - this.percentPicWidth / 2f);
			this.percentleft.width = width;
			width = (int)(this.lineTotalLength * (1f - this.percent) + 2f - this.percentPicWidth / 2f);
			this.percentright.width = width;
			this.percentleft.gameObject.SetActive(true);
			this.percentright.gameObject.SetActive(true);
			if (this.percent == 0f)
			{
				this.percentleft.gameObject.SetActive(false);
			}
			else if (this.percent == 1f)
			{
				this.percentright.gameObject.SetActive(false);
			}
			this.percetPic.transform.localPosition = localPosition;
			this.percentLabel.text = string.Format("{0}%", Mathf.RoundToInt(this.percent * 100f));
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderRate = this.percent;
	}

	private void InitFightProgressOption()
	{
		this.percentGo.SetActive(false);
		int fightOption = Solarmax.Singleton<LocalSettingStorage>.Get().fightOption;
		if (fightOption == 1)
		{
			this.ShowNewProgress1(false, null);
		}
		else
		{
			UIEventListener component = this.percetPic.gameObject.GetComponent<UIEventListener>();
			component.onDrag = new UIEventListener.VectorDelegate(this.OnProgressDrag);
			component.onDragStart = new UIEventListener.VoidDelegate(this.OnProgressDragStart);
			this.lineTotalLength = 1200f;
			this.percentZeroPos = new Vector3(this.percetPic.transform.localPosition.x - this.lineTotalLength / 2f, this.percetPic.transform.localPosition.y, 0f);
			UIAnchor component2 = this.percentGo.GetComponent<UIAnchor>();
			component2.enabled = true;
			if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
			{
				component2.side = UIAnchor.Side.Bottom;
				component2.relativeOffset.x = 0f;
				component2.relativeOffset.y = 0.05f;
			}
			else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 1)
			{
				this.percentGo.transform.eulerAngles = new Vector3(0f, 0f, 90f);
				this.percentGo.transform.localScale = new Vector3(0.56f, 0.8f, 0f);
				component2.side = UIAnchor.Side.Left;
				component2.relativeOffset.x = 0.06f;
				component2.relativeOffset.y = -0.12f;
				this.percentLabel.transform.eulerAngles = new Vector3(0f, 0f, -90f);
				this.percentLabel.transform.localPosition = new Vector3(this.percentLabel.transform.localPosition.x, -this.percentLabel.transform.localPosition.y, this.percentLabel.transform.localPosition.z);
				this.ProcessLine.transform.localPosition = new Vector3(this.ProcessLine.transform.localPosition.x, -75f, this.ProcessLine.transform.localPosition.z);
			}
			else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 2)
			{
				this.percentGo.transform.eulerAngles = new Vector3(0f, 0f, 90f);
				this.percentGo.transform.localScale = new Vector3(0.56f, 0.8f, 0f);
				component2.side = UIAnchor.Side.Right;
				component2.relativeOffset.x = -0.06f;
				component2.relativeOffset.y = -0.04f;
			}
			this.percentGo.SetActive(true);
		}
	}

	private void ShowNewProgress1(bool show, Node node = null)
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 0)
		{
			this.Processdown.SetActive(true);
			this.Processleft.SetActive(false);
			this.Processright.SetActive(false);
			this.SetSelectEffect(this.unitdown[7]);
		}
		else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 1)
		{
			this.Processdown.SetActive(false);
			this.Processleft.SetActive(true);
			this.Processright.SetActive(false);
			this.SetSelectEffect(this.unitleft[7]);
		}
		else if (Solarmax.Singleton<LocalSettingStorage>.Get().sliderMode == 2)
		{
			this.Processdown.SetActive(false);
			this.Processleft.SetActive(false);
			this.Processright.SetActive(true);
			this.SetSelectEffect(this.unitright[7]);
		}
	}

	private void RefreshVoiceChatPanel()
	{
		if (!this.enableVoiceChat)
		{
			this.VoiceChatPanel.SetActive(false);
			return;
		}
		this.VoiceChatPanel.SetActive(true);
		this.VoiceChannelPanel.SetActive(false);
		this.BnSpeaker.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableSpeaker);
		this.BnMicrophone.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone);
		this.BnTeamChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel);
		this.BnRoomChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
	}

	private void RefreshSpeakers()
	{
		foreach (KeyValuePair<int, SequenceFrame> keyValuePair in this.speakers)
		{
			keyValuePair.Value.gameObject.SetActive(this.enableVoiceChat);
		}
	}

	private void SetSelectEffect(GameObject go)
	{
		GameObject gameObject = go.transform.Find("on").gameObject;
		UILabel component = go.transform.Find("num").GetComponent<UILabel>();
		if (gameObject == null)
		{
			return;
		}
		if (this.selectOn != null)
		{
			this.selectOn.SetActive(false);
			this.selectLabel.color = Vector4.one;
		}
		this.selectOn = gameObject;
		this.selectLabel = component;
		this.selectOn.SetActive(true);
		component.color = new Vector4(0f, 0f, 0f, 0.8f);
	}

	public void GiveUpOnClicked()
	{
		if (!this.giveupForQuit)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				3,
				LanguageDataProvider.GetValue(801),
				new EventDelegate(new EventDelegate.Callback(this.GiveUp)),
				null,
				Mathf.CeilToInt(this.giveupCountTime - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime())
			});
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.OnPlayerDirectQuit();
		}
	}

	private void GiveUp()
	{
		Solarmax.Singleton<BattleSystem>.Instance.PlayerGiveUp();
	}

	private void UpdateBattleTimeColor()
	{
		int curProcduceIndex = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.curProcduceIndex;
		if (curProcduceIndex != this.curAddSpeedSegment && curProcduceIndex > this.curAddSpeedSegment)
		{
			this.curAddSpeedSegment = curProcduceIndex;
			if (curProcduceIndex == 1)
			{
				this.battleTime.color = new Color(1f, 1f, 0f);
			}
			if (curProcduceIndex == 2)
			{
				this.battleTime.color = new Color(1f, 0f, 0f);
			}
			this.PlayTimeEffectOut();
		}
		if (this.nPlayEffectTimes == 1)
		{
			this.PlayTimeEffectIn();
		}
	}

	private void PlayTimeEffectOut()
	{
		this.nPlayEffectTimes = 0;
		TweenScale tweenScale = this.battleTime.gameObject.GetComponent<TweenScale>();
		if (tweenScale == null)
		{
			tweenScale = this.battleTime.gameObject.AddComponent<TweenScale>();
		}
		tweenScale.ResetToBeginning();
		tweenScale.from = Vector3.one;
		tweenScale.to = Vector3.one * 1.5f;
		tweenScale.duration = 0.5f;
		tweenScale.SetOnFinished(delegate()
		{
			this.nPlayEffectTimes++;
		});
		tweenScale.Play(true);
	}

	private void PlayTimeEffectIn()
	{
		this.nPlayEffectTimes++;
		TweenScale tweenScale = this.battleTime.gameObject.GetComponent<TweenScale>();
		if (tweenScale == null)
		{
			tweenScale = this.battleTime.gameObject.AddComponent<TweenScale>();
		}
		tweenScale.ResetToBeginning();
		tweenScale.onFinished.Clear();
		tweenScale.from = Vector3.one * 1.5f;
		tweenScale.to = Vector3.one;
		tweenScale.duration = 0.5f;
		tweenScale.Play(true);
	}

	private void PlayTimeWillEnd(bool bStart)
	{
		if (bStart)
		{
			this.battleTime.transform.parent.gameObject.SetActive(true);
		}
		else
		{
			this.battleTime.transform.parent.gameObject.SetActive(true);
		}
	}

	private void OperateTeamGiveUP(int team)
	{
		int num = -1;
		Color32 color = new Color32(204, 204, 204, byte.MaxValue);
		bool flag = this.mapTeam2Index.TryGetValue(team, out num);
		if (flag && num >= 0 && num < this.playerFrames.Length)
		{
			GameObject gameObject = this.playerFrames[num];
			UISprite component = gameObject.transform.Find("flag").GetComponent<UISprite>();
			GameObject gameObject2 = gameObject.transform.Find("Effect_Surrender").gameObject;
			UILabel component2 = gameObject2.transform.Find("pos1/flagname").GetComponent<UILabel>();
			UILabel component3 = gameObject2.transform.Find("pos2/flagdown").GetComponent<UILabel>();
			component.gameObject.SetActive(false);
			component2.gameObject.SetActive(true);
			component2.text = LanguageDataProvider.GetValue(803);
			component3.text = LanguageDataProvider.GetValue(803);
			gameObject2.SetActive(true);
		}
	}

	private void OperateTeamDead(int team)
	{
		int num = -1;
		Color32 c = new Color32(204, 204, 204, byte.MaxValue);
		bool flag = this.mapTeam2Index.TryGetValue(team, out num);
		if (flag && num >= 0 && num < this.playerFrames.Length)
		{
			GameObject gameObject = this.playerFrames[num];
			UISprite component = gameObject.GetComponent<UISprite>();
			UILabel component2 = gameObject.transform.Find("name").GetComponent<UILabel>();
			UISprite component3 = gameObject.transform.Find("bei").GetComponent<UISprite>();
			UISprite component4 = gameObject.transform.Find("flag").GetComponent<UISprite>();
			GameObject gameObject2 = gameObject.transform.Find("Effect_Surrender").gameObject;
			UILabel component5 = gameObject2.transform.Find("pos1/flagname").GetComponent<UILabel>();
			UILabel component6 = gameObject2.transform.Find("pos2/flagdown").GetComponent<UILabel>();
			component.color = c;
			component2.color = c;
			component3.color = c;
			component5.gameObject.SetActive(true);
			component5.text = LanguageDataProvider.GetValue(802);
			component4.enabled = false;
			component6.text = LanguageDataProvider.GetValue(802);
			gameObject2.SetActive(true);
			if (team == (int)Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
			{
				this.battleCoundown.gameObject.SetActive(false);
			}
		}
	}

	public void OnSelectNumber(GameObject go)
	{
		UILabel component = go.transform.Find("num").GetComponent<UILabel>();
		if (component != null)
		{
			string text = component.text;
			if (text.Equals("ALL"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = -1f;
			}
			else if (text.Equals("1"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 1f;
			}
			else if (text.Equals("5"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 5f;
			}
			else if (text.Equals("10"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 10f;
			}
			else if (text.Equals("30"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 30f;
			}
			else if (text.Equals("50"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 50f;
			}
			else if (text.Equals("80"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 80f;
			}
			else if (text.Equals("100"))
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber = 100f;
			}
			this.SetSelectEffect(go);
		}
	}

	public void OnBnSpeakerClick()
	{
		VoiceEngine.EnableSpeaker(!Solarmax.Singleton<LocalSettingStorage>.Get().enableSpeaker);
		bool flag = VoiceEngine.IsSpeakerEnable();
		if (flag == Solarmax.Singleton<LocalSettingStorage>.Get().enableSpeaker)
		{
			return;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableSpeaker = flag;
		this.BnSpeaker.SetChecked(flag);
	}

	private void SwitchMicrophone()
	{
		VoiceEngine.EnableMicrophone(!Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone);
		bool flag = VoiceEngine.IsMicrophoneEnable();
		if (flag == Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone)
		{
			return;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone = flag;
		this.BnMicrophone.SetChecked(flag);
	}

	public void OnBnMicrophoneClick()
	{
		this.VoiceChannelPanel.SetActive(!this.VoiceChannelPanel.activeSelf);
	}

	private void SwitchRoomChannel()
	{
		VoiceEngine.EnableRoomChannel(!Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
		bool flag = VoiceEngine.IsRoomChannelEnable();
		if (flag == Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel)
		{
			return;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel = flag;
		this.BnRoomChannel.SetChecked(flag);
		this.BnTeamChannel.SetChecked(!flag);
	}

	private bool SetMicrophoneStatus(bool enableRoomChannel, bool enableTeamChannel)
	{
		bool flag = enableRoomChannel || enableTeamChannel;
		if (!VoiceEngine.EnableMicrophone(flag))
		{
			return false;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone = flag;
		this.BnMicrophone.SetChecked(flag);
		return true;
	}

	public void OnBnRoomChannelClick()
	{
		bool flag = !Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel;
		if (!this.SetMicrophoneStatus(flag, Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel))
		{
			return;
		}
		if (Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone && !VoiceEngine.EnableRoomChannel(flag))
		{
			return;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel = flag;
		if (flag)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel = false;
		}
		this.BnRoomChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
		this.BnTeamChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel);
		this.LblRoomChannel.SetActive(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
		this.LblTeamChannel.SetActive(Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel);
		this.VoiceChannelPanel.SetActive(false);
	}

	public void OnBnTeamChannelClick()
	{
		bool flag = !Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel;
		if (!this.SetMicrophoneStatus(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel, flag))
		{
			return;
		}
		if (Solarmax.Singleton<LocalSettingStorage>.Get().enableMicrophone && !VoiceEngine.EnableRoomChannel(!flag))
		{
			return;
		}
		Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel = flag;
		if (flag)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel = false;
		}
		this.BnRoomChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
		this.BnTeamChannel.SetChecked(Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel);
		this.LblRoomChannel.SetActive(Solarmax.Singleton<LocalSettingStorage>.Get().enableRoomChannel);
		this.LblTeamChannel.SetActive(Solarmax.Singleton<LocalSettingStorage>.Get().enableTeamChannel);
		this.VoiceChannelPanel.SetActive(false);
	}

	public UILabel populationLabel;

	public UILabel populationValueLabel;

	private float giveupCountTime;

	private bool giveupForQuit;

	public UILabel giveupLabel;

	public GameObject percentGo;

	public GameObject ProcessAram;

	public GameObject ProcessLine;

	public UISprite percentleft;

	public UISprite percentright;

	public UISprite percetPic;

	public UILabel percentLabel;

	public GameObject Processdown;

	public GameObject[] unitdown;

	public GameObject Processleft;

	public GameObject[] unitleft;

	public GameObject Processright;

	public GameObject[] unitright;

	private GameObject selectOn;

	public float sensitive = 1f;

	private float lineTotalLength;

	private Vector3 percentZeroPos = Vector3.zero;

	private float percent;

	private float percentPicWidth;

	public GameObject player;

	public GameObject player2V2;

	private GameObject[] playerFrames = new GameObject[4];

	public GameObject[] players;

	public GameObject[] pvps;

	public UILabel battleTime;

	public UILabel battleCoundown;

	public UILabel popLable1;

	public UILabel popLableValue1;

	public UILabel popLableAdd;

	public Animator clockAni;

	public string clockAniStr;

	public GameObject VoiceChatPanel;

	public CheckBox BnSpeaker;

	public CheckBox BnMicrophone;

	public CheckBox BnTeamChannel;

	public CheckBox BnRoomChannel;

	public GameObject LblTeamChannel;

	public GameObject LblRoomChannel;

	public GameObject VoiceChannelPanel;

	private int battleTimeMax;

	private int curAddSpeedSegment;

	private UILabel selectLabel;

	private Dictionary<int, SequenceFrame> speakers = new Dictionary<int, SequenceFrame>();

	private bool enableVoiceChat;

	private Dictionary<int, PrefabInstance> targetMarkers = new Dictionary<int, PrefabInstance>();

	private Dictionary<int, int> mapTeam2Index = new Dictionary<int, int>();

	private float fUpdateBattleTime;

	private int lastTimeSeconds = -1;

	private int lastTicksNum;

	private UILabel aimsLabel;

	private UILabel allAliveLabel;

	private Vector2 totalDrag;

	private int nPlayEffectTimes;
}
