using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "ChestItem")]
	[Serializable]
	public class ChestItem : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "timeout", DataFormat = DataFormat.TwosComplement)]
		public long timeout
		{
			get
			{
				return this._timeout;
			}
			set
			{
				this._timeout = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "slot", DataFormat = DataFormat.TwosComplement)]
		public int slot
		{
			get
			{
				return this._slot;
			}
			set
			{
				this._slot = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "serial", DataFormat = DataFormat.TwosComplement)]
		public int serial
		{
			get
			{
				return this._serial;
			}
			set
			{
				this._serial = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _id;

		private long _timeout;

		private int _slot;

		private int _serial;

		private IExtension extensionObject;
	}
}
