using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCUseItem")]
	[Serializable]
	public class SCUseItem : IExtensible
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

		[ProtoMember(3, IsRequired = false, Name = "code", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _item_id;

		private long _count;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
