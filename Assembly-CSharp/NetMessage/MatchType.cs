using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MatchType")]
	public enum MatchType
	{
		[ProtoEnum(Name = "MT_Null", Value = 0)]
		MT_Null,
		[ProtoEnum(Name = "MT_Ladder", Value = 1)]
		MT_Ladder,
		[ProtoEnum(Name = "MT_League", Value = 2)]
		MT_League,
		[ProtoEnum(Name = "MT_Room", Value = 3)]
		MT_Room,
		[ProtoEnum(Name = "MT_Sing", Value = 4)]
		MT_Sing,
		[ProtoEnum(Name = "MT_Cooperation", Value = 5)]
		MT_Cooperation
	}
}
