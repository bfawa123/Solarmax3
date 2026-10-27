using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMailRead")]
	[Serializable]
	public class CSMailRead : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "mailid", DataFormat = DataFormat.TwosComplement)]
		public int mailid
		{
			get
			{
				return this._mailid;
			}
			set
			{
				this._mailid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _mailid;

		private IExtension extensionObject;
	}
}
