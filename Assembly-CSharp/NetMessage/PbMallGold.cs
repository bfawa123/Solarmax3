using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbMallGold")]
	[Serializable]
	public class PbMallGold : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
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

		[ProtoMember(3, IsRequired = true, Name = "jewel", DataFormat = DataFormat.TwosComplement)]
		public int jewel
		{
			get
			{
				return this._jewel;
			}
			set
			{
				this._jewel = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _id;

		private int _gold;

		private int _jewel;

		private IExtension extensionObject;
	}
}
