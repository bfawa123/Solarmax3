using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMatchComplete")]
	[Serializable]
	public class CSMatchComplete : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
