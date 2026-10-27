using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class NameConfigProvider : Solarmax.Singleton<NameConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/RandomName.xml";
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

		public List<NameConfig> GetFirstNameList()
		{
			return this.firstNameList;
		}

		public List<NameConfig> GetLastNameList()
		{
			return this.lastNameList;
		}

		public void Load()
		{
			this.firstNameList.Clear();
			this.lastNameList.Clear();
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("randNames");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("randName");
						foreach (XElement element in enumerable)
						{
							NameConfig nameConfig = new NameConfig();
							if (nameConfig.Load(element))
							{
								if (nameConfig.type == 0)
								{
									this.firstNameList.Add(nameConfig);
								}
								else
								{
									this.lastNameList.Add(nameConfig);
								}
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

		private List<NameConfig> firstNameList = new List<NameConfig>();

		private List<NameConfig> lastNameList = new List<NameConfig>();
	}
}
