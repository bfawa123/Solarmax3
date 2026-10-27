using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCTaskGet")]
	[Serializable]
	public class SCTaskGet : IExtensible
	{
		[ProtoMember(1, Name = "taskIds", DataFormat = DataFormat.Default)]
		public List<string> taskIds
		{
			get
			{
				return this._taskIds;
			}
		}

		[ProtoMember(2, Name = "ts", DataFormat = DataFormat.TwosComplement)]
		public List<int> ts
		{
			get
			{
				return this._ts;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _taskIds = new List<string>();

		private readonly List<int> _ts = new List<int>();

		private IExtension extensionObject;
	}
}
