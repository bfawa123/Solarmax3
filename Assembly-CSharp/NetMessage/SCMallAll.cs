using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMallAll")]
	[Serializable]
	public class SCMallAll : IExtensible
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

		[ProtoMember(1, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(2, Name = "box", DataFormat = DataFormat.Default)]
		public List<PbMallBox> box
		{
			get
			{
				return this._box;
			}
		}

		[ProtoMember(3, Name = "gold", DataFormat = DataFormat.Default)]
		public List<PbMallGold> gold
		{
			get
			{
				return this._gold;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private ErrCode _code;

		private readonly List<PbMallBox> _box = new List<PbMallBox>();

		private readonly List<PbMallGold> _gold = new List<PbMallGold>();

		private IExtension extensionObject;
	}
}
