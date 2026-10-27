using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCPong")]
	[Serializable]
	public class SCPong : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("auth=1|to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "timestamp", DataFormat = DataFormat.TwosComplement)]
		public long timestamp
		{
			get
			{
				return this._timestamp;
			}
			set
			{
				this._timestamp = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "serial", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int serial
		{
			get
			{
				return this._serial;
			}
			set
			{
				this._serial = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "auth=1|to=client";

		private long _timestamp;

		private int _serial;

		private IExtension extensionObject;
	}
}
