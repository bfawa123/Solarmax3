using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSChangeRace")]
	[Serializable]
	public class CSChangeRace : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=auto|to=match")]
		public string gateway
		{
			get
			{
				return this._gateway;
			}
			set
			{
				this._gateway = value;
			}
		}

		[ProtoMember(1, IsRequired = true, Name = "race", DataFormat = DataFormat.TwosComplement)]
		public int race
		{
			get
			{
				return this._race;
			}
			set
			{
				this._race = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=auto|to=match";

		private int _race;

		private IExtension extensionObject;
	}
}
