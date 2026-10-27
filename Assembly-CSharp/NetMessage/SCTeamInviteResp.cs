using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCTeamInviteResp")]
	[Serializable]
	public class SCTeamInviteResp : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "userId", DataFormat = DataFormat.TwosComplement)]
		public int userId
		{
			get
			{
				return this._userId;
			}
			set
			{
				this._userId = value;
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

		private int _userId;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
