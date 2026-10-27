using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "BattleReport")]
	[Serializable]
	public class BattleReport : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
		public long id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		[ProtoMember(2, Name = "data", DataFormat = DataFormat.Default)]
		public List<UserData> data
		{
			get
			{
				return this._data;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "play_count", DataFormat = DataFormat.TwosComplement)]
		public int play_count
		{
			get
			{
				return this._play_count;
			}
			set
			{
				this._play_count = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "group", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string group
		{
			get
			{
				return this._group;
			}
			set
			{
				this._group = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "match_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
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

		[ProtoMember(7, IsRequired = false, Name = "time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long time
		{
			get
			{
				return this._time;
			}
			set
			{
				this._time = value;
			}
		}

		[ProtoMember(8, Name = "rank", DataFormat = DataFormat.TwosComplement)]
		public List<int> rank
		{
			get
			{
				return this._rank;
			}
		}

		[ProtoMember(9, Name = "end_type", DataFormat = DataFormat.TwosComplement)]
		public List<EndType> end_type
		{
			get
			{
				return this._end_type;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "sub_type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(CooperationType.CT_1v1)]
		public CooperationType sub_type
		{
			get
			{
				return this._sub_type;
			}
			set
			{
				this._sub_type = value;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "match_type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(MatchType.MT_Null)]
		public MatchType match_type
		{
			get
			{
				return this._match_type;
			}
			set
			{
				this._match_type = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private long _id;

		private readonly List<UserData> _data = new List<UserData>();

		private int _play_count;

		private string _group = string.Empty;

		private string _match_id = string.Empty;

		private long _time;

		private readonly List<int> _rank = new List<int>();

		private readonly List<EndType> _end_type = new List<EndType>();

		private CooperationType _sub_type;

		private MatchType _match_type;

		private IExtension extensionObject;
	}
}
