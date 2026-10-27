using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "UploadOldVersionAchieve")]
	[Serializable]
	public class UploadOldVersionAchieve : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "levelGroupId", DataFormat = DataFormat.Default)]
		public string levelGroupId
		{
			get
			{
				return this._levelGroupId;
			}
			set
			{
				this._levelGroupId = value;
			}
		}

		[ProtoMember(2, Name = "achieveId", DataFormat = DataFormat.Default)]
		public List<string> achieveId
		{
			get
			{
				return this._achieveId;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _levelGroupId;

		private readonly List<string> _achieveId = new List<string>();

		private IExtension extensionObject;
	}
}
