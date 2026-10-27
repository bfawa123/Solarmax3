using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "ClientStorageConst")]
	public enum ClientStorageConst
	{
		[ProtoEnum(Name = "ClientStorageMinIndex", Value = 0)]
		ClientStorageMinIndex,
		[ProtoEnum(Name = "ClientStorageCurLevel", Value = 1)]
		ClientStorageCurLevel,
		[ProtoEnum(Name = "ClientStorageCurGuide", Value = 2)]
		ClientStorageCurGuide,
		[ProtoEnum(Name = "ClientStorageRedPoints", Value = 3)]
		ClientStorageRedPoints,
		[ProtoEnum(Name = "ClientStorageMaxIndex", Value = 127)]
		ClientStorageMaxIndex = 127,
		[ProtoEnum(Name = "ClientStorageMaxValueLen", Value = 64)]
		ClientStorageMaxValueLen = 64
	}
}
