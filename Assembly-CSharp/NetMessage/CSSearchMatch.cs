using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSSearchMatch")]
	[Serializable]
	public class CSSearchMatch : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "key", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string key
		{
			get
			{
				return this._key;
			}
			set
			{
				this._key = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _key = string.Empty;

		private IExtension extensionObject;
	}
}
