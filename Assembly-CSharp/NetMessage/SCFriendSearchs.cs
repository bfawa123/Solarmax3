using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCFriendSearchs")]
	[Serializable]
	public class SCFriendSearchs : IExtensible
	{
		[ProtoMember(1, Name = "datas", DataFormat = DataFormat.Default)]
		public List<SCFriendSearch> datas
		{
			get
			{
				return this._datas;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "ext", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int ext
		{
			get
			{
				return this._ext;
			}
			set
			{
				this._ext = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<SCFriendSearch> _datas = new List<SCFriendSearch>();

		private int _ext;

		private IExtension extensionObject;
	}
}
