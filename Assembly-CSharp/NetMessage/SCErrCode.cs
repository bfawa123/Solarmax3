using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCErrCode")]
	[Serializable]
	public class SCErrCode : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "msg", DataFormat = DataFormat.TwosComplement)]
		public MsgId msg
		{
			get
			{
				return this._msg;
			}
			set
			{
				this._msg = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		private string _gateway = "to=client";

		private MsgId _msg;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
