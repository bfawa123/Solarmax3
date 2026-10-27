using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "BattleType")]
	public enum BattleType
	{
		[ProtoEnum(Name = "BT_Null", Value = 0)]
		BT_Null,
		[ProtoEnum(Name = "Melee", Value = 1)]
		Melee,
		[ProtoEnum(Name = "Group2v2", Value = 2)]
		Group2v2,
		[ProtoEnum(Name = "Group3v3", Value = 3)]
		Group3v3
	}
}
