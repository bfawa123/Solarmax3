using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCKickUserNtf")]
	[Serializable]
	public class SCKickUserNtf : IExtensible
	{
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

		[ProtoMember(2, IsRequired = true, Name = "device_model", DataFormat = DataFormat.Default)]
		public string device_model
		{
			get
			{
				return this._device_model;
			}
			set
			{
				this._device_model = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private string _device_model;

		private IExtension extensionObject;
	}
}
