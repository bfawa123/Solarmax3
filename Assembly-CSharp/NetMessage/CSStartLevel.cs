using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSStartLevel")]
	[Serializable]
	public class CSStartLevel : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "level_name", DataFormat = DataFormat.Default)]
		public string level_name
		{
			get
			{
				return this._level_name;
			}
			set
			{
				this._level_name = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _level_name;

		private IExtension extensionObject;
	}
}
