using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "Map")]
	[Serializable]
	public class Map : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "data", DataFormat = DataFormat.Default)]
		public string data
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

		[ProtoMember(2, IsRequired = true, Name = "owner", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(3, IsRequired = true, Name = "mapid", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(4, Name = "mark", DataFormat = DataFormat.TwosComplement)]
		public List<int> mark
		{
			get
			{
				return this._mark;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "usecount", DataFormat = DataFormat.TwosComplement)]
		public int usecount
		{
			get
			{
				return this._usecount;
			}
			set
			{
				this._usecount = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _data;

		private int _owner;

		private int _mapid;

		private readonly List<int> _mark = new List<int>();

		private int _usecount;

		private IExtension extensionObject;
	}
}
