using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSAchievementLoad")]
	[Serializable]
	public class CSAchievementLoad : IExtensible
	{
		[ProtoMember(1, Name = "lvGroupIds", DataFormat = DataFormat.Default)]
		public List<string> lvGroupIds
		{
			get
			{
				return this._lvGroupIds;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _lvGroupIds = new List<string>();

		private IExtension extensionObject;
	}
}
