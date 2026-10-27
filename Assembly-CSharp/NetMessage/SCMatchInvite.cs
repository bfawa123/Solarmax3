using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchInvite")]
	[Serializable]
	public class SCMatchInvite : IExtensible
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _dstUserId;

		private IExtension extensionObject;
	}
}
