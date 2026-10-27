using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public class UIWindowConfigProvider : Solarmax.Singleton<UIWindowConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/uiwindow.txt";
		}

		public bool IsXML()
		{
			return false;
		}

		public void Load()
		{
			this.mDataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				UIWindowConfig uiwindowConfig = new UIWindowConfig();
				uiwindowConfig.mID = FileReader.ReadInt();
				uiwindowConfig.mName = FileReader.ReadString();
				uiwindowConfig.mPrefabPath = FileReader.ReadString();
				uiwindowConfig.mHideWithDestroy = FileReader.ReadBoolean();
				uiwindowConfig.mUseShowingStack = FileReader.ReadBoolean();
				uiwindowConfig.mOverlapDisplay = FileReader.ReadBoolean();
				uiwindowConfig.mIsGlobalDisplay = FileReader.ReadBoolean();
				this.mDataList.Add(uiwindowConfig.mName, uiwindowConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public UIWindowConfig GetData(string name)
		{
			UIWindowConfig result = null;
			if (!this.mDataList.TryGetValue(name, out result))
			{
				Debug.LogErrorFormat("Can not find ui config of {0}!", new object[]
				{
					name
				});
				return null;
			}
			return result;
		}

		public void LoadExtraData()
		{
		}

		private Dictionary<string, UIWindowConfig> mDataList = new Dictionary<string, UIWindowConfig>();
	}
}
