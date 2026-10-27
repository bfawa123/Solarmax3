using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSLoadMatchList")]
	[Serializable]
	public class CSLoadMatchList : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "c_type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(CooperationType.CT_1v1)]
		public CooperationType c_type
		{
			get
			{
				return this._c_type;
			}
			set
			{
				this._c_type = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "chapter", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string chapter
		{
			get
			{
				return this._chapter;
			}
			set
			{
				this._chapter = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "level", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string level
		{
			get
			{
				return this._level;
			}
			set
			{
				this._level = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "difficulty", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int difficulty
		{
			get
			{
				return this._difficulty;
			}
			set
			{
				this._difficulty = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "start", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int start
		{
			get
			{
				return this._start;
			}
			set
			{
				this._start = value;
			}
		}

		[ProtoMember(6, IsRequired = false, Name = "type", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int type
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

		[ProtoMember(7, IsRequired = false, Name = "optype", DataFormat = DataFormat.TwosComplement)]
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

		private CooperationType _c_type;

		private string _chapter = string.Empty;

		private string _level = string.Empty;

		private int _difficulty;

		private int _start;

		private int _type;

		private int _optype;

		private IExtension extensionObject;
	}
}
