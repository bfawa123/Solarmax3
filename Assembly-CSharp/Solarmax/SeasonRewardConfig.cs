using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class SeasonRewardConfig
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			int num = 1;
			for (;;)
			{
				string att = string.Format("ladderScore{0}", num);
				int intAttribute = element.GetIntAttribute(att, -1);
				if (intAttribute == -1)
				{
					break;
				}
				this.ladderScore.Add(num.ToString(), intAttribute);
				att = string.Format("rewardType{0}", num);
				intAttribute = element.GetIntAttribute(att, -1);
				this.rewardType.Add(num.ToString(), intAttribute);
				att = string.Format("misc{0}", num);
				intAttribute = element.GetIntAttribute(att, -1);
				this.misc.Add(num.ToString(), intAttribute);
				att = string.Format("desc{0}", num);
				int intAttribute2 = element.GetIntAttribute(att, -1);
				this.desc.Add(num.ToString(), intAttribute2);
				num++;
			}
			this.count = num - 1;
			return true;
		}

		public string GetDesc(string key)
		{
			if (!this.desc.ContainsKey(key))
			{
				return string.Empty;
			}
			return LanguageDataProvider.GetValue(this.desc[key]);
		}

		public string id;

		public int count;

		public Dictionary<string, int> misc = new Dictionary<string, int>();

		public Dictionary<string, int> rewardType = new Dictionary<string, int>();

		public Dictionary<string, int> ladderScore = new Dictionary<string, int>();

		public Dictionary<string, int> desc = new Dictionary<string, int>();
	}
}
