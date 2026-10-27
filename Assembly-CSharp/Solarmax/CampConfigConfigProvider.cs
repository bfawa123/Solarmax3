using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class CampConfigConfigProvider : Solarmax.Singleton<CampConfigConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/camp.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			this.dataList.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("camps");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("camp"))
						{
							CampConfig campConfig = new CampConfig();
							if (campConfig.Load(element))
							{
								this.dataList.Add(campConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Camp.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<CampConfig> GetAllData()
		{
			return this.dataList;
		}

		public CampConfig GetData(string id)
		{
			CampConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].id == id)
				{
					result = this.dataList[i];
					break;
				}
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		private List<CampConfig> dataList = new List<CampConfig>();
	}
}
