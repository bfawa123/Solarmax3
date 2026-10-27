using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "ItemChangeInfo")]
	[Serializable]
	public class ItemChangeInfo : IExtensible
	{
		[ProtoMember(1, Name = "items", DataFormat = DataFormat.Default)]
		public List<PackItem> items
		{
			get
			{
				return this._items;
			}
		}

		[ProtoMember(2, Name = "skillids", DataFormat = DataFormat.TwosComplement)]
		public List<int> skillids
		{
			get
			{
				return this._skillids;
			}
		}

		[ProtoMember(3, Name = "add_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> add_num
		{
			get
			{
				return this._add_num;
			}
		}

		[ProtoMember(4, Name = "levelup_num", DataFormat = DataFormat.TwosComplement)]
		public List<int> levelup_num
		{
			get
			{
				return this._levelup_num;
			}
		}

		[ProtoMember(5, Name = "unlock_race", DataFormat = DataFormat.TwosComplement)]
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

		private readonly List<PackItem> _items = new List<PackItem>();

		private readonly List<int> _skillids = new List<int>();

		private readonly List<int> _add_num = new List<int>();

		private readonly List<int> _levelup_num = new List<int>();

		private readonly List<int> _unlock_race = new List<int>();

		private IExtension extensionObject;
	}
}
