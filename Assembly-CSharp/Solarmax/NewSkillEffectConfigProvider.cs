using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class NewSkillEffectConfigProvider : Solarmax.Singleton<NewSkillEffectConfigProvider>, IDataProvider
	{
		public string Path()
		{
			return "data/NewSkillEffect.txt";
		}

		public bool IsXML()
		{
			return false;
		}

		public void Load()
		{
			this.dataList.Clear();
			while (!FileReader.IsEnd())
			{
				FileReader.ReadLine();
				NewSkillEffectConfig newSkillEffectConfig = new NewSkillEffectConfig();
				newSkillEffectConfig.effectId = FileReader.ReadInt("effectId");
				newSkillEffectConfig.desc = FileReader.ReadString("desc");
				newSkillEffectConfig.addType = FileReader.ReadInt("addType");
				newSkillEffectConfig.root = FileReader.ReadString("root");
				newSkillEffectConfig.effectName = FileReader.ReadString("effectName");
				newSkillEffectConfig.animationName = FileReader.ReadString("animationName");
				newSkillEffectConfig.audio = FileReader.ReadString("audio");
				newSkillEffectConfig.removeCondition = (float)FileReader.ReadInt("removeCondition");
				this.dataList.Add(newSkillEffectConfig);
			}
		}

		public bool Verify()
		{
			return true;
		}

		public NewSkillEffectConfig GetData(int effectId)
		{
			NewSkillEffectConfig result = null;
			for (int i = 0; i < this.dataList.Count; i++)
			{
				if (this.dataList[i].effectId == effectId)
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

		private List<NewSkillEffectConfig> dataList = new List<NewSkillEffectConfig>();
	}
}
