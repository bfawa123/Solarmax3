using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMallBoxBuy")]
	[Serializable]
	public class CSMallBoxBuy : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "boxId", DataFormat = DataFormat.TwosComplement)]
		public int boxId
		{
			get
			{
				return this._boxId;
			}
			set
			{
				this._boxId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=data";

		private int _boxId;

		private IExtension extensionObject;
	}
}
