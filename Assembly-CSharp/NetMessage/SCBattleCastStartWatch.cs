using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBattleCastStartWatch")]
	[Serializable]
	public class SCBattleCastStartWatch : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "battleid", DataFormat = DataFormat.TwosComplement)]
		public long battleid
		{
			get
			{
				return this._battleid;
			}
			set
			{
				this._battleid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private long _battleid;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
