using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCUpdateBattleChest")]
	[Serializable]
	public class SCUpdateBattleChest : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "chest_winnum", DataFormat = DataFormat.TwosComplement)]
		public int chest_winnum
		{
			get
			{
				return this._chest_winnum;
			}
			set
			{
				this._chest_winnum = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _chest_winnum;

		private IExtension extensionObject;
	}
}
