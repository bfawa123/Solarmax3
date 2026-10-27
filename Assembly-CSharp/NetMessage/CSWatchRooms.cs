using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSWatchRooms")]
	[Serializable]
	public class CSWatchRooms : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "playernum", DataFormat = DataFormat.TwosComplement)]
		public int playernum
		{
			get
			{
				return this._playernum;
			}
			set
			{
				this._playernum = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=auto|to=match";

		private int _playernum;

		private IExtension extensionObject;
	}
}
