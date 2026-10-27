using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLotterNotes")]
	[Serializable]
	public class SCLotterNotes : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "totalTimes", DataFormat = DataFormat.TwosComplement)]
		public int totalTimes
		{
			get
			{
				return this._totalTimes;
			}
			set
			{
				this._totalTimes = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "lastTime", DataFormat = DataFormat.TwosComplement)]
		public long lastTime
		{
			get
			{
				return this._lastTime;
			}
			set
			{
				this._lastTime = value;
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

		[ProtoMember(4, IsRequired = true, Name = "todayAdLotteryNum", DataFormat = DataFormat.TwosComplement)]
		public int todayAdLotteryNum
		{
			get
			{
				return this._todayAdLotteryNum;
			}
			set
			{
				this._todayAdLotteryNum = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "currLotteryId", DataFormat = DataFormat.TwosComplement)]
		public int currLotteryId
		{
			get
			{
				return this._currLotteryId;
			}
			set
			{
				this._currLotteryId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _totalTimes;

		private long _lastTime;

		private readonly List<LotteryBoxInfo> _lotteryBoxInfo = new List<LotteryBoxInfo>();

		private int _todayAdLotteryNum;

		private int _currLotteryId;

		private IExtension extensionObject;
	}
}
