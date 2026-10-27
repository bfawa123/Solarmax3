using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCQuitBattle")]
	[Serializable]
	public class SCQuitBattle : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=del|to=client")]
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

		[ProtoMember(2, Name = "users", DataFormat = DataFormat.TwosComplement)]
		public List<int> users
		{
			get
			{
				return this._users;
			}
		}

		[ProtoMember(3, Name = "score_mods", DataFormat = DataFormat.TwosComplement)]
		public List<int> score_mods
		{
			get
			{
				return this._score_mods;
			}
		}

		[ProtoMember(4, Name = "rewards", DataFormat = DataFormat.Default)]
		public List<PackItem> rewards
		{
			get
			{
				return this._rewards;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "chest", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public ChestItem chest
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

		[ProtoMember(6, IsRequired = false, Name = "drop_code", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
		public ErrCode drop_code
		{
			get
			{
				return this._drop_code;
			}
			set
			{
				this._drop_code = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=del|to=client";

		private readonly List<int> _users = new List<int>();

		private readonly List<int> _score_mods = new List<int>();

		private readonly List<PackItem> _rewards = new List<PackItem>();

		private ChestItem _chest;

		private ErrCode _drop_code;

		private IExtension extensionObject;
	}
}
