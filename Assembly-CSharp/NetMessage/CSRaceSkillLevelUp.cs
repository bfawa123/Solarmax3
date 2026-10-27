using System;
using System.ComponentModel;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "CSRaceSkillLevelUp")]
	[Serializable]
	public class CSRaceSkillLevelUp : IExtensible
	{
		[ProtoMember(99, IsRequired = false, Name = "gateway", DataFormat = DataFormat.Default)]
		[DefaultValue("to=data")]
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

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private string _gateway = "to=data";

		private int _cur_race;

		private int _skill_index;

		private IExtension extensionObject;
	}
}
