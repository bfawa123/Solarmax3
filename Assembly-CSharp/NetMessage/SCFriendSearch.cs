using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCFriendSearch")]
	[Serializable]
	public class SCFriendSearch : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=client")]
		public string gateway
		{
			get
			{
				return this._gateway;
			}
			set
			{
				this._gateway = value;
			}
		}

		[ProtoMember(1, IsRequired = false, Name = "data", DataFormat = DataFormat.Default)]
		[DefaultValue(null)]
		public UserData data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "followed", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool followed
		{
			get
			{
				return this._followed;
			}
			set
			{
				this._followed = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "following_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int following_count
		{
			get
			{
				return this._following_count;
			}
			set
			{
				this._following_count = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "followers_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int followers_count
		{
			get
			{
				return this._followers_count;
			}
			set
			{
				this._followers_count = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "star_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int star_count
		{
			get
			{
				return this._star_count;
			}
			set
			{
				this._star_count = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "challenge_count", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int challenge_count
		{
			get
			{
				return this._challenge_count;
			}
			set
			{
				this._challenge_count = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private UserData _data;

		private bool _followed;

		private int _following_count;

		private int _followers_count;

		private int _star_count;

		private int _challenge_count;

		private IExtension extensionObject;
	}
}
