using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSAdReward")]
	[Serializable]
	public class CSAdReward : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
