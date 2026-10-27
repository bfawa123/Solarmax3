using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "LeagueInfo")]
	[Serializable]
	public class LeagueInfo : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "id", DataFormat = DataFormat.Default)]
		public string id
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

		[ProtoMember(2, IsRequired = true, Name = "max_num", DataFormat = DataFormat.TwosComplement)]
		public int max_num
		{
			get
			{
				return this._max_num;
			}
			set
			{
				this._max_num = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "cur_num", DataFormat = DataFormat.TwosComplement)]
		public int cur_num
		{
			get
			{
				return this._cur_num;
			}
			set
			{
				this._cur_num = value;
			}
		}

		[ProtoMember(6, IsRequired = true, Name = "desc", DataFormat = DataFormat.Default)]
		public string desc
		{
			get
			{
				return this._desc;
			}
			set
			{
				this._desc = value;
			}
		}

		[ProtoMember(7, IsRequired = true, Name = "signup_start", DataFormat = DataFormat.TwosComplement)]
		public int signup_start
		{
			get
			{
				return this._signup_start;
			}
			set
			{
				this._signup_start = value;
			}
		}

		[ProtoMember(8, IsRequired = true, Name = "signup_finish", DataFormat = DataFormat.TwosComplement)]
		public int signup_finish
		{
			get
			{
				return this._signup_finish;
			}
			set
			{
				this._signup_finish = value;
			}
		}

		[ProtoMember(9, IsRequired = true, Name = "league_start", DataFormat = DataFormat.TwosComplement)]
		public int league_start
		{
			get
			{
				return this._league_start;
			}
			set
			{
				this._league_start = value;
			}
		}

		[ProtoMember(10, IsRequired = true, Name = "league_finish", DataFormat = DataFormat.TwosComplement)]
		public int league_finish
		{
			get
			{
				return this._league_finish;
			}
			set
			{
				this._league_finish = value;
			}
		}

		[ProtoMember(11, Name = "days", DataFormat = DataFormat.TwosComplement)]
		public List<int> days
		{
			get
			{
				return this._days;
			}
		}

		[ProtoMember(12, IsRequired = true, Name = "combat_start", DataFormat = DataFormat.TwosComplement)]
		public int combat_start
		{
			get
			{
				return this._combat_start;
			}
			set
			{
				this._combat_start = value;
			}
		}

		[ProtoMember(13, IsRequired = true, Name = "combat_finish", DataFormat = DataFormat.TwosComplement)]
		public int combat_finish
		{
			get
			{
				return this._combat_finish;
			}
			set
			{
				this._combat_finish = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _id;

		private int _max_num;

		private int _cur_num;

		private string _desc;

		private int _signup_start;

		private int _signup_finish;

		private int _league_start;

		private int _league_finish;

		private readonly List<int> _days = new List<int>();

		private int _combat_start;

		private int _combat_finish;

		private IExtension extensionObject;
	}
}
