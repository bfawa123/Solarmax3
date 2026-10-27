using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLoadChapter")]
	[Serializable]
	public class SCLoadChapter : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "chapter", DataFormat = DataFormat.Default)]
		public string chapter
		{
			get
			{
				return this._chapter;
			}
			set
			{
				this._chapter = value;
			}
		}

		[ProtoMember(2, Name = "level", DataFormat = DataFormat.Default)]
		public List<string> level
		{
			get
			{
				return this._level;
			}
		}

		[ProtoMember(3, Name = "star", DataFormat = DataFormat.TwosComplement)]
		public List<int> star
		{
			get
			{
				return this._star;
			}
		}

		[ProtoMember(4, Name = "score", DataFormat = DataFormat.TwosComplement)]
		public List<int> score
		{
			get
			{
				return this._score;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _chapter;

		private readonly List<string> _level = new List<string>();

		private readonly List<int> _star = new List<int>();

		private readonly List<int> _score = new List<int>();

		private IExtension extensionObject;
	}
}
