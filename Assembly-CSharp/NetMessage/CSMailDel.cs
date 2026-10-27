using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMailDel")]
	[Serializable]
	public class CSMailDel : IExtensible
	{
		[ProtoMember(1, Name = "mailid", DataFormat = DataFormat.TwosComplement)]
		public List<int> mailid
		{
			get
			{
				return this._mailid;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<int> _mailid = new List<int>();

		private IExtension extensionObject;
	}
}
