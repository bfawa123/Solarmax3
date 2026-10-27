using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace GameCore.Loader
{
	public static class XmlLoader
	{
		public static string GetAttribute(this XElement em, string att, string def = null)
		{
			XAttribute xattribute = em.Attribute(att);
			if (xattribute != null)
			{
				return xattribute.Value;
			}
			if (def != null)
			{
				return def;
			}
			string message = string.Format("em:{0}  att:{1} not found", em, att);
			throw new KeyNotFoundException(message);
		}

		public static bool HasAttribute(this XElement em, string att)
		{
			return em.Attribute(att) != null;
		}

		public static int[] GetIntArray(this XElement em, string att, char separator = ',')
		{
			string attribute = em.GetAttribute(att, string.Empty);
			if (string.IsNullOrEmpty(attribute))
			{
				return new int[0];
			}
			string[] array = attribute.Split(new char[]
			{
				separator
			});
			List<int> list = new List<int>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					int item;
					if (int.TryParse(array[i], out item))
					{
						list.Add(item);
					}
				}
			}
			return list.ToArray();
		}

		public static int GetIntAttribute(this XElement em, string att, int def = 0)
		{
			XAttribute xattribute = em.Attribute(att);
			int result = def;
			if (xattribute == null || !int.TryParse(xattribute.Value, out result))
			{
			}
			return result;
		}

		public static float GetFloatAttribute(this XElement em, string att, float def = 0f)
		{
			XAttribute xattribute = em.Attribute(att);
			float result = def;
			if (xattribute == null || !float.TryParse(xattribute.Value, out result))
			{
			}
			return result;
		}

		public static string[] GetStringArray(this XElement em, string att, char separator = ',')
		{
			string attribute = em.GetAttribute(att, string.Empty);
			if (string.IsNullOrEmpty(attribute))
			{
				return new string[0];
			}
			string[] array = attribute.Split(new char[]
			{
				separator
			});
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					list.Add(array[i]);
				}
			}
			return list.ToArray();
		}
	}
}
