using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSDiscountChapter")]
	[Serializable]
	public class CSDiscountChapter : IExtensible
	{
		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private IExtension extensionObject;
	}
}
