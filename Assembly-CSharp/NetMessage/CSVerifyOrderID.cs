using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSVerifyOrderID")]
	[Serializable]
	public class CSVerifyOrderID : IExtensible
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _orderID = string.Empty;

		private IExtension extensionObject;
	}
}
