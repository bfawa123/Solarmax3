using System;
using System.Globalization;
using System.Xml.Linq;
using GameCore.Loader;
using UnityEngine;

namespace Solarmax
{
	public class CampConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.speed = Convert.ToSingle(element.GetAttribute("speed", "1.2"));
			this.attack = Convert.ToSingle(element.GetAttribute("attack", "10"));
			this.maxhp = Convert.ToSingle(element.GetAttribute("maxhp", "100"));
			this.capturedspeed = Convert.ToSingle(element.GetAttribute("capturedspeed", "1"));
			this.occupiedspeed = Convert.ToSingle(element.GetAttribute("occupiedspeed", "1"));
			this.producespeed = Convert.ToSingle(element.GetAttribute("producespeed", "1"));
			this.speedaddition = Convert.ToSingle(element.GetAttribute("speedaddition", "0"));
			this.attackaddition = Convert.ToSingle(element.GetAttribute("attackaddition", "0"));
			this.maxhpaddition = Convert.ToSingle(element.GetAttribute("maxhpaddition", "0"));
			this.hidefly = Convert.ToSingle(element.GetAttribute("hidefly", "0"));
			this.hidepop = Convert.ToSingle(element.GetAttribute("hidepop", "0"));
			this.becapturedspeed = Convert.ToSingle(element.GetAttribute("becapturedspeed", "1"));
			this.stopbecapture = Convert.ToSingle(element.GetAttribute("stopbecapture", "0"));
			this.quickmove = Convert.ToSingle(element.GetAttribute("quickmove", "0"));
			string attribute = element.GetAttribute("campcolor", "ffffff");
			try
			{
				if (attribute.Length == 0)
				{
					this.campcolor = new Color32(0, 0, 0, 0);
				}
				else
				{
					this.campcolor = new Color32(byte.Parse(attribute.Substring(0, 2), NumberStyles.AllowHexSpecifier), byte.Parse(attribute.Substring(2, 2), NumberStyles.AllowHexSpecifier), byte.Parse(attribute.Substring(4, 2), NumberStyles.AllowHexSpecifier), 0);
				}
			}
			catch
			{
				this.campcolor = new Color32(0, 0, 0, 0);
			}
			return true;
		}

		public string id;

		public Color32 campcolor;

		public float speed;

		public float attack;

		public float maxhp;

		public float capturedspeed;

		public float occupiedspeed;

		public float producespeed;

		public float speedaddition;

		public float attackaddition;

		public float maxhpaddition;

		public float hidefly;

		public float hidepop;

		public float becapturedspeed;

		public float stopbecapture;

		public float quickmove;
	}
}
