using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCTaskOk")]
	[Serializable]
	public class SCTaskOk : IExtensible
	{
		[ProtoMember(1, Name = "okTaskIds", DataFormat = DataFormat.Default)]
		public List<string> okTaskIds
		{
			get
			{
				return this._okTaskIds;
			}
		}

		[ProtoMember(2, Name = "failTaskIds", DataFormat = DataFormat.Default)]
		public List<string> failTaskIds
		{
			get
			{
				return this._failTaskIds;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _okTaskIds = new List<string>();

		private readonly List<string> _failTaskIds = new List<string>();

		private IExtension extensionObject;
	}
}
