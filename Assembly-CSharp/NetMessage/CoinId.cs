using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CoinId")]
	public enum CoinId
	{
		[ProtoEnum(Name = "CI_Null", Value = 0)]
		CI_Null,
		[ProtoEnum(Name = "Gold", Value = 1)]
		Gold,
		[ProtoEnum(Name = "Jewel", Value = 2)]
		Jewel
	}
}
