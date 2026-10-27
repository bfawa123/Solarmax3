using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleReportLoad")]
	[Serializable]
	public class CSBattleReportLoad : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("payload=500|to=data")]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "payload=500|to=data";

		private bool _self;

		private int _start;

		private IExtension extensionObject;
	}
}
