using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSMapLoad")]
	[Serializable]
	public class CSMapLoad : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("payload=500|to=data")]
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

		[ProtoMember(1, IsRequired = true, Name = "type", DataFormat = DataFormat.TwosComplement)]
		public CSMapLoad.Type type
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

		[ProtoMember(2, IsRequired = true, Name = "begin", DataFormat = DataFormat.TwosComplement)]
		public int begin
		{
			get
			{
				return this._begin;
			}
			set
			{
				this._begin = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "owner", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int owner
		{
			get
			{
				return this._owner;
			}
			set
			{
				this._owner = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "mapid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int mapid
		{
			get
			{
				return this._mapid;
			}
			set
			{
				this._mapid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "payload=500|to=data";

		private CSMapLoad.Type _type;

		private int _begin;

		private int _owner;

		private int _mapid;

		private IExtension extensionObject;

		[ProtoContract(Name = "Type")]
		public enum Type
		{
			[ProtoEnum(Name = "T_Null", Value = 0)]
			T_Null,
			[ProtoEnum(Name = "ByUniq", Value = 1)]
			ByUniq,
			[ProtoEnum(Name = "ByOwner", Value = 2)]
			ByOwner,
			[ProtoEnum(Name = "ByUseCound", Value = 3)]
			ByUseCound
		}
	}
}
