using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class AIStrategyConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = Convert.ToInt32(element.GetAttribute("id", "1"));
			this.name = element.GetAttribute("name", string.Empty);
			this.actions = element.GetAttribute("actions", string.Empty);
			this.desc = element.GetAttribute("desc", string.Empty);
			if (this.actions != string.Empty)
			{
				string[] array = this.actions.Split(new char[]
				{
					','
				});
				foreach (string value in array)
				{
					this.aiActions.Add(Convert.ToInt32(value));
				}
			}
			return true;
		}

		public int id;

		public string name;

		public string desc;

		public string actions;

		public List<int> aiActions = new List<int>();
	}
}
