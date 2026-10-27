using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCRefereeReq")]
	[Serializable]
	public class SCRefereeReq : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
