using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchInviteReq")]
	[Serializable]
	public class SCMatchInviteReq : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "type", DataFormat = DataFormat.TwosComplement)]
		public MatchType type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "cType", DataFormat = DataFormat.TwosComplement)]
		public CooperationType cType
		{
			get
			{
				return this._cType;
			}
			set
			{
				this._cType = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "mapId", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string mapId
		{
			get
			{
				return this._mapId;
			}
			set
			{
				this._mapId = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "roomId", DataFormat = DataFormat.Default)]
		public string roomId
		{
			get
			{
				return this._roomId;
			}
			set
			{
				this._roomId = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "srcUserId", DataFormat = DataFormat.TwosComplement)]
		public int srcUserId
		{
			get
			{
				return this._srcUserId;
			}
			set
			{
				this._srcUserId = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "dstUserId", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int dstUserId
		{
			get
			{
				return this._dstUserId;
			}
			set
			{
				this._dstUserId = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "srcIcon", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string srcIcon
		{
			get
			{
				return this._srcIcon;
			}
			set
			{
				this._srcIcon = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "srcScore", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int srcScore
		{
			get
			{
				return this._srcScore;
			}
			set
			{
				this._srcScore = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "srcName", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string srcName
		{
			get
			{
				return this._srcName;
			}
			set
			{
				this._srcName = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _type;

		private CooperationType _cType;

		private string _mapId = string.Empty;

		private string _roomId;

		private int _srcUserId;

		private int _dstUserId;

		private string _srcIcon = string.Empty;

		private int _srcScore;

		private string _srcName = string.Empty;

		private IExtension extensionObject;
	}
}
