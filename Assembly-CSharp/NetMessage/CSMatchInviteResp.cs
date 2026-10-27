using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMatchInviteResp")]
	[Serializable]
	public class CSMatchInviteResp : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "accept", DataFormat = DataFormat.Default)]
		public bool accept
		{
			get
			{
				return this._accept;
			}
			set
			{
				this._accept = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "srcUserId", DataFormat = DataFormat.TwosComplement)]
		public int srcUserId
		{
			get
			{
				return this._srcUserId;
			}
			set
			{
				this._srcUserId = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private bool _accept;

		private int _srcUserId;

		private IExtension extensionObject;
	}
}
