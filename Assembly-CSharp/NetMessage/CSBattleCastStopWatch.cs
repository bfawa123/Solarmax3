using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleCastStopWatch")]
	[Serializable]
	public class CSBattleCastStopWatch : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
