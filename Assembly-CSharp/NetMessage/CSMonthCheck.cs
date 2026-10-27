using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMonthCheck")]
	[Serializable]
	public class CSMonthCheck : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "current_month", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(2, IsRequired = true, Name = "check_id", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(3, IsRequired = false, Name = "multipy", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(4, IsRequired = false, Name = "is_repair", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool is_repair
		{
			get
			{
				return this._is_repair;
			}
			set
			{
				this._is_repair = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _current_month;

		private int _check_id;

		private int _multipy;

		private bool _is_repair;

		private IExtension extensionObject;
	}
}
