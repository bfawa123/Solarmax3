using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Pack")]
	[Serializable]
	public class Pack : IExtensible
	{
		[ProtoMember(1, Name = "items", DataFormat = DataFormat.Default)]
		public List<PackItem> items
		{
			get
			{
				return this._items;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<PackItem> _items = new List<PackItem>();

		private IExtension extensionObject;
	}
}
