using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleResume")]
	[Serializable]
	public class CSBattleResume : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=fix|to=battle")]
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

		[ProtoMember(1, IsRequired = true, Name = "startFrameNo", DataFormat = DataFormat.TwosComplement)]
		public int startFrameNo
		{
			get
			{
				return this._startFrameNo;
			}
			set
			{
				this._startFrameNo = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=fix|to=battle";

		private int _startFrameNo;

		private IExtension extensionObject;
	}
}
