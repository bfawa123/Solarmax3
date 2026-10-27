using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "SCRaceSkillLevelUp")]
	[Serializable]
	public class SCRaceSkillLevelUp : IExtensible
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

		[ProtoMember(98, IsRequired = true, Name = "code", DataFormat = DataFormat.TwosComplement)]
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

		[ProtoMember(1, IsRequired = true, Name = "cur_race", DataFormat = DataFormat.TwosComplement)]
		public int cur_race
		{
			get
			{
				return this._cur_race;
			}
			set
			{
				this._cur_race = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "skill_index", DataFormat = DataFormat.TwosComplement)]
		public int skill_index
		{
			get
			{
				return this._skill_index;
			}
			set
			{
				this._skill_index = value;
			}
		}

		[ProtoMember(3, IsRequired = false, Name = "new_skillid", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int new_skillid
		{
			get
			{
				return this._new_skillid;
			}
			set
			{
				this._new_skillid = value;
			}
		}

		[ProtoMember(4, IsRequired = false, Name = "race_level", DataFormat = DataFormat.TwosComplement)]
		[DefaultValue(0)]
		public int race_level
		{
			get
			{
				return this._race_level;
			}
			set
			{
				this._race_level = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=client";

		private ErrCode _code;

		private int _cur_race;

		private int _skill_index;

		private int _new_skillid;

		private int _race_level;

		private IExtension extensionObject;
	}
}
