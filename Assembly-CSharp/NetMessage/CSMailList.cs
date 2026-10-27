using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMailList")]
	[Serializable]
	public class CSMailList : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "start", DataFormat = DataFormat.TwosComplement)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _start;

		private IExtension extensionObject;
	}
}
