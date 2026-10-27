using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCUserInit")]
	[Serializable]
	public class SCUserInit : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "seasonId", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string seasonId
		{
			get
			{
				return this._seasonId;
			}
			set
			{
				this._seasonId = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "ctype", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int ctype
		{
			get
			{
				return this._ctype;
			}
			set
			{
				this._ctype = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "startTime", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int startTime
		{
			get
			{
				return this._startTime;
			}
			set
			{
				this._startTime = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "endTime", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int endTime
		{
			get
			{
				return this._endTime;
			}
			set
			{
				this._endTime = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "lastAdDay", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int lastAdDay
		{
			get
			{
				return this._lastAdDay;
			}
			set
			{
				this._lastAdDay = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "lastAdTimes", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int lastAdTimes
		{
			get
			{
				return this._lastAdTimes;
			}
			set
			{
				this._lastAdTimes = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "pvp_reward_limit", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int pvp_reward_limit
		{
			get
			{
				return this._pvp_reward_limit;
			}
			set
			{
				this._pvp_reward_limit = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "pvp_reward", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int pvp_reward
		{
			get
			{
				return this._pvp_reward;
			}
			set
			{
				this._pvp_reward = value;
			}
		}

		[ProtoMember(9, Name = "top_chapter", DataFormat = DataFormat.Default)]
		public List<string> top_chapter
		{
			get
			{
				return this._top_chapter;
			}
		}

		[ProtoMember(10, Name = "already_evaluated_chapter", DataFormat = DataFormat.Default)]
		public List<string> already_evaluated_chapter
		{
			get
			{
				return this._already_evaluated_chapter;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "next_season_start_time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long next_season_start_time
		{
			get
			{
				return this._next_season_start_time;
			}
			set
			{
				this._next_season_start_time = value;
			}
		}

		[ProtoMember(12, IsRequired = false, Name = "ActivityDegree", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int ActivityDegree
		{
			get
			{
				return this._ActivityDegree;
			}
			set
			{
				this._ActivityDegree = value;
			}
		}

		[ProtoMember(13, IsRequired = false, Name = "online_time_daily", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long online_time_daily
		{
			get
			{
				return this._online_time_daily;
			}
			set
			{
				this._online_time_daily = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _seasonId = string.Empty;

		private int _ctype;

		private int _startTime;

		private int _endTime;

		private int _lastAdDay;

		private int _lastAdTimes;

		private int _pvp_reward_limit;

		private int _pvp_reward;

		private readonly List<string> _top_chapter = new List<string>();

		private readonly List<string> _already_evaluated_chapter = new List<string>();

		private long _next_season_start_time;

		private int _ActivityDegree;

		private long _online_time_daily;

		private IExtension extensionObject;
	}
}
