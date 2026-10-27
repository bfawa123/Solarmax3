using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCSetCurLevel")]
	[Serializable]
	public class SCSetCurLevel : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "cur_level", DataFormat = DataFormat.Default)]
		public string cur_level
		{
			get
			{
				return this._cur_level;
			}
			set
			{
				this._cur_level = value;
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

		private string _cur_level;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
