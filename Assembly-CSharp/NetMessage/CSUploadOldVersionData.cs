using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSUploadOldVersionData")]
	[Serializable]
	public class CSUploadOldVersionData : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userId", DataFormat = DataFormat.TwosComplement)]
		public int userId
		{
			get
			{
				return this._userId;
			}
			set
			{
				this._userId = value;
			}
		}

		[ProtoMember(2, Name = "chapters", DataFormat = DataFormat.Default)]
		public List<UploadOldVersionChapter> chapters
		{
			get
			{
				return this._chapters;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "sum_package", DataFormat = DataFormat.TwosComplement)]
		public int sum_package
		{
			get
			{
				return this._sum_package;
			}
			set
			{
				this._sum_package = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "current_package", DataFormat = DataFormat.TwosComplement)]
		public int current_package
		{
			get
			{
				return this._current_package;
			}
			set
			{
				this._current_package = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _userId;

		private readonly List<UploadOldVersionChapter> _chapters = new List<UploadOldVersionChapter>();

		private int _sum_package;

		private int _current_package;

		private IExtension extensionObject;
	}
}
