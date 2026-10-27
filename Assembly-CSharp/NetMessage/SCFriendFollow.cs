using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCFriendFollow")]
	[Serializable]
	public class SCFriendFollow : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "follow", DataFormat = DataFormat.Default)]
		public bool follow
		{
			get
			{
				return this._follow;
			}
			set
			{
				this._follow = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "data", DataFormat = DataFormat.Default)]
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

		[ProtoMember(4, IsRequired = true, Name = "err", DataFormat = DataFormat.TwosComplement)]
		public ErrCode err
		{
			get
			{
				return this._err;
			}
			set
			{
				this._err = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private int _userid;

		private bool _follow;

		private UserData _data;

		private ErrCode _err;

		private IExtension extensionObject;
	}
}
