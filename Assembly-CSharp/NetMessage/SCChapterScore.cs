using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCChapterScore")]
	[Serializable]
	public class SCChapterScore : IExtensible
	{
		[ProtoMember(1, Name = "chapter", DataFormat = DataFormat.Default)]
		public List<chapterScore> chapter
		{
			get
			{
				return this._chapter;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<chapterScore> _chapter = new List<chapterScore>();

		private IExtension extensionObject;
	}
}
