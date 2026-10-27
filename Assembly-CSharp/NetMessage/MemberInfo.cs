using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "MemberInfo")]
	[Serializable]
	public class MemberInfo : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
		public int id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "name", DataFormat = DataFormat.Default)]
		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "icon", DataFormat = DataFormat.Default)]
		public string icon
		{
			get
			{
				return this._icon;
			}
			set
			{
				this._icon = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "score", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(5, IsRequired = true, Name = "mvp", DataFormat = DataFormat.TwosComplement)]
		public int mvp
		{
			get
			{
				return this._mvp;
			}
			set
			{
				this._mvp = value;
			}
		}

		[ProtoMember(6, IsRequired = true, Name = "battle_num", DataFormat = DataFormat.TwosComplement)]
		public int battle_num
		{
			get
			{
				return this._battle_num;
			}
			set
			{
				this._battle_num = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _id;

		private string _name;

		private string _icon;

		private int _score;

		private int _mvp;

		private int _battle_num;

		private IExtension extensionObject;
	}
}
