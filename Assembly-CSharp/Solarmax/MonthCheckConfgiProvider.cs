using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class MonthCheckConfgiProvider : Solarmax.Singleton<MonthCheckConfgiProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/Monthcheck.xml";
		}

		public void Load()
		{
			this.dataList.Clear();
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("monthchecks");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("monthcheck");
						foreach (XElement xelement2 in enumerable)
						{
							MonthCheckConfig monthCheckConfig = new MonthCheckConfig();
							if (monthCheckConfig.Load(xelement2))
							{
								this.dataList.Add(monthCheckConfig.id, monthCheckConfig);
							}
							IEnumerable<XElement> enumerable2 = xelement2.Elements("dayreward");
							foreach (XElement element in enumerable2)
							{
								DayRewardConfig dayRewardConfig = new DayRewardConfig();
								if (dayRewardConfig.Load(element))
								{
									monthCheckConfig.dayRewards[dayRewardConfig.id] = dayRewardConfig;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Monthcheck.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public Dictionary<int, MonthCheckConfig> dataList = new Dictionary<int, MonthCheckConfig>();
	}
}
