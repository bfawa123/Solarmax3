using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCChangeLadderScore")]
	[Serializable]
	public class SCChangeLadderScore : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "code", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(ErrCode.EC_Null)]
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

		[ProtoMember(2, IsRequired = false, Name = "ladder_score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int ladder_score
		{
			get
			{
				return this._ladder_score;
			}
			set
			{
				this._ladder_score = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _ladder_score;

		private IExtension extensionObject;
	}
}
