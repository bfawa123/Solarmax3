using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCGetUserData")]
	[Serializable]
	public class SCGetUserData : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("auth=1|set=userid|to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "errcode", DataFormat = DataFormat.TwosComplement)]
		public ErrCode errcode
		{
			get
			{
				return this._errcode;
			}
			set
			{
				this._errcode = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "data", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public UserData data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "now", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int now
		{
			get
			{
				return this._now;
			}
			set
			{
				this._now = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "cur_level", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string cur_level
		{
			get
			{
				return this._cur_level;
			}
			set
			{
				this._cur_level = value;
			}
		}

		[ProtoMember(5, Name = "race", DataFormat = DataFormat.Default)]
		public List<RaceData> race
		{
			get
			{
				return this._race;
			}
		}

		[ProtoMember(6, Name = "ad_channel", DataFormat = DataFormat.Default)]
		public List<AdConfig> ad_channel
		{
			get
			{
				return this._ad_channel;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "voice_token", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string voice_token
		{
			get
			{
				return this._voice_token;
			}
			set
			{
				this._voice_token = value;
			}
		}

		[ProtoMember(8, IsRequired = false, Name = "tencent_appid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long tencent_appid
		{
			get
			{
				return this._tencent_appid;
			}
			set
			{
				this._tencent_appid = value;
			}
		}

		[ProtoMember(9, IsRequired = false, Name = "online_time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long online_time
		{
			get
			{
				return this._online_time;
			}
			set
			{
				this._online_time = value;
			}
		}

		[ProtoMember(10, IsRequired = false, Name = "offline_time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long offline_time
		{
			get
			{
				return this._offline_time;
			}
			set
			{
				this._offline_time = value;
			}
		}

		[ProtoMember(11, IsRequired = false, Name = "total_star", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int total_star
		{
			get
			{
				return this._total_star;
			}
			set
			{
				this._total_star = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "auth=1|set=userid|to=client";

		private ErrCode _errcode;

		private UserData _data;

		private int _now;

		private string _cur_level = string.Empty;

		private readonly List<RaceData> _race = new List<RaceData>();

		private readonly List<AdConfig> _ad_channel = new List<AdConfig>();

		private string _voice_token = string.Empty;

		private long _tencent_appid;

		private long _online_time;

		private long _offline_time;

		private int _total_star;

		private IExtension extensionObject;
	}
}
