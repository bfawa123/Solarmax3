using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MatchSynopsis")]
	[Serializable]
	public class MatchSynopsis : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(MatchType.MT_Null)]
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

		[ProtoMember(2, IsRequired = false, Name = "c_type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(CooperationType.CT_1v1)]
		public CooperationType c_type
		{
			get
			{
				return this._c_type;
			}
			set
			{
				this._c_type = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "match_id", DataFormat = DataFormat.Default)]
		public string match_id
		{
			get
			{
				return this._match_id;
			}
			set
			{
				this._match_id = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "map_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string map_id
		{
			get
			{
				return this._map_id;
			}
			set
			{
				this._map_id = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "watch_count", DataFormat = DataFormat.TwosComplement)]
		public int watch_count
		{
			get
			{
				return this._watch_count;
			}
			set
			{
				this._watch_count = value;
			}
		}

		[ProtoMember(6, IsRequired = true, Name = "fight_count", DataFormat = DataFormat.TwosComplement)]
		public int fight_count
		{
			get
			{
				return this._fight_count;
			}
			set
			{
				this._fight_count = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "difficulty", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(8, IsRequired = false, Name = "match_name", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string match_name
		{
			get
			{
				return this._match_name;
			}
			set
			{
				this._match_name = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "match_lock", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool match_lock
		{
			get
			{
				return this._match_lock;
			}
			set
			{
				this._match_lock = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _type;

		private CooperationType _c_type;

		private string _match_id;

		private string _map_id = string.Empty;

		private int _watch_count;

		private int _fight_count;

		private int _difficulty;

		private string _match_name = string.Empty;

		private bool _match_lock;

		private IExtension extensionObject;
	}
}
