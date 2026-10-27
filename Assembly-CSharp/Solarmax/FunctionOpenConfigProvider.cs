using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace Solarmax
{
	public class FunctionOpenConfigProvider : Solarmax.Singleton<FunctionOpenConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/functionopen.xml";
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
					XElement xelement = xdocument.Element("lists");
					if (xelement != null)
					{
						IEnumerable<XElement> enumerable = xelement.Elements("list");
						foreach (XElement element in enumerable)
						{
							FunctionOpenConfig functionOpenConfig = new FunctionOpenConfig();
							if (functionOpenConfig.Load(element))
							{
								this.dataList[(int)functionOpenConfig.functionType] = functionOpenConfig;
								this.levelDic[functionOpenConfig.conditionParam0] = true;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error("{0} resource failed {1}", new object[]
				{
					this.Path(),
					ex.ToString()
				});
			}
		}

		public bool Verify()
		{
			for (int i = 0; i < this.dataList.Length; i++)
			{
				if (this.dataList[i] == null)
				{
					return false;
				}
			}
			return true;
		}

		public void LoadExtraData()
		{
		}

		public string GetFunctionUnlockDesc(FunctionType t)
		{
			if (t < FunctionType.PvpLadder || t >= (FunctionType)this.dataList.Length)
			{
				Debug.LogErrorFormat("Invalid FunctionOpenConditionType {0}!", new object[]
				{
					t
				});
				return string.Empty;
			}
			FunctionOpenConfig functionOpenConfig = this.dataList[(int)t];
			return LanguageDataProvider.GetValue(functionOpenConfig.desc);
		}

		public bool CheckFunctionOpen(FunctionType t)
		{
			return true;
		}

		public FunctionOpenConfig GetNextOpenFuction()
		{
			int num = int.MaxValue;
			FunctionOpenConfig result = null;
			for (int i = 0; i < this.dataList.Length; i++)
			{
				FunctionOpenConfig functionOpenConfig = this.dataList[i];
				if (functionOpenConfig.conditionType != FunctionOpenConditionType.None)
				{
					if (!this.CheckFunctionOpen(functionOpenConfig.functionType))
					{
						if (num > functionOpenConfig.sortId)
						{
							num = functionOpenConfig.sortId;
							result = functionOpenConfig;
						}
					}
				}
			}
			return result;
		}

		public Dictionary<string, bool> GetLevelDic()
		{
			return this.levelDic;
		}

		private FunctionOpenConfig[] dataList = new FunctionOpenConfig[14];

		private Dictionary<string, bool> levelDic = new Dictionary<string, bool>();
	}
}
