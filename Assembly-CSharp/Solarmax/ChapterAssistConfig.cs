using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class ChapterAssistConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", "0");
			this.playerNum = Convert.ToInt32(element.GetAttribute("playerNum", "0"));
			this.name = Convert.ToInt32(element.GetAttribute("name", "0"));
			this.posX = Convert.ToSingle(element.GetAttribute("posX", "0"));
			this.posY = Convert.ToSingle(element.GetAttribute("posY", "0"));
			this.dependChapter = element.GetAttribute("chapter", string.Empty);
			this.playerControl = element.GetAttribute("player_control", string.Empty);
			this.level1 = element.GetAttribute("levelId1", string.Empty);
			this.reward1 = Convert.ToInt32(element.GetAttribute("rewardType1", "0"));
			this.value1 = element.GetAttribute("reward1", "0");
			this.level2 = element.GetAttribute("levelId2", string.Empty);
			this.reward2 = Convert.ToInt32(element.GetAttribute("rewardType2", "0"));
			this.value2 = element.GetAttribute("reward2", "0");
			this.level3 = element.GetAttribute("levelId3", string.Empty);
			this.reward3 = Convert.ToInt32(element.GetAttribute("rewardType3", "0"));
			this.value3 = element.GetAttribute("reward3", "0");
			this.level4 = element.GetAttribute("levelId4", string.Empty);
			this.reward4 = Convert.ToInt32(element.GetAttribute("rewardType4", "0"));
			this.value4 = element.GetAttribute("reward4", "0");
			return true;
		}

		public string id;

		public int name;

		public int playerNum;

		public float posX;

		public float posY;

		public string dependChapter;

		public string playerControl;

		public string level1;

		public int reward1;

		public string value1;

		public string level2;

		public int reward2;

		public string value2;

		public string level3;

		public int reward3;

		public string value3;

		public string level4;

		public int reward4;

		public string value4;
	}
}
