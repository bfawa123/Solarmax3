using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCUpdateTimerChest")]
	[Serializable]
	public class SCUpdateTimerChest : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "chest_gainpoint", DataFormat = DataFormat.TwosComplement)]
		public long chest_gainpoint
		{
			get
			{
				return this._chest_gainpoint;
			}
			set
			{
				this._chest_gainpoint = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private long _chest_gainpoint;

		private IExtension extensionObject;
	}
}
