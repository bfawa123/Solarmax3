using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBattleReportLoad")]
	[Serializable]
	public class SCBattleReportLoad : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "self", DataFormat = DataFormat.Default)]
		public bool self
		{
			get
			{
				return this._self;
			}
			set
			{
				this._self = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "start", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(3, Name = "report", DataFormat = DataFormat.Default)]
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

		private string _gateway = "to=client";

		private bool _self;

		private int _start;

		private readonly List<BattleReport> _report = new List<BattleReport>();

		private IExtension extensionObject;
	}
}
