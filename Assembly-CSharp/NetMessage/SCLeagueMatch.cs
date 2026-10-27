using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLeagueMatch")]
	[Serializable]
	public class SCLeagueMatch : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "league_id", DataFormat = DataFormat.Default)]
		public string league_id
		{
			get
			{
				return this._league_id;
			}
			set
			{
				this._league_id = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "count_down", DataFormat = DataFormat.TwosComplement)]
		public int count_down
		{
			get
			{
				return this._count_down;
			}
			set
			{
				this._count_down = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
		public ErrCode code
		{
			get
			{
				return this._code;
			}
			set
			{
				this._code = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private string _league_id;

		private int _count_down;

		private ErrCode _code;

		private IExtension extensionObject;
	}
}
