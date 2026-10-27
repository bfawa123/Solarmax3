using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class AiConfig
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/aistrategy.xml";
		}

		public void Load()
		{
			this.aiStrategy.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("aistrategys");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("aistrategy"))
						{
							AIStrategyConfig aistrategyConfig = new AIStrategyConfig();
							if (aistrategyConfig.Load(element))
							{
								this.aiStrategy.Add(aistrategyConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error(this.Path() + "resource failed !" + ex.ToString(), new object[0]);
			}
		}

		public bool Delete()
		{
			return true;
		}

		public bool Save()
		{
			return true;
		}

		public bool Verify()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public List<AIStrategyConfig> aiStrategy = new List<AIStrategyConfig>();
	}
}
