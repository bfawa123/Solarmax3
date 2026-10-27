using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCChangeMonthlyCard")]
	[Serializable]
	public class SCChangeMonthlyCard : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "monthly_card_end_time", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(2, IsRequired = false, Name = "last_receive_time", DataFormat = DataFormat.Default)]
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

		[ProtoMember(3, IsRequired = false, Name = "id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "orderID_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string orderID_id
		{
			get
			{
				return this._orderID_id;
			}
			set
			{
				this._orderID_id = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private long _monthly_card_end_time;

		private bool _last_receive_time;

		private string _id = string.Empty;

		private string _orderID_id = string.Empty;

		private IExtension extensionObject;
	}
}
