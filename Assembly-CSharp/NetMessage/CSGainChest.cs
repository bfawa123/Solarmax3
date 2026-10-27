using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSGainChest")]
	[Serializable]
	public class CSGainChest : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "slot", DataFormat = DataFormat.TwosComplement)]
		public int slot
		{
			get
			{
				return this._slot;
			}
			set
			{
				this._slot = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "use_jewel", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool use_jewel
		{
			get
			{
				return this._use_jewel;
			}
			set
			{
				this._use_jewel = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=data";

		private int _slot;

		private bool _use_jewel;

		private IExtension extensionObject;
	}
}
