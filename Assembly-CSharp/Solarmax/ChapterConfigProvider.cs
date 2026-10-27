using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class ChapterConfigProvider : Solarmax.Singleton<ChapterConfigProvider>, IDataProvider
	{
		public bool IsXML()
		{
			return true;
		}

		public string Path()
		{
			return "data/chapter.xml";
		}

		public void Load()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info("ChapterConfigProvider   Load", new object[0]);
			this.dataList.Clear();
			try
			{
				string text = LoadResManager.LoadCustomTxt(this.Path());
				if (!string.IsNullOrEmpty(text))
				{
					XElement xelement = XDocument.Parse(text).Element("chapters");
					if (xelement != null)
					{
						foreach (XElement xelement2 in xelement.Elements("chapter"))
						{
							ChapterConfig chapterConfig = new ChapterConfig();
							if (chapterConfig.Load(xelement2))
							{
								this.dataList.Add(chapterConfig);
							}
							IEnumerable<XElement> enumerable = xelement2.Elements("line");
							chapterConfig.oneChapterLineConfigs.Clear();
							foreach (XElement element in enumerable)
							{
								OneChapterLineConfig oneChapterLineConfig = new OneChapterLineConfig();
								if (oneChapterLineConfig.Load(element))
								{
									chapterConfig.oneChapterLineConfigs.Add(oneChapterLineConfig);
								}
							}
							IEnumerable<XElement> enumerable2 = xelement2.Elements("point");
							chapterConfig.oneChapterPointConfigs.Clear();
							foreach (XElement element2 in enumerable2)
							{
								OneChapterPointConfig oneChapterPointConfig = new OneChapterPointConfig();
								if (oneChapterPointConfig.Load(element2))
								{
									chapterConfig.oneChapterPointConfigs.Add(oneChapterPointConfig.id, oneChapterPointConfig);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/Chapter.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public List<ChapterConfig> GetAllData()
		{
			return this.dataList;
		}

		public ChapterConfig GetData(string id)
		{
			ChapterConfig result = null;
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

		private List<ChapterConfig> dataList = new List<ChapterConfig>();
	}
}
