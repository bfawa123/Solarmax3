using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class NewSkillConfig
	{
		public int skillId;

		public string name;

		public string desc;

		public string icon;

		public string iconDisable;

		public string iconFull;

		public int type;

		public int cost;

		public int logicId;

		public string buffs;

		public float firstcd;

		public bool reuse;

		public float cd;

		public int effectId;

		public string tips;

		public int tipsdir;

		public bool tipsallshow;

		public List<int> buffIds;
	}
}
