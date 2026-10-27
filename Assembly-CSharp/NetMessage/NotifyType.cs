using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "NotifyType")]
	public enum NotifyType
	{
		[ProtoEnum(Name = "NT_Numm", Value = 0)]
		NT_Numm,
		[ProtoEnum(Name = "NT_Popup", Value = 1)]
		NT_Popup,
		[ProtoEnum(Name = "NT_Scroll", Value = 2)]
		NT_Scroll,
		[ProtoEnum(Name = "NT_Error", Value = 3)]
		NT_Error
	}
}
