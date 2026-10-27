using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBuyChapter")]
	[Serializable]
	public class SCBuyChapter : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "chapter", DataFormat = DataFormat.Default)]
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

		private ErrCode _code;

		private string _chapter;

		private IExtension extensionObject;
	}
}
