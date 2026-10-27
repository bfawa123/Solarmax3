using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CooperationType")]
	public enum CooperationType
	{
		[ProtoEnum(Name = "CT_1v1", Value = 0)]
		CT_1v1,
		[ProtoEnum(Name = "CT_2v2", Value = 1)]
		CT_2v2,
		[ProtoEnum(Name = "CT_1v1v1", Value = 2)]
		CT_1v1v1,
		[ProtoEnum(Name = "CT_1v1v1v1", Value = 3)]
		CT_1v1v1v1,
		[ProtoEnum(Name = "CT_2vPC", Value = 4)]
		CT_2vPC,
		[ProtoEnum(Name = "CT_3vPC", Value = 5)]
		CT_3vPC,
		[ProtoEnum(Name = "CT_4vPC", Value = 6)]
		CT_4vPC,
		[ProtoEnum(Name = "CT_3v1", Value = 7)]
		CT_3v1,
		[ProtoEnum(Name = "CT_Null", Value = 8)]
		CT_Null
	}
}
