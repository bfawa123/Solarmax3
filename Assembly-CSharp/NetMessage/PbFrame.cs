using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbFrame")]
	[Serializable]
	public class PbFrame : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "content", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public byte[] content
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

		private byte[] _content;

		private IExtension extensionObject;
	}
}
