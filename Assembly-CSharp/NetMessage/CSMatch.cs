using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMatch")]
	[Serializable]
	public class CSMatch : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("url=rand&set|to=match")]
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

		[ProtoMember(1, IsRequired = false, Name = "match_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string match_id
		{
			get
			{
				return this._match_id;
			}
			set
			{
				this._match_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(BattleType.BT_Null)]
		public BattleType type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "url=rand&set|to=match";

		private string _match_id = string.Empty;

		private BattleType _type;

		private IExtension extensionObject;
	}
}
