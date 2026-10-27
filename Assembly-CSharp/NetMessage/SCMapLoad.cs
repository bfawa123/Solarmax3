using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMapLoad")]
	[Serializable]
	public class SCMapLoad : IExtensible
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

		[ProtoMember(1, Name = "map", DataFormat = DataFormat.Default)]
		public List<Map> map
		{
			get
			{
				return this._map;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "begin", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int begin
		{
			get
			{
				return this._begin;
			}
			set
			{
				this._begin = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private readonly List<Map> _map = new List<Map>();

		private int _begin;

		private IExtension extensionObject;
	}
}
