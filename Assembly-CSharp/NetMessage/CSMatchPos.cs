using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMatchPos")]
	[Serializable]
	public class CSMatchPos : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "index", DataFormat = DataFormat.TwosComplement)]
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

		private int _userid;

		private int _index;

		private IExtension extensionObject;
	}
}
