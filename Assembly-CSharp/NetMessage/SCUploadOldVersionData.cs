using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCUploadOldVersionData")]
	[Serializable]
	public class SCUploadOldVersionData : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "errcode", DataFormat = DataFormat.TwosComplement)]
		public ErrCode errcode
		{
			get
			{
				return this._errcode;
			}
			set
			{
				this._errcode = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "data", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public UserData data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _errcode;

		private UserData _data;

		private IExtension extensionObject;
	}
}
