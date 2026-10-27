using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class SkinConfigProvider : Solarmax.Singleton<SkinConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/Skin.Xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			try
			{
				this.bgList.Clear();
				this.avatarList.Clear();
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					IEnumerable<XElement> enumerable = XDocument.Parse(text).Elements("skins");
					if (enumerable != null)
					{
						foreach (XElement element in enumerable.Elements("skin"))
						{
							SkinConfig skinConfig = new SkinConfig();
							if (skinConfig.Load(element))
							{
								SkinType type = skinConfig.type;
								if (type != SkinType.BG)
								{
									if (type == SkinType.Avatar)
									{
										this.avatarList.Add(skinConfig);
										this.avatarDic[skinConfig.skinImageName] = skinConfig;
									}
								}
								else
								{
									this.bgList.Add(skinConfig);
								}
								this.dataList.Add(skinConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Skin.Xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public SkinConfig GetData(int id)
		{
			SkinConfig result = null;
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

		public SkinConfig GetAvatarData(int id)
		{
			SkinConfig result = null;
			for (int i = 0; i < this.avatarList.Count; i++)
			{
				if (this.avatarList[i].id.Equals(id))
				{
					result = this.avatarList[i];
					break;
				}
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		public List<SkinConfig> GetAllData()
		{
			return this.dataList;
		}

		public List<SkinConfig> GetAllBGData()
		{
			return this.bgList;
		}

		public List<SkinConfig> GetAllAvatarData()
		{
			return this.avatarList;
		}

		public List<SkinConfig> dataList = new List<SkinConfig>();

		public List<SkinConfig> bgList = new List<SkinConfig>();

		public List<SkinConfig> avatarList = new List<SkinConfig>();

		public Dictionary<string, SkinConfig> avatarDic = new Dictionary<string, SkinConfig>();
	}
}
