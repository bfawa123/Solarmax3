using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCWatchRooms")]
	[Serializable]
	public class SCWatchRooms : IExtensible
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

		[ProtoMember(2, Name = "rooms", DataFormat = DataFormat.Default)]
		public List<RoomInfo> rooms
		{
			get
			{
				return this._rooms;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _playernum;

		private readonly List<RoomInfo> _rooms = new List<RoomInfo>();

		private IExtension extensionObject;
	}
}
