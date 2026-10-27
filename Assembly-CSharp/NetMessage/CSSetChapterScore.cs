using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSSetChapterScore")]
	[Serializable]
	public class CSSetChapterScore : IExtensible
	{
		[ProtoMember(1, IsRequired = false, Name = "chapter_id", DataFormat = DataFormat.Default)]
		[DefaultValue("")]
		public string chapter_id
		{
			get
			{
				return this._chapter_id;
			}
			set
			{
				this._chapter_id = value;
			}
		}

		[ProtoMember(2, IsRequired = false, Name = "strategy_score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int strategy_score
		{
			get
			{
				return this._strategy_score;
			}
			set
			{
				this._strategy_score = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "interest_score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int interest_score
		{
			get
			{
				return this._interest_score;
			}
			set
			{
				this._interest_score = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "total_score", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int total_score
		{
			get
			{
				return this._total_score;
			}
			set
			{
				this._total_score = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "multipy", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int multipy
		{
			get
			{
				return this._multipy;
			}
			set
			{
				this._multipy = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _chapter_id = string.Empty;

		private int _strategy_score;

		private int _interest_score;

		private int _total_score;

		private int _multipy;

		private IExtension extensionObject;
	}
}
