using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCChangeMoney")]
	[Serializable]
	public class SCChangeMoney : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "curMoney", DataFormat = DataFormat.TwosComplement)]
		public int curMoney
		{
			get
			{
				return this._curMoney;
			}
			set
			{
				this._curMoney = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "delta", DataFormat = DataFormat.TwosComplement)]
		public int delta
		{
			get
			{
				return this._delta;
			}
			set
			{
				this._delta = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _curMoney;

		private int _delta;

		private IExtension extensionObject;
	}
}
