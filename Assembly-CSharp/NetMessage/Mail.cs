using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Mail")]
	[Serializable]
	public class Mail : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "mailId", DataFormat = DataFormat.TwosComplement)]
		public int mailId
		{
			get
			{
				return this._mailId;
			}
			set
			{
				this._mailId = value;
			}
		}

		[ProtoMember(2, Name = "mailLanguages", DataFormat = DataFormat.Default)]
		public List<MailLanguage> mailLanguages
		{
			get
			{
				return this._mailLanguages;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "Read", DataFormat = DataFormat.Default)]
		public bool Read
		{
			get
			{
				return this._Read;
			}
			set
			{
				this._Read = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "Get", DataFormat = DataFormat.Default)]
		public bool Get
		{
			get
			{
				return this._Get;
			}
			set
			{
				this._Get = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "sendTime", DataFormat = DataFormat.TwosComplement)]
		public int sendTime
		{
			get
			{
				return this._sendTime;
			}
			set
			{
				this._sendTime = value;
			}
		}

		[ProtoMember(6, Name = "mailItems", DataFormat = DataFormat.Default)]
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

		private int _mailId;

		private readonly List<MailLanguage> _mailLanguages = new List<MailLanguage>();

		private bool _Read;

		private bool _Get;

		private int _sendTime;

		private readonly List<MailItemList> _mailItems = new List<MailItemList>();

		private IExtension extensionObject;
	}
}
