using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCStrAttr")]
	[Serializable]
	public class SCStrAttr : IExtensible
	{
		[ProtoMember(1, Name = "attr", DataFormat = DataFormat.TwosComplement)]
		public List<StrAttr> attr
		{
			get
			{
				return this._attr;
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

		private readonly List<StrAttr> _attr = new List<StrAttr>();

		private readonly List<string> _value = new List<string>();

		private IExtension extensionObject;
	}
}
