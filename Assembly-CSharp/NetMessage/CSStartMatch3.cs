using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSStartMatch3")]
	[Serializable]
	public class CSStartMatch3 : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=auto|to=match")]
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

		[ProtoMember(1, IsRequired = false, Name = "hasRace", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool hasRace
		{
			get
			{
				return this._hasRace;
			}
			set
			{
				this._hasRace = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=auto|to=match";

		private bool _hasRace;

		private IExtension extensionObject;
	}
}
