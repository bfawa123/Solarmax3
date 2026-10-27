using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCGetLadderReward")]
	[Serializable]
	public class SCGetLadderReward : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "code", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
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

		[ProtoMember(2, IsRequired = false, Name = "index", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int index
		{
			get
			{
				return this._index;
			}
			set
			{
				this._index = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _index;

		private IExtension extensionObject;
	}
}
