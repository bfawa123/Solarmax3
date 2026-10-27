using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSChapterScore")]
	[Serializable]
	public class CSChapterScore : IExtensible
	{
		[ProtoMember(1, Name = "chapter_id", DataFormat = DataFormat.Default)]
		public List<string> chapter_id
		{
			get
			{
				return this._chapter_id;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _chapter_id = new List<string>();

		private IExtension extensionObject;
	}
}
