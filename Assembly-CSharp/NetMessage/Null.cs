using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Null")]
	[Serializable]
	public class Null : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
