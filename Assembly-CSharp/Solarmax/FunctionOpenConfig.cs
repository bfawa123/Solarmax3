using System;
using System.Xml.Linq;
using GameCore.Loader;
using UnityEngine;

namespace Solarmax
{
	public class FunctionOpenConfig : ICfgEntry
	{
		public bool Load(XElement element)
		{
			int num = Convert.ToInt32(element.GetAttribute("function", "-1"));
			if (num < 0 || num >= 14)
			{
				Debug.LogErrorFormat("Invalid function type {0}!", new object[]
				{
					num
				});
				return false;
			}
			this.functionType = (FunctionType)num;
			this.desc = Convert.ToInt32(element.GetAttribute("desc", string.Empty));
			int num2 = Convert.ToInt32(element.GetAttribute("condition", "-1"));
			if (num2 < 0 || num2 >= 2)
			{
				Debug.LogErrorFormat("Invalid function {0} open condition type {1}!", new object[]
				{
					this.functionType,
					num2
				});
				return false;
			}
			this.conditionType = (FunctionOpenConditionType)num2;
			this.conditionParam0 = element.GetAttribute("misc", "0");
			this.sortId = Convert.ToInt32(this.conditionParam0);
			return true;
		}

		public FunctionType functionType;

		public int desc;

		public FunctionOpenConditionType conditionType;

		public string conditionParam0;

		public int sortId;
	}
}
