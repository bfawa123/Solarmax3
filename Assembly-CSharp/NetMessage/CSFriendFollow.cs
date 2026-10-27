using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSFriendFollow")]
	[Serializable]
	public class CSFriendFollow : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("payload=100|to=data")]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "payload=100|to=data";

		private int _userid;

		private bool _follow;

		private IExtension extensionObject;
	}
}
