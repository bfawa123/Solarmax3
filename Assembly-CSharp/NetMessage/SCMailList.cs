using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMailList")]
	[Serializable]
	public class SCMailList : IExtensible
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

		[ProtoMember(2, IsRequired = false, Name = "start", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		[ProtoMember(3, Name = "mail", DataFormat = DataFormat.Default)]
		public List<Mail> mail
		{
			get
			{
				return this._mail;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _start;

		private readonly List<Mail> _mail = new List<Mail>();

		private IExtension extensionObject;
	}
}
