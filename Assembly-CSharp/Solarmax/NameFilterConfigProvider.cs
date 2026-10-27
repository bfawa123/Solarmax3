using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class NameFilterConfigProvider : Solarmax.Singleton<NameFilterConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/NameFilter.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void LoadExtraData()
		{
		}

		public bool Verify()
		{
			return true;
		}

		public List<NameFilterConfig> GetFilterList()
		{
			return this.nameFilterList;
		}

		public void Load()
		{
			this.nameFilterList.Clear();
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("names");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("name");
						foreach (XElement element in enumerable)
						{
							NameFilterConfig nameFilterConfig = new NameFilterConfig();
							if (nameFilterConfig.Load(element))
							{
								this.nameFilterList.Add(nameFilterConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/RandomName.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool nameCheck(string str)
		{
			for (int i = 0; i < this.nameFilterList.Count; i++)
			{
				string desc = this.nameFilterList[i].desc;
				bool flag = str.Contains(desc);
				if (flag)
				{
					return true;
				}
			}
			return false;
		}

		private List<NameFilterConfig> nameFilterList = new List<NameFilterConfig>();
	}
}
