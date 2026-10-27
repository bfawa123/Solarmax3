using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSSetCurLevel")]
	[Serializable]
	public class CSSetCurLevel : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "cur_level", DataFormat = DataFormat.Default)]
		public string cur_level
		{
			get
			{
				return this._cur_level;
			}
			set
			{
				this._cur_level = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=data";

		private string _cur_level;

		private IExtension extensionObject;
	}
}
