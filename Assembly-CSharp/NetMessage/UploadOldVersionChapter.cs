using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "UploadOldVersionChapter")]
	[Serializable]
	public class UploadOldVersionChapter : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "chapterId", DataFormat = DataFormat.Default)]
		public string chapterId
		{
			get
			{
				return this._chapterId;
			}
			set
			{
				this._chapterId = value;
			}
		}

		[ProtoMember(2, Name = "levels", DataFormat = DataFormat.Default)]
		public List<UploadOldVersionLevel> levels
		{
			get
			{
				return this._levels;
			}
		}

		[ProtoMember(3, Name = "achieves", DataFormat = DataFormat.Default)]
		public List<UploadOldVersionAchieve> achieves
		{
			get
			{
				return this._achieves;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _chapterId;

		private readonly List<UploadOldVersionLevel> _levels = new List<UploadOldVersionLevel>();

		private readonly List<UploadOldVersionAchieve> _achieves = new List<UploadOldVersionAchieve>();

		private IExtension extensionObject;
	}
}
