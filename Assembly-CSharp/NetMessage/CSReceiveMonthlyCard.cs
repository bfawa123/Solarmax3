using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSReceiveMonthlyCard")]
	[Serializable]
	public class CSReceiveMonthlyCard : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
