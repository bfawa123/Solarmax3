using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLeagueAdd")]
	[Serializable]
	public class SCLeagueAdd : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
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

		[ProtoMember(1, IsRequired = true, Name = "league_id", DataFormat = DataFormat.Default)]
		public string league_id
		{
			get
			{
				return this._league_id;
			}
			set
			{
				this._league_id = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "league", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public LeagueInfo league
		{
			get
			{
				return this._league;
			}
			set
			{
				this._league = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "self", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public MemberInfo self
		{
			get
			{
				return this._self;
			}
			set
			{
				this._self = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private string _league_id;

		private ErrCode _code;

		private LeagueInfo _league;

		private MemberInfo _self;

		private IExtension extensionObject;
	}
}
