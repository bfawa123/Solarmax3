using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSPushMonthCheck")]
	[Serializable]
	public class CSPushMonthCheck : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
