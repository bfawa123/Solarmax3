using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBattleMarkTarget")]
	[Serializable]
	public class SCBattleMarkTarget : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userId", DataFormat = DataFormat.TwosComplement)]
		public int userId
		{
			get
			{
				return this._userId;
			}
			set
			{
				this._userId = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "targetNodeTag", DataFormat = DataFormat.Default)]
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

		private int _userId;

		private string _targetNodeTag;

		private IExtension extensionObject;
	}
}
