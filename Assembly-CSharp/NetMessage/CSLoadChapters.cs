using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLoadChapters")]
	[Serializable]
	public class CSLoadChapters : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
