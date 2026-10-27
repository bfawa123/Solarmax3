using System;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "PveRankReport")]
	[Serializable]
	public class PveRankReport : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "accountid", DataFormat = DataFormat.Default)]
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

		[ProtoMember(2, IsRequired = true, Name = "playerName", DataFormat = DataFormat.Default)]
		public string playerName
		{
			get
			{
				return this._playerName;
			}
			set
			{
				this._playerName = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "playerIcon", DataFormat = DataFormat.Default)]
		public string playerIcon
		{
			get
			{
				return this._playerIcon;
			}
			set
			{
				this._playerIcon = value;
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

		private string _accountid;

		private string _playerName;

		private string _playerIcon;

		private int _score;

		private IExtension extensionObject;
	}
}
