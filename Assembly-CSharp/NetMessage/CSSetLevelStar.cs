using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSSetLevelStar")]
	[Serializable]
	public class CSSetLevelStar : IExtensible
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

		[ProtoMember(3, IsRequired = false, Name = "score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
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

		private string _level_name;

		private int _star;

		private int _score;

		private IExtension extensionObject;
	}
}
