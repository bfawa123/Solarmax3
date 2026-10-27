using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "TaskRewardType")]
	public enum TaskRewardType
	{
		[ProtoEnum(Name = "TRT_Gold", Value = 1)]
		TRT_Gold = 1,
		[ProtoEnum(Name = "TRT_HeadIcon", Value = 2)]
		TRT_HeadIcon,
		[ProtoEnum(Name = "TRT_Bg", Value = 3)]
		TRT_Bg,
		[ProtoEnum(Name = "TRT_Activity_Level", Value = 4)]
		TRT_Activity_Level
	}
}
