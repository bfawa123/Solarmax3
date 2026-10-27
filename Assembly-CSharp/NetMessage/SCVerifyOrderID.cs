using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCVerifyOrderID")]
	[Serializable]
	public class SCVerifyOrderID : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "orderID", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string orderID
		{
			get
			{
				return this._orderID;
			}
			set
			{
				this._orderID = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "ret", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
		public ErrCode ret
		{
			get
			{
				return this._ret;
			}
			set
			{
				this._ret = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _orderID = string.Empty;

		private ErrCode _ret;

		private IExtension extensionObject;
	}
}
