using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCDiscountChapter")]
	[Serializable]
	public class SCDiscountChapter : IExtensible
	{
		[ProtoMember(1, Name = "chapter", DataFormat = DataFormat.Default)]
		public List<discountChapter> chapter
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

		private readonly List<discountChapter> _chapter = new List<discountChapter>();

		private IExtension extensionObject;
	}
}
