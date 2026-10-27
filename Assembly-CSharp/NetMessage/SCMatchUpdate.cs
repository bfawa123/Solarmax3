using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCMatchUpdate")]
	[Serializable]
	public class SCMatchUpdate : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "typ", DataFormat = DataFormat.TwosComplement)]
		public MatchType typ
		{
			get
			{
				return this._typ;
			}
			set
			{
				this._typ = value;
			}
		}

		[ProtoMember(2, Name = "user_added", DataFormat = DataFormat.Default)]
		public List<UserData> user_added
		{
			get
			{
				return this._user_added;
			}
		}

		[ProtoMember(3, Name = "index_added", DataFormat = DataFormat.TwosComplement)]
		public List<int> index_added
		{
			get
			{
				return this._index_added;
			}
		}

		[ProtoMember(4, Name = "index_deled", DataFormat = DataFormat.TwosComplement)]
		public List<int> index_deled
		{
			get
			{
				return this._index_deled;
			}
		}

		[ProtoMember(6, Name = "kick", DataFormat = DataFormat.Default)]
		public List<bool> kick
		{
			get
			{
				return this._kick;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "masterid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int masterid
		{
			get
			{
				return this._masterid;
			}
			set
			{
				this._masterid = value;
			}
		}

		[ProtoMember(7, Name = "change_from", DataFormat = DataFormat.TwosComplement)]
		public List<int> change_from
		{
			get
			{
				return this._change_from;
			}
		}

		[ProtoMember(8, Name = "change_to", DataFormat = DataFormat.TwosComplement)]
		public List<int> change_to
		{
			get
			{
				return this._change_to;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private MatchType _typ;

		private readonly List<UserData> _user_added = new List<UserData>();

		private readonly List<int> _index_added = new List<int>();

		private readonly List<int> _index_deled = new List<int>();

		private readonly List<bool> _kick = new List<bool>();

		private int _masterid;

		private readonly List<int> _change_from = new List<int>();

		private readonly List<int> _change_to = new List<int>();

		private IExtension extensionObject;
	}
}
