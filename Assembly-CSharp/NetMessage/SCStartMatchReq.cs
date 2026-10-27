using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCStartMatchReq")]
	[Serializable]
	public class SCStartMatchReq : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "cType", DataFormat = DataFormat.TwosComplement)]
		public CooperationType cType
		{
			get
			{
				return this._cType;
			}
			set
			{
				this._cType = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(4, IsRequired = false, Name = "quick", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool quick
		{
			get
			{
				return this._quick;
			}
			set
			{
				this._quick = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _typ;

		private CooperationType _cType;

		private ErrCode _code;

		private bool _quick;

		private IExtension extensionObject;
	}
}
