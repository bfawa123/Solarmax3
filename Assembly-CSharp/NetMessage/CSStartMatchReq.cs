using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSStartMatchReq")]
	[Serializable]
	public class CSStartMatchReq : IExtensible
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

		[ProtoMember(3, IsRequired = false, Name = "misc_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string misc_id
		{
			get
			{
				return this._misc_id;
			}
			set
			{
				this._misc_id = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "has_race", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool has_race
		{
			get
			{
				return this._has_race;
			}
			set
			{
				this._has_race = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "nPlayerNum", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(6, IsRequired = false, Name = "mapId", DataFormat = DataFormat.Default)]
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

		[ProtoMember(7, IsRequired = false, Name = "quick", DataFormat = DataFormat.Default)]
		[DefaultValue(true)]
		public bool quick
		{
			get
			{
				return this._quick;
			}
			set
			{
				this._quick = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "chapterid", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string chapterid
		{
			get
			{
				return this._chapterid;
			}
			set
			{
				this._chapterid = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "difficulty", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int difficulty
		{
			get
			{
				return this._difficulty;
			}
			set
			{
				this._difficulty = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "bEnterWatch", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool bEnterWatch
		{
			get
			{
				return this._bEnterWatch;
			}
			set
			{
				this._bEnterWatch = value;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "level", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _typ;

		private CooperationType _cType;

		private string _misc_id = string.Empty;

		private bool _has_race;

		private int _nPlayerNum;

		private string _mapId = string.Empty;

		private bool _quick = true;

		private string _chapterid = string.Empty;

		private int _difficulty;

		private bool _bEnterWatch;

		private string _level = string.Empty;

		private IExtension extensionObject;
	}
}
