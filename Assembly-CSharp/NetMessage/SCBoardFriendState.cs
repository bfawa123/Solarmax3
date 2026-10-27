using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCBoardFriendState")]
	[Serializable]
	public class SCBoardFriendState : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "friend_id", DataFormat = DataFormat.TwosComplement)]
		public int friend_id
		{
			get
			{
				return this._friend_id;
			}
			set
			{
				this._friend_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "online", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int online
		{
			get
			{
				return this._online;
			}
			set
			{
				this._online = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "onBattle", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int onBattle
		{
			get
			{
				return this._onBattle;
			}
			set
			{
				this._onBattle = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _friend_id;

		private int _online;

		private int _onBattle;

		private IExtension extensionObject;
	}
}
