using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleReportPlay")]
	[Serializable]
	public class CSBattleReportPlay : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("payload=900|to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "battleid", DataFormat = DataFormat.TwosComplement)]
		public long battleid
		{
			get
			{
				return this._battleid;
			}
			set
			{
				this._battleid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "payload=900|to=data";

		private long _battleid;

		private IExtension extensionObject;
	}
}
