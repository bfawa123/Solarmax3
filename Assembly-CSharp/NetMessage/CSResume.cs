using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSResume")]
	[Serializable]
	public class CSResume : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=self")]
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

		[ProtoMember(1, IsRequired = false, Name = "startFrameNo", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
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

		private string _gateway = "to=self";

		private int _startFrameNo;

		private IExtension extensionObject;
	}
}
