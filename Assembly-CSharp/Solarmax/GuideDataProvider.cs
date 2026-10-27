using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Solarmax
{
	public class GuideDataProvider : Solarmax.Singleton<GuideDataProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/playerguide.xml";
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
					XElement xelement = XDocument.Parse(text).Element("playerguides");
					if (xelement != null)
					{
						foreach (XElement element in xelement.Elements("playerguide"))
						{
							CTagGuideConfig ctagGuideConfig = new CTagGuideConfig();
							if (ctagGuideConfig.Load(element))
							{
								this.mDataList.Add(ctagGuideConfig.id, ctagGuideConfig);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("data/playerguide.xml resource failed " + ex.ToString(), new object[0]);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public CTagGuideConfig GetValue(int id)
		{
			CTagGuideConfig result;
			this.mDataList.TryGetValue(id, out result);
			return result;
		}

		public List<CTagGuideConfig> GetGuideConfigByCondition(GuildCondition eCon, string strCondition, int nCurCompltedID)
		{
			List<CTagGuideConfig> list = new List<CTagGuideConfig>();
			int hashCode = strCondition.GetHashCode();
			foreach (CTagGuideConfig ctagGuideConfig in this.mDataList.Values)
			{
				if (eCon != GuildCondition.GC_Level || !Solarmax.Singleton<LevelDataHandler>.Get().IsUnLock(strCondition))
				{
					if (ctagGuideConfig.startCondition == eCon && ctagGuideConfig.windowHashCode == hashCode && ctagGuideConfig.id > nCurCompltedID)
					{
						list.Add(ctagGuideConfig);
					}
				}
			}
			return list;
		}

		public void LoadExtraData()
		{
		}

		private Dictionary<int, CTagGuideConfig> mDataList = new Dictionary<int, CTagGuideConfig>();
	}
}
