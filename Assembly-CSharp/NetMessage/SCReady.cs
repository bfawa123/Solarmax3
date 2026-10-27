using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCReady")]
	[Serializable]
	public class SCReady : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=set|to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "match_id", DataFormat = DataFormat.Default)]
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

		[ProtoMember(2, Name = "data", DataFormat = DataFormat.Default)]
		public List<UserData> data
		{
			get
			{
				return this._data;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "group", DataFormat = DataFormat.Default)]
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

		[ProtoMember(4, IsRequired = false, Name = "random_seed", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int random_seed
		{
			get
			{
				return this._random_seed;
			}
			set
			{
				this._random_seed = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "battleid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long battleid
		{
			get
			{
				return this._battleid;
			}
			set
			{
				this._battleid = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "misc_id", DataFormat = DataFormat.Default)]
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

		[ProtoMember(8, IsRequired = false, Name = "match_type", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(9, IsRequired = false, Name = "ai_type", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string ai_type
		{
			get
			{
				return this._ai_type;
			}
			set
			{
				this._ai_type = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "ai_param", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int ai_param
		{
			get
			{
				return this._ai_param;
			}
			set
			{
				this._ai_param = value;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "sub_type", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(12, IsRequired = false, Name = "voice_join_token", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string voice_join_token
		{
			get
			{
				return this._voice_join_token;
			}
			set
			{
				this._voice_join_token = value;
			}
		}

		[ProtoMember(13, IsRequired = false, Name = "voice_team_join_token", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string voice_team_join_token
		{
			get
			{
				return this._voice_team_join_token;
			}
			set
			{
				this._voice_team_join_token = value;
			}
		}

		[ProtoMember(14, IsRequired = false, Name = "voice_join_channe_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string voice_join_channe_id
		{
			get
			{
				return this._voice_join_channe_id;
			}
			set
			{
				this._voice_join_channe_id = value;
			}
		}

		[ProtoMember(15, IsRequired = false, Name = "voice_team_join_channe_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string voice_team_join_channe_id
		{
			get
			{
				return this._voice_team_join_channe_id;
			}
			set
			{
				this._voice_team_join_channe_id = value;
			}
		}

		[ProtoMember(16, Name = "Tencent_cloud_token", DataFormat = DataFormat.TwosComplement)]
		public List<int> Tencent_cloud_token
		{
			get
			{
				return this._Tencent_cloud_token;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=set|to=client";

		private string _match_id;

		private readonly List<UserData> _data = new List<UserData>();

		private string _group = string.Empty;

		private int _random_seed;

		private long _battleid;

		private string _misc_id = string.Empty;

		private MatchType _match_type;

		private string _ai_type = string.Empty;

		private int _ai_param;

		private CooperationType _sub_type;

		private string _voice_join_token = string.Empty;

		private string _voice_team_join_token = string.Empty;

		private string _voice_join_channe_id = string.Empty;

		private string _voice_team_join_channe_id = string.Empty;

		private readonly List<int> _Tencent_cloud_token = new List<int>();

		private IExtension extensionObject;
	}
}
