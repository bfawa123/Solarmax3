using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSGenerateOrderID")]
	[Serializable]
	public class CSGenerateOrderID : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "productID", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string productID
		{
			get
			{
				return this._productID;
			}
			set
			{
				this._productID = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "num", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _productID = string.Empty;

		private int _num;

		private IExtension extensionObject;
	}
}
