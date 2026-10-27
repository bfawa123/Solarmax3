using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSUseItem")]
	[Serializable]
	public class CSUseItem : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "item_id", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int item_id
		{
			get
			{
				return this._item_id;
			}
			set
			{
				this._item_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long count
		{
			get
			{
				return this._count;
			}
			set
			{
				this._count = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _item_id;

		private long _count;

		private IExtension extensionObject;
	}
}
