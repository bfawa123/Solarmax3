using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCNotify")]
	[Serializable]
	public class SCNotify : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "typ", DataFormat = DataFormat.TwosComplement)]
		public NotifyType typ
		{
			get
			{
				return this._typ;
			}
			set
			{
				this._typ = value;
			}
		}

		[ProtoMember(2, Name = "sarg", DataFormat = DataFormat.Default)]
		public List<string> sarg
		{
			get
			{
				return this._sarg;
			}
		}

		[ProtoMember(3, Name = "narg", DataFormat = DataFormat.TwosComplement)]
		public List<long> narg
		{
			get
			{
				return this._narg;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "text", DataFormat = DataFormat.Default)]
		public string text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private NotifyType _typ;

		private readonly List<string> _sarg = new List<string>();

		private readonly List<long> _narg = new List<long>();

		private string _text;

		private IExtension extensionObject;
	}
}
