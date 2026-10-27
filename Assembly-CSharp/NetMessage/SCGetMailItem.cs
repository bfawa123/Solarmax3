using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCGetMailItem")]
	[Serializable]
	public class SCGetMailItem : IExtensible
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

		[ProtoMember(2, IsRequired = false, Name = "mailid", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(3, Name = "mailItems", DataFormat = DataFormat.Default)]
		public List<MailItemList> mailItems
		{
			get
			{
				return this._mailItems;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _mailid;

		private readonly List<MailItemList> _mailItems = new List<MailItemList>();

		private IExtension extensionObject;
	}
}
