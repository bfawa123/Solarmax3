using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class LotteryConfigProvider : Solarmax.Singleton<LotteryConfigProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/Lottery.xml";
		}

		public void Load()
		{
			this.dataDict.Clear();
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("lotterys");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("lottery");
						foreach (XElement element in enumerable)
						{
							LotteryConfig lotteryConfig = new LotteryConfig();
							if (lotteryConfig.Load(element))
							{
								this.dataDict[lotteryConfig.id] = lotteryConfig;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Lottery.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<int, LotteryConfig> dataDict = new Dictionary<int, LotteryConfig>();
	}
}
