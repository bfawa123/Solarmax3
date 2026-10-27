using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Chests")]
	[Serializable]
	public class Chests : IExtensible
	{
		[ProtoMember(1, Name = "items", DataFormat = DataFormat.Default)]
		public List<ChestItem> items
		{
			get
			{
				return this._items;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "chest_gainpoint", DataFormat = DataFormat.TwosComplement)]
		public long chest_gainpoint
		{
			get
			{
				return this._chest_gainpoint;
			}
			set
			{
				this._chest_gainpoint = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "chest_gainconsume", DataFormat = DataFormat.TwosComplement)]
		public long chest_gainconsume
		{
			get
			{
				return this._chest_gainconsume;
			}
			set
			{
				this._chest_gainconsume = value;
			}
		}

		[ProtoMember(4, IsRequired = true, Name = "chest_winnum", DataFormat = DataFormat.TwosComplement)]
		public int chest_winnum
		{
			get
			{
				return this._chest_winnum;
			}
			set
			{
				this._chest_winnum = value;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "chest_neednum", DataFormat = DataFormat.TwosComplement)]
		public int chest_neednum
		{
			get
			{
				return this._chest_neednum;
			}
			set
			{
				this._chest_neednum = value;
			}
		}

		[ProtoMember(6, IsRequired = true, Name = "chest_timeboxid", DataFormat = DataFormat.TwosComplement)]
		public int chest_timeboxid
		{
			get
			{
				return this._chest_timeboxid;
			}
			set
			{
				this._chest_timeboxid = value;
			}
		}

		[ProtoMember(7, IsRequired = true, Name = "chest_winboxid", DataFormat = DataFormat.TwosComplement)]
		public int chest_winboxid
		{
			get
			{
				return this._chest_winboxid;
			}
			set
			{
				this._chest_winboxid = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<ChestItem> _items = new List<ChestItem>();

		private long _chest_gainpoint;

		private long _chest_gainconsume;

		private int _chest_winnum;

		private int _chest_neednum;

		private int _chest_timeboxid;

		private int _chest_winboxid;

		private IExtension extensionObject;
	}
}
