using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLoadChapters")]
	[Serializable]
	public class SCLoadChapters : IExtensible
	{
		[ProtoMember(1, Name = "chapter", DataFormat = DataFormat.Default)]
		public List<string> chapter
		{
			get
			{
				return this._chapter;
			}
		}

		[ProtoMember(2, Name = "star", DataFormat = DataFormat.TwosComplement)]
		public List<int> star
		{
			get
			{
				return this._star;
			}
		}

		[ProtoMember(3, Name = "finish_level_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> finish_level_num
		{
			get
			{
				return this._finish_level_num;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _chapter = new List<string>();

		private readonly List<int> _star = new List<int>();

		private readonly List<int> _finish_level_num = new List<int>();

		private IExtension extensionObject;
	}
}
