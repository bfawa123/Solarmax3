using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLotteryInfo")]
	[Serializable]
	public class CSLotteryInfo : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "optype", DataFormat = DataFormat.TwosComplement)]
		public int optype
		{
			get
			{
				return this._optype;
			}
			set
			{
				this._optype = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "currLotteryId", DataFormat = DataFormat.TwosComplement)]
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

		private int _optype;

		private int _currLotteryId;

		private IExtension extensionObject;
	}
}
