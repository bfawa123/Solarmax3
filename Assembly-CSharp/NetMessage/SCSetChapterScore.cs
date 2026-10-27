using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCSetChapterScore")]
	[Serializable]
	public class SCSetChapterScore : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "errcode", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _errcode;

		private IExtension extensionObject;
	}
}
