using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLotteryAward")]
	[Serializable]
	public class CSLotteryAward : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "boxId", DataFormat = DataFormat.TwosComplement)]
		public int boxId
		{
			get
			{
				return this._boxId;
			}
			set
			{
				this._boxId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _boxId;

		private IExtension extensionObject;
	}
}
