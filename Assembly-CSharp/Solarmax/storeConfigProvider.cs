using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class storeConfigProvider : Solarmax.Singleton<storeConfigProvider>, IDataProvider
	{
		public string[] ProductIds { get; private set; }

		public string Path()
		{
			return "data/Store.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			try
			{
				string text = LoadResManager.LoadTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XDocument xdocument = XDocument.Parse(text);
					IEnumerable<XElement> enumerable = xdocument.Elements("stores");
					if (enumerable != null)
					{
						List<string> list = new List<string>();
						IEnumerable<XElement> enumerable2 = enumerable.Elements("store");
						foreach (XElement element in enumerable2)
						{
							StoreConfig storeConfig = new StoreConfig();
							if (storeConfig.Load(element))
							{
								this.dataList.Add(storeConfig);
								if (storeConfig.type == 0)
								{
									this.MAX_SHOW_ADS_NUM++;
								}
								else
								{
									list.Add(storeConfig.id);
								}
							}
						}
						this.ProductIds = list.ToArray();
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Store.Xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public StoreConfig GetData(string id)
		{
			StoreConfig result = null;
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

		public bool SetProductInfoList(SDKProductInfoList list)
		{
			for (int i = 0; i < this.dataList.Count; i++)
			{
				StoreConfig storeConfig = this.dataList[i];
				if (storeConfig.type != 0)
				{
					SDKProductInfo sdkproductInfo = list.FindProduct(storeConfig.id);
					if (sdkproductInfo != null)
					{
						storeConfig.info = sdkproductInfo;
					}
				}
			}
			return true;
		}

		public int MAX_SHOW_ADS_NUM;

		public List<StoreConfig> dataList = new List<StoreConfig>();
	}
}
