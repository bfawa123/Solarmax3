using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLoadRank")]
	[Serializable]
	public class SCLoadRank : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "self", DataFormat = DataFormat.TwosComplement)]
		public int self
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

		[ProtoMember(3, Name = "data", DataFormat = DataFormat.Default)]
		public List<UserData> data
		{
			get
			{
				return this._data;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _self;

		private int _start;

		private readonly List<UserData> _data = new List<UserData>();

		private IExtension extensionObject;
	}
}
