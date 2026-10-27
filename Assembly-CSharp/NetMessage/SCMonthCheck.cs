using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMonthCheck")]
	[Serializable]
	public class SCMonthCheck : IExtensible
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

		[ProtoMember(2, IsRequired = false, Name = "current_month", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int current_month
		{
			get
			{
				return this._current_month;
			}
			set
			{
				this._current_month = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "check_id", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int check_id
		{
			get
			{
				return this._check_id;
			}
			set
			{
				this._check_id = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "multipy", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int multipy
		{
			get
			{
				return this._multipy;
			}
			set
			{
				this._multipy = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "check_time", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0L)]
		public long check_time
		{
			get
			{
				return this._check_time;
			}
			set
			{
				this._check_time = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "repair_check_num", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int repair_check_num
		{
			get
			{
				return this._repair_check_num;
			}
			set
			{
				this._repair_check_num = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private ErrCode _code;

		private int _current_month;

		private int _check_id;

		private int _multipy;

		private long _check_time;

		private int _repair_check_num;

		private IExtension extensionObject;
	}
}
