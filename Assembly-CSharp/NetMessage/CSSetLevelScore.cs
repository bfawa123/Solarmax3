using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSSetLevelScore")]
	[Serializable]
	public class CSSetLevelScore : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "group_id", DataFormat = DataFormat.Default)]
		public string group_id
		{
			get
			{
				return this._group_id;
			}
			set
			{
				this._group_id = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "accountid", DataFormat = DataFormat.Default)]
		public string accountid
		{
			get
			{
				return this._accountid;
			}
			set
			{
				this._accountid = value;
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _level_name;

		private string _group_id;

		private string _accountid;

		private int _score;

		private IExtension extensionObject;
	}
}
