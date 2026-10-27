using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public class Converter
	{
		public static bool ConvertBool(string data)
		{
			data = data.Trim().ToLowerInvariant();
			return data == "true" || data == "1";
		}

		public static T ConvertNumber<T>(string data)
		{
			T result = default(T);
			try
			{
				result = (T)((object)Convert.ChangeType(data, typeof(T)));
			}
			catch (Exception ex)
			{
				string message = string.Format("ConvertNumber data:{0}, error:{1}, stack:{2}", data, ex.Message, ex.StackTrace);
                Solarmax.Singleton<LoggerSystem>.Instance.Error(message, new object[0]);
			}
			return result;
		}

		public static Vector3 ConvertVector3D(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return Vector3.zero;
			}
			string[] array = data.Split(Converter.cListSplitString, StringSplitOptions.None);
			Vector3 zero = Vector3.zero;
			if (array.Length > 0)
			{
				zero.x = Convert.ToSingle(array[0]);
			}
			if (array.Length > 1)
			{
				zero.y = Convert.ToSingle(array[1]);
			}
			if (array.Length > 2)
			{
				zero.z = Convert.ToSingle(array[2]);
			}
			return zero;
		}

		public static bool CanConvertVector3D(string data)
		{
			bool result = false;
			try
			{
				if (!string.IsNullOrEmpty(data))
				{
					string[] array = data.Split(Converter.cListSplitString, StringSplitOptions.None);
					for (int i = 0; i < array.Length; i++)
					{
						float num = float.Parse(array[i]);
					}
				}
				result = true;
			}
			catch (Exception ex)
			{
			}
			return result;
		}

		public static List<T> ConvertNumberList<T>(string data)
		{
			List<T> list = new List<T>();
			string[] array = data.Split(Converter.cListSplitString, StringSplitOptions.None);
			if (array == null || array.Length == 0)
			{
				return list;
			}
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (!string.IsNullOrEmpty(array[i]))
					{
						list.Add((T)((object)Convert.ChangeType(array[i], typeof(T))));
					}
				}
			}
			catch (Exception ex)
			{
				string message = string.Format("ConvertNumberList data:{0}, error:{1}, stack:{2}", data, ex.Message, ex.StackTrace);
                Solarmax.Singleton<LoggerSystem>.Instance.Error(message, new object[0]);
			}
			return list;
		}

		private static string[] cListSplitString = new string[]
		{
			",",
			" ",
			", ",
			"|"
		};
	}
}
