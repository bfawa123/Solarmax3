using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMatchInvite")]
	[Serializable]
	public class CSMatchInvite : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "dstUserId", DataFormat = DataFormat.TwosComplement)]
		public int dstUserId
		{
			get
			{
				return this._dstUserId;
			}
			set
			{
				this._dstUserId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _dstUserId;

		private IExtension extensionObject;
	}
}
