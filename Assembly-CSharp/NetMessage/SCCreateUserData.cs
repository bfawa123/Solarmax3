using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCCreateUserData")]
	[Serializable]
	public class SCCreateUserData : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("auth=1|set=userid|to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "errcode", DataFormat = DataFormat.TwosComplement)]
		public ErrCode errcode
		{
			get
			{
				return this._errcode;
			}
			set
			{
				this._errcode = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "data", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public UserData data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "auth=1|set=userid|to=client";

		private ErrCode _errcode;

		private UserData _data;

		private IExtension extensionObject;
	}
}
