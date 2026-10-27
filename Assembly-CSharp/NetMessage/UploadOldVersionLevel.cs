using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "UploadOldVersionLevel")]
	[Serializable]
	public class UploadOldVersionLevel : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "levelId", DataFormat = DataFormat.Default)]
		public string levelId
		{
			get
			{
				return this._levelId;
			}
			set
			{
				this._levelId = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "star", DataFormat = DataFormat.TwosComplement)]
		public int star
		{
			get
			{
				return this._star;
			}
			set
			{
				this._star = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "score", DataFormat = DataFormat.TwosComplement)]
		public int score
		{
			get
			{
				return this._score;
			}
			set
			{
				this._score = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _levelId;

		private int _star;

		private int _score;

		private IExtension extensionObject;
	}
}
