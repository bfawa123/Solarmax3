using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchPos")]
	[Serializable]
	public class SCMatchPos : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
