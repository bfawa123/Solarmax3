using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "EndEvent")]
	[Serializable]
	public class EndEvent : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "userid", DataFormat = DataFormat.TwosComplement)]
		public int userid
		{
			get
			{
				return this._userid;
			}
			set
			{
				this._userid = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "end_type", DataFormat = DataFormat.TwosComplement)]
		public EndType end_type
		{
			get
			{
				return this._end_type;
			}
			set
			{
				this._end_type = value;
			}
		}

		[ProtoMember(3, IsRequired = true, Name = "end_frame", DataFormat = DataFormat.TwosComplement)]
		public int end_frame
		{
			get
			{
				return this._end_frame;
			}
			set
			{
				this._end_frame = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "end_destroy", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int end_destroy
		{
			get
			{
				return this._end_destroy;
			}
			set
			{
				this._end_destroy = value;
			}
		}

		[ProtoMember(5, IsRequired = false, Name = "end_survive", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int end_survive
		{
			get
			{
				return this._end_survive;
			}
			set
			{
				this._end_survive = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _userid;

		private EndType _end_type;

		private int _end_frame;

		private int _end_destroy;

		private int _end_survive;

		private IExtension extensionObject;
	}
}
