using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSGetMailItem")]
	[Serializable]
	public class CSGetMailItem : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "mailid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
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
