using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "LotteryBoxInfo")]
	[Serializable]
	public class LotteryBoxInfo : IExtensible
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

		[ProtoMember(2, IsRequired = false, Name = "get", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool get
		{
			get
			{
				return this._get;
			}
			set
			{
				this._get = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _awardId;

		private bool _get;

		private IExtension extensionObject;
	}
}
