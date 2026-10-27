using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class SkinConfig
	{
		public bool Load(XElement element)
		{
			this.id = Convert.ToInt32(element.GetAttribute("id", string.Empty));
			this.skinImageName = element.GetAttribute("SkinImageName", string.Empty);
			this.goodValue = Convert.ToDouble(element.GetAttribute("GoldValue", "0"));
			this.bgImage = element.GetAttribute("BGimage", string.Empty);
			this.url = element.GetAttribute("URL", string.Empty);
			this.skinType = element.GetIntAttribute("type", -1);
			if (this.id < 1000)
			{
				this.type = SkinType.BG;
			}
			else if (this.id >= 1000)
			{
				this.type = SkinType.Avatar;
				if (this.skinType == 1)
				{
                    Solarmax.Singleton<AssetManager>.Get().AddEffect(this.bgImage);
				}
			}
			this.unlock = true;
			return true;
		}

		public int id;

		public string skinImageName;

		public double goodValue;

		public bool unlock = true;

		public string bgImage;

		public string url;

		public int skinType;

		public SkinType type;
	}
}
