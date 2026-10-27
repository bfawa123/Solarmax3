using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "discountChapter")]
	[Serializable]
	public class discountChapter : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "chapter_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string chapter_id
		{
			get
			{
				return this._chapter_id;
			}
			set
			{
				this._chapter_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "gold", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int gold
		{
			get
			{
				return this._gold;
			}
			set
			{
				this._gold = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _chapter_id = string.Empty;

		private int _gold;

		private IExtension extensionObject;
	}
}
