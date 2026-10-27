using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSTeamInviteResp")]
	[Serializable]
	public class CSTeamInviteResp : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=match")]
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

		[ProtoMember(1, IsRequired = true, Name = "leaderId", DataFormat = DataFormat.TwosComplement)]
		public int leaderId
		{
			get
			{
				return this._leaderId;
			}
			set
			{
				this._leaderId = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "timestamp", DataFormat = DataFormat.TwosComplement)]
		public int timestamp
		{
			get
			{
				return this._timestamp;
			}
			set
			{
				this._timestamp = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "accept", DataFormat = DataFormat.Default)]
		[DefaultValue(false)]
		public bool accept
		{
			get
			{
				return this._accept;
			}
			set
			{
				this._accept = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=match";

		private int _leaderId;

		private int _timestamp;

		private bool _accept;

		private IExtension extensionObject;
	}
}
