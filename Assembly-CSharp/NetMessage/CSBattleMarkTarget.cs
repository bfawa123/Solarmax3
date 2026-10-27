using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSBattleMarkTarget")]
	[Serializable]
	public class CSBattleMarkTarget : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "targetNodeTag", DataFormat = DataFormat.Default)]
		public string targetNodeTag
		{
			get
			{
				return this._targetNodeTag;
			}
			set
			{
				this._targetNodeTag = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _targetNodeTag;

		private IExtension extensionObject;
	}
}
