using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NetMessage;
using ProtoBuf;
using Solarmax;
using UnityEngine;

public class PacketHelper
{
	private readonly Dictionary<int, int> roomUserSeatIndex = new Dictionary<int, int>();

	private void RemoveRoomSeatByIndex(int seatIndex)
	{
		List<int> list = null;
		foreach (KeyValuePair<int, int> keyValuePair in this.roomUserSeatIndex)
		{
			if (keyValuePair.Value == seatIndex)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(keyValuePair.Key);
			}
		}
		if (list == null)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.roomUserSeatIndex.Remove(list[i]);
		}
	}

	private void SwapRoomSeatIndex(int fromIndex, int toIndex)
	{
		int num = int.MinValue;
		int num2 = int.MinValue;
		foreach (KeyValuePair<int, int> keyValuePair in this.roomUserSeatIndex)
		{
			if (keyValuePair.Value == fromIndex)
			{
				num = keyValuePair.Key;
			}
			else if (keyValuePair.Value == toIndex)
			{
				num2 = keyValuePair.Key;
			}
		}
		if (num != int.MinValue)
		{
			this.roomUserSeatIndex[num] = toIndex;
		}
		if (num2 != int.MinValue)
		{
			this.roomUserSeatIndex[num2] = fromIndex;
		}
	}

	private void Log(string fmt, params object[] args)
	{
		string message = string.Format(fmt, args);
		Solarmax.Singleton<LoggerSystem>.Instance.Debug(message, new object[0]);
	}

	private static int CalcMaxCampInMap(MapConfig table)
	{
		int num = 0;
		if (table != null)
		{
			if (table.mbcList != null)
			{
				for (int i = 0; i < table.mbcList.Count; i++)
				{
					MapBuildingConfig mapBuildingConfig = table.mbcList[i];
					if (mapBuildingConfig != null && mapBuildingConfig.camption > num)
					{
						num = mapBuildingConfig.camption;
					}
				}
			}
			if (table.mpcList != null)
			{
				for (int j = 0; j < table.mpcList.Count; j++)
				{
					MapPlayerConfig mapPlayerConfig = table.mpcList[j];
					if (mapPlayerConfig != null && mapPlayerConfig.camption > num)
					{
						num = mapPlayerConfig.camption;
					}
				}
			}
		}
		return num;
	}

	private static void EnsurePlayerCount(MapConfig table, int desired)
	{
		if (table == null)
		{
			return;
		}
		if (desired < 0)
		{
			desired = 0;
		}
		if (desired > LocalPlayer.MaxTeamNum - 1)
		{
			desired = LocalPlayer.MaxTeamNum - 1;
		}
		if (table.player_count < desired)
		{
			table.player_count = desired;
		}
	}

	public void RegisterAllPacketHandler()
	{
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(2, new MessageHandler(this.OnMatch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(22, new MessageHandler(this.OnMatchFailed));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(6, new MessageHandler(this.OnNetStartBattle));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(8, new MessageHandler(this.OnNetFrame));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(10, new MessageHandler(this.OnFinishBattle));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(12, new MessageHandler(this.OnRequestUser));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(14, new MessageHandler(this.OnCreateUser));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(18, new MessageHandler(this.OnErrCode));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(24, new MessageHandler(this.OnPong));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(26, new MessageHandler(this.OnLoadRank));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(28, new MessageHandler(this.OnReady));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(38, new MessageHandler(this.OnFriendFollow));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(40, new MessageHandler(this.OnFriendLoad));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(42, new MessageHandler(this.OnFriendNotify));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(44, new MessageHandler(this.OnFriendRecommend));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(59, new MessageHandler(this.OnFriendLikeSearch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(46, new MessageHandler(this.OnFriendSearch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(36, new MessageHandler(this.OnBoardFriendState));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(62, new MessageHandler(this.OnTeamCreate));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(64, new MessageHandler(this.OnTeamInvite));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(60, new MessageHandler(this.OnTeamUpdate));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(66, new MessageHandler(this.OnTeamDelete));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(68, new MessageHandler(this.OnTeamStart));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(70, new MessageHandler(this.OnTeamInviteReq));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(72, new MessageHandler(this.OnTeamInviteResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(52, new MessageHandler(this.OnBattleReportLoad));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(54, new MessageHandler(this.onBattleReportPlay));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(58, new MessageHandler(this.OnBattleResume));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(282, new MessageHandler(this.OnRequestSCUserInit));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(81, new MessageHandler(this.OnGetLeagueInfo));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(83, new MessageHandler(this.OnGetLeagueList));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(85, new MessageHandler(this.OnLeagueSignUp));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(87, new MessageHandler(this.OnLeagueRank));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(89, new MessageHandler(this.OnLeagueMatch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(93, new MessageHandler(this.OnRequestRoomList));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(95, new MessageHandler(this.OnRequestJoinRoom));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(97, new MessageHandler(this.OnRequestCreateRoom));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(99, new MessageHandler(this.OnRequestQuitRoom));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(101, new MessageHandler(this.RoomRefresh));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(103, new MessageHandler(this.RoomListRefresh));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(107, new MessageHandler(this.OnMatch2CurNum));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(109, new MessageHandler(this.ONMatchGameBack));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(111, new MessageHandler(this.OnStarMatch3));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(113, new MessageHandler(this.OnMatch3Notify));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(115, new MessageHandler(this.OnChangeRace));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(131, new MessageHandler(this.OnStartSelectRace));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(117, new MessageHandler(this.OnSelectRaceNotify));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(119, new MessageHandler(this.OnGetRaceData));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(121, new MessageHandler(this.OnRaceSkillLevelUp));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(123, new MessageHandler(this.OnPackUpdate));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(133, new MessageHandler(this.OnOpenTimerChest));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(135, new MessageHandler(this.OnOpenBattleChest));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(149, new MessageHandler(this.OnUpdateTimeChest));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(147, new MessageHandler(this.OnUpdateBattleChest));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(127, new MessageHandler(this.OnStartBox));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(129, new MessageHandler(this.OnOpenBox));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(137, new MessageHandler(this.OnNotifyBox));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(139, new MessageHandler(this.OnSingleMatch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(159, new MessageHandler(this.OnSetCurrentLevelResult));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(157, new MessageHandler(this.OnChangeName));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(163, new MessageHandler(this.OnRaceNotify));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(167, new MessageHandler(this.OnClientStorageLoad));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(165, new MessageHandler(this.OnClientStorageSet));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(171, new MessageHandler(this.OnLeagueQuitResult));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(173, new MessageHandler(this.OnReconnectResumeResult));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(169, new MessageHandler(this.OnKickUserNtf));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(175, new MessageHandler(this.OnServerNotify));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(177, new MessageHandler(this.OnStartMatchRequest));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(179, new MessageHandler(this.OnMatchInit));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(181, new MessageHandler(this.OnMatchUpdate));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(185, new MessageHandler(this.OnQuitMatch));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(183, new MessageHandler(this.OnMatchComplete));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(197, new MessageHandler(this.OnMatchPos));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(501, new MessageHandler(this.OnBattleMarkTarget));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(209, new MessageHandler(this.OnLoadChapters));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(211, new MessageHandler(this.OnLoadOneChapter));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(201, new MessageHandler(this.OnSetLevelStar));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(203, new MessageHandler(this.OnStartLevel));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(205, new MessageHandler(this.OnIntAttr));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(218, new MessageHandler(this.OnRequestPveRankReport));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(220, new MessageHandler(this.OnRequestBuyChapter));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(216, new MessageHandler(this.OnRequestSetLevelSorce));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(212, new MessageHandler(this.OnMoneyChange));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(214, new MessageHandler(this.OnGenPresignedUrl));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(252, new MessageHandler(this.OnRequestTaskOk));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(254, new MessageHandler(this.OnPullTaskOk));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(262, new MessageHandler(this.OnStartMatchInvite));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(264, new MessageHandler(this.OnSCMatchInviteReq));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(266, new MessageHandler(this.OnSCMatchInviteResp));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(268, new MessageHandler(this.OnBuySkinResp));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(294, new MessageHandler(this.OnGenerateOrderID));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(270, new MessageHandler(this.OnVerityOrder));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(292, new MessageHandler(this.OnSCAdeward));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(302, new MessageHandler(this.OnSendAchievementSet));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(304, new MessageHandler(this.OnPullAchievements));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(272, new MessageHandler(this.OnSCReceiveMonthlyCard));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(273, new MessageHandler(this.OnSCChangeMonthlyCard));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(311, new MessageHandler(this.OnUploadOldData));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(321, new MessageHandler(this.OnLoadBattleRooms));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(324, new MessageHandler(this.OnEditRoomWatchName));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(326, new MessageHandler(this.OnEditRoomLock));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(331, new MessageHandler(this.OnClaimSeasonReward));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(342, new MessageHandler(this.OnChangeSeasonScore));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(402, new MessageHandler(this.OnPushMonthCheck));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(401, new MessageHandler(this.OnMonthCheck));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(344, new MessageHandler(this.OnAccumulMoneyUpdate));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(351, new MessageHandler(this.OnReposeActiveChapter));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(353, new MessageHandler(this.OnResponseActiveChapterPrice));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(355, new MessageHandler(this.OnSetChapterScoreResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(551, new MessageHandler(this.OnLotteryInfoResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(553, new MessageHandler(this.OnLotteryNotesResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(555, new MessageHandler(this.OnLotteryAwardResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(361, new MessageHandler(this.OnUseItemResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(363, new MessageHandler(this.OnDecomposeItemResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(187, new MessageHandler(this.OnMailListResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(191, new MessageHandler(this.OnDeleteMailResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(189, new MessageHandler(this.OnReadMailResponse));
		Solarmax.Singleton<NetSystem>.Instance.GetConnector().RegisterHandler(604, new MessageHandler(this.OnGetMailItemsResponse));
	}

	public IEnumerator ConnectServer(bool bTips = true)
	{
        //Solarmax.Singleton<NetSystem>.Instance.ping.Pong(100f);
        //Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NetworkStatus, new object[]
        //{
        //	true
        //});
        string addr = string.Empty;
        string ip = string.Empty;
        int port = 0;
        addr = GatewayRequest.Response.Host.Trim();
        string[] addrs = addr.Split(new char[]
        {
            ':'
        });
        ip = addrs[0];
        port = int.Parse(addrs[1]);
        this.ServerAddress = ip;
        while (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTING)
        {
            yield return 1;
        }
        Solarmax.Singleton<NetSystem>.Instance.Connect(ip, port);
        while (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTING)
        {
            yield return 1;
        }
        if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
        {
            Solarmax.Singleton<NetSystem>.Instance.ping.Pong(100f);
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NetworkStatus, new object[]
            {
                true
            });
        }
        else
        {
            if (bTips)
            {
                Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(207), 1f);
            }
            Solarmax.Singleton<NetSystem>.Instance.Close();
            Solarmax.Singleton<NetSystem>.Instance.ping.Pong(-1f);
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NetworkStatus, new object[]
            {
                false
            });
        }
        yield break;
	}

	public void SendProto<T>(int MessageID, T proto) where T : class
	{
		Solarmax.Singleton<NetSystem>.Instance.Send<T>(MessageID, proto, true);
	}

	public void Match(bool single, bool team, string matchId)
	{
		CSMatch csmatch = new CSMatch();
		if (single)
		{
			csmatch.type = BattleType.Melee;
		}
		else if (team)
		{
			csmatch.type = BattleType.Group2v2;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = matchId;
			csmatch.match_id = matchId;
		}
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMatch>(1, csmatch, true);
	}

	private void OnMatch(int msgID, PacketEvent msgBody)
	{
		MemoryStream source = msgBody.Data as MemoryStream;
		SCMatch scmatch = Serializer.Deserialize<SCMatch>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatch, new object[]
		{
			scmatch.count_down
		});
	}

	private void OnMatchFailed(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	public void OnReady(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCReady proto = Serializer.Deserialize<SCReady>(source);
		this.OnReady(proto);
	}

	private void RandomTeamMember()
	{
		int num = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(0, 3) + 1;
		if (num == 1)
		{
			int[] array = new int[]
			{
				0,
				1,
				2,
				3
			};
			for (int i = 0; i < 4; i++)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[i] = array[i];
			}
		}
		else if (num == 2)
		{
			int[] array2 = new int[]
			{
				0,
				2,
				1,
				3
			};
			for (int j = 0; j < 4; j++)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[j] = array2[j];
			}
		}
		else if (num == 3)
		{
			int[] array3 = new int[]
			{
				0,
				3,
				2,
				1
			};
			for (int k = 0; k < 4; k++)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[k] = array3[k];
			}
		}
		else
		{
			int[] array4 = new int[]
			{
				0,
				3,
				1,
				2
			};
		}
	}

	public void OnReady(SCReady proto)
	{
		Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(true, false);
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.bStartBattle = true;
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.replay = true;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType = proto.sub_type;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType = proto.match_type;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.battleId = proto.battleid;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.voiceRoomId = proto.voice_join_channe_id;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.voiceRoomToken = null;
        if (proto.Tencent_cloud_token != null)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.voiceRoomToken = proto.Tencent_cloud_token.ToArray();
		}
		if (proto.match_type == MatchType.MT_Ladder)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType = BattlePlayType.Normalize;
		}
		else if (proto.match_type == MatchType.MT_Room)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType = BattlePlayType.Normalize;
		}
		else if (proto.match_type == MatchType.MT_Sing)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.SingleLevel;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType = BattlePlayType.Replay;
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.replay = false;
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.replaySingle = true;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType = BattlePlayType.Normalize;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = proto.match_id;
		Solarmax.Singleton<LocalPlayer>.Get().battleMap = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.seed = proto.random_seed;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.seed = proto.random_seed;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
		{
			Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(proto.match_id, 0);
		}
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(proto.match_id);
		MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(proto.match_id);
		if (data2 == null)
		{
			data2 = new MapConfig(proto.match_id);
			data2.Load();
		}
		if (data2 == null)
		{
			return;
		}
		for (int numResetTeam = 1; numResetTeam < LocalPlayer.MaxTeamNum; numResetTeam++)
		{
			Team teamReset = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)numResetTeam);
			if (teamReset != null)
			{
				CampConfig dataReset = Solarmax.Singleton<CampConfigConfigProvider>.Instance.GetData(numResetTeam.ToString());
				if (dataReset != null)
				{
					teamReset.SetCampConfig(dataReset);
				}
			}
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
		{
			CampConfig data3 = Solarmax.Singleton<CampConfigConfigProvider>.Instance.GetData("9");
			if (data3 != null)
			{
				Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(TEAM.Team_4);
				if (team != null)
				{
					team.SetCampConfig(data3);
				}
			}
		}
		int numHumanPlayers = 4;
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
		{
			numHumanPlayers = 2;
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1 || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
		{
			numHumanPlayers = 3;
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC && Solarmax.Singleton<BattleSystem>.Instance.battleData.matchPlayerNum == 5)
		{
			numHumanPlayers = 5;
		}
		bool flagCoopPlayerControl = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1;
		int[] arrayCamp = null;
		if (flagCoopPlayerControl && Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.TryGetPlayerControl(proto.match_id, numHumanPlayers, out arrayCamp))
		{
			MapConfig mapConfig = new MapConfig(data2.name);
			mapConfig.Load();
			if (mapConfig != null)
			{
				data2 = mapConfig;
			}
		}
		int numCampFromControl = 0;
		if (flagCoopPlayerControl && arrayCamp != null)
		{
			for (int i = 0; i < numHumanPlayers && i < arrayCamp.Length; i++)
			{
				if (arrayCamp[i] > numCampFromControl)
				{
					numCampFromControl = arrayCamp[i];
				}
			}
		}
		int numCampFromMap = PacketHelper.CalcMaxCampInMap(data2);
		int numCampNeed = Mathf.Max(data2.player_count, Mathf.Max(numCampFromMap, numCampFromControl));
		PacketHelper.EnsurePlayerCount(data2, numCampNeed);
		for (int j = 0; j < data2.teamFriendList.Count - 1; j += 2)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[j + 1]).teamFriend[data2.teamFriendList[j]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[j]).teamFriend[data2.teamFriendList[j + 1]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[j]).teamFriend[data2.teamFriendList[j]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[j + 1]).teamFriend[data2.teamFriendList[j + 1]] = true;
			LoggerSystem.CodeComments("代码注释-结盟控制：由地图文件中的结盟对来设置结盟");
		}
		for (int num = 1; num <= Mathf.Min(LocalPlayer.MaxTeamNum - 1, Mathf.Max(4, data2.player_count)); num++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)num);
			if (team != null)
			{
				team.teamFriend[num] = true;
			}
		}
		if (string.IsNullOrEmpty(data2.defaultai))
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = -1;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = int.Parse(data2.defaultai);
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy < 0)
		{
			if (data == null)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = 3;
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = Solarmax.Singleton<AIStrategyConfigProvider>.Instance.GetAIStrategyByName(data.aiType);
			}
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy < 0)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = 1;
		}
		if (data != null)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiParam = data.aiParam;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.dyncDiffType = data.dyncDiffType;
		}
		int[] array = new int[proto.data.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i;
		}
		string text = proto.group;
        if (string.IsNullOrEmpty(text) && proto.match_type != MatchType.MT_Sing)
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1)
			{
				text = "0|1";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
			{
				this.RandomTeamMember();
				text = "0,1|2,3";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1)
			{
				text = "0|1|2";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_1v1v1v1)
			{
				text = "0|1|2|3";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC)
			{
				text = "0,1|";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC)
			{
				text = "0,1,2|";
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
			{
				text = ((numHumanPlayers == 5) ? "0,1,2,3,4|" : "0,1,2,3|");
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
			{
				text = "0,1,2|3";
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			string[] array2 = text.Split(new char[]
			{
				'|'
			});
			int num = 0;
			while (num < array2.Length && !string.IsNullOrEmpty(array2[num]))
			{
				string[] array3 = array2[num].Split(new char[]
				{
					','
				});
				for (int j = 0; j < array3.Length; j++)
				{
					int num2 = int.Parse(array3[j]);
					array[num2] = num;
				}
				num++;
			}
			Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight = true;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight = false;
		}
		BattleData battleData2 = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		CooperationType battleSubType2 = (battleData2 != null) ? battleData2.battleSubType : CooperationType.CT_Null;
		bool flagFivePlayerCoop = battleData2 != null && battleData2.matchPlayerNum == 5 && (battleSubType2 == CooperationType.CT_2vPC || battleSubType2 == CooperationType.CT_3vPC || battleSubType2 == CooperationType.CT_4vPC);
		int[] userIndexToTeam = null;
		if (proto.match_type != MatchType.MT_Sing && flagCoopPlayerControl && arrayCamp != null && arrayCamp.Length >= numHumanPlayers)
		{
			userIndexToTeam = new int[proto.data.Count];
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			foreach (KeyValuePair<int, int> keyValuePair in this.roomUserSeatIndex)
			{
				if (keyValuePair.Key > 0 && keyValuePair.Value >= 0)
				{
					list.Add(keyValuePair);
				}
			}
			list.Sort(delegate(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
			{
				return a.Value.CompareTo(b.Value);
			});
			for (int i = 0; i < list.Count && i < arrayCamp.Length; i++)
			{
				dictionary[list[i].Key] = arrayCamp[i];
			}
			for (int i = 0; i < userIndexToTeam.Length; i++)
			{
				userIndexToTeam[i] = (i + 1);
			}
			int num3 = 0;
			for (int j = 0; j < proto.data.Count && num3 < numHumanPlayers; j++)
			{
				UserData userData = proto.data[j];
				if (userData != null && userData.userid > 0)
				{
					int num4;
					if (dictionary.TryGetValue(userData.userid, out num4))
					{
						userIndexToTeam[j] = num4;
					}
					else
					{
						userIndexToTeam[j] = arrayCamp[num3];
						num3++;
					}
				}
			}
		}
		if (proto.match_type == MatchType.MT_Sing)
		{
			if (proto.data.Count > 1)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.useCommonEndCondition = true;
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.useCommonEndCondition = false;
			}
		}
		bool flag = false;
		int k;
        for (k = 0; k < proto.data.Count; k++)
		{
			UserData userData;
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
			{
				userData = proto.data[Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[k]];
			}
			else
			{
				userData = proto.data[k];
			}
			TEAM team = (TEAM)(k + 1);
			if (userIndexToTeam != null && k >= 0 && k < userIndexToTeam.Length)
			{
				team = (TEAM)userIndexToTeam[k];
			}
			if (proto.match_type == MatchType.MT_Sing)
			{
				team = (TEAM)((userData.userid <= 0) ? (-userData.userid) : userData.userid);
			}
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2v2)
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[k] < 2)
				{
					team2.groupID = 0;
				}
				else
				{
					team2.groupID = 1;
				}
				for (int m = 0; m < proto.data.Count; m++)
				{
					int tmpGroupId = Solarmax.Singleton<BattleSystem>.Instance.battleData.pvpUserToTeamIndex[m] < 2 ? 0 : 1;
					if (tmpGroupId == team2.groupID)
					{
						// 这里 m 要对应实际的 team 号，前面的 team 是 k + 1，这里的 index 要用 m + 1
						// TODO: MT_Sing 是否也要像前面一样适配？这样就数组越界了
						team2.teamFriend[m + 1] = true;
					}
                }
			}
			else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3v1)
			{
				// 针对合作模式独立出来，避免影响混战的逻辑（理论上并不会影响，为了减少测试）
				team2.groupID = array[k];
				for (int m = 0; m < proto.data.Count; m++)
				{
					if (array[m] == array[k])
					{
						int numFriendTeam = m + 1;
						if (userIndexToTeam != null && m >= 0 && m < userIndexToTeam.Length)
						{
							numFriendTeam = userIndexToTeam[m];
						}
						team2.teamFriend[numFriendTeam] = true;
					}
				}
			}
			else
			{
				team2.groupID = array[k];
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType == BattlePlayType.Replay && proto.match_type == MatchType.MT_Sing && userData.userid < 0)
			{
				userData.userid += -10;
			}
			team2.playerData.Init(userData);
			if (userData.userid > 0)
			{
				if (proto.match_type == MatchType.MT_Sing)
				{
					Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam = team;
					flag = true;
				}
				else if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == userData.userid)
				{
                    Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam = team;
					flag = true;
				}
				team2.aiEnable = false;
			}
			else
			{
				int num3;
				if (proto.match_type == MatchType.MT_Sing)
				{
					num3 = data2.teamAiType[(int)team];
				}
				else
				{
					num3 = data2.teamAiType[(int)team];
					if (num3 < 0)
					{
						num3 = Solarmax.Singleton<AIStrategyConfigProvider>.Instance.GetAIStrategyByName(proto.ai_type);
					}
				}
				if (num3 < 0)
				{
					num3 = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy;
				}
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.AddAI(team2, num3, team2.playerData.level, Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel, (float)proto.ai_param);
				team2.playerData.name = AIManager.GetAIName(team2.playerData.userId);
				team2.playerData.icon = AIManager.GetAIIcon(team2.playerData.userId);
				team2.aiEnable = true;
				this.Log("加入了AI：{0}, 类型：{1}", new object[]
				{
					team2.playerData.name,
					Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.aiData[(int)team].aiStrategy
				});
			}
		}
		if (!flag && proto.match_type != MatchType.MT_Sing)
		{
			int userId2 = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId;
			if (userId2 > 0)
			{
				Team team4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(userId2);
				if (team4 != null && team4.team != TEAM.Neutral)
				{
					Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam = team4.team;
					flag = true;
				}
			}
		}
        if (!flag)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState = GameState.Watcher;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState = GameState.Game;
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_2vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_3vPC || Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType == CooperationType.CT_4vPC)
		{
			List<int> list = new List<int>();
			MapConfig data3 = data2;
			if (data3 != null)
			{
				foreach (MapBuildingConfig mapBuildingConfig in data3.mbcList)
				{
					list.Add(mapBuildingConfig.camption);
				}
			}
			for (int l = 1; l < LocalPlayer.MaxTeamNum; l++)
			{
				if (list.Contains(l))
				{
					Team team3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)l);
					if (team3 == null || team3.playerData.userId > 0)
					{
						continue;
					}
					team3.playerData.userId = -10000 - l;
					team3.playerData.name = AIManager.GetAIName(team3.playerData.userId);
					team3.playerData.icon = AIManager.GetAIIcon(team3.playerData.userId);
					int num4 = data2.teamAiType[l];
					if (num4 < 0)
					{
						num4 = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy;
					}
					Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.AddAI(team3, num4, team3.playerData.level, Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel, 1f);
					team3.aiEnable = true;
					Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("加入了AI：{0}, 类型：{1}", team3.playerData.name, Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.aiData[l].aiStrategy), new object[0]);
				}
			}
		}
        Solarmax.Singleton<EffectManager>.Get().PreloadEffect();
	}

	public void OnNetStartBattle(int msgID, PacketEvent msgBody)
	{
		MemoryStream source = msgBody.Data as MemoryStream;
		SCStartBattle proto = Serializer.Deserialize<SCStartBattle>(source);
		this.OnNetStartBattle(proto);
	}

	public void OnNetStartBattle(SCStartBattle proto)
	{
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.Reset();
		string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(matchId);
		if (data == null)
		{
			data = new MapConfig(matchId);
			data.Load();
		}
		if (data == null)
		{
			return;
		}
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		CooperationType subType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
		int numHumanPlayers = 4;
		if (subType == CooperationType.CT_1v1 || subType == CooperationType.CT_2vPC)
		{
			numHumanPlayers = 2;
		}
		else if (subType == CooperationType.CT_1v1v1 || subType == CooperationType.CT_3vPC)
		{
			numHumanPlayers = 3;
		}
		else if (subType == CooperationType.CT_4vPC && Solarmax.Singleton<BattleSystem>.Instance.battleData.matchPlayerNum == 5)
		{
			numHumanPlayers = 5;
		}
		else if (subType == CooperationType.CT_1v1v1v1 || subType == CooperationType.CT_2v2 || subType == CooperationType.CT_3v1 || subType == CooperationType.CT_4vPC)
		{
			numHumanPlayers = 4;
		}
		int[] array;
		bool flagCoopPlayerControl = subType == CooperationType.CT_2vPC || subType == CooperationType.CT_3vPC || subType == CooperationType.CT_4vPC || subType == CooperationType.CT_3v1;
		if (!flagCoopPlayerControl)
		{
			array = null;
		}
		else
		{
			Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.TryGetPlayerControl(matchId, numHumanPlayers, out array);
		}
		MapConfig mapConfig = new MapConfig(data.name);
		mapConfig.Load();
		if (mapConfig == null)
		{
			mapConfig = data;
		}
		int numCampFromControl = 0;
		if (flagCoopPlayerControl && array != null)
		{
			for (int j = 0; j < numHumanPlayers && j < array.Length; j++)
			{
				if (array[j] > numCampFromControl)
				{
					numCampFromControl = array[j];
				}
			}
		}
		int numCampFromMap = PacketHelper.CalcMaxCampInMap(mapConfig);
		int numCampNeed = Mathf.Max(mapConfig.player_count, Mathf.Max(numCampFromMap, numCampFromControl));
		PacketHelper.EnsurePlayerCount(mapConfig, numCampNeed);
		int num3 = (mapConfig != null) ? mapConfig.player_count : data.player_count;
		for (int i = 0; i < num3; i++)
		{
			TEAM team = (TEAM)(i + 1);
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(team);
			team2.StartTeam();
			list2.Add(i + 1);
			if (team2.playerData.userId > 0)
			{
				list.Add(team2.playerData.userId);
				team2.aiEnable = false;
				continue;
			}
			team2.playerData.userId = -10000 - i;
			if (string.IsNullOrEmpty(team2.playerData.name))
			{
				team2.playerData.name = AIManager.GetAIName(team2.playerData.userId);
			}
			if (string.IsNullOrEmpty(team2.playerData.icon))
			{
				team2.playerData.icon = AIManager.GetAIIcon(team2.playerData.userId);
			}
			int num4 = ((mapConfig != null) ? mapConfig.teamAiType[i + 1] : data.teamAiType[i + 1]);
			if (num4 < 0)
			{
				num4 = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy;
			}
			if (num4 < 0)
			{
				num4 = 3;
			}
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.AddAI(team2, num4, team2.playerData.level, Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel, 1f);
			team2.aiEnable = true;
			list.Add(team2.playerData.userId);
		}
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateScene(false, mapConfig, list2, false);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.useAI)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.Start(1f);
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.useAI = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.Start(1f);
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("PreviewWindow");
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.replay = Solarmax.Singleton<BattleSystem>.Instance.battleData.isReplay;
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 1f;
	}

	private static void ApplyTeamCampRemap(int[] newToOld, int maxTeam)
	{
		if (newToOld == null)
		{
			return;
		}
		TeamManager teamManager = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager;
		if (teamManager == null)
		{
			return;
		}
		int num = Mathf.Min(maxTeam, newToOld.Length - 1);
		for (int i = 1; i <= num; i++)
		{
			int num2 = newToOld[i];
			if (num2 <= 0)
			{
				continue;
			}
			CampConfig data = Solarmax.Singleton<CampConfigConfigProvider>.Instance.GetData(num2.ToString());
			if (data == null)
			{
				continue;
			}
			Team team = teamManager.GetTeam((TEAM)i);
			if (team != null)
			{
				team.SetCampConfig(data);
			}
		}
	}

	private static int[] ApplyCampRemap(MapConfig table, int[] desiredCamps, int numHumanPlayers)
	{
		if (table == null || desiredCamps == null || desiredCamps.Length < numHumanPlayers)
		{
			return null;
		}
		int num = PacketHelper.CalcMaxCampInMap(table);
		for (int i = 0; i < numHumanPlayers; i++)
		{
			if (desiredCamps[i] > num)
			{
				num = desiredCamps[i];
			}
		}
		if (num < numHumanPlayers)
		{
			num = numHumanPlayers;
		}
		if (num > LocalPlayer.MaxTeamNum - 1)
		{
			num = LocalPlayer.MaxTeamNum - 1;
		}
		if (table.player_count < numHumanPlayers)
		{
			table.player_count = numHumanPlayers;
		}
		int[] array = new int[num + 1];
		int[] array2 = new int[num + 1];
		for (int i = 0; i <= num; i++)
		{
			array[i] = i;
			array2[i] = i;
		}
		for (int j = 0; j < numHumanPlayers; j++)
		{
			int target = j + 1;
			int desiredOld = desiredCamps[j];
			if (desiredOld <= 0 || desiredOld > num)
			{
				return null;
			}
			int oldAtTarget = array2[target];
			int newOfDesired = array[desiredOld];
			if (newOfDesired == target)
			{
				continue;
			}
			array[desiredOld] = target;
			array[oldAtTarget] = newOfDesired;
			array2[target] = desiredOld;
			array2[newOfDesired] = oldAtTarget;
		}
		if (table.mbcList != null)
		{
			for (int k = 0; k < table.mbcList.Count; k++)
			{
				MapBuildingConfig mapBuildingConfig = table.mbcList[k];
				if (mapBuildingConfig != null)
				{
					int camption = mapBuildingConfig.camption;
					if (camption > 0 && camption <= num)
					{
						mapBuildingConfig.camption = array[camption];
					}
				}
			}
		}
		if (table.mpcList != null)
		{
			for (int l = 0; l < table.mpcList.Count; l++)
			{
				MapPlayerConfig mapPlayerConfig = table.mpcList[l];
				if (mapPlayerConfig != null)
				{
					int camption2 = mapPlayerConfig.camption;
					if (camption2 > 0 && camption2 <= num)
					{
						mapPlayerConfig.camption = array[camption2];
					}
				}
			}
		}
		return array2;
	}

	public void OnNetFrame(int msgID, PacketEvent msgBody)
	{
		MemoryStream source = msgBody.Data as MemoryStream;
		SCFrame frame = Serializer.Deserialize<SCFrame>(source);
		this.OnNetFrame(frame);
	}

	public void OnNetFrame(SCFrame frame)
	{
        Solarmax.Singleton<BattleSystem>.Instance.OnRecievedFramePacket(frame);
	}

	private void OnFinishBattle(int msgID, PacketEvent msgBody)
	{
		this.Log("OnFinishBattle", new object[0]);
		MemoryStream source = msgBody.Data as MemoryStream;
		SCFinishBattle scfinishBattle = Serializer.Deserialize<SCFinishBattle>(source);
		for (int i = 0; i < scfinishBattle.users.Count; i++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(scfinishBattle.users[i]);
			if (team != null)
			{
				team.scoreMod = scfinishBattle.score_mods[i];
				team.resultOrder = -3 + i;
				team.resultRank = scfinishBattle.rank[i];
				team.resultEndtype = scfinishBattle.end_type[i];
				team.rewardMoney = scfinishBattle.reward_mods[i];
				team.rewardMuitly = scfinishBattle.reward_multipy[i];
				if (scfinishBattle.mvp_num.Count > 0)
				{
					team.leagueMvp = scfinishBattle.mvp_num[i];
				}
				if (Solarmax.Singleton<LocalPlayer>.Get().playerData.userId == scfinishBattle.users[i])
				{
					Solarmax.Singleton<SeasonRewardModel>.Get().seasonMaxScore = scfinishBattle.max_score;
				}
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinished, new object[]
		{
			scfinishBattle
		});
	}

	public void SendMoveMessaeg(Node from, Node to)
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		if (battleData.gameState != GameState.Game)
		{
			return;
		}
		if (from == null || to == null)
		{
			return;
		}
		TEAM currentTeam = battleData.currentTeam;
		int shipFactCount = from.GetShipFactCount((int)currentTeam);
		if (shipFactCount == 0)
		{
			return;
		}
		FramePacket framePacket = new FramePacket();
		framePacket.type = 0;
		framePacket.move = new MovePacket();
		bool flag = false;
		CooperationType battleSubType = battleData.battleSubType;
		int numHumanPlayers = 4;
		if (battleSubType == CooperationType.CT_1v1 || battleSubType == CooperationType.CT_2vPC)
		{
			numHumanPlayers = 2;
		}
		else if (battleSubType == CooperationType.CT_1v1v1 || battleSubType == CooperationType.CT_3vPC)
		{
			numHumanPlayers = 3;
		}
		else if (battleSubType == CooperationType.CT_4vPC && battleData.matchPlayerNum == 5)
		{
			numHumanPlayers = 5;
		}
		else if (battleSubType == CooperationType.CT_1v1v1v1 || battleSubType == CooperationType.CT_2v2 || battleSubType == CooperationType.CT_3v1 || battleSubType == CooperationType.CT_4vPC)
		{
			numHumanPlayers = 4;
		}
		int[] array;
		// 地图切换后 matchId 会变成第二张地图ID，但 player_control 是按原始关卡ID配置的，需用 levelId 查找
		string playerControlLevelId = string.IsNullOrEmpty(battleData.levelId) ? battleData.matchId : battleData.levelId;
		if (Solarmax.Singleton<ChapterAssistConfigProvider>.Instance.TryGetPlayerControl(playerControlLevelId, numHumanPlayers, out array))
		{
			flag = true;
		}
		framePacket.move.team = (flag ? currentTeam : TEAM.Neutral);
		framePacket.move.from = from.tag;
		framePacket.move.to = to.tag;
		if (Solarmax.Singleton<LocalSettingStorage>.Get().fightOption == 1)
		{
			int num = (int)Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderNumber;
			framePacket.move.optype = 1;
			if (num > 0 && shipFactCount <= num)
			{
				num = shipFactCount;
			}
			if (num == -1)
			{
				num = shipFactCount;
			}
			framePacket.move.rate = (float)num;
		}
		else
		{
			framePacket.move.optype = 0;
			framePacket.move.rate = Solarmax.Singleton<BattleSystem>.Instance.battleData.sliderRate;
		}
		byte[] content = Json.EnCodeBytes(framePacket);
		PbFrame pbFrame = new PbFrame();
		CSFrame csframe = new CSFrame();
		pbFrame.content = content;
		csframe.frame = pbFrame;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
	}

	public void SendSkillPacket(int skillId)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState != GameState.Game)
		{
			return;
		}
		byte[] content = Json.EnCodeBytes(new FramePacket
		{
			type = 1,
			skill = new SkillPacket
			{
				skillID = skillId,
				from = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam,
				to = TEAM.Neutral,
				tag = string.Empty
			}
		});
		PbFrame pbFrame = new PbFrame();
		CSFrame csframe = new CSFrame();
		pbFrame.content = content;
		csframe.frame = pbFrame;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
	}

	public void RequestUser()
	{
		string localAccount = Solarmax.Singleton<LocalPlayer>.Get().GetLocalAccount();
		CSGetUserData csgetUserData = new CSGetUserData();
		csgetUserData.account = localAccount;
		csgetUserData.token = Solarmax.Singleton<LocalAccountStorage>.Get().token;
		csgetUserData.web_test = Solarmax.Singleton<LocalAccountStorage>.Get().webTest;
		csgetUserData.app_version = UpgradeUtil.GetAppVersion();
		csgetUserData.imei_md5 = Solarmax.Singleton<EngineSystem>.Instance.GetUUID();
		csgetUserData.channel = Solarmax.Singleton<ThirdPartySystem>.Instance.GetChannel();
		csgetUserData.device_model = Solarmax.Singleton<EngineSystem>.Instance.GetDeviceModel();
		csgetUserData.os_version = Solarmax.Singleton<EngineSystem>.Instance.GetOS();
		csgetUserData.ver_code = (long)UpgradeUtil.GetVersionConfig().VersionCode;
		csgetUserData.version_name = UpgradeUtil.GetVersionConfig().VersionName;
		//this.OnRequestUser(1, default(PacketEvent));
        Solarmax.Singleton<NetSystem>.Instance.Send<CSGetUserData>(11, csgetUserData, false);
        MonoSingleton<FlurryAnalytis>.Instance.LogEvent("RequestUserStart");
	}

	private void OnRequestUser(int msgId, PacketEvent msg)
	{
        Solarmax.Singleton<LoggerSystem>.Instance.Info("PacketHelper  onRequestUser", new object[0]);
        MemoryStream source = msg.Data as MemoryStream;
        SCGetUserData scgetUserData = Serializer.Deserialize<SCGetUserData>(source);
        Debug.LogFormat("OnRequestUser: {0}", new object[]
        {
            scgetUserData.errcode
        });
        VoiceEngine.SetAppId(scgetUserData.tencent_appid.ToString());
        if (scgetUserData.data != null && scgetUserData.data.data_version < 1)
        {
            PlayerPrefs.DeleteAll();
            CSClearData csclearData = new CSClearData();
            csclearData.version = 1;
            Solarmax.Singleton<NetSystem>.Instance.Send<CSClearData>(340, csclearData, false);
        }
        if (scgetUserData.errcode == ErrCode.EC_Ok || scgetUserData.errcode == ErrCode.EC_NeedResume)
        {
            UserData data = scgetUserData.data;
            Solarmax.Singleton<LocalPlayer>.Get().playerData.Init(data);
            Solarmax.Singleton<LocalPlayer>.Get().playerData.InitRace(scgetUserData.race);
            Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver = false;
            this.OnUserDataInit();
        }
        if (scgetUserData.data != null && scgetUserData.data.chapterBuy != null && scgetUserData.data.chapterBuy.Count > 0)
        {
            for (int i = 0; i < scgetUserData.data.chapterBuy.Count; i++)
            {
                Solarmax.Singleton<LevelDataHandler>.Get().UnLockPayChapter(scgetUserData.data.chapterBuy[i]);
            }
        }
        if (scgetUserData.data != null && scgetUserData.data.skinBuy != null && scgetUserData.data.skinBuy.Count > 0)
        {
            for (int j = 0; j < scgetUserData.data.skinBuy.Count; j++)
            {
                Solarmax.Singleton<CollectionModel>.Get().UnLock(scgetUserData.data.skinBuy[j]);
            }
        }
        if (scgetUserData.data != null && scgetUserData.data.RechargeData != null)
        {
            if (scgetUserData.data.RechargeData.first_recharge_mark != null && scgetUserData.data.RechargeData.first_recharge_mark.Count > 0)
            {
                for (int k = 0; k < scgetUserData.data.RechargeData.first_recharge_mark.Count; k++)
                {
                    Solarmax.Singleton<LocalPlayer>.Get().AddBuy(scgetUserData.data.RechargeData.first_recharge_mark[k]);
                }
            }
            Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive = scgetUserData.data.RechargeData.last_receive_time;
            Solarmax.Singleton<LocalPlayer>.Get().month_card_end = scgetUserData.data.RechargeData.monthly_card_end_time;
        }
        if (scgetUserData.data != null)
        {
            int id = 0;
            if (int.TryParse(scgetUserData.data.LadderID, out id))
            {
                Solarmax.Singleton<SeasonRewardModel>.Get().Init(id, scgetUserData.data.maxscore, scgetUserData.data.LadderReward);
            }
        }
        if (scgetUserData.data != null && scgetUserData.data.pack != null)
        {
            Solarmax.Singleton<ItemDataHandler>.Get().InitPage(scgetUserData.data.pack);
        }
        if (scgetUserData.now > 0)
        {
            Solarmax.Singleton<TimeSystem>.Instance.SetServerTime(scgetUserData.now);
        }
        Solarmax.Singleton<LocalPlayer>.Get().InitAntiConfig(scgetUserData.online_time, scgetUserData.offline_time);
        string singleCurrentLevel = Solarmax.Singleton<LocalAccountStorage>.Get().singleCurrentLevel;
        string guideFightLevel = Solarmax.Singleton<LocalAccountStorage>.Get().guideFightLevel;
        if (GuideManager.Guide != null && !string.IsNullOrEmpty(guideFightLevel))
        {
            GuideManager.Guide.InitCompletedGuide(guideFightLevel);
        }
        Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightLevel = singleCurrentLevel;
        Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel = new Dictionary<AdChannel, AdConfig>();
        foreach (AdConfig adConfig in scgetUserData.ad_channel)
        {
            AdChannel channel = adConfig.channel;
            if (channel != AdChannel.AD_MIMENG)
            {
                if (channel == AdChannel.AD_PANGOLIN)
                {
                    AdConfig value = new AdConfig();
                    Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel[AdChannel.AD_PANGOLIN] = value;
                }
            }
            else
            {
                AdConfig adConfig2 = new AdConfig();
                adConfig2.app_id = adConfig.app_id;
                adConfig2.horizontal_video_id = adConfig.horizontal_video_id;
                Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel[AdChannel.AD_MIMENG] = adConfig2;
            }
        }
        MonoSingleton<FlurryAnalytis>.Instance.FlurryRequestUserSuccess(scgetUserData.errcode.ToString());
        AppsFlyerTool.FlyerLoginEvent();
        Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Login, 1);
        Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.RequestUserResult, new object[]
        {
            scgetUserData.errcode
        });
        //Solarmax.Singleton<LoggerSystem>.Instance.Info("On Request User", new object[0]);
        //VoiceEngine.SetAppId(1108207306L.ToString());
        //UserData userData = new UserData();
        //PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
        //string localName = Solarmax.Singleton<LocalPlayer>.Get().GetLocalName();
        //if (!string.IsNullOrEmpty(localName))
        //{
        //	Solarmax.Singleton<LoggerSystem>.Instance.Info("Get Local name: " + localName, new object[0]);
        //	userData.name = localName;
        //}
        //else
        //{
        //	userData.name = "";
        //}
        //int userId = playerData.userId;
        //userData.userid = playerData.userId;
        //int money = playerData.money;
        //userData.gold = playerData.money;
        //userData.account_id = "喵喵";
        //userData.battle_count = 0;
        //userData.mvp_count = 0;
        //Solarmax.Singleton<LocalPlayer>.Get().playerData.Init(userData);
        //if (playerData.raceList.Count < 1)
        //{
        //	RaceData raceData = new RaceData();
        //	raceData.level = 1;
        //	raceData.race = 1;
        //	Solarmax.Singleton<LocalPlayer>.Get().playerData.InitRace(new List<RaceData>
        //	{
        //		raceData
        //	});
        //}
        //else
        //{
        //	Solarmax.Singleton<LocalPlayer>.Get().playerData.InitRace(playerData.raceList);
        //}
        //Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver = false;
        //this.OnUserDataInit();
        //Solarmax.Singleton<LevelDataHandler>.Get().UnLockPayChapter("0");
        //Solarmax.Singleton<CollectionModel>.Get().UnLock(0);
        //Solarmax.Singleton<LocalPlayer>.Get().AddBuy("0");
        //int id = 1;
        //Solarmax.Singleton<SeasonRewardModel>.Get().Init(id, 99999, new List<bool>
        //{
        //	true
        //});
        //Solarmax.Singleton<ItemDataHandler>.Get().InitPage(new Pack());
        //Solarmax.Singleton<LocalPlayer>.Get().InitAntiConfig(0L, 0L);
        //string singleCurrentLevel = Solarmax.Singleton<LocalAccountStorage>.Get().singleCurrentLevel;
        //string guideFightLevel = Solarmax.Singleton<LocalAccountStorage>.Get().guideFightLevel;
        //if (GuideManager.Guide != null && !string.IsNullOrEmpty(guideFightLevel))
        //{
        //	GuideManager.Guide.InitCompletedGuide(guideFightLevel);
        //}
        //Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightLevel = singleCurrentLevel;
        //Solarmax.Singleton<LocalPlayer>.Get().playerData.adchannel = new Dictionary<AdChannel, AdConfig>();
        //MonoSingleton<FlurryAnalytis>.Instance.FlurryRequestUserSuccess(ErrCode.EC_Ok.ToString());
        //AppsFlyerTool.FlyerLoginEvent();
        //Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Login, 1);
        //Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.RequestUserResult, new object[]
        //{
        //	ErrCode.EC_Ok
        //});
    }

	private void OnUserDataInit()
	{
		PlayerData playerData = Solarmax.Singleton<LocalPlayer>.Get().playerData;
		VoiceEngine.Init(playerData.userId);
		MonoSingleton<FlurryAnalytis>.Instance.FlurryUserDataInit();
		MiGameAnalytics.MiAnalyticsUserDataInit();
	}

	public void CreateUser(string name, string iconPath)
	{
		string localAccount = Solarmax.Singleton<LocalPlayer>.Get().GetLocalAccount();
		if (string.IsNullOrEmpty(localAccount))
		{
			Debug.LogError("注册时本地账号为空");
		}
		CSCreateUserData cscreateUserData = new CSCreateUserData();
		cscreateUserData.account = localAccount;
		cscreateUserData.name = name;
		cscreateUserData.icon = iconPath;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSCreateUserData>(13, cscreateUserData, false);
		MonoSingleton<FlurryAnalytis>.Instance.FlurryCreateUserSuccess();
	}

	private void OnCreateUser(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCCreateUserData sccreateUserData = Serializer.Deserialize<SCCreateUserData>(source);
		if (sccreateUserData.errcode == ErrCode.EC_Ok)
		{
			UserData data = sccreateUserData.data;
			Solarmax.Singleton<LocalPlayer>.Get().playerData.Init(data);
			if (sccreateUserData.data != null && sccreateUserData.data.chapterBuy != null && sccreateUserData.data.chapterBuy.Count > 0)
			{
				for (int i = 0; i < sccreateUserData.data.chapterBuy.Count; i++)
				{
					Solarmax.Singleton<LevelDataHandler>.Get().UnLockPayChapter(sccreateUserData.data.chapterBuy[i]);
				}
			}
			if (sccreateUserData.data != null)
			{
				int id = 0;
				if (int.TryParse(sccreateUserData.data.LadderID, out id))
				{
					Solarmax.Singleton<SeasonRewardModel>.Get().Init(id, sccreateUserData.data.maxscore, sccreateUserData.data.LadderReward);
				}
			}
			this.OnUserDataInit();
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("CreateUserSuccess", "info", data.userid.ToString());
		}
		else
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("CreateUserFailed", "info", sccreateUserData.errcode.ToString());
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.CreateUserResult, new object[]
		{
			sccreateUserData.errcode
		});
	}

	public void ChangeIcon(string iconPath)
	{
		Solarmax.Singleton<LocalPlayer>.Get().playerData.icon = iconPath;
		CSSetIcon cssetIcon = new CSSetIcon();
		cssetIcon.icon = iconPath;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSSetIcon>(15, cssetIcon, true);
	}

	private void OnErrCode(int msgId, PacketEvent msg)
	{
	}

	public void PingNet()
	{
        //Solarmax.Singleton<LoggerSystem>.Instance.Info("ping net", new object[0]);
        //PacketEvent msg = new PacketEvent(1, DateTime.Now.ToBinary());
        //this.OnPong(1, msg);
        CSPing csping = new CSPing();
		csping.timestamp = DateTime.Now.ToBinary();
		//csping.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000;
        Solarmax.Singleton<NetSystem>.Instance.Send<CSPing>(23, csping, false);
    }

    private void OnPong(int msgId, PacketEvent msg)
	{
        //float num = (float)(DateTime.Now - DateTime.FromBinary((long)msg.Data)).TotalMilliseconds;
        MemoryStream source = msg.Data as MemoryStream;
        SCPong scpong = Serializer.Deserialize<SCPong>(source);
		float num = (float)(DateTime.Now - DateTime.FromBinary(scpong.timestamp)).TotalMilliseconds;
		//float num = DateTimeOffset.UtcNow.ToUnixTimeSeconds() * 1000 - scpong.timestamp;
        Solarmax.Singleton<NetSystem>.Instance.ping.Pong(num);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PingRefresh, new object[]
		{
			num
		});
	}

	public void LoadRank(int start, int type)
	{
		CSLoadRank csloadRank = new CSLoadRank();
		csloadRank.start = start;
		csloadRank.type = type;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLoadRank>(25, csloadRank, true);
	}

	private void OnLoadRank(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLoadRank scloadRank = Serializer.Deserialize<SCLoadRank>(source);
		List<PlayerData> list = new List<PlayerData>();
		for (int i = 0; i < scloadRank.data.Count; i++)
		{
			PlayerData playerData = new PlayerData();
			playerData.Init(scloadRank.data[i]);
			list.Add(playerData);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.LoadRankList, new object[]
		{
			list,
			scloadRank.start,
			scloadRank.self
		});
	}

	public void FriendFollow(int userId, bool follow)
	{
		CSFriendFollow csfriendFollow = new CSFriendFollow();
		csfriendFollow.userid = userId;
		csfriendFollow.follow = follow;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFriendFollow>(37, csfriendFollow, true);
	}

	private void OnFriendFollow(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCFriendFollow scfriendFollow = Serializer.Deserialize<SCFriendFollow>(source);
		if (scfriendFollow.err == ErrCode.EC_Ok)
		{
			SimplePlayerData simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(scfriendFollow.data);
			if (scfriendFollow.follow)
			{
				if (!Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(simplePlayerData))
				{
					Solarmax.Singleton<FriendDataHandler>.Get().AddMyFollow(simplePlayerData);
				}
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1145), 1f);
			}
			else
			{
				Solarmax.Singleton<FriendDataHandler>.Get().DelMyFollow(simplePlayerData);
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendFollowResult, new object[]
		{
			scfriendFollow.userid,
			scfriendFollow.follow,
			scfriendFollow.err
		});
	}

	public void FriendLoad(int start, bool myfollow)
	{
		//new PacketEvent(1, new CSFriendLoad
		//{
		//	start = start
		//});
		CSFriendLoad csfriendLoad = new CSFriendLoad();
		csfriendLoad.start = start;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFriendLoad>(39, csfriendLoad, true);
    }

	private void OnFriendLoad(int msgId, PacketEvent msg)
	{
		//SCFriendLoad scfriendLoad = (SCFriendLoad)msg.Data;
		//new List<SimplePlayerData>();
        MemoryStream source = msg.Data as MemoryStream;
        SCFriendLoad scfriendLoad = Serializer.Deserialize<SCFriendLoad>(source);
        List<SimplePlayerData> list = new List<SimplePlayerData>();
        Solarmax.Singleton<FriendDataHandler>.Get().Release();
		for (int i = 0; i < scfriendLoad.data.Count; i++)
		{
			SimplePlayerData simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(scfriendLoad.data[i]);
			Solarmax.Singleton<FriendDataHandler>.Get().AddMyFollow(simplePlayerData);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendLoadResult, new object[]
		{
			scfriendLoad.start
		});
	}

	public void OnSearchRoomRequest(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return;
		}
		CSSearchMatch cssearchMatch = new CSSearchMatch();
		cssearchMatch.key = key;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSSearchMatch>(322, cssearchMatch, true);
	}

	private void OnLoadBattleRooms(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLoadMatchList scloadMatchList = Serializer.Deserialize<SCLoadMatchList>(source);
		if (scloadMatchList.room.Count > 0)
		{
			List<MatchSynopsis> list = new List<MatchSynopsis>();
			for (int i = 0; i < scloadMatchList.room.Count; i++)
			{
				list.Add(new MatchSynopsis
				{
					type = scloadMatchList.room[i].type,
					c_type = scloadMatchList.room[i].c_type,
					match_id = scloadMatchList.room[i].match_id,
					map_id = scloadMatchList.room[i].map_id,
					watch_count = scloadMatchList.room[i].watch_count,
					fight_count = scloadMatchList.room[i].fight_count,
					difficulty = scloadMatchList.room[i].difficulty,
					match_name = scloadMatchList.room[i].match_name,
					match_lock = scloadMatchList.room[i].match_lock
				});
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSearchBattleRoomsResult, new object[]
			{
				list,
				scloadMatchList.page,
				scloadMatchList.optype
			});
		}
		else
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSearchBattleRoomsNull, new object[0]);
		}
	}

	private void OnEditRoomWatchName(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCEditWatchName sceditWatchName = Serializer.Deserialize<SCEditWatchName>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnModifyRoomNameResult, new object[]
		{
			sceditWatchName.match_id,
			sceditWatchName.match_name
		});
	}

	private void OnEditRoomLock(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCEditWatchLock sceditWatchLock = Serializer.Deserialize<SCEditWatchLock>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnModifyRoomLockResult, new object[]
		{
			sceditWatchLock.match_id,
			sceditWatchLock.match_lock
		});
	}

	private void OnFriendNotify(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCFriendNotify scfriendNotify = Serializer.Deserialize<SCFriendNotify>(source);
		SimplePlayerData simplePlayerData = new SimplePlayerData();
		simplePlayerData.Init(scfriendNotify.data);
		if (scfriendNotify.follow)
		{
			string message = string.Format(LanguageDataProvider.GetValue(1138), simplePlayerData.name);
			Tips.Make(Tips.TipsType.FlowUp, message, 1f);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendNotifyResult, new object[]
		{
			simplePlayerData,
			scfriendNotify.follow
		});
	}

	public void FriendRecommend(int start)
	{
		CSFriendRecommend csfriendRecommend = new CSFriendRecommend();
		csfriendRecommend.start = start;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFriendRecommend>(43, csfriendRecommend, true);
	}

	private void OnFriendRecommend(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCFriendRecommend scfriendRecommend = Serializer.Deserialize<SCFriendRecommend>(source);
		List<SimplePlayerData> list = new List<SimplePlayerData>();
		for (int i = 0; i < scfriendRecommend.data.Count; i++)
		{
			SimplePlayerData simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(scfriendRecommend.data[i]);
			list.Add(simplePlayerData);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendRecommendResult, new object[]
		{
			scfriendRecommend.start,
			list
		});
	}

	public void FriendSearch(string name, int userId, int ext = 0)
	{
		CSFriendSearch csfriendSearch = new CSFriendSearch();
		csfriendSearch.name = name;
		csfriendSearch.userid = userId;
		csfriendSearch.ext = ext;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFriendSearch>(45, csfriendSearch, true);
	}

	private void OnFriendLikeSearch(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCFriendSearchs scfriendSearchs = Serializer.Deserialize<SCFriendSearchs>(source);
		if (scfriendSearchs.datas != null)
		{
			List<SCFriendSearch> list = new List<SCFriendSearch>();
			for (int i = 0; i < scfriendSearchs.datas.Count; i++)
			{
				list.Add(new SCFriendSearch
				{
					data = scfriendSearchs.datas[i].data
				});
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendSearchResultAll, new object[]
			{
				list
			});
		}
	}

	private void OnFriendSearch(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCFriendSearch scfriendSearch = Serializer.Deserialize<SCFriendSearch>(source);
		SimplePlayerData simplePlayerData = null;
		bool flag = false;
		int num = 0;
		int num2 = 0;
		int num3 = 33;
		int num4 = 89;
		int num5 = 0;
		int num6 = 0;
		if (scfriendSearch.data != null)
		{
			simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(scfriendSearch.data);
			flag = scfriendSearch.followed;
			num = scfriendSearch.following_count;
			num2 = scfriendSearch.followers_count;
			num3 = scfriendSearch.data.mvp_count;
			num4 = scfriendSearch.data.battle_count;
			num5 = scfriendSearch.star_count;
			num6 = scfriendSearch.challenge_count;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFriendSearchResult, new object[]
		{
			simplePlayerData,
			flag,
			num,
			num2,
			num3,
			num4,
			num5,
			0,
			num6
		});
	}

	private void OnBoardFriendState(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBoardFriendState scboardFriendState = Serializer.Deserialize<SCBoardFriendState>(source);
		if (Solarmax.Singleton<FriendDataHandler>.Get().IsMyFriend(scboardFriendState.friend_id))
		{
			SimplePlayerData friend = Solarmax.Singleton<FriendDataHandler>.Get().GetFriend(scboardFriendState.friend_id);
			int onStats = friend.onStats;
			if (onStats != 0)
			{
				if (onStats != 1)
				{
					if (onStats == 2)
					{
						if (scboardFriendState.online == 0 || scboardFriendState.online == 1)
						{
							if (scboardFriendState.onBattle == 1)
							{
								Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 1);
							}
							else
							{
								Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 2);
							}
						}
						else
						{
							Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 0);
						}
					}
				}
				else if (scboardFriendState.online == 0 || scboardFriendState.online == 1)
				{
					if (scboardFriendState.onBattle == 0 || scboardFriendState.onBattle == 1)
					{
						Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 1);
					}
					else
					{
						Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 2);
					}
				}
				else
				{
					Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 0);
				}
			}
			else if (scboardFriendState.online == 0 || scboardFriendState.online == 2)
			{
				Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 0);
			}
			else if (scboardFriendState.onBattle == 1)
			{
				Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 1);
			}
			else
			{
				Solarmax.Singleton<FriendDataHandler>.Get().ChangeFriendState(scboardFriendState.friend_id, 2);
			}
		}
	}

	public void TeamCreate(bool is2v2, bool is3v3)
	{
		Solarmax.Singleton<TeamInviteData>.Get().Reset();
		CSTeamCreate csteamCreate = new CSTeamCreate();
		if (is2v2)
		{
			csteamCreate.type = BattleType.Group2v2;
		}
		else if (is3v3)
		{
			csteamCreate.type = BattleType.Group3v3;
		}
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTeamCreate>(61, csteamCreate, true);
	}

	private void OnTeamCreate(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamCreate scteamCreate = Serializer.Deserialize<SCTeamCreate>(source);
		if (scteamCreate.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<TeamInviteData>.Get().isLeader = true;
		}
		else
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(914), 1f);
		}
	}

	public void TeamInvite(int userId)
	{
		CSTeamInvite csteamInvite = new CSTeamInvite();
		csteamInvite.userId = userId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTeamInvite>(63, csteamInvite, true);
	}

	private void OnTeamInvite(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamInvite scteamInvite = Serializer.Deserialize<SCTeamInvite>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamInviteResult, new object[]
		{
			scteamInvite.code,
			scteamInvite.userId
		});
	}

	private void OnTeamUpdate(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamUpdate scteamUpdate = Serializer.Deserialize<SCTeamUpdate>(source);
		Solarmax.Singleton<TeamInviteData>.Get().battleType = scteamUpdate.type;
		Solarmax.Singleton<TeamInviteData>.Get().version = scteamUpdate.version;
		Solarmax.Singleton<TeamInviteData>.Get().teamPlayers.Clear();
		for (int i = 0; i < scteamUpdate.simUsers.Count; i++)
		{
			SimplePlayerData simplePlayerData = new SimplePlayerData();
			simplePlayerData.Init(scteamUpdate.simUsers[i]);
			Solarmax.Singleton<TeamInviteData>.Get().teamPlayers.Add(simplePlayerData);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamUpdate, new object[0]);
	}

	public void TeamLeave(int leaderId)
	{
		CSTeamLeave csteamLeave = new CSTeamLeave();
		csteamLeave.leaderId = leaderId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTeamLeave>(65, csteamLeave, true);
	}

	private void OnTeamDelete(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamDel scteamDel = Serializer.Deserialize<SCTeamDel>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamDelete, new object[]
		{
			scteamDel.code,
			scteamDel.leaderId
		});
	}

	public void TeamStart()
	{
		CSTeamStart proto = new CSTeamStart();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTeamStart>(67, proto, true);
	}

	private void OnTeamStart(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamStart scteamStart = Serializer.Deserialize<SCTeamStart>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamStart, new object[]
		{
			scteamStart.code,
			scteamStart.leaderId
		});
	}

	private void OnTeamInviteReq(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamInviteReq scteamInviteReq = Serializer.Deserialize<SCTeamInviteReq>(source);
		Solarmax.Singleton<TeamInviteData>.Get().Reset();
		SimplePlayerData simplePlayerData = new SimplePlayerData();
		simplePlayerData.Init(scteamInviteReq.leader);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState != GameState.Game)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("TeamNotifyWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamInviteRequest, new object[]
			{
				scteamInviteReq.type,
				simplePlayerData,
				scteamInviteReq.timestamp
			});
		}
	}

	public void TeamInviteResponse(bool accept, int leaderId, int timeStamp)
	{
		CSTeamInviteResp csteamInviteResp = new CSTeamInviteResp();
		csteamInviteResp.leaderId = leaderId;
		csteamInviteResp.timestamp = timeStamp;
		csteamInviteResp.accept = accept;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTeamInviteResp>(71, csteamInviteResp, true);
	}

	private void OnTeamInviteResponse(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTeamInviteResp scteamInviteResp = Serializer.Deserialize<SCTeamInviteResp>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTeamInviteResponse, new object[]
		{
			scteamInviteResp.code,
			scteamInviteResp.userId
		});
	}

	public void BattleReportLoad(bool self, int start)
	{
		CSBattleReportLoad csbattleReportLoad = new CSBattleReportLoad();
		csbattleReportLoad.self = self;
		csbattleReportLoad.start = start;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSBattleReportLoad>(51, csbattleReportLoad, true);
	}

	private void OnBattleReportLoad(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBattleReportLoad scbattleReportLoad = Serializer.Deserialize<SCBattleReportLoad>(source);
		List<BattleReportData> list = new List<BattleReportData>();
		for (int i = 0; i < scbattleReportLoad.report.Count; i++)
		{
			BattleReportData battleReportData = new BattleReportData();
			battleReportData.Init(scbattleReportLoad.report[i]);
			list.Add(battleReportData);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBattleReportLoad, new object[]
		{
			false,
			scbattleReportLoad.start,
			list
		});
	}

	public void BattleReportPlay(BattleReportData brd)
	{
		Solarmax.Singleton<BattleSystem>.Instance.replayManager.reportData = brd;
		CSBattleReportPlay csbattleReportPlay = new CSBattleReportPlay();
		csbattleReportPlay.battleid = brd.id;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSBattleReportPlay>(53, csbattleReportPlay, true);
	}

	public void onBattleReportPlay(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBattleReportPlay scbattleReportPlay = Serializer.Deserialize<SCBattleReportPlay>(source);
		PbSCFrames report = scbattleReportPlay.report;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBattleReportPlay, new object[]
		{
			report
		});
	}

	public void BattleResume(int frame)
	{
		CSBattleResume csbattleResume = new CSBattleResume();
		csbattleResume.startFrameNo = frame;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSBattleResume>(57, csbattleResume, true);
	}

	public void OnBattleResume(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBattleResume scbattleResume = Serializer.Deserialize<SCBattleResume>(source);
		PbSCFrames report = scbattleResume.report;
		Solarmax.Singleton<AssetManager>.Get().LoadBattleResources();
		GameState gameState = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState;
		if (gameState == GameState.Game || gameState == GameState.GameWatch || gameState == GameState.Watcher)
		{
			for (int i = 0; i < report.frames.Count; i++)
			{
				SCFrame frame = report.frames[i];
				this.OnNetFrame(frame);
			}
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.RunToFrame(Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame() / 5 + report.frames.Count);
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.SilentMode(true);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.resumingFrame = (Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame() / 5 + report.frames.Count) * 5;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.isResumeBattle = true;
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("ResumingWindow");
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(true, false);
			this.OnReady(report.ready);
			this.OnNetStartBattle(report.start);
			for (int j = 0; j < report.frames.Count; j++)
			{
				SCFrame frame2 = report.frames[j];
				this.OnNetFrame(frame2);
			}
			int count = report.frames.Count;
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.runFrameCount = 20;
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.RunToFrame(count);
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.SilentMode(true);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.resumingFrame = (count + count / 200) * 5;
		}
		if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("CommonDialogWindow"))
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
		}
	}

	public void RequestLeagueInfo()
	{
		CSGetLeagueInfo proto = new CSGetLeagueInfo();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGetLeagueInfo>(80, proto, true);
	}

	private void OnGetLeagueInfo(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGetLeagueInfo scgetLeagueInfo = Serializer.Deserialize<SCGetLeagueInfo>(source);
		if (scgetLeagueInfo.code != ErrCode.EC_Ok)
		{
			this.RequestLeagueList(0);
		}
		else
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGetLeagueInfoResult, new object[]
			{
				scgetLeagueInfo.league,
				scgetLeagueInfo.self
			});
		}
	}

	public void RequestLeagueList(int index)
	{
		CSLeagueList csleagueList = new CSLeagueList();
		csleagueList.start = index;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLeagueList>(82, csleagueList, true);
	}

	private void OnGetLeagueList(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLeagueList scleagueList = Serializer.Deserialize<SCLeagueList>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLeagueListResult, new object[]
		{
			scleagueList.start,
			scleagueList.league_info
		});
	}

	public void RequestLeagueSignUp(string leagueID)
	{
		CSLeagueAdd csleagueAdd = new CSLeagueAdd();
		csleagueAdd.league_id = leagueID;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLeagueAdd>(84, csleagueAdd, true);
	}

	private void OnLeagueSignUp(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLeagueAdd scleagueAdd = Serializer.Deserialize<SCLeagueAdd>(source);
		if (scleagueAdd.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGetLeagueInfoResult, new object[]
			{
				scleagueAdd.league,
				scleagueAdd.self
			});
		}
		else
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLeagueSignUpResult, new object[]
			{
				scleagueAdd.self,
				scleagueAdd.league_id
			});
		}
	}

	public void RequestLeagueRank(string leagueID, int index)
	{
		CSLeagueRank csleagueRank = new CSLeagueRank();
		csleagueRank.league_id = leagueID;
		csleagueRank.start = index;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLeagueRank>(86, csleagueRank, true);
	}

	private void OnLeagueRank(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLeagueRank scleagueRank = Serializer.Deserialize<SCLeagueRank>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLeagueRankResult, new object[]
		{
			scleagueRank.start,
			scleagueRank.members,
			scleagueRank.self,
			scleagueRank.self_rank
		});
	}

	public void MatchLeague(string leagueID)
	{
		CSLeagueMatch csleagueMatch = new CSLeagueMatch();
		csleagueMatch.league_id = leagueID;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLeagueMatch>(88, csleagueMatch, true);
	}

	private void OnLeagueMatch(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLeagueMatch scleagueMatch = Serializer.Deserialize<SCLeagueMatch>(source);
		if (scleagueMatch.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CustomSelectWindowNew");
			Solarmax.Singleton<UISystem>.Get().HideWindow("LeagueWindow");
			this.FakeStartBattle("PVP");
			Solarmax.Singleton<UISystem>.Get().ShowWindow("PVPWaitWindow");
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLeagueMatchResult, new object[]
		{
			scleagueMatch.code,
			scleagueMatch.count_down
		});
	}

	public void QuitLeague()
	{
		CSLeagueQuit proto = new CSLeagueQuit();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLeagueQuit>(170, proto, true);
	}

	private void OnLeagueQuitResult(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLeagueQuit scleagueQuit = Serializer.Deserialize<SCLeagueQuit>(source);
		if (scleagueQuit.code == ErrCode.EC_Ok)
		{
			this.RequestLeagueInfo();
		}
		else
		{
			Tips.Make(LanguageDataProvider.GetValue(752));
		}
	}

	public void RequestRoomList(int players)
	{
		CSWatchRooms cswatchRooms = new CSWatchRooms();
		cswatchRooms.playernum = players;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSWatchRooms>(92, cswatchRooms, true);
	}

	private void OnRequestRoomList(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCWatchRooms scwatchRooms = Serializer.Deserialize<SCWatchRooms>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGetRoomList, new object[]
		{
			scwatchRooms.rooms
		});
	}

	public void RequestUnWatchRooms()
	{
		CSUnWatchRooms proto = new CSUnWatchRooms();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSUnWatchRooms>(104, proto, true);
	}

	public void RequestJoinRoom(int roomid)
	{
		CSJoinRoom csjoinRoom = new CSJoinRoom();
		csjoinRoom.roomid = roomid;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSJoinRoom>(94, csjoinRoom, true);
	}

	private void OnRequestJoinRoom(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCJoinRoom scjoinRoom = Serializer.Deserialize<SCJoinRoom>(source);
		if (scjoinRoom.code != ErrCode.EC_Ok)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = scjoinRoom.room.matchid;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnJoinRoom, new object[]
		{
			scjoinRoom.data
		});
	}

	public void RequestCreateRoom(string matchid)
	{
		CSCreateRoom cscreateRoom = new CSCreateRoom();
		cscreateRoom.matchid = matchid;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSCreateRoom>(96, cscreateRoom, true);
	}

	private void OnRequestCreateRoom(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCCreateRoom sccreateRoom = Serializer.Deserialize<SCCreateRoom>(source);
		if (sccreateRoom.code != ErrCode.EC_Ok)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = sccreateRoom.room.matchid;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCreateRoom, new object[]
		{
			sccreateRoom.data
		});
	}

	public void RequestQuitRoom()
	{
		CSQuitRoom proto = new CSQuitRoom();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSQuitRoom>(98, proto, true);
	}

	private void OnRequestQuitRoom(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCQuitRoom scquitRoom = Serializer.Deserialize<SCQuitRoom>(source);
		if (scquitRoom.code != ErrCode.EC_Ok)
		{
			return;
		}
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	private void RoomRefresh(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCRoomRefresh scroomRefresh = Serializer.Deserialize<SCRoomRefresh>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRoomRefresh, new object[]
		{
			scroomRefresh.data
		});
	}

	private void RoomListRefresh(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCRoomListRefresh scroomListRefresh = Serializer.Deserialize<SCRoomListRefresh>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRoomListREfresh, new object[]
		{
			scroomListRefresh.roomid,
			scroomListRefresh.playernum
		});
	}

	private void OnMatch2CurNum(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatch2CurNum scmatch2CurNum = Serializer.Deserialize<SCMatch2CurNum>(source);
		Solarmax.Singleton<LocalPlayer>.Get().CurBattlePlayerNum = scmatch2CurNum.playernum;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRoomListREfresh, new object[]
		{
			scmatch2CurNum.playernum
		});
	}

	public void MatchGame2()
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
		CSStartMatch2 proto = new CSStartMatch2();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSStartMatch2>(108, proto, true);
	}

	private void ONMatchGameBack(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCStartMatch2 scstartMatch = Serializer.Deserialize<SCStartMatch2>(source);
		if (scstartMatch.code != ErrCode.EC_Ok)
		{
			return;
		}
		Solarmax.Singleton<UISystem>.Get().ShowWindow("WaitWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartMatch2, new object[0]);
		Solarmax.Singleton<UISystem>.Get().HideWindow("SelectRaceWindow");
	}

	public void SendSelectRace(int index)
	{
		CSChangeRace cschangeRace = new CSChangeRace();
		cschangeRace.race = index;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSChangeRace>(114, cschangeRace, true);
	}

	public void SendRaceEntry()
	{
		CSCommitRace proto = new CSCommitRace();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSCommitRace>(154, proto, true);
	}

	public void StartMatch3(bool bHavRace = false)
	{
		Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(true, false);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
		this.StartMatchReq(MatchType.MT_Ladder, string.Empty, string.Empty, CooperationType.CT_1v1v1v1, 4, false, string.Empty, -1, string.Empty, false);
	}

	private void OnStarMatch3(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCStartMatch3 scstartMatch = Serializer.Deserialize<SCStartMatch3>(source);
		if (scstartMatch.code != ErrCode.EC_Ok)
		{
			Tips.Make(Tips.TipsType.FlowUp, string.Format(LanguageDataProvider.GetValue(111), scstartMatch.code), 1f);
			return;
		}
	}

	private void OnMatch3Notify(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatch3Notify scmatch3Notify = Serializer.Deserialize<SCMatch3Notify>(source);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = scmatch3Notify.matchid;
		Solarmax.Singleton<LocalPlayer>.Get().battleMap = scmatch3Notify.matchid;
		List<PlayerData> list = new List<PlayerData>();
		list.Add(null);
		list.Add(null);
		list.Add(null);
		list.Add(null);
		for (int i = 0; i < scmatch3Notify.user.Count; i++)
		{
			PlayerData playerData = new PlayerData();
			playerData.Init(scmatch3Notify.user[i]);
			int index = scmatch3Notify.useridx[i];
			list[index] = playerData;
			if (playerData.userId <= 0)
			{
				playerData.name = AIManager.GetAIName(playerData.userId);
				playerData.icon = AIManager.GetAIIcon(playerData.userId);
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatch3Notify, new object[]
		{
			list
		});
		Solarmax.Singleton<LocalPlayer>.Get().mathPlayer = list;
	}

	private void OnChangeRace(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCChangeRace scchangeRace = Serializer.Deserialize<SCChangeRace>(source);
		if (scchangeRace.code != ErrCode.EC_Ok)
		{
			Tips.Make(Tips.TipsType.FlowUp, string.Format(LanguageDataProvider.GetValue(111), scchangeRace.code), 1f);
			return;
		}
	}

	private void OnStartSelectRace(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCStartSelectRace proto = Serializer.Deserialize<SCStartSelectRace>(source);
		this.OnStartSelectRace(proto);
	}

	private void OnStartSelectRace(SCStartSelectRace proto)
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("SelectRaceWindow");
		Solarmax.Singleton<UISystem>.Get().HideWindow("PVPWaitWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartSelectRace, new object[]
		{
			proto.races
		});
	}

	private void OnSelectRaceNotify(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCSelectRaceNotify message = Serializer.Deserialize<SCSelectRaceNotify>(source);
		this.OnSelectRaceNotify(message);
	}

	private void OnSelectRaceNotify(SCSelectRaceNotify message)
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRoomListREfresh, new object[]
		{
			message.user,
			message.race,
			message.ok
		});
		Debug.Log("OnSelectRaceNotify");
	}

	public void RequestRaceData()
	{
		CSGetRaceData proto = new CSGetRaceData();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGetRaceData>(118, proto, true);
	}

	public void RequestNoticeData()
	{
		CSClientStorageLoad proto = new CSClientStorageLoad();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSClientStorageLoad>(166, proto, true);
	}

	public void RequestSetNoticeData(int idx, string value)
	{
		CSClientStorageSet csclientStorageSet = new CSClientStorageSet();
		csclientStorageSet.index.Add(idx);
		csclientStorageSet.value.Add(value);
		Solarmax.Singleton<NetSystem>.Instance.Send<CSClientStorageSet>(164, csclientStorageSet, true);
	}

	private void OnGetRaceData(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGetRaceData scgetRaceData = Serializer.Deserialize<SCGetRaceData>(source);
		Solarmax.Singleton<LocalPlayer>.Get().playerData.InitRace(scgetRaceData.races);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGetRaceData, new object[]
		{
			scgetRaceData.races
		});
	}

	public void RequestRaceSkillLevelUp(int race, int skillIndex)
	{
		CSRaceSkillLevelUp csraceSkillLevelUp = new CSRaceSkillLevelUp();
		csraceSkillLevelUp.cur_race = race;
		csraceSkillLevelUp.skill_index = skillIndex;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSRaceSkillLevelUp>(120, csraceSkillLevelUp, true);
	}

	private void OnRaceSkillLevelUp(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCRaceSkillLevelUp scraceSkillLevelUp = Serializer.Deserialize<SCRaceSkillLevelUp>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRaceSkillLevelUp, new object[]
		{
			scraceSkillLevelUp.cur_race,
			scraceSkillLevelUp.cur_race,
			scraceSkillLevelUp.race_level,
			scraceSkillLevelUp.skill_index,
			scraceSkillLevelUp.new_skillid
		});
	}

	private void OnPackUpdate(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCPackUpdate scpackUpdate = Serializer.Deserialize<SCPackUpdate>(source);
		for (int i = 0; i < scpackUpdate.modified.Count; i++)
		{
			PackItem item = scpackUpdate.modified[i];
			Solarmax.Singleton<ItemDataHandler>.Get().ModifyItem(item);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUpdateKnapsack, new object[0]);
	}

	public void StartBox(int index)
	{
		CSUnlockChest csunlockChest = new CSUnlockChest();
		csunlockChest.slot = index;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSUnlockChest>(126, csunlockChest, true);
	}

	private void NotifyChestErrCode(ErrCode code)
	{
		switch (code)
		{
		case ErrCode.EC_ChestLocked:
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(630), 1f);
			return;
		case ErrCode.EC_ChestNoExist:
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(631), 1f);
			return;
		case ErrCode.EC_ChestJewelNotEnough:
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(632), 1f);
			return;
		case ErrCode.EC_ChestTimeNotEnough:
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(633), 1f);
			return;
		case ErrCode.EC_ChestWinNumNotEnough:
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(634), 1f);
			return;
		}
		Tips.Make(Tips.TipsType.FlowUp, code.ToString(), 1f);
	}

	private void OnStartBox(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCUnlockChest scunlockChest = Serializer.Deserialize<SCUnlockChest>(source);
		if (scunlockChest.code != ErrCode.EC_Ok)
		{
			this.NotifyChestErrCode(scunlockChest.code);
		}
		else
		{
			if (scunlockChest.time2unlock > 0)
			{
				int slot = scunlockChest.slot;
				ChessItem[] chesses = Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses;
				long num = (long)scunlockChest.time2unlock;
				chesses[slot].timeout = num;
				if (num > 0L)
				{
					ChessItem chessItem = chesses[slot];
					DateTime dateTime = new DateTime(1970, 1, 1);
					chessItem.timefinish = dateTime.AddSeconds((double)scunlockChest.time2unlock);
				}
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
		}
	}

	public void OpenBox(int index, bool useGold = false)
	{
		CSGainChest csgainChest = new CSGainChest();
		csgainChest.slot = index;
		csgainChest.use_jewel = useGold;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGainChest>(128, csgainChest, true);
	}

	private void OnOpenBox(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGainChest scgainChest = Serializer.Deserialize<SCGainChest>(source);
		if (scgainChest.code != ErrCode.EC_Ok)
		{
			this.NotifyChestErrCode(scgainChest.code);
		}
		else
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ChestWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[]
			{
				scgainChest
			});
		}
	}

	private void OnNotifyBox(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCAddChest scaddChest = Serializer.Deserialize<SCAddChest>(source);
		if (scaddChest.added != null)
		{
			int slot = scaddChest.added.slot;
			Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot] = new ChessItem();
			Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot].id = scaddChest.added.id;
			long timeout = scaddChest.added.timeout;
			Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot].timeout = timeout;
			if (timeout > 0L)
			{
				ChessItem chessItem = Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot];
				DateTime dateTime = new DateTime(1970, 1, 1);
				chessItem.timefinish = dateTime.AddSeconds((double)scaddChest.added.timeout);
			}
			Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot].slot = scaddChest.added.slot;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
		}
	}

	public void OpenLeftBox(int type)
	{
		if (type == 0)
		{
			CSGainTimerChest proto = new CSGainTimerChest();
			Solarmax.Singleton<NetSystem>.Instance.Send<CSGainTimerChest>(132, proto, true);
		}
		else
		{
			CSGainBattleChest proto2 = new CSGainBattleChest();
			Solarmax.Singleton<NetSystem>.Instance.Send<CSGainBattleChest>(134, proto2, true);
		}
	}

	private void OnOpenTimerChest(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGainTimerChest scgainTimerChest = Serializer.Deserialize<SCGainTimerChest>(source);
		if (scgainTimerChest.code != ErrCode.EC_Ok)
		{
			this.NotifyChestErrCode(scgainTimerChest.code);
		}
		else
		{
			Solarmax.Singleton<LocalPlayer>.Get().playerData.timechest = scgainTimerChest.time_out;
			Debug.Log(string.Format("OnOpenTimerChest:{0}", scgainTimerChest.time_out));
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ChestWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestTime, new object[]
			{
				scgainTimerChest
			});
		}
	}

	private void OnOpenBattleChest(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGainBattleChest scgainBattleChest = Serializer.Deserialize<SCGainBattleChest>(source);
		if (scgainBattleChest.code != ErrCode.EC_Ok)
		{
			this.NotifyChestErrCode(scgainBattleChest.code);
		}
		else
		{
			Solarmax.Singleton<LocalPlayer>.Get().playerData.curbattlechest = (int)scgainBattleChest.num;
			Debug.Log(string.Format("OnOpenBattleChest:{0}", scgainBattleChest.num));
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ChestWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestBattle, new object[]
			{
				scgainBattleChest
			});
		}
	}

	private void OnUpdateTimeChest(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCUpdateTimerChest scupdateTimerChest = Serializer.Deserialize<SCUpdateTimerChest>(source);
		Solarmax.Singleton<LocalPlayer>.Get().playerData.timechest = scupdateTimerChest.chest_gainpoint;
		Debug.Log(string.Format("OnUpdateTimeChest:{0}", scupdateTimerChest.chest_gainpoint));
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
	}

	private void OnUpdateBattleChest(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCUpdateBattleChest scupdateBattleChest = Serializer.Deserialize<SCUpdateBattleChest>(source);
		Solarmax.Singleton<LocalPlayer>.Get().playerData.curbattlechest = scupdateBattleChest.chest_winnum;
		Debug.Log(string.Format("OnUpdateBattleChest:{0}", Solarmax.Singleton<LocalPlayer>.Get().playerData.curbattlechest));
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
	}

	public void RequestGMCommand(string cmd)
	{
		CSGMCmd csgmcmd = new CSGMCmd();
		csgmcmd.cmd = cmd;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGMCmd>(124, csgmcmd, true);
	}

	public void RequestSingleMatch(string matchId, GameType etype, bool isFireEvent = true)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("PacketHelper  RequestSingleMatch", new object[0]);
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(false, true);
		Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = etype;
		Solarmax.Singleton<BattleSystem>.Instance.bStartBattle = true;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = matchId;
		Solarmax.Singleton<LocalPlayer>.Get().battleMap = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.seed = DateTime.UtcNow.Millisecond;
		int seed = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.seed;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.aiRand.seed = seed;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.teamFight = false;
		Debug.Log("Replayer Battle Seed = " + seed.ToString());
		List<int> list = new List<int>();
		MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(matchId);
		if (data != null)
		{
			foreach (MapBuildingConfig mapBuildingConfig in data.mbcList)
			{
				list.Add(mapBuildingConfig.camption);
			}
		}
		int num = 0;
		MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(matchId);
		LevelConfig data3 = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchId);
		int num2 = (data3.playerTeam >= 1) ? data3.playerTeam : 1;
		if (string.IsNullOrEmpty(data2.defaultai))
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = -1;
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = int.Parse(data2.defaultai);
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy < 0)
		{
			LevelConfig data4 = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(matchId);
			if (data4 == null)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = 3;
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = Solarmax.Singleton<AIStrategyConfigProvider>.Instance.GetAIStrategyByName(data4.aiType);
			}
		}
		for (int i = 0; i < data2.teamFriendList.Count - 1; i += 2)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[i + 1]).teamFriend[data2.teamFriendList[i]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[i]).teamFriend[data2.teamFriendList[i + 1]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[i]).teamFriend[data2.teamFriendList[i]] = true;
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)data2.teamFriendList[i + 1]).teamFriend[data2.teamFriendList[i + 1]] = true;
			LoggerSystem.CodeComments("代码注释-结盟控制：由地图文件中的结盟对来设置结盟");
		}
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy < 0)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy = 1;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.winType = data3.winType;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1 = data3.winTypeParam1;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2 = data3.winTypeParam2;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.loseType = data3.loseType;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam1 = data3.loseTypeParam1;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam2 = data3.loseTypeParam2;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.runWithScript = false;
		List<int> list2 = new List<int>();
		for (int j = 0; j < LocalPlayer.MaxTeamNum - 1; j++)
		{
			if (list.Contains(j + 1))
			{
				Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)(j + 1));
				if (team.team == (TEAM)num2)
				{
					team.playerData.Init(Solarmax.Singleton<LocalPlayer>.Get().playerData);
					Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam = team.team;
				}
				else
				{
					team.playerData.userId = -10000 - j;
					team.playerData.name = AIManager.GetAIName(team.playerData.userId);
					team.playerData.icon = AIManager.GetAIIcon(team.playerData.userId);
					int num3 = data2.teamAiType[j + 1];
					if (num3 < 0)
					{
						num3 = Solarmax.Singleton<BattleSystem>.Instance.battleData.aiStrategy;
					}
					Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.AddAI(team, num3, team.playerData.level, Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel, 1f);
					team.aiEnable = true;
					Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("加入了AI：{0}, 类型：{1}", team.playerData.name, Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.aiData[j + 1].aiStrategy), new object[0]);
				}
				list2.Add(team.playerData.userId);
				num++;
				if (num >= data2.player_count)
				{
					break;
				}
			}
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState = GameState.Game;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.Reset();
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateScene(list2, false, false);
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.useAI)
		{
			Solarmax.Singleton<BattleSystem>.Instance.sceneManager.aiManager.Start(1f);
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.useAI = true;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.replay = true;
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 1f;
		Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);
		ReplayCollectManager.Get().CreateReplayStruct(matchId, seed);
		if (isFireEvent)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartSingleBattle, new object[0]);
		}
		AppsFlyerTool.FlyerPveBattleStartEvent();
	}

	private void OnSingleMatch(int msgid, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCSingleMatch scsingleMatch = Serializer.Deserialize<SCSingleMatch>(source);
		if (scsingleMatch.code != ErrCode.EC_Ok)
		{
			Tips.Make(LanguageDataProvider.GetValue(209) + scsingleMatch.code.ToString());
			return;
		}
	}

	private void FakeStartBattle(string mapId)
	{
		Solarmax.Singleton<BattleSystem>.Instance.battleData.isFakeBattle = true;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId = mapId;
		for (int i = 0; i < 4; i++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)(i + 1));
			team.groupID = 99;
		}
		Solarmax.Singleton<AssetManager>.Get().FakeLoadBattleResources();
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.CreateScene(null, false, false);
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.replay = true;
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 1f;
		Solarmax.Singleton<BattleSystem>.Instance.sceneManager.FadePlanet(true, 0.1f);
		Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.IN, 0.1f);
		Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
		List<global::Packet> list = new List<global::Packet>();
		for (int j = 0; j < 600; j++)
		{
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.AddFrame(j + 1, list.ToArray());
		}
	}

	public void RequestSetCurrentLevel(string matchId, string guidId, string guidlevel)
	{
		string cur_level = string.Format("{0},{1},{2}", matchId, guidId, guidlevel);
		CSSetCurLevel cssetCurLevel = new CSSetCurLevel();
		cssetCurLevel.cur_level = cur_level;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSSetCurLevel>(158, cssetCurLevel, true);
	}

	private void OnSetCurrentLevelResult(int msgid, PacketEvent msg)
	{
	}

	public void ChangeName(string name)
	{
		PacketEvent msg = new PacketEvent(1, name);
		this.OnChangeName(1, msg);
	}

	private void OnChangeName(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRename, new object[]
		{
			ErrCode.EC_Ok
		});
	}

	private void OnRaceNotify(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCRaceNotify scraceNotify = Serializer.Deserialize<SCRaceNotify>(source);
		if (scraceNotify.raceid.Count < 1)
		{
			return;
		}
	}

	public void LoadClientStorage()
	{
		CSClientStorageLoad proto = new CSClientStorageLoad();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSClientStorageLoad>(166, proto, true);
	}

	private void OnClientStorageLoad(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCClientStorageLoad scclientStorageLoad = Serializer.Deserialize<SCClientStorageLoad>(source);
		for (int i = 0; i < scclientStorageLoad.values.Count; i++)
		{
			Solarmax.Singleton<LocalPlayer>.Get().playerData.clientStorages[i] = scclientStorageLoad.values[i];
		}
		if (scclientStorageLoad.values.Count >= 3)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStorageLoaded, new object[]
			{
				scclientStorageLoad.values[3]
			});
		}
	}

	public void SetClientStorage(int storageIndex, string vv)
	{
		CSClientStorageSet csclientStorageSet = new CSClientStorageSet();
		csclientStorageSet.index.Add(storageIndex);
		csclientStorageSet.value.Add(vv);
		Solarmax.Singleton<NetSystem>.Instance.Send<CSClientStorageSet>(164, csclientStorageSet, true);
	}

	private void OnClientStorageSet(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCClientStorageSet scclientStorageSet = Serializer.Deserialize<SCClientStorageSet>(source);
		if (scclientStorageSet.code == ErrCode.EC_Ok)
		{
			for (int i = 0; i < scclientStorageSet.index.Count; i++)
			{
				int num = scclientStorageSet.index[i];
				string text = scclientStorageSet.value[i];
				Solarmax.Singleton<LocalPlayer>.Get().playerData.clientStorages[num] = text;
			}
		}
	}

	public void ReconnectResume(int frame = -2)
	{
		CSResume csresume = new CSResume();
		if (frame > -2)
		{
			csresume.startFrameNo = frame;
		}
		Solarmax.Singleton<NetSystem>.Instance.Send<CSResume>(172, csresume, false);
	}

	public void RequestCancelBattle()
	{
		byte[] content = Json.EnCodeBytes(new FramePacket
		{
			type = 2,
			giveup = new GiveUpPacket
			{
				team = TEAM.Team_1
			}
		});
		PbFrame pbFrame = new PbFrame();
		CSFrame csframe = new CSFrame();
		pbFrame.content = content;
		csframe.frame = pbFrame;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSFrame>(7, csframe, false);
	}

	private void OnReconnectResumeResult(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCResume scresume = Serializer.Deserialize<SCResume>(source);
		if (scresume.code != ErrCode.EC_Ok)
		{
			Solarmax.Singleton<BattleSystem>.Instance.Reset();
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType = scresume.sub_type;
		if (scresume.match != null)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType = scresume.match.typ;
		}
		if (scresume.match != null)
		{
			if (scresume.match.typ == MatchType.MT_Ladder)
			{
				if (scresume.sub_type == CooperationType.CT_2v2 && !scresume.quick)
				{
					Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
					Solarmax.Singleton<UISystem>.Instance.ShowWindow("Room2V2Window");
				}
				else
				{
					Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
					Solarmax.Singleton<UISystem>.Get().ShowWindow("PVPWaitWindow");
				}
				this.OnMatchInit(scresume.match);
			}
			else if (scresume.match.typ == MatchType.MT_Room)
			{
				Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("RoomWaitWindow");
				this.OnMatchInit(scresume.match);
			}
		}
		else if (scresume.start != null)
		{
			Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
			this.OnStartSelectRace(scresume.start);
			this.OnSelectRaceNotify(scresume.notify);
		}
		else if (scresume.report != null)
		{
			PbSCFrames report = scresume.report;
			Solarmax.Singleton<AssetManager>.Get().LoadBattleResources();
			GameState gameState = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameState;
			if (gameState == GameState.Game || gameState == GameState.GameWatch || gameState == GameState.Watcher)
			{
				for (int i = 0; i < report.frames.Count; i++)
				{
					SCFrame frame = report.frames[i];
					this.OnNetFrame(frame);
				}
				Solarmax.Singleton<BattleSystem>.Instance.lockStep.RunToFrame(Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame() / 5 + report.frames.Count);
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.SilentMode(true);
				Solarmax.Singleton<BattleSystem>.Instance.battleData.resumingFrame = (Solarmax.Singleton<BattleSystem>.Instance.GetCurrentFrame() / 5 + report.frames.Count) * 5;
				Solarmax.Singleton<BattleSystem>.Instance.battleData.isResumeBattle = true;
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("ResumingWindow");
			}
			else
			{
				Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(true, false);
				Solarmax.Singleton<BattleSystem>.Instance.battleData.resumingFrame = (report.frames.Count + report.frames.Count / 200) * 5;
				this.OnReady(report.ready);
				this.OnNetStartBattle(report.start);
				for (int j = 0; j < report.frames.Count; j++)
				{
					SCFrame frame2 = report.frames[j];
					this.OnNetFrame(frame2);
				}
				Solarmax.Singleton<BattleSystem>.Instance.lockStep.runFrameCount = 20;
				Solarmax.Singleton<BattleSystem>.Instance.lockStep.RunToFrame(report.frames.Count);
				Solarmax.Singleton<BattleSystem>.Instance.sceneManager.SilentMode(true);
				Solarmax.Singleton<BattleSystem>.Instance.battleData.isResumeBattle = true;
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("ResumingWindow");
				string matchId = Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId;
				if (!string.IsNullOrEmpty(matchId))
				{
					LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(matchId);
					if (data != null)
					{
						Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectChapter(data.chapter);
						Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(matchId, data.levelGroup);
					}
				}
			}
		}
		if (Solarmax.Singleton<UISystem>.Get().IsWindowVisible("CommonDialogWindow"))
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
		}
		VoiceEngine.OnReconnectResume();
	}

	private void OnKickUserNtf(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCKickUserNtf sckickUserNtf = Serializer.Deserialize<SCKickUserNtf>(source);
		if (sckickUserNtf.code == ErrCode.EC_AAOvertime)
		{
			Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver = true;
			if (Solarmax.Singleton<BattleSystem>.Instance.bStartBattle)
			{
				return;
			}
			Solarmax.Singleton<LocalPlayer>.Get().AccountTokenOver();
		}
		else
		{
			Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver = true;
			if (Solarmax.Singleton<BattleSystem>.Instance.bStartBattle)
			{
				return;
			}
			Solarmax.Singleton<NetSystem>.Instance.Close();
			string device_model = sckickUserNtf.device_model;
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogAntiWindow");
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
			EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
			EventId id = EventId.OnCommonDialog;
			object[] array = new object[3];
			array[0] = 1;
			array[1] = LanguageDataProvider.Format(505, new object[]
			{
				device_model
			});
			array[2] = new EventDelegate(delegate()
			{
				Solarmax.Singleton<NetSystem>.Instance.DisConnectedCallback();
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("LogoWindow");
			});
			instance.FireEvent(id, array);
		}
	}

	private void OnServerNotify(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCNotify scnotify = Serializer.Deserialize<SCNotify>(source);
		if (scnotify.typ == NotifyType.NT_Popup)
		{
			Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog, new object[]
			{
				1,
				scnotify.text
			});
		}
		else if (scnotify.typ == NotifyType.NT_Scroll)
		{
			Tips.Make(Tips.TipsType.FlowLeft, scnotify.text, 2f);
			Tips.Make(Tips.TipsType.FlowLeft, scnotify.text, 2f);
		}
		else if (scnotify.typ == NotifyType.NT_Error)
		{
			Tips.Make(Tips.TipsType.FlowUp, scnotify.text, 1f);
		}
	}

	public void StartMatchReq(MatchType type, string misc_id, string map_id, CooperationType cType, int nPlayerNum, bool bQuick = false, string chapterId = "", int nDifficult = -1, string GroupID = "", bool bEnterWatch = false)
	{
		if (type == MatchType.MT_Ladder)
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPlayMode(true, false);
			Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType = GameType.PVP;
		}
		if (string.IsNullOrEmpty(map_id))
		{
			string text = null;
			if (cType == CooperationType.CT_3v1)
			{
				text = "44";
			}
			else if (cType == CooperationType.CT_1v1)
			{
				text = "41";
			}
			else if (cType == CooperationType.CT_1v1v1)
			{
				text = "42";
			}
			else if (cType == CooperationType.CT_2v2 || cType == CooperationType.CT_1v1v1v1)
			{
				text = "40";
			}
			if (!string.IsNullOrEmpty(text))
			{
				LevelConfigConfigProvider levelConfigConfigProvider = Solarmax.Singleton<LevelConfigConfigProvider>.Get();
				if (levelConfigConfigProvider != null)
				{
					List<LevelConfig> levelsByPrefix = levelConfigConfigProvider.GetLevelsByPrefix(text);
					if (levelsByPrefix != null && levelsByPrefix.Count > 0)
					{
						LevelConfig levelConfig = levelsByPrefix[UnityEngine.Random.Range(0, levelsByPrefix.Count)];
						if (levelConfig != null)
						{
							if (string.IsNullOrEmpty(GroupID))
							{
								GroupID = levelConfig.id;
							}
							map_id = levelConfig.map;
							if (string.IsNullOrEmpty(map_id))
							{
								map_id = levelConfig.id;
							}
						}
					}
				}
			}
		}
		CSStartMatchReq csstartMatchReq = new CSStartMatchReq();
		csstartMatchReq.typ = type;
		csstartMatchReq.cType = cType;
		csstartMatchReq.nPlayerNum = nPlayerNum;
		if (!string.IsNullOrEmpty(misc_id))
		{
			csstartMatchReq.misc_id = misc_id;
		}
		if (!string.IsNullOrEmpty(map_id))
		{
			csstartMatchReq.mapId = map_id;
		}
		csstartMatchReq.has_race = false;
		csstartMatchReq.quick = bQuick;
		csstartMatchReq.chapterid = chapterId;
		csstartMatchReq.difficulty = nDifficult;
		csstartMatchReq.bEnterWatch = bEnterWatch;
		csstartMatchReq.level = GroupID;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSStartMatchReq>(176, csstartMatchReq, true);
	}

	private void OnStartMatchRequest(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCStartMatchReq scstartMatchReq = Serializer.Deserialize<SCStartMatchReq>(source);
		if (scstartMatchReq.code != ErrCode.EC_Ok)
		{
			if (scstartMatchReq.code == ErrCode.EC_MatchIsFull)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_MatchIsMember)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_NotInMatch)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_NotMaster)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_RoomNotExist)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(906), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_OnlyInvite)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2229), 1f);
			}
			else if (scstartMatchReq.code == ErrCode.EC_NotBuyThisChapter)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2141), 1f);
			}
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.matchType = scstartMatchReq.typ;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType = scstartMatchReq.cType;
		if (scstartMatchReq.typ == MatchType.MT_Ladder)
		{
			if (scstartMatchReq.code != ErrCode.EC_Ok)
			{
				Tips.Make(Tips.TipsType.FlowUp, string.Format(LanguageDataProvider.GetValue(111), scstartMatchReq.code), 1f);
				return;
			}
			if (scstartMatchReq.quick)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("PVPWaitWindow");
				Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = false;
			}
			else
			{
				Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("Room2V2Window");
				Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
			}
		}
		else if (scstartMatchReq.typ == MatchType.MT_Room)
		{
			if (scstartMatchReq.cType == CooperationType.CT_2vPC || scstartMatchReq.cType == CooperationType.CT_3vPC || scstartMatchReq.cType == CooperationType.CT_4vPC)
			{
				if (scstartMatchReq.quick)
				{
					Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
					Solarmax.Singleton<UISystem>.Get().ShowWindow("PVPWaitWindow");
					Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = false;
				}
				else if (scstartMatchReq.code == ErrCode.EC_Ok)
				{
					Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
					Solarmax.Singleton<UISystem>.Instance.ShowWindow("RoomWaitWindow");
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartMatchResult, new object[]
					{
						scstartMatchReq.code
					});
					Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
				}
			}
			else if (scstartMatchReq.code == ErrCode.EC_Ok)
			{
				Solarmax.Singleton<UISystem>.Instance.HideAllWindow();
				Solarmax.Singleton<UISystem>.Instance.ShowWindow("RoomWaitWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartMatchResult, new object[]
				{
					scstartMatchReq.code
				});
				Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = true;
			}
		}
	}

	private void OnMatchInit(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchInit proto = Serializer.Deserialize<SCMatchInit>(source);
		this.OnMatchInit(proto);
	}

	private void OnMatchInit(SCMatchInit proto)
	{
		this.roomUserSeatIndex.Clear();
		if (proto != null && proto.user != null && proto.useridx != null)
		{
			int num = Mathf.Min(proto.user.Count, proto.useridx.Count);
			for (int i = 0; i < num; i++)
			{
				UserData userData = proto.user[i];
				if (userData != null && userData.userid > 0)
				{
					this.roomUserSeatIndex[userData.userid] = proto.useridx[i];
				}
			}
		}
		if (Solarmax.Singleton<BattleSystem>.Instance != null && Solarmax.Singleton<BattleSystem>.Instance.battleData != null)
		{
			Solarmax.Singleton<BattleSystem>.Instance.battleData.matchPlayerNum = proto.nPlayerNum;
		}
		string matchid = proto.matchid;
		MatchType typ = proto.typ;
		if (typ == MatchType.MT_Ladder)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchInit, new object[]
			{
				matchid,
				proto.miscid,
				proto.user,
				proto.useridx,
				proto.nPlayerNum,
				proto.countdown,
				proto.bLock,
				string.Empty
			});
		}
		else if (typ == MatchType.MT_Room)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchInit, new object[]
			{
				matchid,
				proto.miscid,
				proto.user,
				proto.useridx,
				proto.masterid,
				proto.countdown,
				proto.bLock,
				proto.roomName
			});
		}
	}

	private void OnMatchUpdate(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchUpdate scmatchUpdate = Serializer.Deserialize<SCMatchUpdate>(source);
		if (scmatchUpdate != null)
		{
			for (int i = 0; i < scmatchUpdate.index_deled.Count; i++)
			{
				this.RemoveRoomSeatByIndex(scmatchUpdate.index_deled[i]);
			}
			int num = Mathf.Min(scmatchUpdate.user_added.Count, scmatchUpdate.index_added.Count);
			for (int j = 0; j < num; j++)
			{
				UserData userData = scmatchUpdate.user_added[j];
				if (userData != null && userData.userid > 0)
				{
					this.roomUserSeatIndex[userData.userid] = scmatchUpdate.index_added[j];
				}
			}
			int num2 = Mathf.Min(scmatchUpdate.change_from.Count, scmatchUpdate.change_to.Count);
			for (int k = 0; k < num2; k++)
			{
				this.SwapRoomSeatIndex(scmatchUpdate.change_from[k], scmatchUpdate.change_to[k]);
			}
		}
		MatchType typ = scmatchUpdate.typ;
		if (scmatchUpdate.typ == MatchType.MT_Ladder)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchUpdate, new object[]
			{
				scmatchUpdate.user_added,
				scmatchUpdate.index_added,
				scmatchUpdate.index_deled
			});
		}
		else if (scmatchUpdate.typ == MatchType.MT_Room)
		{
			if (scmatchUpdate.masterid > 0)
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchUpdate, new object[]
				{
					scmatchUpdate.user_added,
					scmatchUpdate.index_added,
					scmatchUpdate.index_deled,
					scmatchUpdate.kick,
					scmatchUpdate.change_from,
					scmatchUpdate.change_to,
					scmatchUpdate.masterid
				});
			}
			else
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchUpdate, new object[]
				{
					scmatchUpdate.user_added,
					scmatchUpdate.index_added,
					scmatchUpdate.index_deled,
					scmatchUpdate.kick,
					scmatchUpdate.change_from,
					scmatchUpdate.change_to
				});
			}
		}
	}

	public void MatchComplete()
	{
		CSMatchComplete proto = new CSMatchComplete();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMatchComplete>(182, proto, true);
	}

	private void OnMatchComplete(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchComplete scmatchComplete = Serializer.Deserialize<SCMatchComplete>(source);
		if (scmatchComplete.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("RoomWaitWindow");
			Solarmax.Singleton<UISystem>.Get().HideWindow("Room2V2Window");
			if (!Solarmax.Singleton<UISystem>.Get().IsWindowVisible("PVPWaitWindow"))
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("PVPWaitWindow");
			}
			Solarmax.Singleton<LocalPlayer>.Get().IsCanOpenAntiWindow = false;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnHideQuitMatchButton, new object[0]);
		}
		else if (scmatchComplete.code == ErrCode.EC_MatchIsFull)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
		}
		else if (scmatchComplete.code == ErrCode.EC_MatchIsMember)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
		}
		else if (scmatchComplete.code == ErrCode.EC_NotInMatch)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
		}
		else if (scmatchComplete.code == ErrCode.EC_NotMaster)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(905), 1f);
		}
		else if (scmatchComplete.code == ErrCode.EC_RoomNotExist)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(906), 1f);
		}
		else if (scmatchComplete.code == ErrCode.EC_SysUnknown)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2173), 1f);
		}
	}

	public void RequestMatchMovePos(int userId, int toIndex)
	{
		CSMatchPos csmatchPos = new CSMatchPos();
		csmatchPos.userid = userId;
		csmatchPos.index = toIndex;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMatchPos>(196, csmatchPos, true);
	}

	private void OnMatchPos(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchPos scmatchPos = Serializer.Deserialize<SCMatchPos>(source);
		if (scmatchPos.code != ErrCode.EC_Ok)
		{
			Tips.Make(LanguageDataProvider.GetValue(911));
		}
	}

	public void RequestBattleMarkTarget(string targetNodeTag)
	{
		CSBattleMarkTarget proto = new CSBattleMarkTarget
		{
			targetNodeTag = targetNodeTag
		};
		Solarmax.Singleton<NetSystem>.Instance.Send<CSBattleMarkTarget>(500, proto, true);
	}

	private void OnBattleMarkTarget(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBattleMarkTarget scbattleMarkTarget = Serializer.Deserialize<SCBattleMarkTarget>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.BattleMarkTarget, new object[]
		{
			scbattleMarkTarget.userId,
			scbattleMarkTarget.targetNodeTag
		});
	}

	public void QuitMatch(int userId = -1)
	{
		CSQuitMatch csquitMatch = new CSQuitMatch();
		if (userId > 0)
		{
			csquitMatch.userid = userId;
		}
		Solarmax.Singleton<NetSystem>.Instance.Send<CSQuitMatch>(184, csquitMatch, true);
	}

	public void QuitBattle(int userId = -1)
	{
		CSQuitBattle csquitBattle = new CSQuitBattle();
		EndEvent endEvent = new EndEvent();
		endEvent.userid = Solarmax.Singleton<LocalPlayer>.Get().playerData.userId;
		endEvent.end_type = EndType.ET_Giveup;
		endEvent.end_frame = 0;
		endEvent.end_destroy = 0;
		endEvent.end_survive = 0;
		csquitBattle.events.Add(endEvent);
		Solarmax.Singleton<NetSystem>.Instance.Send<CSQuitBattle>(90, csquitBattle, true);
	}

	private void OnQuitMatch(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCQuitMatch scquitMatch = Serializer.Deserialize<SCQuitMatch>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchQuit, new object[]
		{
			scquitMatch.code,
			scquitMatch.userid
		});
	}

	public void RequestChapters()
	{
		this.OnLoadChapters(1, default(PacketEvent));
	}

	private void OnLoadChapters(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("PacketHelper   OnLoadChapters", new object[0]);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLoadChaptersResult, new object[]
		{
			0
		});
	}

	public void RequestOneChapter(string chapterId)
	{
		PacketEvent msg = new PacketEvent(1, chapterId);
		this.OnLoadOneChapter(1, msg);
	}

	public void RequestOneChapterCache(string chapterId)
	{
		this.RequestOneChapter(chapterId);
	}

	public void OnLoadOneChapter(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLoadOneChapterResult, new object[]
		{
			msg.Data
		});
	}

	public void SetLevelStar(string chapter, string levelID, int nStar, int nScore)
	{
		CSSetLevelStar cssetLevelStar = new CSSetLevelStar();
		cssetLevelStar.level_name = levelID;
		cssetLevelStar.star = nStar;
		cssetLevelStar.score = nScore;
		object[] data = new object[]
		{
			chapter,
			levelID,
			nStar,
			nScore
		};
		PacketEvent msg = new PacketEvent(1, data);
		this.OnSetLevelStar(1, msg);
	}

	private void OnSetLevelStar(int msgId, PacketEvent msg)
	{
		object[] args = (object[])msg.Data;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSetLevelStarResult, args);
	}

	public void RequestStartLevel(string levelId)
	{
		CSStartLevel csstartLevel = new CSStartLevel();
		csstartLevel.level_name = levelId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSStartLevel>(202, csstartLevel, true);
	}

	private void OnStartLevel(int msgId, PacketEvent msg)
	{
	}

	private void OnIntAttr(int msgId, PacketEvent msg)
	{
	}

	private void OnMoneyChange(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCChangeMoney scchangeMoney = Serializer.Deserialize<SCChangeMoney>(source);
		Solarmax.Singleton<LocalPlayer>.Get().SetMoney(scchangeMoney.curMoney);
	}

	public void GenPresignedUrl(string objectName, string method, string contentType, string file, int eventId)
	{
		CSGenerateUrl csgenerateUrl = new CSGenerateUrl();
		csgenerateUrl.objectname = objectName;
		csgenerateUrl.method = method;
		csgenerateUrl.contenttype = contentType;
		csgenerateUrl.file = file;
		csgenerateUrl.eventId = eventId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGenerateUrl>(213, csgenerateUrl, true);
	}

	private void OnGenPresignedUrl(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGenerateUrl scgenerateUrl = Serializer.Deserialize<SCGenerateUrl>(source);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent((EventId)scgenerateUrl.eventId, new object[]
		{
			scgenerateUrl
		});
	}

	public void RequestSetLevelSorce(string levelId, string groupId, string account, int score)
	{
		PacketEvent msg = new PacketEvent(1, new object[]
		{
			levelId,
			groupId,
			account,
			score
		});
		this.OnRequestSetLevelSorce(1, msg);
	}

	public void OnRequestSetLevelSorce(int msgId, PacketEvent msg)
	{
		object[] array = msg.Data as object[];
		ChapterLevelInfo level = Solarmax.Singleton<LevelDataHandler>.Get().GetLevel(array[0] as string);
		if (level != null)
		{
			ChapterLevelGroup chapterLevelGroup = Solarmax.Singleton<LevelDataHandler>.Get().FindGroupLevel(level.groupId);
			if (chapterLevelGroup != null)
			{
				int score = chapterLevelGroup.Score;
				level.Score = (int)array[3];
			}
		}
	}

	public void OnRequestBuyChapter(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBuyChapter scbuyChapter = Serializer.Deserialize<SCBuyChapter>(source);
		if (scbuyChapter.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<LevelDataHandler>.Get().UnLockPayChapter(scbuyChapter.chapter);
			Solarmax.Singleton<LevelDataHandler>.Get().AddBuyChapterInfo(scbuyChapter.chapter);
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnHaveNewChapterUnlocked, new object[]
			{
				scbuyChapter.chapter
			});
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(scbuyChapter.chapter);
			MonoSingleton<FlurryAnalytis>.Instance.FlurryMoneyCostEvent("1", scbuyChapter.chapter, data.costGold.ToString());
			MiGameAnalytics.MiAnalyticsMoneyCostEvent("1", scbuyChapter.chapter, data.costGold.ToString());
			AppsFlyerTool.FlyerMoneyCostEvent("1", scbuyChapter.chapter, data.costGold.ToString());
		}
		else if (scbuyChapter.code == ErrCode.EC_ChapterNotExist)
		{
			Tips.Make(LanguageDataProvider.GetValue(1150));
		}
		else if (scbuyChapter.code == ErrCode.EC_ChapterBought)
		{
			Tips.Make(LanguageDataProvider.GetValue(1149));
		}
	}

	public void RequestPveRankReport(string levelId)
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(levelId);
		if (data == null)
		{
			return;
		}
		CSPveRankReportLoad cspveRankReportLoad = new CSPveRankReportLoad();
		cspveRankReportLoad.levelId = data.levelGroup;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSPveRankReportLoad>(217, cspveRankReportLoad, true);
	}

	private void OnRequestPveRankReport(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCPveRankReportLoad scpveRankReportLoad = Serializer.Deserialize<SCPveRankReportLoad>(source);
		string levelId = scpveRankReportLoad.levelId;
		List<FriendSimplePlayer> list = new List<FriendSimplePlayer>();
		for (int i = 0; i < scpveRankReportLoad.report.Count; i++)
		{
			FriendSimplePlayer friendSimplePlayer = new FriendSimplePlayer();
			friendSimplePlayer.Init(scpveRankReportLoad.report[i]);
			list.Add(friendSimplePlayer);
		}
		List<FriendSimplePlayer> list2 = new List<FriendSimplePlayer>();
		for (int j = 0; j < scpveRankReportLoad.report_friend.Count; j++)
		{
			FriendSimplePlayer friendSimplePlayer2 = new FriendSimplePlayer();
			friendSimplePlayer2.Init(scpveRankReportLoad.report_friend[j]);
			list2.Add(friendSimplePlayer2);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnPveRankReportLoad, new object[]
		{
			true,
			list,
			list2
		});
	}

	public void RequestTaskOk(List<string> levelIds, int multiply = 1)
	{
		if (!this.CheckNetReachability())
		{
			return;
		}
		CSTaskOk cstaskOk = new CSTaskOk();
		foreach (string text in levelIds)
		{
			if (!Solarmax.Singleton<TaskConfigProvider>.Get().dataList.ContainsKey(text) || Solarmax.Singleton<TaskConfigProvider>.Get().dataList[text].status != TaskStatus.Received)
			{
				cstaskOk.taskIds.Add(text);
			}
		}
		cstaskOk.multipy = multiply;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSTaskOk>(251, cstaskOk, true);
	}

	public void OnRequestTaskOk(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCTaskOk sctaskOk = Serializer.Deserialize<SCTaskOk>(source);
		List<string> list = new List<string>();
		for (int i = 0; i < sctaskOk.okTaskIds.Count; i++)
		{
			TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Instance.GetTask(sctaskOk.okTaskIds[i]);
			if (task != null)
			{
				task.status = TaskStatus.Received;
				if (task.reward2Type == Solarmax.RewardType.Degree)
				{
					Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Degree, task.reward2Value);
				}
				if (task.rewardType == Solarmax.RewardType.Degree)
				{
					Solarmax.Singleton<TaskModel>.Get().FinishTaskEvent(FinishConntion.Degree, task.rewardValue);
				}
				list.Add(sctaskOk.okTaskIds[i]);
			}
		}
		List<string> list2 = new List<string>();
		for (int j = 0; j < sctaskOk.failTaskIds.Count; j++)
		{
			list2.Add(sctaskOk.failTaskIds[j]);
		}
		if (Solarmax.Singleton<TaskModel>.Get().onRequestTaskOk != null)
		{
			Solarmax.Singleton<TaskModel>.Get().onRequestTaskOk(list, list2);
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnTaskOkEvent, new object[]
		{
			list,
			list2
		});
	}

	public void PullTaskOk()
	{
		new CSTaskGet();
		this.OnPullTaskOk(1, default(PacketEvent));
	}

	public void OnPullTaskOk(int msgId, PacketEvent msg)
	{
		List<string> list = new List<string>();
		for (int i = 0; i <= 15; i++)
		{
			list.Add(string.Format("{0}", 100 + i));
		}
		new DateTime(1970, 1, 1);
		for (int j = 0; j < list.Count; j++)
		{
			TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Instance.GetTask(list[j]);
			if (task != null)
			{
				if (task != null && task.taskType == Solarmax.TaskType.Level)
				{
					task.status = TaskStatus.Received;
				}
				if (task != null && task.taskType == Solarmax.TaskType.Daily)
				{
					task.status = TaskStatus.Received;
				}
			}
		}
	}

	public bool CheckNetReachability()
	{
		return true;
	}

	public void StartMatchInvite(int userID)
	{
		CSMatchInvite csmatchInvite = new CSMatchInvite();
		csmatchInvite.dstUserId = userID;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMatchInvite>(261, csmatchInvite, true);
	}

	public void OnStartMatchInvite(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchInvite scmatchInvite = Serializer.Deserialize<SCMatchInvite>(source);
		if (scmatchInvite.code == ErrCode.EC_MatchIsFull)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
		}
		else if (scmatchInvite.code == ErrCode.EC_MatchIsMember)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
		}
		else if (scmatchInvite.code == ErrCode.EC_NotInMatch)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
		}
		else if (scmatchInvite.code == ErrCode.EC_MatchInviteRefuse)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1144), 1f);
		}
		else if (scmatchInvite.code == ErrCode.EC_NotBuyThisChapter)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2141), 1f);
		}
	}

	public void StartCSMatchInviteResp(bool accept, int inviteID)
	{
		CSMatchInviteResp csmatchInviteResp = new CSMatchInviteResp();
		csmatchInviteResp.accept = accept;
		csmatchInviteResp.srcUserId = inviteID;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMatchInviteResp>(265, csmatchInviteResp, true);
	}

	public void OnSCMatchInviteReq(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchInviteReq scmatchInviteReq = Serializer.Deserialize<SCMatchInviteReq>(source);
		if (Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.StartCSMatchInviteResp(false, scmatchInviteReq.srcUserId);
			return;
		}
		if (!Solarmax.Singleton<BattleSystem>.Instance.bStartBattle)
		{
			int srcUserId = scmatchInviteReq.srcUserId;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMatchBeInviteRepose, new object[]
			{
				srcUserId,
				scmatchInviteReq.srcIcon,
				scmatchInviteReq.srcName,
				scmatchInviteReq.srcScore
			});
		}
		else
		{
			this.StartCSMatchInviteResp(false, scmatchInviteReq.srcUserId);
		}
	}

	public void OnSCMatchInviteResp(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMatchInviteResp scmatchInviteResp = Serializer.Deserialize<SCMatchInviteResp>(source);
		if (scmatchInviteResp.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<LocalPlayer>.Get().HomeWindow = "HomeWindow";
		}
		else if (scmatchInviteResp.code == ErrCode.EC_MatchIsFull)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
		}
		else if (scmatchInviteResp.code == ErrCode.EC_MatchIsMember)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
		}
		else if (scmatchInviteResp.code == ErrCode.EC_NotInMatch)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1142), 1f);
		}
		else if (scmatchInviteResp.code == ErrCode.EC_MatchInviteRefuse)
		{
			string message = string.Format("{0} {1}", scmatchInviteResp.dstName, LanguageDataProvider.GetValue(1144));
			Tips.Make(Tips.TipsType.FlowUp, message, 1f);
		}
	}

	public void OnBuySkinResp(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCBuySkin scbuySkin = Serializer.Deserialize<SCBuySkin>(source);
		if (scbuySkin.ret == ErrCode.EC_Ok)
		{
			SkinConfig data = Solarmax.Singleton<SkinConfigProvider>.Instance.GetData(scbuySkin.id);
			if (data != null)
			{
				if (data.type == SkinType.BG)
				{
					MonoSingleton<FlurryAnalytis>.Instance.FlurryMoneyCostEvent("3", data.id.ToString(), data.goodValue.ToString());
					MiGameAnalytics.MiAnalyticsMoneyCostEvent("3", data.id.ToString(), data.goodValue.ToString());
				}
				if (data.type == SkinType.Avatar)
				{
					MonoSingleton<FlurryAnalytis>.Instance.FlurryMoneyCostEvent("2", data.id.ToString(), data.goodValue.ToString());
					MiGameAnalytics.MiAnalyticsMoneyCostEvent("2", data.id.ToString(), data.goodValue.ToString());
				}
				Solarmax.Singleton<CollectionModel>.Get().UnLock(scbuySkin.id);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnBuySkinRespose, new object[]
			{
				scbuySkin.id
			});
		}
		else if (scbuySkin.ret == ErrCode.EC_SkinNotExist)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(902), 1f);
		}
		else if (scbuySkin.ret == ErrCode.EC_SkinBought)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(903), 1f);
		}
	}

	public void RequestUserInit()
	{
		this.OnRequestSCUserInit(0, default(PacketEvent));
	}

	public void OnRequestSCUserInit(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<LocalPlayer>.Get().SeasonType = 0;
		Solarmax.Singleton<LocalPlayer>.Get().SeasonStartTime = 0;
		Solarmax.Singleton<LocalPlayer>.Get().SeasonEndTime = 99999;
		Solarmax.Singleton<LocalPlayer>.Get().nextSasonStart = 99999.0;
		Solarmax.Singleton<LocalPlayer>.Get().nMaxAccumulMoney = 999999;
		Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney = 99999;
		Solarmax.Singleton<LocalPlayer>.Get().mActivityDegree = 99999;
		Solarmax.Singleton<LocalPlayer>.Get().mOnLineTime = 999999f;
		Solarmax.Singleton<LocalPvpSeasonSystem>.Get().pvpType = 0;
		Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonStart = 0;
		Solarmax.Singleton<LocalPvpSeasonSystem>.Get().seasonEnd = 999999;
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSeason();
		Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList.Clear();
		Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList.Add("0");
		Solarmax.Singleton<LevelDataHandler>.Instance.InitEvaluationChapter(new List<string>());
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUpdateLeagueMode, new object[0]);
		Solarmax.Singleton<SeasonRewardModel>.Get().Init(1);
		Solarmax.Singleton<TaskModel>.Get().Init();
	}

	public void StartVerityOrder(string orderID)
	{
		if (string.IsNullOrEmpty(orderID))
		{
			return;
		}
		CSVerifyOrderID csverifyOrderID = new CSVerifyOrderID();
		csverifyOrderID.orderID = orderID;
		Debug.LogFormat("CSVerifyOrderID - orderId: {0}", new object[]
		{
			orderID
		});
		Solarmax.Singleton<NetSystem>.Instance.Send<CSVerifyOrderID>(269, csverifyOrderID, true);
	}

	public void OnVerityOrder(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCVerifyOrderID scverifyOrderID = Serializer.Deserialize<SCVerifyOrderID>(source);
		Debug.LogFormat("SCVerifyOrderID - orderId: {0}, result: {1}", new object[]
		{
			scverifyOrderID.orderID,
			scverifyOrderID.ret
		});
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnVerityOrder, new object[]
		{
			scverifyOrderID.ret,
			scverifyOrderID.orderID
		});
	}

	public void StartCSAdReward()
	{
		Debug.Log("AdsSDK, StartCSAdReward: 2");
		CSAdReward proto = new CSAdReward();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSAdReward>(291, proto, false);
	}

	public void StartCSAdRewardCache()
	{
		Debug.Log("AdsSDK, StartCSAdRewardCache: 2");
		CSAdReward proto = new CSAdReward();
		Solarmax.Singleton<NetSystem>.Instance.Send2Cache<CSAdReward>(291, proto);
	}

	public void OnSCAdeward(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCAdReward scadReward = Serializer.Deserialize<SCAdReward>(source);
		Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum++;
		string rewardbyNum = Solarmax.Singleton<CollectionModel>.Get().GetRewardbyNum(Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum);
		MonoSingleton<FlurryAnalytis>.Instance.FlurryRewardCoinADSEvent(rewardbyNum);
		AppsFlyerTool.FlyerRewardCoinADSEvent();
		MiGameAnalytics.MiAnalyticsRewardCoinADSEvent(rewardbyNum);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnAdsShowEvent, new object[0]);
	}

	public void OnGenerateOrderID(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGenerateOrderID scgenerateOrderID = Serializer.Deserialize<SCGenerateOrderID>(source);
		Debug.LogFormat("SCGenerateOrderID - productID: {0}, num: {1}, orderId: {2}", new object[]
		{
			scgenerateOrderID.productID,
			scgenerateOrderID.num,
			scgenerateOrderID.OrderID
		});
		if (scgenerateOrderID != null && !string.IsNullOrEmpty(scgenerateOrderID.OrderID))
		{
			StoreConfig data = Solarmax.Singleton<storeConfigProvider>.Instance.GetData(scgenerateOrderID.productID);
			if (data != null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.FlurryCreateOrderIDEvent(data.name, scgenerateOrderID.productID, scgenerateOrderID.num, data.GetCurrencyDesc(), (double)data.GetPrice(), scgenerateOrderID.OrderID);
				AppsFlyerTool.FlyerCreateOrderIDEvent(data.name, scgenerateOrderID.productID, scgenerateOrderID.num, data.GetCurrencyDesc(), (double)data.GetPrice(), scgenerateOrderID.OrderID);
				MiGameAnalytics.MiAnalyticsCreateOrderIDEvent(data.name, scgenerateOrderID.productID, scgenerateOrderID.num, data.GetCurrencyDesc(), (double)data.GetPrice(), scgenerateOrderID.OrderID);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGenerateOrderID, new object[]
			{
				scgenerateOrderID.OrderID,
				scgenerateOrderID.productID,
				scgenerateOrderID.num
			});
		}
	}

	public void SendAchievementSet(string levelgroup, List<string> achieveId)
	{
		CSAchievementSet csachievementSet = new CSAchievementSet();
		csachievementSet.lvGroupId = levelgroup;
		foreach (string item in achieveId)
		{
			csachievementSet.achieveIds.Add(item);
		}
		object[] data = new object[]
		{
			levelgroup,
			achieveId
		};
		PacketEvent msg = new PacketEvent(1, data);
		this.OnSendAchievementSet(1, msg);
	}

	public void OnSendAchievementSet(int msgId, PacketEvent msg)
	{
		object[] array = msg.Data as object[];
		string groupID = array[0] as string;
		foreach (string achieveID in (array[1] as List<string>))
		{
			Solarmax.Singleton<AchievementModel>.Get().SetAchievement(groupID, achieveID, true, false);
		}
	}

	public void PullAchievements(List<string> levelgroups)
	{
		CSAchievementLoad csachievementLoad = new CSAchievementLoad();
		foreach (string item in levelgroups)
		{
			csachievementLoad.lvGroupIds.Add(item);
		}
		new PacketEvent(1, csachievementLoad);
	}

	public void OnPullAchievements(int msgId, PacketEvent msg)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("PacketHelper  OnPullAchievements", new object[0]);
		bool flag = false;
		string key = string.Empty;
		Dictionary<string, bool> levelDic = Solarmax.Singleton<FunctionOpenConfigProvider>.Get().GetLevelDic();
		SCAchievementLoad scachievementLoad = (SCAchievementLoad)msg.Data;
		List<string> list = new List<string>();
		foreach (PbAchievement pbAchievement in scachievementLoad.achieves)
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (string text in pbAchievement.achieveIds)
			{
				dictionary[text] = true;
				Solarmax.Singleton<AchievementModel>.Get().SetAchievement(pbAchievement.lvGroupId, text, true, false);
				if (Solarmax.Singleton<AchievementModel>.Get().achievementGroups.ContainsKey(pbAchievement.lvGroupId) && Solarmax.Singleton<AchievementModel>.Get().achievementGroups[pbAchievement.lvGroupId].idAchievements.ContainsKey(text) && Solarmax.Singleton<AchievementModel>.Get().achievementGroups[pbAchievement.lvGroupId].idAchievements[text].types[0] == AchievementType.PassDiffcult)
				{
					Achievement achievement = Solarmax.Singleton<AchievementModel>.Get().achievementGroups[pbAchievement.lvGroupId].idAchievements[text];
					TaskConfig task = Solarmax.Singleton<TaskConfigProvider>.Get().GetTask(achievement.taskId);
					if (task != null && task.status != TaskStatus.Received)
					{
						list.Add(task.id);
					}
				}
				if (levelDic != null && pbAchievement.lvGroupId.EndsWith("0"))
				{
					key = string.Format("{0}0", pbAchievement.lvGroupId);
					if (levelDic.ContainsKey(key) && !flag)
					{
						flag = true;
					}
				}
			}
			AchievementModel.achieveDic[pbAchievement.lvGroupId] = dictionary;
		}
		AchievementModel.responseCount++;
		if (AchievementModel.responseCount == AchievementModel.requestCount)
		{
			Solarmax.Singleton<LocalAchievementStorage>.Get().CheckSyncData(AchievementModel.achieveDic);
		}
		Solarmax.Singleton<LevelDataHandler>.Get().ResetChapterStars();
		Solarmax.Singleton<LevelDataHandler>.Get().UnlockChapters(false);
		Solarmax.Singleton<LevelDataHandler>.Get().UnlockChaptersbyStar();
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAchievement();
		if (flag)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnPullAchievementData, null);
		}
		if (list.Count > 0)
		{
			Solarmax.Singleton<TaskModel>.Get().ClaimAllReward(list, null, 1);
		}
	}

	public void CSReceiveMonthlyCard()
	{
		CSReceiveMonthlyCard proto = new CSReceiveMonthlyCard();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSReceiveMonthlyCard>(271, proto, true);
	}

	public void OnSCReceiveMonthlyCard(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCReceiveMonthlyCard screceiveMonthlyCard = Serializer.Deserialize<SCReceiveMonthlyCard>(source);
		if (screceiveMonthlyCard.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive = false;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMothCard, new object[0]);
	}

	public void OnSCChangeMonthlyCard(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCChangeMonthlyCard scchangeMonthlyCard = Serializer.Deserialize<SCChangeMonthlyCard>(source);
		if (scchangeMonthlyCard != null)
		{
			Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive = scchangeMonthlyCard.last_receive_time;
			Solarmax.Singleton<LocalPlayer>.Get().month_card_end = scchangeMonthlyCard.monthly_card_end_time;
			MonthCheckModel monthCheckModel = Solarmax.Singleton<MonthCheckModel>.Get();
			if (monthCheckModel != null)
			{
				monthCheckModel.SetCheckedTime();
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMothCard, new object[0]);
			if (string.IsNullOrEmpty(scchangeMonthlyCard.id))
			{
				return;
			}
			if (!Solarmax.Singleton<LocalPlayer>.Get().IsBuyed(scchangeMonthlyCard.id))
			{
				Solarmax.Singleton<LocalPlayer>.Get().AddBuy(scchangeMonthlyCard.id);
			}
			StoreConfig data = Solarmax.Singleton<storeConfigProvider>.Instance.GetData(scchangeMonthlyCard.id);
			if (data != null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.FlurryPaymentEvent(data.name, data.id, 1, data.GetCurrencyDesc(), (double)data.GetPrice(), scchangeMonthlyCard.orderID_id);
				MiGameAnalytics.MiAnalyticsPaymentEvent(data.name, data.id, 1, data.GetCurrencyDesc(), (double)data.GetPrice(), scchangeMonthlyCard.orderID_id);
				MonoSingleton<FlurryAnalytis>.Instance.LogOrderComplete(data.name, data.id, 1, data.GetCurrencyDesc(), (double)data.GetPrice(), scchangeMonthlyCard.orderID_id);
				MiGameAnalytics.MiAnalyticsLogOrderComplete(data.name, data.id, 1, data.GetCurrencyDesc(), (double)data.GetPrice(), scchangeMonthlyCard.orderID_id);
				AppsFlyerTool.FlyerPaymentEvent(data.name, data.id, data.GetCurrencyDesc(), (double)data.GetPrice(), scchangeMonthlyCard.orderID_id);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateMoney, new object[0]);
		}
	}

	public void UploadOldData()
	{
		if (OldLocalStorageSystem.oldData == null || OldLocalStorageSystem.oldData.chapters.Count == 0)
		{
			CSUploadOldVersionData csuploadOldVersionData = new CSUploadOldVersionData();
			csuploadOldVersionData.sum_package = 0;
			csuploadOldVersionData.current_package = 0;
			Solarmax.Singleton<NetSystem>.Instance.Send<CSUploadOldVersionData>(310, csuploadOldVersionData, true);
			return;
		}
		for (int i = 0; i < OldLocalStorageSystem.oldData.chapters.Count; i++)
		{
			CSUploadOldVersionData dataTemp = new CSUploadOldVersionData();
			dataTemp.chapters.Add(OldLocalStorageSystem.oldData.chapters[i]);
			dataTemp.userId = OldLocalStorageSystem.oldData.userId;
			dataTemp.sum_package = OldLocalStorageSystem.oldData.chapters.Count;
			dataTemp.current_package = i + 1;
			global::Coroutine.DelayDo(0.02f * (float)i, new EventDelegate(delegate()
			{
				Solarmax.Singleton<NetSystem>.Instance.Send<CSUploadOldVersionData>(310, dataTemp, true);
			}));
		}
	}

	public void OnUploadOldData(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCUploadOldVersionData scuploadOldVersionData = Serializer.Deserialize<SCUploadOldVersionData>(source);
		if (scuploadOldVersionData != null)
		{
			if (scuploadOldVersionData.errcode == ErrCode.EC_Ok)
			{
				UserData data = scuploadOldVersionData.data;
				Solarmax.Singleton<LocalPlayer>.Get().playerData.Init(data);
				Solarmax.Singleton<LocalPlayer>.Get().isAccountTokenOver = false;
				this.OnUserDataInit();
			}
			if (scuploadOldVersionData.data != null && scuploadOldVersionData.data.chapterBuy != null && scuploadOldVersionData.data.chapterBuy.Count > 0)
			{
				for (int i = 0; i < scuploadOldVersionData.data.chapterBuy.Count; i++)
				{
					Solarmax.Singleton<LevelDataHandler>.Get().UnLockPayChapter(scuploadOldVersionData.data.chapterBuy[i]);
				}
			}
			if (scuploadOldVersionData.data != null && scuploadOldVersionData.data.skinBuy != null && scuploadOldVersionData.data.skinBuy.Count > 0)
			{
				for (int j = 0; j < scuploadOldVersionData.data.skinBuy.Count; j++)
				{
					Solarmax.Singleton<CollectionModel>.Get().UnLock(scuploadOldVersionData.data.skinBuy[j]);
				}
			}
			if (scuploadOldVersionData.data != null && scuploadOldVersionData.data.RechargeData != null)
			{
				if (scuploadOldVersionData.data.RechargeData.first_recharge_mark != null && scuploadOldVersionData.data.RechargeData.first_recharge_mark.Count > 0)
				{
					for (int k = 0; k < scuploadOldVersionData.data.RechargeData.first_recharge_mark.Count; k++)
					{
						Solarmax.Singleton<LocalPlayer>.Get().AddBuy(scuploadOldVersionData.data.RechargeData.first_recharge_mark[k]);
					}
				}
				Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive = scuploadOldVersionData.data.RechargeData.last_receive_time;
				Solarmax.Singleton<LocalPlayer>.Get().month_card_end = scuploadOldVersionData.data.RechargeData.monthly_card_end_time;
			}
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUploadOldVersionData, new object[0]);
	}

	public void ClaimSeasonReward(int type)
	{
		CSGetLadderReward csgetLadderReward = new CSGetLadderReward();
		csgetLadderReward.index = type;
		Solarmax.Singleton<SeasonRewardModel>.Get().claimRewardType = type;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGetLadderReward>(330, csgetLadderReward, true);
	}

	public void OnClaimSeasonReward(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGetLadderReward scgetLadderReward = Serializer.Deserialize<SCGetLadderReward>(source);
		if (scgetLadderReward != null && scgetLadderReward.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<EventSystem>.Get().FireEvent(EventId.OnSeasonReward, new object[]
			{
				scgetLadderReward.index
			});
		}
	}

	public void ChangeSeasonScore(int change)
	{
		CSChangeLadderScore cschangeLadderScore = new CSChangeLadderScore();
		cschangeLadderScore.change_score = change;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSChangeLadderScore>(341, cschangeLadderScore, true);
	}

	public void ChangePvpReward(int nMultiy)
	{
		CSRVPReward csrvpreward = new CSRVPReward();
		csrvpreward.multipy = nMultiy;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSRVPReward>(343, csrvpreward, true);
	}

	public void OnChangeSeasonScore(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCChangeLadderScore scchangeLadderScore = Serializer.Deserialize<SCChangeLadderScore>(source);
		if (scchangeLadderScore != null && scchangeLadderScore.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<LocalPlayer>.Get().playerData.score = scchangeLadderScore.ladder_score;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLadderScoreChange, null);
		}
	}

	public void RequstMonthCheck()
	{
		CSPushMonthCheck proto = new CSPushMonthCheck();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSPushMonthCheck>(403, proto, true);
	}

	public void OnPushMonthCheck(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCPushMonthCheck scpushMonthCheck = Serializer.Deserialize<SCPushMonthCheck>(source);
		if (scpushMonthCheck != null && scpushMonthCheck.code == ErrCode.EC_Ok)
		{
			MonthCheckModel monthCheckModel = Solarmax.Singleton<MonthCheckModel>.Get();
			monthCheckModel.checkedId = scpushMonthCheck.checked_id;
			monthCheckModel.nextCheckId = scpushMonthCheck.checked_id + 1;
			monthCheckModel.currentMonth = scpushMonthCheck.current_month;
			monthCheckModel.SetCheckedTime(scpushMonthCheck.check_time);
			monthCheckModel.repair_check_num = scpushMonthCheck.repair_check_num;
			if (monthCheckModel.needCheck)
			{
				monthCheckModel.couldRepairId = scpushMonthCheck.checked_id + monthCheckModel.repair_check_num + 1;
			}
			else
			{
				monthCheckModel.couldRepairId = scpushMonthCheck.checked_id + monthCheckModel.repair_check_num;
			}
			monthCheckModel.todayCheckId = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime().Day;
			monthCheckModel.couldRepairId = Math.Min(monthCheckModel.todayCheckId, monthCheckModel.couldRepairId);
		}
	}

	public void MonthCheck(int month, int checkId, int multiply, bool repair = false)
	{
		CSMonthCheck csmonthCheck = new CSMonthCheck();
		csmonthCheck.current_month = month;
		csmonthCheck.check_id = checkId;
		csmonthCheck.multipy = multiply;
		csmonthCheck.is_repair = repair;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMonthCheck>(400, csmonthCheck, true);
	}

	public void OnMonthCheck(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMonthCheck scmonthCheck = Serializer.Deserialize<SCMonthCheck>(source);
		if (scmonthCheck != null && scmonthCheck.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<MonthCheckModel>.Get().needCheck = false;
			MonthCheckModel monthCheckModel = Solarmax.Singleton<MonthCheckModel>.Get();
			monthCheckModel.needCheck = false;
			monthCheckModel.checkedId = scmonthCheck.check_id;
			monthCheckModel.SetCheckedTimeEX(scmonthCheck.check_time);
			monthCheckModel.repair_check_num = scmonthCheck.repair_check_num;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMonthCheckSuccess, new object[]
			{
				scmonthCheck.current_month,
				scmonthCheck.check_id,
				scmonthCheck.multipy
			});
		}
	}

	public void OnAccumulMoneyUpdate(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCRVPReward scrvpreward = Serializer.Deserialize<SCRVPReward>(source);
		if (scrvpreward != null)
		{
			Solarmax.Singleton<LocalPlayer>.Get().nCurAccumulMoney = scrvpreward.pvp_reward;
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateAccumulMoney, new object[0]);
		}
	}

	public void StartRequestActiveChapter(List<ChapterInfo> ls)
	{
		CSChapterScore cschapterScore = new CSChapterScore();
		if (ls != null)
		{
			for (int i = 0; i < ls.Count; i++)
			{
				cschapterScore.chapter_id.Add(ls[i].id);
			}
		}
		Solarmax.Singleton<NetSystem>.Instance.Send<CSChapterScore>(350, cschapterScore, true);
	}

	public void OnReposeActiveChapter(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCChapterScore scchapterScore = Serializer.Deserialize<SCChapterScore>(source);
		if (scchapterScore != null && scchapterScore.chapter != null)
		{
			for (int i = 0; i < scchapterScore.chapter.Count; i++)
			{
				chapterScore score = scchapterScore.chapter[i];
				Solarmax.Singleton<LevelDataHandler>.Instance.RefrushChapterInfo(score);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnShowActiveChapters, new object[0]);
		}
	}

	public void StartRequestActiveChapterPrice()
	{
		CSDiscountChapter proto = new CSDiscountChapter();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSDiscountChapter>(352, proto, true);
	}

	public void OnResponseActiveChapterPrice(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCDiscountChapter scdiscountChapter = Serializer.Deserialize<SCDiscountChapter>(source);
		if (scdiscountChapter != null && scdiscountChapter.chapter != null)
		{
			for (int i = 0; i < scdiscountChapter.chapter.Count; i++)
			{
				discountChapter score = scdiscountChapter.chapter[i];
				Solarmax.Singleton<LevelDataHandler>.Instance.RefrushChapterInfoPrice(score);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnShowActiveChapters, new object[0]);
		}
	}

	public void StartSetChapterScore(string chapterid, int strategy, int interest, int total, int multiply)
	{
		CSSetChapterScore cssetChapterScore = new CSSetChapterScore();
		cssetChapterScore.chapter_id = chapterid;
		cssetChapterScore.interest_score = interest;
		cssetChapterScore.strategy_score = strategy;
		cssetChapterScore.total_score = total;
		cssetChapterScore.multipy = multiply;
		Solarmax.Singleton<LevelDataHandler>.Instance.AddEvaluation(chapterid);
		Solarmax.Singleton<NetSystem>.Instance.Send<CSSetChapterScore>(354, cssetChapterScore, true);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefrushPingFenBtn, new object[0]);
	}

	public void OnSetChapterScoreResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCSetChapterScore scsetChapterScore = Serializer.Deserialize<SCSetChapterScore>(source);
		if (scsetChapterScore.errcode == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnPingFenSunccess, new object[0]);
		}
	}

	public void LotteryFromServer(int type, int lotteryId)
	{
		Debug.Log("LotteryFromServer type " + type);
		CSLotteryInfo cslotteryInfo = new CSLotteryInfo();
		cslotteryInfo.optype = type;
		cslotteryInfo.currLotteryId = lotteryId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLotteryInfo>(550, cslotteryInfo, true);
	}

	private void OnLotteryInfoResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCLotteryInfo sclotteryInfo = Serializer.Deserialize<SCLotteryInfo>(source);
		Debug.Log("OnLotteryInfoResponse error code " + sclotteryInfo.errcode);
		Solarmax.Singleton<LuckModel>.Get().lotteryInfo = sclotteryInfo;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLotteryResultDone, new object[0]);
	}

	public void LotteryNotesFromServer()
	{
		CSLotteryNotes proto = new CSLotteryNotes();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLotteryNotes>(552, proto, true);
	}

	private void OnLotteryNotesResponse(int msgId, PacketEvent msg)
	{
		Debug.Log("OnLotteryNotesResponse");
		MemoryStream source = msg.Data as MemoryStream;
		SCLotterNotes lotteryNotes = Serializer.Deserialize<SCLotterNotes>(source);
		Solarmax.Singleton<LuckModel>.Get().lotteryNotes = lotteryNotes;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLotteryNotesDone, new object[0]);
	}

	public void LotteryAward(int awardId)
	{
		CSLotteryAward cslotteryAward = new CSLotteryAward();
		cslotteryAward.boxId = awardId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSLotteryAward>(554, cslotteryAward, true);
	}

	private void OnLotteryAwardResponse(int msgId, PacketEvent msg)
	{
		Debug.Log("OnLotteryAwardResponse");
		MemoryStream source = msg.Data as MemoryStream;
		SCLotteryAward lotteryAward = Serializer.Deserialize<SCLotteryAward>(source);
		Solarmax.Singleton<LuckModel>.Get().lotteryAward = lotteryAward;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnLotteryExtraAwardAccepted, new object[0]);
	}

	public void RequestUseItem(int typeID, int count)
	{
		CSUseItem csuseItem = new CSUseItem();
		csuseItem.item_id = typeID;
		csuseItem.count = (long)count;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSUseItem>(360, csuseItem, true);
	}

	public void OnUseItemResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCUseItem scuseItem = Serializer.Deserialize<SCUseItem>(source);
		if (scuseItem.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("CompositeWindow");
			Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
			{
				RewardTipsWindow.ViewType.RewardItem,
				new RewardTipsModel((int)scuseItem.count, global::RewardType.Prop, false, scuseItem.item_id)
			}));
		}
	}

	public void RequestDecomposeItem(int sn, int count)
	{
		CSDecomposeItem csdecomposeItem = new CSDecomposeItem();
		csdecomposeItem.id = sn;
		csdecomposeItem.count = (long)count;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSDecomposeItem>(362, csdecomposeItem, true);
	}

	public void OnDecomposeItemResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCDecomposeItem scdecomposeItem = Serializer.Deserialize<SCDecomposeItem>(source);
		if (scdecomposeItem.code == ErrCode.EC_Ok)
		{
			Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2307), 3f);
			Solarmax.Singleton<UISystem>.Get().HideWindow("DecompositionWindow");
		}
	}

	public void GetMailList(int start)
	{
		CSMailList proto = new CSMailList();
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMailList>(186, proto, true);
	}

	private void OnMailListResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMailList scmailList = Serializer.Deserialize<SCMailList>(source);
		if (scmailList.code == ErrCode.EC_Ok)
		{
			Solarmax.Singleton<MailModel>.Get().mailList = scmailList;
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMailListResponse, new object[0]);
	}

	public void ReadMail(int mailId)
	{
		CSMailRead csmailRead = new CSMailRead();
		csmailRead.mailid = mailId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMailRead>(188, csmailRead, true);
	}

	private void OnReadMailResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMailRead scmailRead = Serializer.Deserialize<SCMailRead>(source);
		if (scmailRead.code == ErrCode.EC_Ok)
		{
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMailReadResponse, new object[0]);
	}

	public void DeleteMail(int mailId)
	{
		CSMailDel csmailDel = new CSMailDel();
		csmailDel.mailid.Add(mailId);
		Solarmax.Singleton<NetSystem>.Instance.Send<CSMailDel>(190, csmailDel, true);
	}

	private void OnDeleteMailResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCMailDel scmailDel = Serializer.Deserialize<SCMailDel>(source);
		if (scmailDel.code == ErrCode.EC_Ok)
		{
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMailDelResponse, new object[0]);
	}

	public void GetMailItems(int mailId)
	{
		CSGetMailItem csgetMailItem = new CSGetMailItem();
		csgetMailItem.mailid = mailId;
		Solarmax.Singleton<NetSystem>.Instance.Send<CSGetMailItem>(603, csgetMailItem, true);
	}

	private void OnGetMailItemsResponse(int msgId, PacketEvent msg)
	{
		MemoryStream source = msg.Data as MemoryStream;
		SCGetMailItem scgetMailItem = Serializer.Deserialize<SCGetMailItem>(source);
		if (scgetMailItem.code == ErrCode.EC_Ok)
		{
		}
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnMailGetItemResponse, new object[0]);
	}

	public string ServerAddress;
}
