using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class ItemConfigProvider : Solarmax.Singleton<ItemConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/item.xml";
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
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					XElement xelement = xdocument.Element("items");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("item");
						foreach (XElement element in enumerable)
						{
							ItemConfig itemConfig = new ItemConfig();
							if (itemConfig.Load(element))
							{
								this.dataList.Add(itemConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/item.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<ItemConfig> GetAllData()
		{
			return this.dataList;
		}

		public ItemConfig GetData(int id)
		{
			ItemConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].id.Equals(id))
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

		private List<ItemConfig> dataList = new List<ItemConfig>();
	}
}
