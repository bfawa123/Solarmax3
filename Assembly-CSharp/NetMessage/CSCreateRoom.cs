using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSCreateRoom")]
	[Serializable]
	public class CSCreateRoom : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "matchid", DataFormat = DataFormat.Default)]
		public string matchid
		{
			get
			{
				return this._matchid;
			}
			set
			{
				this._matchid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=auto|to=match";

		private string _matchid;

		private IExtension extensionObject;
	}
}
