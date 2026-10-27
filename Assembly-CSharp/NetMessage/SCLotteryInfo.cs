using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLotteryInfo")]
	[Serializable]
	public class SCLotteryInfo : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "retlId", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long retlId
		{
			get
			{
				return this._retlId;
			}
			set
			{
				this._retlId = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "errcode", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private long _retlId;

		private ErrCode _errcode;

		private IExtension extensionObject;
	}
}
