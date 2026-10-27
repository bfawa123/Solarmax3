using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MsgId")]
	public enum MsgId
	{
		[ProtoEnum(Name = "ID_Null", Value = 0)]
		ID_Null,
		[ProtoEnum(Name = "ID_CSMatch", Value = 1)]
		ID_CSMatch,
		[ProtoEnum(Name = "ID_SCMatch", Value = 2)]
		ID_SCMatch,
		[ProtoEnum(Name = "ID_SCStartBattle", Value = 6)]
		ID_SCStartBattle = 6,
		[ProtoEnum(Name = "ID_CSFrame", Value = 7)]
		ID_CSFrame,
		[ProtoEnum(Name = "ID_SCFrame", Value = 8)]
		ID_SCFrame,
		[ProtoEnum(Name = "ID_CSFinishBattle", Value = 9)]
		ID_CSFinishBattle,
		[ProtoEnum(Name = "ID_SCFinishBattle", Value = 10)]
		ID_SCFinishBattle,
		[ProtoEnum(Name = "ID_CSGetUserData", Value = 11)]
		ID_CSGetUserData,
		[ProtoEnum(Name = "ID_SCGetUserData", Value = 12)]
		ID_SCGetUserData,
		[ProtoEnum(Name = "ID_CSCreateUserData", Value = 13)]
		ID_CSCreateUserData,
		[ProtoEnum(Name = "ID_SCCreateUserData", Value = 14)]
		ID_SCCreateUserData,
		[ProtoEnum(Name = "ID_CSSetIcon", Value = 15)]
		ID_CSSetIcon,
		[ProtoEnum(Name = "ID_SCErrCode", Value = 18)]
		ID_SCErrCode = 18,
		[ProtoEnum(Name = "ID_SCSyncUserData", Value = 20)]
		ID_SCSyncUserData = 20,
		[ProtoEnum(Name = "ID_SCMatchFail", Value = 22)]
		ID_SCMatchFail = 22,
		[ProtoEnum(Name = "ID_CSPing", Value = 23)]
		ID_CSPing,
		[ProtoEnum(Name = "ID_SCPong", Value = 24)]
		ID_SCPong,
		[ProtoEnum(Name = "ID_CSLoadRank", Value = 25)]
		ID_CSLoadRank,
		[ProtoEnum(Name = "ID_SCLoadRank", Value = 26)]
		ID_SCLoadRank,
		[ProtoEnum(Name = "ID_SCReady", Value = 28)]
		ID_SCReady = 28,
		[ProtoEnum(Name = "ID_CSMapSave", Value = 29)]
		ID_CSMapSave,
		[ProtoEnum(Name = "ID_CSMapDel", Value = 31)]
		ID_CSMapDel = 31,
		[ProtoEnum(Name = "ID_CSMapLoad", Value = 33)]
		ID_CSMapLoad = 33,
		[ProtoEnum(Name = "ID_SCMapLoad", Value = 34)]
		ID_SCMapLoad,
		[ProtoEnum(Name = "ID_CSMapMark", Value = 35)]
		ID_CSMapMark,
		[ProtoEnum(Name = "ID_SCBoardFriendState", Value = 36)]
		ID_SCBoardFriendState,
		[ProtoEnum(Name = "ID_CSFriendFollow", Value = 37)]
		ID_CSFriendFollow,
		[ProtoEnum(Name = "ID_SCFriendFollow", Value = 38)]
		ID_SCFriendFollow,
		[ProtoEnum(Name = "ID_CSFriendLoad", Value = 39)]
		ID_CSFriendLoad,
		[ProtoEnum(Name = "ID_SCFriendLoad", Value = 40)]
		ID_SCFriendLoad,
		[ProtoEnum(Name = "ID_SCFriendNotify", Value = 42)]
		ID_SCFriendNotify = 42,
		[ProtoEnum(Name = "ID_CSFriendRecommend", Value = 43)]
		ID_CSFriendRecommend,
		[ProtoEnum(Name = "ID_SCFriendRecommend", Value = 44)]
		ID_SCFriendRecommend,
		[ProtoEnum(Name = "ID_CSFriendSearch", Value = 45)]
		ID_CSFriendSearch,
		[ProtoEnum(Name = "ID_SCFriendSearch", Value = 46)]
		ID_SCFriendSearch,
		[ProtoEnum(Name = "ID_SCRefereeReq", Value = 47)]
		ID_SCRefereeReq,
		[ProtoEnum(Name = "ID_CSRefereeRep", Value = 48)]
		ID_CSRefereeRep,
		[ProtoEnum(Name = "ID_SCRefereeStop", Value = 50)]
		ID_SCRefereeStop = 50,
		[ProtoEnum(Name = "ID_CSBattleReportLoad", Value = 51)]
		ID_CSBattleReportLoad,
		[ProtoEnum(Name = "ID_SCBattleReportLoad", Value = 52)]
		ID_SCBattleReportLoad,
		[ProtoEnum(Name = "ID_CSBattleReportPlay", Value = 53)]
		ID_CSBattleReportPlay,
		[ProtoEnum(Name = "ID_SCBattleReportPlay", Value = 54)]
		ID_SCBattleReportPlay,
		[ProtoEnum(Name = "ID_CSBattleResume", Value = 57)]
		ID_CSBattleResume = 57,
		[ProtoEnum(Name = "ID_SCBattleResume", Value = 58)]
		ID_SCBattleResume,
		[ProtoEnum(Name = "ID_SCFriendSearchs", Value = 59)]
		ID_SCFriendSearchs,
		[ProtoEnum(Name = "ID_SCTeamUpdate", Value = 60)]
		ID_SCTeamUpdate,
		[ProtoEnum(Name = "ID_CSTeamCreate", Value = 61)]
		ID_CSTeamCreate,
		[ProtoEnum(Name = "ID_SCTeamCreate", Value = 62)]
		ID_SCTeamCreate,
		[ProtoEnum(Name = "ID_CSTeamInvite", Value = 63)]
		ID_CSTeamInvite,
		[ProtoEnum(Name = "ID_SCTeamInvite", Value = 64)]
		ID_SCTeamInvite,
		[ProtoEnum(Name = "ID_CSTeamLeave", Value = 65)]
		ID_CSTeamLeave,
		[ProtoEnum(Name = "ID_SCTeamDel", Value = 66)]
		ID_SCTeamDel,
		[ProtoEnum(Name = "ID_CSTeamStart", Value = 67)]
		ID_CSTeamStart,
		[ProtoEnum(Name = "ID_SCTeamStart", Value = 68)]
		ID_SCTeamStart,
		[ProtoEnum(Name = "ID_SCTeamInviteReq", Value = 70)]
		ID_SCTeamInviteReq = 70,
		[ProtoEnum(Name = "ID_CSTeamInviteResp", Value = 71)]
		ID_CSTeamInviteResp,
		[ProtoEnum(Name = "ID_SCTeamInviteResp", Value = 72)]
		ID_SCTeamInviteResp,
		[ProtoEnum(Name = "ID_CSGetLeagueInfo", Value = 80)]
		ID_CSGetLeagueInfo = 80,
		[ProtoEnum(Name = "ID_SCGetLeagueInfo", Value = 81)]
		ID_SCGetLeagueInfo,
		[ProtoEnum(Name = "ID_CSLeagueList", Value = 82)]
		ID_CSLeagueList,
		[ProtoEnum(Name = "ID_SCLeagueList", Value = 83)]
		ID_SCLeagueList,
		[ProtoEnum(Name = "ID_CSLeagueAdd", Value = 84)]
		ID_CSLeagueAdd,
		[ProtoEnum(Name = "ID_SCLeagueAdd", Value = 85)]
		ID_SCLeagueAdd,
		[ProtoEnum(Name = "ID_CSLeagueRank", Value = 86)]
		ID_CSLeagueRank,
		[ProtoEnum(Name = "ID_SCLeagueRank", Value = 87)]
		ID_SCLeagueRank,
		[ProtoEnum(Name = "ID_CSLeagueMatch", Value = 88)]
		ID_CSLeagueMatch,
		[ProtoEnum(Name = "ID_SCLeagueMatch", Value = 89)]
		ID_SCLeagueMatch,
		[ProtoEnum(Name = "ID_CSQuitBattle", Value = 90)]
		ID_CSQuitBattle,
		[ProtoEnum(Name = "ID_SCQuitBattle", Value = 91)]
		ID_SCQuitBattle,
		[ProtoEnum(Name = "ID_CSWatchRooms", Value = 92)]
		ID_CSWatchRooms,
		[ProtoEnum(Name = "ID_SCWatchRooms", Value = 93)]
		ID_SCWatchRooms,
		[ProtoEnum(Name = "ID_CSJoinRoom", Value = 94)]
		ID_CSJoinRoom,
		[ProtoEnum(Name = "ID_SCJoinRoom", Value = 95)]
		ID_SCJoinRoom,
		[ProtoEnum(Name = "ID_CSCreateRoom", Value = 96)]
		ID_CSCreateRoom,
		[ProtoEnum(Name = "ID_SCCreateRoom", Value = 97)]
		ID_SCCreateRoom,
		[ProtoEnum(Name = "ID_CSQuitRoom", Value = 98)]
		ID_CSQuitRoom,
		[ProtoEnum(Name = "ID_SCQuitRoom", Value = 99)]
		ID_SCQuitRoom,
		[ProtoEnum(Name = "ID_SCRoomRefresh", Value = 101)]
		ID_SCRoomRefresh = 101,
		[ProtoEnum(Name = "ID_SCRoomListRefresh", Value = 103)]
		ID_SCRoomListRefresh = 103,
		[ProtoEnum(Name = "ID_CSUnWatchRooms", Value = 104)]
		ID_CSUnWatchRooms,
		[ProtoEnum(Name = "ID_SCUnWatchRooms", Value = 105)]
		ID_SCUnWatchRooms,
		[ProtoEnum(Name = "ID_SCMatch2CurNum", Value = 107)]
		ID_SCMatch2CurNum = 107,
		[ProtoEnum(Name = "ID_CSStartMatch2", Value = 108)]
		ID_CSStartMatch2,
		[ProtoEnum(Name = "ID_SCStartMatch2", Value = 109)]
		ID_SCStartMatch2,
		[ProtoEnum(Name = "ID_CSStartMatch3", Value = 110)]
		ID_CSStartMatch3,
		[ProtoEnum(Name = "ID_SCStartMatch3", Value = 111)]
		ID_SCStartMatch3,
		[ProtoEnum(Name = "ID_SCMatch3Notify", Value = 113)]
		ID_SCMatch3Notify = 113,
		[ProtoEnum(Name = "ID_CSChangeRace", Value = 114)]
		ID_CSChangeRace,
		[ProtoEnum(Name = "ID_SCChangeRace", Value = 115)]
		ID_SCChangeRace,
		[ProtoEnum(Name = "ID_SCSelectRaceNotify", Value = 117)]
		ID_SCSelectRaceNotify = 117,
		[ProtoEnum(Name = "ID_CSGetRaceData", Value = 118)]
		ID_CSGetRaceData,
		[ProtoEnum(Name = "ID_SCGetRaceData", Value = 119)]
		ID_SCGetRaceData,
		[ProtoEnum(Name = "ID_CSRaceSkillLevelUp", Value = 120)]
		ID_CSRaceSkillLevelUp,
		[ProtoEnum(Name = "ID_SCRaceSkillLevelUp", Value = 121)]
		ID_SCRaceSkillLevelUp,
		[ProtoEnum(Name = "ID_SCPackUpdate", Value = 123)]
		ID_SCPackUpdate = 123,
		[ProtoEnum(Name = "ID_CSGMCmd", Value = 124)]
		ID_CSGMCmd,
		[ProtoEnum(Name = "ID_CSUnlockChest", Value = 126)]
		ID_CSUnlockChest = 126,
		[ProtoEnum(Name = "ID_SCUnlockChest", Value = 127)]
		ID_SCUnlockChest,
		[ProtoEnum(Name = "ID_CSGainChest", Value = 128)]
		ID_CSGainChest,
		[ProtoEnum(Name = "ID_SCGainChest", Value = 129)]
		ID_SCGainChest,
		[ProtoEnum(Name = "ID_SCStartSelectRace", Value = 131)]
		ID_SCStartSelectRace = 131,
		[ProtoEnum(Name = "ID_CSGainTimerChest", Value = 132)]
		ID_CSGainTimerChest,
		[ProtoEnum(Name = "ID_SCGainTimerChest", Value = 133)]
		ID_SCGainTimerChest,
		[ProtoEnum(Name = "ID_CSGainBattleChest", Value = 134)]
		ID_CSGainBattleChest,
		[ProtoEnum(Name = "ID_SCGainBattleChest", Value = 135)]
		ID_SCGainBattleChest,
		[ProtoEnum(Name = "ID_SCAddChest", Value = 137)]
		ID_SCAddChest = 137,
		[ProtoEnum(Name = "ID_CSSingleMatch", Value = 138)]
		ID_CSSingleMatch,
		[ProtoEnum(Name = "ID_SCSingleMatch", Value = 139)]
		ID_SCSingleMatch,
		[ProtoEnum(Name = "ID_CSMallOpen", Value = 140)]
		ID_CSMallOpen,
		[ProtoEnum(Name = "ID_SCMallAll", Value = 141)]
		ID_SCMallAll,
		[ProtoEnum(Name = "ID_CSMallBoxBuy", Value = 142)]
		ID_CSMallBoxBuy,
		[ProtoEnum(Name = "ID_SCMallBoxBuy", Value = 143)]
		ID_SCMallBoxBuy,
		[ProtoEnum(Name = "ID_CSMallJewelBuy", Value = 144)]
		ID_CSMallJewelBuy,
		[ProtoEnum(Name = "ID_SCMallJewelBuy", Value = 145)]
		ID_SCMallJewelBuy,
		[ProtoEnum(Name = "ID_SCUpdateBattleChest", Value = 147)]
		ID_SCUpdateBattleChest = 147,
		[ProtoEnum(Name = "ID_SCUpdateTimerChest", Value = 149)]
		ID_SCUpdateTimerChest = 149,
		[ProtoEnum(Name = "ID_CSStartMatch4", Value = 150)]
		ID_CSStartMatch4,
		[ProtoEnum(Name = "ID_SCStartMatch4", Value = 151)]
		ID_SCStartMatch4,
		[ProtoEnum(Name = "ID_SCMatch4Info", Value = 153)]
		ID_SCMatch4Info = 153,
		[ProtoEnum(Name = "ID_CSCommitRace", Value = 154)]
		ID_CSCommitRace,
		[ProtoEnum(Name = "ID_SCCommitRace", Value = 155)]
		ID_SCCommitRace,
		[ProtoEnum(Name = "ID_CSRename", Value = 156)]
		ID_CSRename,
		[ProtoEnum(Name = "ID_SCRename", Value = 157)]
		ID_SCRename,
		[ProtoEnum(Name = "ID_CSSetCurLevel", Value = 158)]
		ID_CSSetCurLevel,
		[ProtoEnum(Name = "ID_SCSetCurLevel", Value = 159)]
		ID_SCSetCurLevel,
		[ProtoEnum(Name = "ID_CSLog", Value = 160)]
		ID_CSLog,
		[ProtoEnum(Name = "ID_SCLog", Value = 161)]
		ID_SCLog,
		[ProtoEnum(Name = "ID_SCRaceNotify", Value = 163)]
		ID_SCRaceNotify = 163,
		[ProtoEnum(Name = "ID_CSClientStorageSet", Value = 164)]
		ID_CSClientStorageSet,
		[ProtoEnum(Name = "ID_SCClientStorageSet", Value = 165)]
		ID_SCClientStorageSet,
		[ProtoEnum(Name = "ID_CSClientStorageLoad", Value = 166)]
		ID_CSClientStorageLoad,
		[ProtoEnum(Name = "ID_SCClientStorageLoad", Value = 167)]
		ID_SCClientStorageLoad,
		[ProtoEnum(Name = "ID_SCKickUserNtf", Value = 169)]
		ID_SCKickUserNtf = 169,
		[ProtoEnum(Name = "ID_CSLeagueQuit", Value = 170)]
		ID_CSLeagueQuit,
		[ProtoEnum(Name = "ID_SCLeagueQuit", Value = 171)]
		ID_SCLeagueQuit,
		[ProtoEnum(Name = "ID_CSResume", Value = 172)]
		ID_CSResume,
		[ProtoEnum(Name = "ID_SCResume", Value = 173)]
		ID_SCResume,
		[ProtoEnum(Name = "ID_SCNotify", Value = 175)]
		ID_SCNotify = 175,
		[ProtoEnum(Name = "ID_CSStartMatchReq", Value = 176)]
		ID_CSStartMatchReq,
		[ProtoEnum(Name = "ID_SCStartMatchReq", Value = 177)]
		ID_SCStartMatchReq,
		[ProtoEnum(Name = "ID_SCMatchInit", Value = 179)]
		ID_SCMatchInit = 179,
		[ProtoEnum(Name = "ID_SCMatchUpdate", Value = 181)]
		ID_SCMatchUpdate = 181,
		[ProtoEnum(Name = "ID_CSMatchComplete", Value = 182)]
		ID_CSMatchComplete,
		[ProtoEnum(Name = "ID_SCMatchComplete", Value = 183)]
		ID_SCMatchComplete,
		[ProtoEnum(Name = "ID_CSQuitMatch", Value = 184)]
		ID_CSQuitMatch,
		[ProtoEnum(Name = "ID_SCQuitMatch", Value = 185)]
		ID_SCQuitMatch,
		[ProtoEnum(Name = "ID_CSMailList", Value = 186)]
		ID_CSMailList,
		[ProtoEnum(Name = "ID_SCMailList", Value = 187)]
		ID_SCMailList,
		[ProtoEnum(Name = "ID_CSMailRead", Value = 188)]
		ID_CSMailRead,
		[ProtoEnum(Name = "ID_SCMailRead", Value = 189)]
		ID_SCMailRead,
		[ProtoEnum(Name = "ID_CSMailDel", Value = 190)]
		ID_CSMailDel,
		[ProtoEnum(Name = "ID_SCMailDel", Value = 191)]
		ID_SCMailDel,
		[ProtoEnum(Name = "ID_CSMailSend", Value = 192)]
		ID_CSMailSend,
		[ProtoEnum(Name = "ID_SCMailSend", Value = 193)]
		ID_SCMailSend,
		[ProtoEnum(Name = "ID_SCMailNotify", Value = 195)]
		ID_SCMailNotify = 195,
		[ProtoEnum(Name = "ID_CSMatchPos", Value = 196)]
		ID_CSMatchPos,
		[ProtoEnum(Name = "ID_SCMatchPos", Value = 197)]
		ID_SCMatchPos,
		[ProtoEnum(Name = "ID_CSSetLevelStar", Value = 200)]
		ID_CSSetLevelStar = 200,
		[ProtoEnum(Name = "ID_SCSetLevelStar", Value = 201)]
		ID_SCSetLevelStar,
		[ProtoEnum(Name = "ID_CSStartLevel", Value = 202)]
		ID_CSStartLevel,
		[ProtoEnum(Name = "ID_SCStartLevel", Value = 203)]
		ID_SCStartLevel,
		[ProtoEnum(Name = "ID_SCIntAttr", Value = 205)]
		ID_SCIntAttr = 205,
		[ProtoEnum(Name = "ID_SCStrAttr", Value = 207)]
		ID_SCStrAttr = 207,
		[ProtoEnum(Name = "ID_CSLoadChapters", Value = 208)]
		ID_CSLoadChapters,
		[ProtoEnum(Name = "ID_SCLoadChapters", Value = 209)]
		ID_SCLoadChapters,
		[ProtoEnum(Name = "ID_CSLoadChapter", Value = 210)]
		ID_CSLoadChapter,
		[ProtoEnum(Name = "ID_SCLoadChapter", Value = 211)]
		ID_SCLoadChapter,
		[ProtoEnum(Name = "ID_SCChangeMoney", Value = 212)]
		ID_SCChangeMoney,
		[ProtoEnum(Name = "ID_CSGenerateUrl", Value = 213)]
		ID_CSGenerateUrl,
		[ProtoEnum(Name = "ID_SCGenerateUrl", Value = 214)]
		ID_SCGenerateUrl,
		[ProtoEnum(Name = "ID_CSSetLevelScore", Value = 215)]
		ID_CSSetLevelScore,
		[ProtoEnum(Name = "ID_SCSetLevelScore", Value = 216)]
		ID_SCSetLevelScore,
		[ProtoEnum(Name = "ID_CSPveRankReportLoad", Value = 217)]
		ID_CSPveRankReportLoad,
		[ProtoEnum(Name = "ID_SCPveRankReportLoad", Value = 218)]
		ID_SCPveRankReportLoad,
		[ProtoEnum(Name = "ID_CSBuyChapter", Value = 219)]
		ID_CSBuyChapter,
		[ProtoEnum(Name = "ID_SCBuyChapter", Value = 220)]
		ID_SCBuyChapter,
		[ProtoEnum(Name = "ID_CSTaskOk", Value = 251)]
		ID_CSTaskOk = 251,
		[ProtoEnum(Name = "ID_SCTaskOk", Value = 252)]
		ID_SCTaskOk,
		[ProtoEnum(Name = "ID_CSTaskGet", Value = 253)]
		ID_CSTaskGet,
		[ProtoEnum(Name = "ID_SCTaskGet", Value = 254)]
		ID_SCTaskGet,
		[ProtoEnum(Name = "ID_CSMatchInvite", Value = 261)]
		ID_CSMatchInvite = 261,
		[ProtoEnum(Name = "ID_SCMatchInvite", Value = 262)]
		ID_SCMatchInvite,
		[ProtoEnum(Name = "ID_SCMatchInviteReq", Value = 264)]
		ID_SCMatchInviteReq = 264,
		[ProtoEnum(Name = "ID_CSMatchInviteResp", Value = 265)]
		ID_CSMatchInviteResp,
		[ProtoEnum(Name = "ID_SCMatchInviteResp", Value = 266)]
		ID_SCMatchInviteResp,
		[ProtoEnum(Name = "ID_CSBuySkin", Value = 267)]
		ID_CSBuySkin,
		[ProtoEnum(Name = "ID_SCBuySkin", Value = 268)]
		ID_SCBuySkin,
		[ProtoEnum(Name = "ID_CSVerifyOrderID", Value = 269)]
		ID_CSVerifyOrderID,
		[ProtoEnum(Name = "ID_SCVerifyOrderID", Value = 270)]
		ID_SCVerifyOrderID,
		[ProtoEnum(Name = "ID_CSReceiveMonthlyCard", Value = 271)]
		ID_CSReceiveMonthlyCard,
		[ProtoEnum(Name = "ID_SCReceiveMonthlyCard", Value = 272)]
		ID_SCReceiveMonthlyCard,
		[ProtoEnum(Name = "ID_SCChangeMonthlyCard", Value = 273)]
		ID_SCChangeMonthlyCard,
		[ProtoEnum(Name = "ID_CSUserInit", Value = 281)]
		ID_CSUserInit = 281,
		[ProtoEnum(Name = "ID_SCUserInit", Value = 282)]
		ID_SCUserInit,
		[ProtoEnum(Name = "ID_CSAdReward", Value = 291)]
		ID_CSAdReward = 291,
		[ProtoEnum(Name = "ID_SCAdReward", Value = 292)]
		ID_SCAdReward,
		[ProtoEnum(Name = "ID_CSGenerateOrderID", Value = 293)]
		ID_CSGenerateOrderID,
		[ProtoEnum(Name = "ID_SCGenerateOrderID", Value = 294)]
		ID_SCGenerateOrderID,
		[ProtoEnum(Name = "ID_CSAchievementSet", Value = 301)]
		ID_CSAchievementSet = 301,
		[ProtoEnum(Name = "ID_SCAchievementSet", Value = 302)]
		ID_SCAchievementSet,
		[ProtoEnum(Name = "ID_CSAchievementLoad", Value = 303)]
		ID_CSAchievementLoad,
		[ProtoEnum(Name = "ID_SCAchievementLoad", Value = 304)]
		ID_SCAchievementLoad,
		[ProtoEnum(Name = "ID_CSUploadOldVersionData", Value = 310)]
		ID_CSUploadOldVersionData = 310,
		[ProtoEnum(Name = "ID_SCUploadOldVersionData", Value = 311)]
		ID_SCUploadOldVersionData,
		[ProtoEnum(Name = "ID_CSLoadMatchList", Value = 320)]
		ID_CSLoadMatchList = 320,
		[ProtoEnum(Name = "ID_SCLoadMatchList", Value = 321)]
		ID_SCLoadMatchList,
		[ProtoEnum(Name = "ID_CSSearchMatch", Value = 322)]
		ID_CSSearchMatch,
		[ProtoEnum(Name = "ID_CSEditWatchName", Value = 323)]
		ID_CSEditWatchName,
		[ProtoEnum(Name = "ID_SCEditWatchName", Value = 324)]
		ID_SCEditWatchName,
		[ProtoEnum(Name = "ID_CSEditWatchLock", Value = 325)]
		ID_CSEditWatchLock,
		[ProtoEnum(Name = "ID_SCEditWatchLock", Value = 326)]
		ID_SCEditWatchLock,
		[ProtoEnum(Name = "ID_CSGetLadderReward", Value = 330)]
		ID_CSGetLadderReward = 330,
		[ProtoEnum(Name = "ID_SCGetLadderReward", Value = 331)]
		ID_SCGetLadderReward,
		[ProtoEnum(Name = "ID_CSClearData", Value = 340)]
		ID_CSClearData = 340,
		[ProtoEnum(Name = "ID_CSChangeLadderScore", Value = 341)]
		ID_CSChangeLadderScore,
		[ProtoEnum(Name = "ID_SCChangeLadderScore", Value = 342)]
		ID_SCChangeLadderScore,
		[ProtoEnum(Name = "ID_CSRVPReward", Value = 343)]
		ID_CSRVPReward,
		[ProtoEnum(Name = "ID_SCRVPReward", Value = 344)]
		ID_SCRVPReward,
		[ProtoEnum(Name = "ID_CSChapterScore", Value = 350)]
		ID_CSChapterScore = 350,
		[ProtoEnum(Name = "ID_SCChapterScore", Value = 351)]
		ID_SCChapterScore,
		[ProtoEnum(Name = "ID_CSDiscountChapter", Value = 352)]
		ID_CSDiscountChapter,
		[ProtoEnum(Name = "ID_SCDiscountChapter", Value = 353)]
		ID_SCDiscountChapter,
		[ProtoEnum(Name = "ID_CSSetChapterScore", Value = 354)]
		ID_CSSetChapterScore,
		[ProtoEnum(Name = "ID_SCSetChapterScore", Value = 355)]
		ID_SCSetChapterScore,
		[ProtoEnum(Name = "ID_CSUseItem", Value = 360)]
		ID_CSUseItem = 360,
		[ProtoEnum(Name = "ID_SCUseItem", Value = 361)]
		ID_SCUseItem,
		[ProtoEnum(Name = "ID_CSDecomposeItem", Value = 362)]
		ID_CSDecomposeItem,
		[ProtoEnum(Name = "ID_SCDecomposeItem", Value = 363)]
		ID_SCDecomposeItem,
		[ProtoEnum(Name = "ID_CSMonthCheck", Value = 400)]
		ID_CSMonthCheck = 400,
		[ProtoEnum(Name = "ID_SCMonthCheck", Value = 401)]
		ID_SCMonthCheck,
		[ProtoEnum(Name = "ID_SCPushMonthCheck", Value = 402)]
		ID_SCPushMonthCheck,
		[ProtoEnum(Name = "ID_CSPushMonthCheck", Value = 403)]
		ID_CSPushMonthCheck,
		[ProtoEnum(Name = "ID_CSBattleMarkTarget", Value = 500)]
		ID_CSBattleMarkTarget = 500,
		[ProtoEnum(Name = "ID_SCBattleMarkTarget", Value = 501)]
		ID_SCBattleMarkTarget,
		[ProtoEnum(Name = "ID_CSLotteryInfo", Value = 550)]
		ID_CSLotteryInfo = 550,
		[ProtoEnum(Name = "ID_SCLotteryInfo", Value = 551)]
		ID_SCLotteryInfo,
		[ProtoEnum(Name = "ID_CSLotteryNotes", Value = 552)]
		ID_CSLotteryNotes,
		[ProtoEnum(Name = "ID_SCLotterNotes", Value = 553)]
		ID_SCLotterNotes,
		[ProtoEnum(Name = "ID_CSLotteryAward", Value = 554)]
		ID_CSLotteryAward,
		[ProtoEnum(Name = "ID_SCLotteryAward", Value = 555)]
		ID_SCLotteryAward,
		[ProtoEnum(Name = "ID_CSGetMailItem", Value = 603)]
		ID_CSGetMailItem = 603,
		[ProtoEnum(Name = "ID_SCGetMailItem", Value = 604)]
		ID_SCGetMailItem
	}
}
