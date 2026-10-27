using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSClientStorageSet")]
	[Serializable]
	public class CSClientStorageSet : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=self")]
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

		[ProtoMember(1, Name = "index", DataFormat = DataFormat.TwosComplement)]
		public List<int> index
		{
			get
			{
				return this._index;
			}
		}

		[ProtoMember(2, Name = "value", DataFormat = DataFormat.Default)]
		public List<string> value
		{
			get
			{
				return this._value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=self";

		private readonly List<int> _index = new List<int>();

		private readonly List<string> _value = new List<string>();

		private IExtension extensionObject;
	}
}
