using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCJoinRoom")]
	[Serializable]
	public class SCJoinRoom : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		[ProtoMember(3, Name = "data", DataFormat = DataFormat.Default)]
		public List<UserData> data
		{
			get
			{
				return this._data;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "room", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public RoomInfo room
		{
			get
			{
				return this._room;
			}
			set
			{
				this._room = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _roomid;

		private ErrCode _code;

		private readonly List<UserData> _data = new List<UserData>();

		private RoomInfo _room;

		private IExtension extensionObject;
	}
}
