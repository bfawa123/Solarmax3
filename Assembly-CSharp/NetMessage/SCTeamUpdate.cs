using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCTeamUpdate")]
	[Serializable]
	public class SCTeamUpdate : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "type", DataFormat = DataFormat.TwosComplement)]
		public BattleType type
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

		[ProtoMember(2, Name = "simUsers", DataFormat = DataFormat.Default)]
		public List<UserData> simUsers
		{
			get
			{
				return this._simUsers;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "version", DataFormat = DataFormat.TwosComplement)]
		public int version
		{
			get
			{
				return this._version;
			}
			set
			{
				this._version = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private BattleType _type;

		private readonly List<UserData> _simUsers = new List<UserData>();

		private int _version;

		private IExtension extensionObject;
	}
}
