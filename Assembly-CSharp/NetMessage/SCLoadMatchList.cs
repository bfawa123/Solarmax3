using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCLoadMatchList")]
	[Serializable]
	public class SCLoadMatchList : IExtensible
	{
		[ProtoMember(1, Name = "room", DataFormat = DataFormat.Default)]
		public List<MatchSynopsis> room
		{
			get
			{
				return this._room;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "page", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int page
		{
			get
			{
				return this._page;
			}
			set
			{
				this._page = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "optype", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int optype
		{
			get
			{
				return this._optype;
			}
			set
			{
				this._optype = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private readonly List<MatchSynopsis> _room = new List<MatchSynopsis>();

		private int _page;

		private int _optype;

		private IExtension extensionObject;
	}
}
