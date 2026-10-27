using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleCastList")]
	[Serializable]
	public class CSBattleCastList : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "start", DataFormat = DataFormat.TwosComplement)]
		public int start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _start;

		private IExtension extensionObject;
	}
}
