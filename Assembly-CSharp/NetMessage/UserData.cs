using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "UserData")]
	[Serializable]
	public class UserData : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "name", DataFormat = DataFormat.Default)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "icon", DataFormat = DataFormat.Default)]
		public string icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				this._icon = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "score", DataFormat = DataFormat.TwosComplement)]
		public int score
		{
			get
			{
				return this._score;
			}
			set
			{
				this._score = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "level", DataFormat = DataFormat.TwosComplement)]
		public int level
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

		[ProtoMember(6, IsRequired = false, Name = "online", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool online
		{
			get
			{
				return this._online;
			}
			set
			{
				this._online = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "onBattle", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool onBattle
		{
			get
			{
				return this._onBattle;
			}
			set
			{
				this._onBattle = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "score_mod", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int score_mod
		{
			get
			{
				return this._score_mod;
			}
			set
			{
				this._score_mod = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "destroy_num", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int destroy_num
		{
			get
			{
				return this._destroy_num;
			}
			set
			{
				this._destroy_num = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "survive_num", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int survive_num
		{
			get
			{
				return this._survive_num;
			}
			set
			{
				this._survive_num = value;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "battle_race", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public RaceData battle_race
		{
			get
			{
				return this._battle_race;
			}
			set
			{
				this._battle_race = value;
			}
		}

		[ProtoMember(12, IsRequired = false, Name = "pack", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public Pack pack
		{
			get
			{
				return this._pack;
			}
			set
			{
				this._pack = value;
			}
		}

		[ProtoMember(13, IsRequired = false, Name = "chest", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public Chests chest
		{
			get
			{
				return this._chest;
			}
			set
			{
				this._chest = value;
			}
		}

		[ProtoMember(14, Name = "RaceUseCount", DataFormat = DataFormat.TwosComplement)]
		public List<int> RaceUseCount
		{
			get
			{
				return this._RaceUseCount;
			}
		}

		[ProtoMember(15, Name = "RaceWinCount", DataFormat = DataFormat.TwosComplement)]
		public List<int> RaceWinCount
		{
			get
			{
				return this._RaceWinCount;
			}
		}

		[ProtoMember(16, IsRequired = false, Name = "battle_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int battle_count
		{
			get
			{
				return this._battle_count;
			}
			set
			{
				this._battle_count = value;
			}
		}

		[ProtoMember(17, IsRequired = false, Name = "mvp_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int mvp_count
		{
			get
			{
				return this._mvp_count;
			}
			set
			{
				this._mvp_count = value;
			}
		}

		[ProtoMember(18, IsRequired = false, Name = "gold", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				this._gold = value;
			}
		}

		[ProtoMember(19, IsRequired = false, Name = "account_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string account_id
		{
			get
			{
				return this._account_id;
			}
			set
			{
				this._account_id = value;
			}
		}

		[ProtoMember(20, Name = "chapterBuy", DataFormat = DataFormat.Default)]
		public List<string> chapterBuy
		{
			get
			{
				return this._chapterBuy;
			}
		}

		[ProtoMember(21, Name = "skinBuy", DataFormat = DataFormat.TwosComplement)]
		public List<int> skinBuy
		{
			get
			{
				return this._skinBuy;
			}
		}

		[ProtoMember(22, IsRequired = false, Name = "RechargeData", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public RechargeData RechargeData
		{
			get
			{
				return this._RechargeData;
			}
			set
			{
				this._RechargeData = value;
			}
		}

		[ProtoMember(23, Name = "LadderReward", DataFormat = DataFormat.Default)]
		public List<bool> LadderReward
		{
			get
			{
				return this._LadderReward;
			}
		}

		[ProtoMember(24, IsRequired = false, Name = "maxscore", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int maxscore
		{
			get
			{
				return this._maxscore;
			}
			set
			{
				this._maxscore = value;
			}
		}

		[ProtoMember(25, IsRequired = false, Name = "LadderID", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string LadderID
		{
			get
			{
				return this._LadderID;
			}
			set
			{
				this._LadderID = value;
			}
		}

		[ProtoMember(26, IsRequired = false, Name = "data_version", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int data_version
		{
			get
			{
				return this._data_version;
			}
			set
			{
				this._data_version = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _userid;

		private string _name;

		private string _icon;

		private int _score;

		private int _level;

		private bool _online;

		private bool _onBattle;

		private int _score_mod;

		private int _destroy_num;

		private int _survive_num;

		private RaceData _battle_race;

		private Pack _pack;

		private Chests _chest;

		private readonly List<int> _RaceUseCount = new List<int>();

		private readonly List<int> _RaceWinCount = new List<int>();

		private int _battle_count;

		private int _mvp_count;

		private int _gold;

		private string _account_id = string.Empty;

		private readonly List<string> _chapterBuy = new List<string>();

		private readonly List<int> _skinBuy = new List<int>();

		private RechargeData _RechargeData;

		private readonly List<bool> _LadderReward = new List<bool>();

		private int _maxscore;

		private string _LadderID = string.Empty;

		private int _data_version;

		private IExtension extensionObject;
	}
}
