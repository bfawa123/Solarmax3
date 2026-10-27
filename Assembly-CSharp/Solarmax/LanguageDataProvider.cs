using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Solarmax
{
	public class LanguageDataProvider : Solarmax.Singleton<LanguageDataProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/dictionary.xml";
		}

		public bool IsXML()
		{
			return true;
		}

		public void Load()
		{
			this.mDataList.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("languages");
					if (xelement == null)
					{
						return;
					}
					string languageNameConfig = this.GetLanguageNameConfig();
					foreach (XElement element in xelement.Elements("language"))
					{
						LanguageConfig languageConfig = new LanguageConfig();
						if (languageConfig.Load(element, languageNameConfig))
						{
							this.mDataList.Add(languageConfig.mID, languageConfig);
						}
					}
				}
                Solarmax.Singleton<AchievementConfigProvider>.Get().SetDescs();
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/dictionary.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public SystemLanguage GetLanguage()
		{
			SystemLanguage systemLanguage = Application.systemLanguage;
			Debug.LogFormat("GetLanguage: system language is: {0}", new object[]
			{
				systemLanguage
			});
			int localLaugue = Solarmax.Singleton<LocalAccountStorage>.Get().GetLocalLaugue();
			if (localLaugue >= 0)
			{
				systemLanguage = (SystemLanguage)localLaugue;
				Debug.LogFormat("GetLanguage: Use LocalAccountStorage language: {0}", new object[]
				{
					systemLanguage
				});
				return systemLanguage;
			}
            Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage = (int)systemLanguage;
			Debug.LogFormat("GetLanguage: Use system language: {0}", new object[]
			{
				systemLanguage
			});
			return systemLanguage;
		}

		public string GetLanguageNameConfig()
		{
			SystemLanguage language = this.GetLanguage();
			if (language != SystemLanguage.ChineseSimplified)
			{
				if (language == SystemLanguage.ChineseTraditional)
				{
					return "traditionalChinese";
				}
				if (language != SystemLanguage.Chinese)
				{
					return "english";
				}
			}
			return "chinese";
		}

		public bool Verify()
		{
			return true;
		}

		public string GetData(int id)
		{
			LanguageConfig languageConfig = null;
			if (this.mDataList.TryGetValue(id, out languageConfig))
			{
				return languageConfig.mData;
			}
			return string.Empty;
		}

		public static string GetValue(int id)
		{
			return Solarmax.Singleton<LanguageDataProvider>.Instance.GetData(id);
		}

		public static string Format(int id, params object[] args)
		{
			string value = LanguageDataProvider.GetValue(id);
			return string.Format(value, args);
		}

		public void LoadExtraData()
		{
		}

		private Dictionary<int, LanguageConfig> mDataList = new Dictionary<int, LanguageConfig>();
	}
}
