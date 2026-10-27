using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCResume")]
	[Serializable]
	public class SCResume : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
		public string gateway
		{
			get
			{
				return this._gateway;
			}
			set
			{
				this._gateway = value;
			}
		}

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

		[ProtoMember(2, IsRequired = false, Name = "match", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCMatchInit match
		{
			get
			{
				return this._match;
			}
			set
			{
				this._match = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "start", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCStartSelectRace start
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

		[ProtoMember(4, IsRequired = false, Name = "notify", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public SCSelectRaceNotify notify
		{
			get
			{
				return this._notify;
			}
			set
			{
				this._notify = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "report", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public PbSCFrames report
		{
			get
			{
				return this._report;
			}
			set
			{
				this._report = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "sub_type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(CooperationType.CT_1v1)]
		public CooperationType sub_type
		{
			get
			{
				return this._sub_type;
			}
			set
			{
				this._sub_type = value;
			}
		}

		[ProtoMember(7, IsRequired = false, Name = "quick", DataFormat = DataFormat.Default)]
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

		private string _gateway = "to=client";

		private ErrCode _code;

		private SCMatchInit _match;

		private SCStartSelectRace _start;

		private SCSelectRaceNotify _notify;

		private PbSCFrames _report;

		private CooperationType _sub_type;

		private bool _quick;

		private IExtension extensionObject;
	}
}
