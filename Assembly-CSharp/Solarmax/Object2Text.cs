using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Solarmax
{
	public class Object2Text
	{
		public static string GenerateText<T>(List<T> list)
		{
			if (list.Count == 0)
			{
				return string.Empty;
			}
			T t = list[0];
			Type type = t.GetType();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("#");
			foreach (PropertyInfo propertyInfo in type.GetProperties())
			{
				stringBuilder.Append(propertyInfo.Name);
				stringBuilder.Append('\t');
			}
			stringBuilder.Append('\r');
			stringBuilder.Append('\n');
			stringBuilder.Append("#");
			foreach (PropertyInfo propertyInfo2 in type.GetProperties())
			{
				stringBuilder.Append(propertyInfo2.PropertyType);
				stringBuilder.Append('\t');
			}
			stringBuilder.Append('\r');
			stringBuilder.Append('\n');
			foreach (T t2 in list)
			{
				foreach (PropertyInfo propertyInfo3 in type.GetProperties())
				{
					stringBuilder.Append(propertyInfo3.GetValue(t2, null));
					stringBuilder.Append('\t');
				}
				stringBuilder.Append('\r');
				stringBuilder.Append('\n');
			}
			return stringBuilder.ToString();
		}
	}
}
