using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatch3Notify")]
	[Serializable]
	public class SCMatch3Notify : IExtensible
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

		[ProtoMember(1, Name = "user", DataFormat = DataFormat.Default)]
		public List<UserData> user
		{
			get
			{
				return this._user;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "matchid", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string matchid
		{
			get
			{
				return this._matchid;
			}
			set
			{
				this._matchid = value;
			}
		}

		[ProtoMember(3, Name = "useridx", DataFormat = DataFormat.TwosComplement)]
		public List<int> useridx
		{
			get
			{
				return this._useridx;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private readonly List<UserData> _user = new List<UserData>();

		private string _matchid = string.Empty;

		private readonly List<int> _useridx = new List<int>();

		private IExtension extensionObject;
	}
}
