using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Result")]
	[Serializable]
	public class Result : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "kill_num", DataFormat = DataFormat.TwosComplement)]
		public int kill_num
		{
			get
			{
				return this._kill_num;
			}
			set
			{
				this._kill_num = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "survive_num", DataFormat = DataFormat.TwosComplement)]
		public int survive_num
		{
			get
			{
				return this._survive_num;
			}
			set
			{
				this._survive_num = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _userid;

		private int _kill_num;

		private int _survive_num;

		private IExtension extensionObject;
	}
}
