using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBattleCastList")]
	[Serializable]
	public class SCBattleCastList : IExtensible
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

		[ProtoMember(2, Name = "report", DataFormat = DataFormat.Default)]
		public List<BattleReport> report
		{
			get
			{
				return this._report;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _start;

		private readonly List<BattleReport> _report = new List<BattleReport>();

		private IExtension extensionObject;
	}
}
