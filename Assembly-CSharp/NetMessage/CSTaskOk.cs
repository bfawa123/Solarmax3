using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSTaskOk")]
	[Serializable]
	public class CSTaskOk : IExtensible
	{
		[ProtoMember(1, Name = "taskIds", DataFormat = DataFormat.Default)]
		public List<string> taskIds
		{
			get
			{
				return this._taskIds;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "multipy", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int multipy
		{
			get
			{
				return this._multipy;
			}
			set
			{
				this._multipy = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _taskIds = new List<string>();

		private int _multipy;

		private IExtension extensionObject;
	}
}
