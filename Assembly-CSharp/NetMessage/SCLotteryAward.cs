using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLotteryAward")]
	[Serializable]
	public class SCLotteryAward : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "awardId", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int awardId
		{
			get
			{
				return this._awardId;
			}
			set
			{
				this._awardId = value;
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

		[ProtoMember(3, Name = "lotteryBoxInfo", DataFormat = DataFormat.Default)]
		public List<LotteryBoxInfo> lotteryBoxInfo
		{
			get
			{
				return this._lotteryBoxInfo;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _awardId;

		private ErrCode _errcode;

		private readonly List<LotteryBoxInfo> _lotteryBoxInfo = new List<LotteryBoxInfo>();

		private IExtension extensionObject;
	}
}
