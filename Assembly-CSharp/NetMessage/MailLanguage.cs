using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MailLanguage")]
	[Serializable]
	public class MailLanguage : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "language", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(LanguageType.Chinese)]
		public LanguageType language
		{
			get
			{
				return this._language;
			}
			set
			{
				this._language = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "title", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string title
		{
			get
			{
				return this._title;
			}
			set
			{
				this._title = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "content", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string content
		{
			get
			{
				return this._content;
			}
			set
			{
				this._content = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private LanguageType _language;

		private string _title = string.Empty;

		private string _content = string.Empty;

		private IExtension extensionObject;
	}
}
