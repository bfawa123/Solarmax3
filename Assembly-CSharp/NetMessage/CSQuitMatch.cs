using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSQuitMatch")]
	[Serializable]
	public class CSQuitMatch : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _userid;

		private IExtension extensionObject;
	}
}
