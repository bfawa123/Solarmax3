using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBuySkin")]
	[Serializable]
	public class SCBuySkin : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "ret", DataFormat = DataFormat.TwosComplement)]
		public ErrCode ret
		{
			get
			{
				return this._ret;
			}
			set
			{
				this._ret = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
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

		private ErrCode _ret;

		private int _id;

		private IExtension extensionObject;
	}
}
