using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "IntAttr")]
	public enum IntAttr
	{
		[ProtoEnum(Name = "IA_Begin", Value = 0)]
		IA_Begin,
		[ProtoEnum(Name = "IA_Power", Value = 0)]
		IA_Power = 0,
		[ProtoEnum(Name = "IA_End", Value = 1)]
		IA_End
	}
}
