using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MapNodeConfig : ICfgEntry
	{
		public int id { get; set; }

		public string type { get; set; }

		public int typeEnum { get; set; }

		public int aiWeight { get; set; }

		public int aiUnitLost { get; set; }

		public int sizeType { get; set; }

		public float size { get; set; }

		public int hp { get; set; }

		public int food { get; set; }

		public int createshipnum { get; set; }

		public float createship { get; set; }

		public float attackrange { get; set; }

		public float attackspeed { get; set; }

		public float attackpower { get; set; }

		public float nodesize { get; set; }

		public string perfab { get; set; }

		public string skills { get; set; }

		public string imageName { get; set; }

		public int reside { get; set; }

		public bool Load(XElement element)
		{
			this.id = Convert.ToInt32(element.GetAttribute("id", "0"));
			this.type = element.GetAttribute("type", "0");
			this.typeEnum = Convert.ToInt32(element.GetAttribute("typeEnum", "0"));
			this.aiWeight = Convert.ToInt32(element.GetAttribute("aiWeight", "0"));
			this.aiUnitLost = Convert.ToInt32(element.GetAttribute("aiUnitLost", "0"));
			this.sizeType = Convert.ToInt32(element.GetAttribute("sizeType", "0"));
			this.size = Convert.ToSingle(element.GetAttribute("size", "0"));
			this.hp = Convert.ToInt32(element.GetAttribute("hp", "0"));
			this.food = Convert.ToInt32(element.GetAttribute("food", "0"));
			this.createshipnum = Convert.ToInt32(element.GetAttribute("createshipnum", "0"));
			this.createship = Convert.ToSingle(element.GetAttribute("createship", "0"));
			this.attackrange = Convert.ToSingle(element.GetAttribute("attackrange", "0"));
			this.attackspeed = Convert.ToSingle(element.GetAttribute("attackspeed", "0"));
			this.attackpower = Convert.ToSingle(element.GetAttribute("attackpower", "0"));
			this.nodesize = Convert.ToSingle(element.GetAttribute("nodesize", "0"));
			this.perfab = element.GetAttribute("perfab", "NULL");
			this.skills = element.GetAttribute("skills", "NULL");
			this.imageName = Convert.ToString(element.GetAttribute("ImageName", string.Empty));
			this.reside = Convert.ToInt32(element.GetAttribute("Reside", "0"));
			if (this.reside < 0 || this.reside > this.food)
			{
				this.reside = 0;
			}
			this.isShowRange = (element.GetIntAttribute("isShowRange", 0) != 0);
			return true;
		}

		public bool isShowRange;
	}
}
