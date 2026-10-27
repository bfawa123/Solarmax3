using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLotteryNotes")]
	[Serializable]
	public class CSLotteryNotes : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
