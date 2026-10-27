using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PbAchievement")]
	[Serializable]
	public class PbAchievement : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "lvGroupId", DataFormat = DataFormat.Default)]
		public string lvGroupId
		{
			get
			{
				return this._lvGroupId;
			}
			set
			{
				this._lvGroupId = value;
			}
		}

		[ProtoMember(2, Name = "achieveIds", DataFormat = DataFormat.Default)]
		public List<string> achieveIds
		{
			get
			{
				return this._achieveIds;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _lvGroupId;

		private readonly List<string> _achieveIds = new List<string>();

		private IExtension extensionObject;
	}
}
