using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBuySkin")]
	[Serializable]
	public class CSBuySkin : IExtensible
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _id;

		private IExtension extensionObject;
	}
}
