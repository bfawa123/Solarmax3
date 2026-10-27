using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleCastStartWatch")]
	[Serializable]
	public class CSBattleCastStartWatch : IExtensible
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private long _battleid;

		private IExtension extensionObject;
	}
}
