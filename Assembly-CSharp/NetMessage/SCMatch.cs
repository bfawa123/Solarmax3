using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatch")]
	[Serializable]
	public class SCMatch : IExtensible
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

		[ProtoMember(3, IsRequired = true, Name = "count_down", DataFormat = DataFormat.TwosComplement)]
		public int count_down
		{
			get
			{
				return this._count_down;
			}
			set
			{
				this._count_down = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _count_down;

		private IExtension extensionObject;
	}
}
