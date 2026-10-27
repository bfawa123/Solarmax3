using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "LotteryOpTypeDefine")]
	public enum LotteryOpTypeDefine
	{
		[ProtoEnum(Name = "Lottery_FREE", Value = 1)]
		Lottery_FREE = 1,
		[ProtoEnum(Name = "Lottery_AD", Value = 2)]
		Lottery_AD,
		[ProtoEnum(Name = "Lottery_GOLD", Value = 3)]
		Lottery_GOLD
	}
}
