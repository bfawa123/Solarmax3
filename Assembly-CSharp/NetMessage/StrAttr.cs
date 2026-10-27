using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "StrAttr")]
	public enum StrAttr
	{
		[ProtoEnum(Name = "SA_Begin", Value = 0)]
		SA_Begin,
		[ProtoEnum(Name = "SA_LastLevel", Value = 0)]
		SA_LastLevel = 0,
		[ProtoEnum(Name = "SA_End", Value = 1)]
		SA_End
	}
}
