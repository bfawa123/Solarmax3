using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "LanguageType")]
	public enum LanguageType
	{
		[ProtoEnum(Name = "Chinese", Value = 0)]
		Chinese,
		[ProtoEnum(Name = "English", Value = 1)]
		English,
		[ProtoEnum(Name = "TraditionalChinese", Value = 2)]
		TraditionalChinese
	}
}
