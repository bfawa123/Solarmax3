using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCEditWatchLock")]
	[Serializable]
	public class SCEditWatchLock : IExtensible
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

		[ProtoMember(2, IsRequired = false, Name = "match_lock", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool match_lock
		{
			get
			{
				return this._match_lock;
			}
			set
			{
				this._match_lock = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _match_id = string.Empty;

		private bool _match_lock;

		private IExtension extensionObject;
	}
}
