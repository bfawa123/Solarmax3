using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCQuitMatch")]
	[Serializable]
	public class SCQuitMatch : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "typ", DataFormat = DataFormat.TwosComplement)]
		public MatchType typ
		{
			get
			{
				return this._typ;
			}
			set
			{
				this._typ = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(3, IsRequired = false, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _typ;

		private ErrCode _code;

		private int _userid;

		private IExtension extensionObject;
	}
}
