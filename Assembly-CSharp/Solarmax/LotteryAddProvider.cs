using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class LotteryAddProvider : Solarmax.Singleton<LotteryAddProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/Lottery_add.xml";
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
					XElement xelement = xdocument.Element("lotteery_adds");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("lottery_add");
						foreach (XElement element in enumerable)
						{
							LotteryAddConfig lotteryAddConfig = new LotteryAddConfig();
							if (lotteryAddConfig.Load(element))
							{
								this.dataDict[lotteryAddConfig.id] = lotteryAddConfig;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Lottery_add.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<int, LotteryAddConfig> dataDict = new Dictionary<int, LotteryAddConfig>();
	}
}
