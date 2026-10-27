using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLoadChapter")]
	[Serializable]
	public class CSLoadChapter : IExtensible
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _chapter;

		private IExtension extensionObject;
	}
}
