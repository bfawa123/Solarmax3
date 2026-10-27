using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMallBoxBuy")]
	[Serializable]
	public class SCMallBoxBuy : IExtensible
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

		[ProtoMember(2, IsRequired = true, Name = "boxId", DataFormat = DataFormat.TwosComplement)]
		public int boxId
		{
			get
			{
				return this._boxId;
			}
			set
			{
				this._boxId = value;
			}
		}

		[ProtoMember(3, Name = "items", DataFormat = DataFormat.Default)]
		public List<PackItem> items
		{
			get
			{
				return this._items;
			}
		}

		[ProtoMember(4, Name = "skillids", DataFormat = DataFormat.TwosComplement)]
		public List<int> skillids
		{
			get
			{
				return this._skillids;
			}
		}

		[ProtoMember(5, Name = "add_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> add_num
		{
			get
			{
				return this._add_num;
			}
		}

		[ProtoMember(6, Name = "levelup_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> levelup_num
		{
			get
			{
				return this._levelup_num;
			}
		}

		[ProtoMember(7, Name = "unlock_race", DataFormat = DataFormat.TwosComplement)]
		public List<int> unlock_race
		{
			get
			{
				return this._unlock_race;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private ErrCode _code;

		private int _boxId;

		private readonly List<PackItem> _items = new List<PackItem>();

		private readonly List<int> _skillids = new List<int>();

		private readonly List<int> _add_num = new List<int>();

		private readonly List<int> _levelup_num = new List<int>();

		private readonly List<int> _unlock_race = new List<int>();

		private IExtension extensionObject;
	}
}
