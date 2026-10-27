using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSClearData")]
	[Serializable]
	public class CSClearData : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "version", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _version;

		private IExtension extensionObject;
	}
}
