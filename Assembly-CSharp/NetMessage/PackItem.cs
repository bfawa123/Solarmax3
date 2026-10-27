using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PackItem")]
	[Serializable]
	public class PackItem : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "itemid", DataFormat = DataFormat.TwosComplement)]
		public int itemid
		{
			get
			{
				return this._itemid;
			}
			set
			{
				this._itemid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "num", DataFormat = DataFormat.TwosComplement)]
		public int num
		{
			get
			{
				return this._num;
			}
			set
			{
				this._num = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "id", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _itemid;

		private int _num;

		private int _id;

		private IExtension extensionObject;
	}
}
