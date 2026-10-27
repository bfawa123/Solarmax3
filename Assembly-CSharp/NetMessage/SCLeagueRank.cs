using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLeagueRank")]
	[Serializable]
	public class SCLeagueRank : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "start", DataFormat = DataFormat.TwosComplement)]
		public int start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		[ProtoMember(3, Name = "members", DataFormat = DataFormat.Default)]
		public List<MemberInfo> members
		{
			get
			{
				return this._members;
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

		[ProtoMember(5, IsRequired = false, Name = "self_rank", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int self_rank
		{
			get
			{
				return this._self_rank;
			}
			set
			{
				this._self_rank = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private string _league_id;

		private int _start;

		private readonly List<MemberInfo> _members = new List<MemberInfo>();

		private MemberInfo _self;

		private int _self_rank;

		private IExtension extensionObject;
	}
}
