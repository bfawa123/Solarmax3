using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "RechargeData")]
	[Serializable]
	public class RechargeData : IExtensible
	{
		[ProtoMember(1, Name = "first_recharge_mark", DataFormat = DataFormat.Default)]
		public List<string> first_recharge_mark
		{
			get
			{
				return this._first_recharge_mark;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "monthly_card_end_time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long monthly_card_end_time
		{
			get
			{
				return this._monthly_card_end_time;
			}
			set
			{
				this._monthly_card_end_time = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "last_receive_time", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool last_receive_time
		{
			get
			{
				return this._last_receive_time;
			}
			set
			{
				this._last_receive_time = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<string> _first_recharge_mark = new List<string>();

		private long _monthly_card_end_time;

		private bool _last_receive_time;

		private IExtension extensionObject;
	}
}
