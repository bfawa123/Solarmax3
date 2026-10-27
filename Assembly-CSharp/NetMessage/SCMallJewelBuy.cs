using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMallJewelBuy")]
	[Serializable]
	public class SCMallJewelBuy : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
		public string gateway
		{
			get
			{
				return this._gateway;
			}
			set
			{
				this._gateway = value;
			}
		}

		[ProtoMember(1, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(2, IsRequired = true, Name = "jewelId", DataFormat = DataFormat.TwosComplement)]
		public int jewelId
		{
			get
			{
				return this._jewelId;
			}
			set
			{
				this._jewelId = value;
			}
		}

		[ProtoMember(3, Name = "items", DataFormat = DataFormat.Default)]
		public List<PackItem> items
		{
			get
			{
				return this._items;
			}
		}

		[ProtoMember(4, Name = "add_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> add_num
		{
			get
			{
				return this._add_num;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private ErrCode _code;

		private int _jewelId;

		private readonly List<PackItem> _items = new List<PackItem>();

		private readonly List<int> _add_num = new List<int>();

		private IExtension extensionObject;
	}
}
