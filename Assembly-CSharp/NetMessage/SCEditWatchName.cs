using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCEditWatchName")]
	[Serializable]
	public class SCEditWatchName : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "match_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string match_id
		{
			get
			{
				return this._match_id;
			}
			set
			{
				this._match_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "match_name", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string match_name
		{
			get
			{
				return this._match_name;
			}
			set
			{
				this._match_name = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _match_id = string.Empty;

		private string _match_name = string.Empty;

		private IExtension extensionObject;
	}
}
