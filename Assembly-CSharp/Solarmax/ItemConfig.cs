using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class ItemConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = Convert.ToInt32(element.GetAttribute("id", "-1"));
			this.name = Convert.ToInt32(element.GetAttribute("name", "0"));
			this.icon = element.GetAttribute("icon", string.Empty);
			this.desc = Convert.ToInt32(element.GetAttribute("desc", "0"));
			this.maxRepeat = Convert.ToInt32(element.GetAttribute("maxRepeat", "1"));
			this.Deprice = Convert.ToInt32(element.GetAttribute("Deprice", "1"));
			this.Coprice = Convert.ToInt32(element.GetAttribute("Coprice", "1"));
			this.func = (ITEMFUNCTION)Convert.ToInt32(element.GetAttribute("func", "0"));
			this.resultType = (ProductType)Convert.ToInt32(element.GetAttribute("resultType", "0"));
			this.resultID = Convert.ToInt32(element.GetAttribute("resultID", "-1"));
			this.needCount = Convert.ToInt32(element.GetAttribute("needCount", "1"));
			return true;
		}

		public int id;

		public int name;

		public string icon = string.Empty;

		public int desc;

		public int maxRepeat = 1;

		public int Deprice = 1;

		public int Coprice = 1;

		public ITEMFUNCTION func;

		public ProductType resultType;

		public int resultID;

		public int needCount = 1;
	}
}
