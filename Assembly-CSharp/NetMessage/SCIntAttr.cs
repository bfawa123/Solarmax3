using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCIntAttr")]
	[Serializable]
	public class SCIntAttr : IExtensible
	{
		[ProtoMember(1, Name = "attr", DataFormat = DataFormat.TwosComplement)]
		public List<IntAttr> attr
		{
			get
			{
				return this._attr;
			}
		}

		[ProtoMember(2, Name = "value", DataFormat = DataFormat.TwosComplement)]
		public List<int> value
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

		private readonly List<IntAttr> _attr = new List<IntAttr>();

		private readonly List<int> _value = new List<int>();

		private IExtension extensionObject;
	}
}
