using System;
using System.Collections.Generic;
using ProtoBuf;

namespace NetMessage
{
	[ProtoContract(Name = "RaceData")]
	[Serializable]
	public class RaceData : IExtensible
	{
		[ProtoMember(1, IsRequired = true, Name = "race", DataFormat = DataFormat.TwosComplement)]
		public int race
		{
			get
			{
				return this._race;
			}
			set
			{
				this._race = value;
			}
		}

		[ProtoMember(2, IsRequired = true, Name = "level", DataFormat = DataFormat.TwosComplement)]
		public int level
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

		[ProtoMember(3, Name = "skills", DataFormat = DataFormat.TwosComplement)]
		public List<int> skills
		{
			get
			{
				return this._skills;
			}
		}

		[ProtoMember(4, Name = "skill_locks", DataFormat = DataFormat.Default)]
		public List<bool> skill_locks
		{
			get
			{
				return this._skill_locks;
			}
		}

		[ProtoMember(5, IsRequired = true, Name = "race_lock", DataFormat = DataFormat.Default)]
		public bool race_lock
		{
			get
			{
				return this._race_lock;
			}
			set
			{
				this._race_lock = value;
			}
		}

		IExtension IExtensible.GetExtensionObject(bool createIfMissing)
		{
			return Extensible.GetExtensionObject(ref this.extensionObject, createIfMissing);
		}

		private int _race;

		private int _level;

		private readonly List<int> _skills = new List<int>();

		private readonly List<bool> _skill_locks = new List<bool>();

		private bool _race_lock;

		private IExtension extensionObject;
	}
}
