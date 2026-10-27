using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "ErrCode")]
	public enum ErrCode
	{
		[ProtoEnum(Name = "EC_Null", Value = 0)]
		EC_Null,
		[ProtoEnum(Name = "EC_SysUnknown", Value = -1)]
		EC_SysUnknown = -1,
		[ProtoEnum(Name = "EC_SysBusy", Value = -2)]
		EC_SysBusy = -2,
		[ProtoEnum(Name = "EC_RedisOpFailed", Value = -3)]
		EC_RedisOpFailed = -3,
		[ProtoEnum(Name = "EC_Offline", Value = -4)]
		EC_Offline = -4,
		[ProtoEnum(Name = "EC_InvalidMsg", Value = -5)]
		EC_InvalidMsg = -5,
		[ProtoEnum(Name = "EC_OnBattle", Value = -6)]
		EC_OnBattle = -6,
		[ProtoEnum(Name = "EC_MgoOpFailed", Value = -7)]
		EC_MgoOpFailed = -7,
		[ProtoEnum(Name = "EC_Ok", Value = 1)]
		EC_Ok = 1,
		[ProtoEnum(Name = "EC_MapSaveMaxMapCount", Value = 2)]
		EC_MapSaveMaxMapCount,
		[ProtoEnum(Name = "EC_NoSuchMap", Value = 3)]
		EC_NoSuchMap,
		[ProtoEnum(Name = "EC_GoldNotEnough", Value = 4)]
		EC_GoldNotEnough,
		[ProtoEnum(Name = "EC_NoExist", Value = 5)]
		EC_NoExist,
		[ProtoEnum(Name = "EC_AccountExist", Value = 6)]
		EC_AccountExist,
		[ProtoEnum(Name = "EC_NameExist", Value = 7)]
		EC_NameExist,
		[ProtoEnum(Name = "EC_InvalidName", Value = 8)]
		EC_InvalidName,
		[ProtoEnum(Name = "EC_UserIsOnline", Value = 9)]
		EC_UserIsOnline,
		[ProtoEnum(Name = "EC_TeamInviteRefuse", Value = 10)]
		EC_TeamInviteRefuse,
		[ProtoEnum(Name = "EC_TeamDBParseFailed", Value = 13)]
		EC_TeamDBParseFailed = 13,
		[ProtoEnum(Name = "EC_TeamDBDataInvalid", Value = 14)]
		EC_TeamDBDataInvalid,
		[ProtoEnum(Name = "EC_TeamNoExist", Value = 15)]
		EC_TeamNoExist,
		[ProtoEnum(Name = "EC_TeamFull", Value = 16)]
		EC_TeamFull,
		[ProtoEnum(Name = "EC_MapExist", Value = 20)]
		EC_MapExist = 20,
		[ProtoEnum(Name = "EC_RefereeBusy", Value = 30)]
		EC_RefereeBusy = 30,
		[ProtoEnum(Name = "EC_InBattle", Value = 40)]
		EC_InBattle = 40,
		[ProtoEnum(Name = "EC_NeedResume", Value = 41)]
		EC_NeedResume,
		[ProtoEnum(Name = "EC_CanNotResume", Value = 42)]
		EC_CanNotResume,
		[ProtoEnum(Name = "EC_LeagueIsFull", Value = 50)]
		EC_LeagueIsFull = 50,
		[ProtoEnum(Name = "EC_LeagueLevelNotMatch", Value = 51)]
		EC_LeagueLevelNotMatch,
		[ProtoEnum(Name = "EC_LeagueNotOpen", Value = 52)]
		EC_LeagueNotOpen,
		[ProtoEnum(Name = "EC_LeagueIn", Value = 53)]
		EC_LeagueIn,
		[ProtoEnum(Name = "EC_LeagueNotExist", Value = 54)]
		EC_LeagueNotExist,
		[ProtoEnum(Name = "EC_LeagueNotIn", Value = 55)]
		EC_LeagueNotIn,
		[ProtoEnum(Name = "EC_LeagueNotInMatchTime", Value = 56)]
		EC_LeagueNotInMatchTime,
		[ProtoEnum(Name = "EC_RoomNoExist", Value = 57)]
		EC_RoomNoExist,
		[ProtoEnum(Name = "EC_RoomAlreadyIn", Value = 58)]
		EC_RoomAlreadyIn,
		[ProtoEnum(Name = "EC_RoomInvalidMatch", Value = 59)]
		EC_RoomInvalidMatch,
		[ProtoEnum(Name = "EC_RoomNotIn", Value = 60)]
		EC_RoomNotIn,
		[ProtoEnum(Name = "EC_RoomBattleState", Value = 61)]
		EC_RoomBattleState,
		[ProtoEnum(Name = "EC_RaceSkillIndexErr", Value = 62)]
		EC_RaceSkillIndexErr,
		[ProtoEnum(Name = "EC_RaceSkillInvalidSkill", Value = 63)]
		EC_RaceSkillInvalidSkill,
		[ProtoEnum(Name = "EC_RaceCostNotEnough", Value = 64)]
		EC_RaceCostNotEnough,
		[ProtoEnum(Name = "EC_ChestUnlocking", Value = 70)]
		EC_ChestUnlocking = 70,
		[ProtoEnum(Name = "EC_ChestLocked", Value = 71)]
		EC_ChestLocked,
		[ProtoEnum(Name = "EC_ChestLootSlotFull", Value = 72)]
		EC_ChestLootSlotFull,
		[ProtoEnum(Name = "EC_ChestLootNothing", Value = 73)]
		EC_ChestLootNothing,
		[ProtoEnum(Name = "EC_ChestNoExist", Value = 74)]
		EC_ChestNoExist,
		[ProtoEnum(Name = "EC_ChestJewelNotEnough", Value = 75)]
		EC_ChestJewelNotEnough,
		[ProtoEnum(Name = "EC_ChestTimeNotEnough", Value = 76)]
		EC_ChestTimeNotEnough,
		[ProtoEnum(Name = "EC_ChestWinNumNotEnough", Value = 77)]
		EC_ChestWinNumNotEnough,
		[ProtoEnum(Name = "EC_AccountTakenOver", Value = 78)]
		EC_AccountTakenOver,
		[ProtoEnum(Name = "EC_MatchIsFull", Value = 79)]
		EC_MatchIsFull,
		[ProtoEnum(Name = "EC_MatchIsMember", Value = 80)]
		EC_MatchIsMember,
		[ProtoEnum(Name = "EC_NotInMatch", Value = 81)]
		EC_NotInMatch,
		[ProtoEnum(Name = "EC_NotMaster", Value = 82)]
		EC_NotMaster,
		[ProtoEnum(Name = "EC_RoomNotExist", Value = 83)]
		EC_RoomNotExist,
		[ProtoEnum(Name = "EC_PosInvalid", Value = 84)]
		EC_PosInvalid,
		[ProtoEnum(Name = "EC_NoSuchLevel", Value = 85)]
		EC_NoSuchLevel,
		[ProtoEnum(Name = "EC_NoInChapter", Value = 86)]
		EC_NoInChapter,
		[ProtoEnum(Name = "EC_NoDependLevel", Value = 87)]
		EC_NoDependLevel,
		[ProtoEnum(Name = "EC_PowerNotEnough", Value = 88)]
		EC_PowerNotEnough,
		[ProtoEnum(Name = "EC_ChapterNotExist", Value = 90)]
		EC_ChapterNotExist = 90,
		[ProtoEnum(Name = "EC_ChapterBought", Value = 91)]
		EC_ChapterBought,
		[ProtoEnum(Name = "EC_MatchInviteRefuse", Value = 95)]
		EC_MatchInviteRefuse = 95,
		[ProtoEnum(Name = "EC_NeedUpdate", Value = 110)]
		EC_NeedUpdate = 110,
		[ProtoEnum(Name = "EC_NeedUpload", Value = 111)]
		EC_NeedUpload,
		[ProtoEnum(Name = "EC_SkinNotExist", Value = 120)]
		EC_SkinNotExist = 120,
		[ProtoEnum(Name = "EC_SkinBought", Value = 121)]
		EC_SkinBought,
		[ProtoEnum(Name = "EC_NotBuyThisChapter", Value = 130)]
		EC_NotBuyThisChapter = 130,
		[ProtoEnum(Name = "EC_OrderFail", Value = 131)]
		EC_OrderFail,
		[ProtoEnum(Name = "EC_NoMonthCard", Value = 132)]
		EC_NoMonthCard,
		[ProtoEnum(Name = "EC_TodayReceived", Value = 133)]
		EC_TodayReceived,
		[ProtoEnum(Name = "EC_OnlyInvite", Value = 134)]
		EC_OnlyInvite,
		[ProtoEnum(Name = "EC_LadderRewardReceived", Value = 140)]
		EC_LadderRewardReceived = 140,
		[ProtoEnum(Name = "EC_LadderNoReward", Value = 141)]
		EC_LadderNoReward,
		[ProtoEnum(Name = "EC_LadderNotReached", Value = 142)]
		EC_LadderNotReached,
		[ProtoEnum(Name = "EC_LadderSeasonEnd", Value = 143)]
		EC_LadderSeasonEnd,
		[ProtoEnum(Name = "EC_AAOvertime", Value = 145)]
		EC_AAOvertime = 145,
		[ProtoEnum(Name = "EC_AlreadyEvaluated", Value = 146)]
		EC_AlreadyEvaluated
	}
}
