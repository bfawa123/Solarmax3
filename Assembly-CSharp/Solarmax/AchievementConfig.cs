using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class AchievementConfig
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.difficult = element.GetIntAttribute("difficult", 0);
			this.descs = new List<int>();
			this.outDescs = new List<int>();
			this.types = new List<int>();
			this.miscs = new List<string>();
			this.dicMiscs = new Dictionary<string, List<string>>();
			this.typeToMisc = new Dictionary<int, string>();
			for (int i = 1; i < 4; i++)
			{
				string att = string.Format("desc{0}", i);
				int intAttribute = element.GetIntAttribute(att, 0);
				if (intAttribute == 0)
				{
					break;
				}
				string att2 = string.Format("outDesc{0}", i);
				int intAttribute2 = element.GetIntAttribute(att2, 0);
				string att3 = string.Format("type{0}", i);
				int intAttribute3 = element.GetIntAttribute(att3, 0);
				string att4 = string.Format("misc{0}", i);
				string attribute = element.GetAttribute(att4, string.Empty);
				this.descs.Add(intAttribute);
				this.outDescs.Add(intAttribute2);
				this.types.Add(intAttribute3);
				this.miscs.Add(attribute);
				this.typeToMisc[intAttribute3] = attribute;
				string[] array = attribute.Split(new char[]
				{
					','
				});
				this.dicMiscs[attribute] = new List<string>();
				foreach (string item in array)
				{
					this.dicMiscs[attribute].Add(item);
				}
			}
			this.taskId = element.GetAttribute("taskId", string.Empty);
			this.chapterNameId = element.GetIntAttribute("chapterName", 0);
			this.levelNameId = element.GetIntAttribute("levelName", 0);
			return true;
		}

		public void SetDesc()
		{
			this.chapterDesc = this.GetDesc(false, null);
			this.levelDesc = this.GetDesc(true, null);
		}

		public string GetDesc(bool isLevel, List<int> param = null)
		{
			string text = string.Empty;
			string str = string.Empty;
			string arg = string.Empty;
			int num = this.difficult;
			if (num != 0)
			{
				if (num != 1)
				{
					if (num == 2)
					{
						str = string.Format("[[FFFFFF]{0}[-]]", LanguageDataProvider.GetValue(2106));
						arg = string.Format("[FEB268FF]{0}[-]", LanguageDataProvider.GetValue(2106));
					}
				}
				else
				{
					str = string.Format("[[FFFFFF]{0}[-]]", LanguageDataProvider.GetValue(2105));
					arg = string.Format("[FEB268FF]{0}[-]", LanguageDataProvider.GetValue(2105));
				}
			}
			else
			{
				str = string.Format("[[FFFFFF]{0}[-]]", LanguageDataProvider.GetValue(2104));
				arg = string.Format("[FEB268FF]{0}[-]", LanguageDataProvider.GetValue(2104));
			}
			text += str;
			for (int i = 0; i < this.descs.Count; i++)
			{
				switch (this.types[i])
				{
				case 0:
					text += string.Format(LanguageDataProvider.GetValue(this.descs[i]), arg);
					this.chapterDescWithoutDiff = string.Format(LanguageDataProvider.GetValue(this.descs[i]), arg);
					break;
				case 1:
				case 2:
				case 3:
				case 6:
				case 7:
				case 8:
					if (isLevel)
					{
						text += string.Format(LanguageDataProvider.GetValue(this.descs[i]), this.GetColorStr((param != null) ? param[i].ToString() : "0"), this.GetColorStr(this.miscs[i]));
					}
					else
					{
						text += string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.miscs[i]));
						this.chapterDescWithoutDiff = string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.miscs[i]));
					}
					break;
				case 4:
				case 5:
					text += string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.miscs[i]));
					this.chapterDescWithoutDiff = string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.miscs[i]));
					break;
				case 9:
					if (isLevel)
					{
						text += string.Format(LanguageDataProvider.GetValue(this.descs[i]), this.GetColorStr(this.dicMiscs[this.miscs[i]][0]), this.GetColorStr((param != null) ? param[i].ToString() : "0"), this.GetColorStr(this.dicMiscs[this.miscs[i]][1]));
					}
					else
					{
						text += string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.dicMiscs[this.miscs[i]][0]), this.GetColorStr(this.dicMiscs[this.miscs[i]][1]));
						this.chapterDescWithoutDiff = string.Format(LanguageDataProvider.GetValue(this.outDescs[i]), this.GetColorStr(this.dicMiscs[this.miscs[i]][0]), this.GetColorStr(this.dicMiscs[this.miscs[i]][1]));
					}
					break;
				}
			}
			return text;
		}

		public List<string> GetMisc(int t)
		{
			return this.dicMiscs[this.typeToMisc[t]];
		}

		private string GetColorStr(string str)
		{
			return string.Format("[FEB268FF]{0}[-]", str);
		}

		public string id;

		public int difficult;

		public List<int> descs;

		public List<int> outDescs;

		public List<int> types;

		public List<string> miscs;

		public Dictionary<string, List<string>> dicMiscs;

		public Dictionary<int, string> typeToMisc;

		public string taskId;

		public int chapterNameId;

		public int levelNameId;

		public string chapterDesc;

		public string chapterDescWithoutDiff;

		public string levelDesc;
	}
}
