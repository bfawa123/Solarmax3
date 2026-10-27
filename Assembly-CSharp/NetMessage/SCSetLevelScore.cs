using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCSetLevelScore")]
	[Serializable]
	public class SCSetLevelScore : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "upLoad", DataFormat = DataFormat.Default)]
		public bool upLoad
		{
			get
			{
				return this._upLoad;
			}
			set
			{
				this._upLoad = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _level_name;

		private bool _upLoad;

		private IExtension extensionObject;
	}
}
