using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class StoreConfig
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.name = element.GetAttribute("name", string.Empty);
			this.desc = element.GetAttribute("desc", string.Empty);
			this.type = Convert.ToInt32(element.GetAttribute("Types", "0"));
			this.Icon = element.GetAttribute("Icon", string.Empty);
			this.GoldValue = Convert.ToInt32(element.GetAttribute("GoldValue", "0"));
			this.reminbiValue = Convert.ToInt32(element.GetAttribute("RenminbiValue", "0"));
			this.usValue = Convert.ToInt32(element.GetAttribute("USValue", "0"));
			this.indianValue = Convert.ToInt32(element.GetAttribute("IndianValue", "0"));
			this.cd = Convert.ToInt32(element.GetAttribute("CD", "0"));
			return true;
		}

		public float GetPrice()
		{
			if (this.info != null)
			{
				return this.info.feeValue;
			}
			return (float)this.reminbiValue;
		}

		public string GetPriceDesc()
		{
			if (this.info != null)
			{
				string text = Solarmax.Singleton<CultureInfoDict>.Get().FormatCurrency(this.info.feeValue, this.info.currencyType);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return string.Format("{0}￥", this.reminbiValue);
		}

		public string GetCurrencyDesc()
		{
			if (this.info != null)
			{
				return this.info.currencyType;
			}
			return "CNY";
		}

		public string id;

		public string name;

		public string desc;

		public string Icon;

		public int GoldValue;

		public int reminbiValue;

		public int usValue;

		public int indianValue;

		public int cd;

		public int type;

		public SDKProductInfo info;
	}
}
