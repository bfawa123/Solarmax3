using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "AdChannel")]
	public enum AdChannel
	{
		[ProtoEnum(Name = "AD_MIMENG", Value = 0)]
		AD_MIMENG,
		[ProtoEnum(Name = "AD_PANGOLIN", Value = 1)]
		AD_PANGOLIN
	}
}
