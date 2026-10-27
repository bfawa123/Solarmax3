using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class CTagGuideConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = Convert.ToInt32(element.GetAttribute("id", "-1"));
			this.type = (GuideType)Convert.ToInt32(element.GetAttribute("type", "0"));
			this.startCondition = (GuildCondition)Convert.ToInt32(element.GetAttribute("startCondition", "0"));
			this.moveType = (BtnMoveType)Convert.ToInt32(element.GetAttribute("moveType", "0"));
			this.window = element.GetAttribute("windowname", string.Empty);
			string[] array = this.window.Split(new char[]
			{
				','
			});
			this.window = array[0];
			this.windowHashCode = array[0].GetHashCode();
			if (array.Length > 1)
			{
				this.ctrlname = array[1];
			}
			else
			{
				this.ctrlname = string.Empty;
			}
			this.srcX = Convert.ToSingle(element.GetAttribute("srcX", "0"));
			this.srcY = Convert.ToSingle(element.GetAttribute("srcY", "0"));
			this.dstX = Convert.ToSingle(element.GetAttribute("dstX", "0"));
			this.dstY = Convert.ToSingle(element.GetAttribute("dstY", "0"));
			this.angle = Convert.ToSingle(element.GetAttribute("angle", "0"));
			this.duration = Convert.ToSingle(element.GetAttribute("duration", "0"));
			this.coordsinates = (Coordinates)Convert.ToInt32(element.GetAttribute("coordsinates", "0"));
			this.prevID = Convert.ToInt32(element.GetAttribute("prevID", "-1"));
			string[] array2 = element.GetAttribute("nextID", string.Empty).Split(new char[]
			{
				','
			});
			this.nextID = new List<int>();
			for (int i = 0; i < array2.Length; i++)
			{
				if (!string.IsNullOrEmpty(array2[i]))
				{
					this.nextID.Add(int.Parse(array2[i]));
				}
			}
			this.tipID = Convert.ToInt32(element.GetAttribute("tipID", "0"));
			this.endCondition1 = (GuildEndEvent)Convert.ToInt32(element.GetAttribute("endCondition1", "0"));
			this.endCondition2 = (GuildEndEvent)Convert.ToInt32(element.GetAttribute("endCondition2", "0"));
			this.windowType = Convert.ToInt32(element.GetAttribute("windowType", "0"));
			this.endGuide = Convert.ToInt32(element.GetAttribute("endOtherID", "0"));
			this.aniName = element.GetAttribute("animation", string.Empty);
			this.OcclusionBG = Convert.ToInt32(element.GetAttribute("occlusionBG", "0"));
			return true;
		}

		public int id;

		public GuideType type;

		public GuildCondition startCondition;

		public BtnMoveType moveType;

		public int windowHashCode;

		public string window;

		public string ctrlname;

		public float srcX;

		public float srcY;

		public float dstX;

		public float dstY;

		public float angle;

		public float duration;

		public Coordinates coordsinates;

		public int prevID;

		public List<int> nextID;

		public int tipID;

		public GuildEndEvent endCondition1;

		public GuildEndEvent endCondition2;

		public int windowType;

		public int endGuide;

		public string aniName;

		public int OcclusionBG;
	}
}
