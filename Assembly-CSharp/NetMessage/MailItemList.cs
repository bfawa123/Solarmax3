using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MailItemList")]
	[Serializable]
	public class MailItemList : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "itemId", DataFormat = DataFormat.TwosComplement)]
		public int itemId
		{
			get
			{
				return this._itemId;
			}
			set
			{
				this._itemId = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "itemCount", DataFormat = DataFormat.TwosComplement)]
		public int itemCount
		{
			get
			{
				return this._itemCount;
			}
			set
			{
				this._itemCount = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "itemType", DataFormat = DataFormat.TwosComplement)]
		public int itemType
		{
			get
			{
				return this._itemType;
			}
			set
			{
				this._itemType = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _itemId;

		private int _itemCount;

		private int _itemType;

		private IExtension extensionObject;
	}
}
