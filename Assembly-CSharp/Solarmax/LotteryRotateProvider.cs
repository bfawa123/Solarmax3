using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class LotteryRotateProvider : Solarmax.Singleton<LotteryRotateProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/Lottery_rotate.xml";
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
					XElement xelement = xdocument.Element("lottery_rotates");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("lottery_rotate");
						foreach (XElement element in enumerable)
						{
							LotteryRotateConfig lotteryRotateConfig = new LotteryRotateConfig();
							if (lotteryRotateConfig.Load(element))
							{
								this.dataDict[lotteryRotateConfig.id] = lotteryRotateConfig;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Lottery_rotate.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<int, LotteryRotateConfig> dataDict = new Dictionary<int, LotteryRotateConfig>();
	}
}
