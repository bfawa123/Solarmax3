using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSUserInit")]
	[Serializable]
	public class CSUserInit : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
