using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSChangeLadderScore")]
	[Serializable]
	public class CSChangeLadderScore : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "change_score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int change_score
		{
			get
			{
				return this._change_score;
			}
			set
			{
				this._change_score = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _change_score;

		private IExtension extensionObject;
	}
}
