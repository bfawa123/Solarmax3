using System;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class OneChapterPointConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.x = Convert.ToSingle(element.GetAttribute("x", "0.0"));
			this.y = Convert.ToSingle(element.GetAttribute("y", "0.0"));
			this.z = Convert.ToSingle(element.GetAttribute("z", "0.0"));
			this.level = element.GetAttribute("level", "0");
			this.size = Convert.ToSingle(element.GetAttribute("size", "1.0"));
			this.linkPointList = element.GetAttribute("linkPointList", string.Empty).Split(new char[]
			{
				','
			});
			this.greekLetter = element.GetAttribute("greekLetter", string.Empty);
			this.greekPosX = Convert.ToSingle(element.GetAttribute("greekPosX", "0.0"));
			this.greekPosY = Convert.ToSingle(element.GetAttribute("greekPosY", "0.0"));
			this.starName = Convert.ToInt32(element.GetAttribute("starName", "0"));
			this.starNamePosX = Convert.ToSingle(element.GetAttribute("starNamePosX", "0.0"));
			this.starNamePosY = Convert.ToSingle(element.GetAttribute("starNamePosY", "0.0"));
			return true;
		}

		public string id;

		public float x;

		public float y;

		public float z;

		public string level;

		public float size;

		public string[] linkPointList;

		public string greekLetter;

		public float greekPosX;

		public float greekPosY;

		public int starName;

		public float starNamePosX;

		public float starNamePosY;
	}
}
