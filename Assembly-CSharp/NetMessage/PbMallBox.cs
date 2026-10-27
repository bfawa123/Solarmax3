using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbMallBox")]
	[Serializable]
	public class PbMallBox : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "gold", DataFormat = DataFormat.TwosComplement)]
		public int gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				this._gold = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _boxId;

		private int _gold;

		private IExtension extensionObject;
	}
}
