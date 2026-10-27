using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class SeasonRewardProvider : Solarmax.Singleton<SeasonRewardProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/Reward.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					IEnumerable<XElement> enumerable = xdocument.Elements("rewards");
					if (enumerable != null)
					{
						IEnumerable<XElement> enumerable2 = enumerable.Elements("reward");
						foreach (XElement element in enumerable2)
						{
							SeasonRewardConfig seasonRewardConfig = new SeasonRewardConfig();
							if (seasonRewardConfig.Load(element))
							{
								this.dataList.Add(seasonRewardConfig.id, seasonRewardConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Reward.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<string, SeasonRewardConfig> dataList = new Dictionary<string, SeasonRewardConfig>();
	}
}
