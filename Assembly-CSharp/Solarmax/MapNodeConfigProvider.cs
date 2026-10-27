using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class MapNodeConfigProvider : Solarmax.Singleton<MapNodeConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/mapnode.xml";
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
					XElement xelement = XDocument.Parse(text).Element("mapNodes");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("node"))
						{
							MapNodeConfig mapNodeConfig = new MapNodeConfig();
							if (mapNodeConfig.Load(element))
							{
								this.dataList.Add(mapNodeConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/mapnode.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<MapNodeConfig> GetAllData()
		{
			return this.dataList;
		}

		public MapNodeConfig GetData(string type, int sizeType)
		{
			MapNodeConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].type.Equals(type) && this.dataList[i].sizeType.Equals(sizeType))
				{
					return this.dataList[i];
				}
			}
			return result;
		}

		public MapNodeConfig GetData(string type)
		{
			MapNodeConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].type.Equals(type))
				{
					return this.dataList[i];
				}
			}
			return result;
		}

		public MapNodeConfig GetData(int id)
		{
			MapNodeConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].id == id)
				{
					result = this.dataList[i];
				}
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		private List<MapNodeConfig> dataList = new List<MapNodeConfig>();
	}
}
