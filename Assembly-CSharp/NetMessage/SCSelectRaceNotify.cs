using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCSelectRaceNotify")]
	[Serializable]
	public class SCSelectRaceNotify : IExtensible
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

		[ProtoMember(1, Name = "user", DataFormat = DataFormat.TwosComplement)]
		public List<int> user
		{
			get
			{
				return this._user;
			}
		}

		[ProtoMember(2, Name = "race", DataFormat = DataFormat.TwosComplement)]
		public List<int> race
		{
			get
			{
				return this._race;
			}
		}

		[ProtoMember(3, Name = "ok", DataFormat = DataFormat.Default)]
		public List<bool> ok
		{
			get
			{
				return this._ok;
			}
		}

		[ProtoMember(4, Name = "useridx", DataFormat = DataFormat.TwosComplement)]
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

		private readonly List<int> _user = new List<int>();

		private readonly List<int> _race = new List<int>();

		private readonly List<bool> _ok = new List<bool>();

		private readonly List<int> _useridx = new List<int>();

		private IExtension extensionObject;
	}
}
