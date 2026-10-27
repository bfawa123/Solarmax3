using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchInviteResp")]
	[Serializable]
	public class SCMatchInviteResp : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "dstUserId", DataFormat = DataFormat.TwosComplement)]
		public int dstUserId
		{
			get
			{
				return this._dstUserId;
			}
			set
			{
				this._dstUserId = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "dstName", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string dstName
		{
			get
			{
				return this._dstName;
			}
			set
			{
				this._dstName = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _dstUserId;

		private string _dstName = string.Empty;

		private IExtension extensionObject;
	}
}
