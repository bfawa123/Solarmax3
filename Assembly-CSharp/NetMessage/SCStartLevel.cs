using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCStartLevel")]
	[Serializable]
	public class SCStartLevel : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "level_name", DataFormat = DataFormat.Default)]
		public string level_name
		{
			get
			{
				return this._level_name;
			}
			set
			{
				this._level_name = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private string _level_name;

		private IExtension extensionObject;
	}
}
