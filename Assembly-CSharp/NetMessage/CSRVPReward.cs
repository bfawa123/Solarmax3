using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSRVPReward")]
	[Serializable]
	public class CSRVPReward : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "multipy", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int multipy
		{
			get
			{
				return this._multipy;
			}
			set
			{
				this._multipy = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _multipy;

		private IExtension extensionObject;
	}
}
