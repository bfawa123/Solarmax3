using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "RoomInfo")]
	[Serializable]
	public class RoomInfo : IExtensible
	{
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

		[ProtoMember(2, IsRequired = true, Name = "matchid", DataFormat = DataFormat.Default)]
		public string matchid
		{
			get
			{
				return this._matchid;
			}
			set
			{
				this._matchid = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "playernum", DataFormat = DataFormat.TwosComplement)]
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

		private int _roomid;

		private string _matchid;

		private int _playernum;

		private IExtension extensionObject;
	}
}
