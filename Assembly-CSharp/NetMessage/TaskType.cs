using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "TaskType")]
	public enum TaskType
	{
		[ProtoEnum(Name = "TT_Once", Value = 1)]
		TT_Once = 1,
		[ProtoEnum(Name = "TT_Daily1", Value = 2)]
		TT_Daily1,
		[ProtoEnum(Name = "TT_Daily2", Value = 3)]
		TT_Daily2
	}
}
