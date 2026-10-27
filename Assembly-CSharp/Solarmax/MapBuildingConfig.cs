using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameCore.Loader;

namespace Solarmax
{
	public class MapBuildingConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			this.id = element.GetAttribute("id", string.Empty);
			this.type = element.GetAttribute("type", string.Empty);
			this.size = Convert.ToInt32(element.GetAttribute("size", "1"));
			this.x = Convert.ToSingle(element.GetAttribute("x", "0.0"));
			this.y = Convert.ToSingle(element.GetAttribute("y", "0.0"));
			this.camption = Convert.ToInt32(element.GetAttribute("camption", "0"));
			this.tag = element.GetAttribute("tag", string.Empty);
			this.connected = element.GetAttribute("connected", string.Empty);
			this.appear = Convert.ToInt32(element.GetAttribute("appear", "0"));
			string attribute = element.GetAttribute("orbit", "0");
			int num;
			if (int.TryParse(attribute, out num))
			{
				this.orbit = num;
			}
			else if (!string.IsNullOrEmpty(attribute) && attribute.Equals("custom", StringComparison.OrdinalIgnoreCase))
			{
				this.orbit = (int)RevolutionType.RT_Custom;
			}
			else
			{
				try
				{
					this.orbit = (int)((RevolutionType)Enum.Parse(typeof(RevolutionType), attribute, true));
				}
				catch
				{
					this.orbit = 0;
				}
			}
			this.orbitParam1 = element.GetAttribute("orbitParam1", string.Empty);
			this.orbitParam2 = element.GetAttribute("orbitParam2", string.Empty);
			this.orbitParam3 = element.GetAttribute("orbitParam3", string.Empty);
			this.orbitParam4 = element.GetAttribute("orbitParam4", string.Empty);
			this.equation = element.GetAttribute("Equation", string.Empty);
			this.equationX = element.GetAttribute("EquationX", string.Empty);
			this.equationY = element.GetAttribute("EquationY", string.Empty);
			this.orbitXChoose = element.GetAttribute("orbit_x_choose", string.Empty);
			this.orbitTChoose = element.GetAttribute("orbit_t_choose", string.Empty);
			this.orbitBox = element.GetAttribute("orbit_box", string.Empty);
			if (this.orbit == (int)RevolutionType.RT_Custom)
			{
				if (string.IsNullOrEmpty(this.orbitParam1) && !string.IsNullOrEmpty(this.equation))
				{
					this.orbitParam1 = this.equation;
				}
				if (string.IsNullOrEmpty(this.orbitParam1) && !string.IsNullOrEmpty(this.equationX))
				{
					this.orbitParam1 = this.equationX;
				}
				if (string.IsNullOrEmpty(this.orbitParam3) && !string.IsNullOrEmpty(this.equationY))
				{
					this.orbitParam3 = this.equationY;
				}
				if (string.IsNullOrEmpty(this.orbitParam2) && !string.IsNullOrEmpty(this.orbitXChoose))
				{
					this.orbitParam2 = this.orbitXChoose;
				}
				if (string.IsNullOrEmpty(this.orbitParam2) && !string.IsNullOrEmpty(this.orbitTChoose))
				{
					this.orbitParam2 = this.orbitTChoose;
				}
				if (string.IsNullOrEmpty(this.orbitParam2) && !string.IsNullOrEmpty(this.orbitBox))
				{
					this.orbitParam2 = this.orbitBox;
				}
			}
			this.orbitClockWise = Convert.ToBoolean(element.GetAttribute("orbitClockWise", "false"));
			this.fAngle = Convert.ToSingle(element.GetAttribute("fAngle", "0.0"));
			this.lasergunAngle = Convert.ToSingle(element.GetAttribute("lasergunAngle", "0.0"));
			this.lasergunRange = Convert.ToSingle(element.GetAttribute("lasergunRange", "0.0"));
			this.lasergunRotateSkip = Convert.ToSingle(element.GetAttribute("lasergunRotateSkip", "30.0"));
			this.transformBulidingID = element.GetAttribute("transformBulidingID", string.Empty);
			this.curseDelay = Convert.ToSingle(element.GetAttribute("curseDelay", "0.0"));
			this.aistrategy = element.GetAttribute("aistrategy", "-1");
			this.fbpRange = Convert.ToSingle(element.GetAttribute("bpRange", "1.0"));
			this.shipProduceOverride = Convert.ToInt32(element.GetAttribute("shipProduceOverride", "-1"));
			this.orbitRevoSpeed = Convert.ToInt32(element.GetAttribute("orbitRevoSpeed", "12"));
                        this.protectedObjectTags = element.GetAttribute("protectedObject", string.Empty);
                        string attribute2 = element.GetAttribute("Invincible", string.Empty);
                        if (string.IsNullOrEmpty(attribute2))
                        {
                                attribute2 = element.GetAttribute("Invibcible", "0");
                        }
                        this.invincibleMinutes = Convert.ToSingle(string.IsNullOrEmpty(attribute2) ? "0" : attribute2);
			if (this.transformBulidingID != string.Empty)
			{
				foreach (string value in this.transformBulidingID.Split(new char[]
				{
					','
				}))
				{
					this.buildIds.Add(Convert.ToInt32(value));
				}
			}
			return true;
		}

		public string id;

		public string type;

		public int size;

		public float x;

		public float y;

		public int camption;

		public string tag;

		public string connected;

		public int appear;

		public int orbit;

		public string orbitParam1;

		public string orbitParam2;

		public string orbitParam3;

		public string orbitParam4;

		public string equation;

		public string equationX;

		public string equationY;

		public string orbitXChoose;

		public string orbitTChoose;

		public string orbitBox;

		public bool orbitClockWise;

		public float fAngle;

		public float lasergunAngle;

		public float lasergunRange;

		public float lasergunRotateSkip;

		public string transformBulidingID;

		public float curseDelay;

		public string aistrategy;

		public float fbpRange;

		public List<int> buildIds = new List<int>();

		public int shipProduceOverride;

		public int orbitRevoSpeed;

                public string protectedObjectTags;

                public float invincibleMinutes;
	}
}
