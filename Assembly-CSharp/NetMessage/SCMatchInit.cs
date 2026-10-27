using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchInit")]
	[Serializable]
	public class SCMatchInit : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "typ", DataFormat = DataFormat.TwosComplement)]
		public MatchType typ
		{
			get
			{
				return this._typ;
			}
			set
			{
				this._typ = value;
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

		[ProtoMember(3, IsRequired = false, Name = "miscid", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string miscid
		{
			get
			{
				return this._miscid;
			}
			set
			{
				this._miscid = value;
			}
		}

		[ProtoMember(4, Name = "user", DataFormat = DataFormat.Default)]
		public List<UserData> user
		{
			get
			{
				return this._user;
			}
		}

		[ProtoMember(5, Name = "useridx", DataFormat = DataFormat.TwosComplement)]
		public List<int> useridx
		{
			get
			{
				return this._useridx;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "masterid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int masterid
		{
			get
			{
				return this._masterid;
			}
			set
			{
				this._masterid = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "countdown", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int countdown
		{
			get
			{
				return this._countdown;
			}
			set
			{
				this._countdown = value;
			}
		}

		[ProtoMember(8, IsRequired = true, Name = "nPlayerNum", DataFormat = DataFormat.TwosComplement)]
		public int nPlayerNum
		{
			get
			{
				return this._nPlayerNum;
			}
			set
			{
				this._nPlayerNum = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "bLock", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool bLock
		{
			get
			{
				return this._bLock;
			}
			set
			{
				this._bLock = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "roomName", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string roomName
		{
			get
			{
				return this._roomName;
			}
			set
			{
				this._roomName = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _typ;

		private string _matchid;

		private string _miscid = string.Empty;

		private readonly List<UserData> _user = new List<UserData>();

		private readonly List<int> _useridx = new List<int>();

		private int _masterid;

		private int _countdown;

		private int _nPlayerNum;

		private bool _bLock;

		private string _roomName = string.Empty;

		private IExtension extensionObject;
	}
}
