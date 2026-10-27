using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCRoomListRefresh")]
	[Serializable]
	public class SCRoomListRefresh : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "roomid", DataFormat = DataFormat.TwosComplement)]
		public int roomid
		{
			get
			{
				return this._roomid;
			}
			set
			{
				this._roomid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "playernum", DataFormat = DataFormat.TwosComplement)]
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

		private string _gateway = "to=client";

		private int _roomid;

		private int _playernum;

		private IExtension extensionObject;
	}
}
