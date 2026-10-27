using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "EndType")]
	public enum EndType
	{
		[ProtoEnum(Name = "ET_Dead", Value = 0)]
		ET_Dead,
		[ProtoEnum(Name = "ET_Giveup", Value = 1)]
		ET_Giveup,
		[ProtoEnum(Name = "ET_Win", Value = 2)]
		ET_Win,
		[ProtoEnum(Name = "ET_Timeout", Value = 3)]
		ET_Timeout
	}
}
