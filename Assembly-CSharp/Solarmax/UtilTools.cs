using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Solarmax
{
	public class UtilTools
	{
		public static string GetCallStack()
		{
			StackFrame[] frames = new StackTrace().GetFrames();
			string text = string.Empty;
			foreach (StackFrame stackFrame in frames)
			{
				text += string.Format("File:{0}, Line:{1}, Col:{2}, Method:{3}\r\n", new object[]
				{
					stackFrame.GetFileName(),
					stackFrame.GetFileLineNumber(),
					stackFrame.GetFileColumnNumber(),
					stackFrame.GetMethod().ToString()
				});
			}
			return text;
		}

		public static string BinToHex(byte[] data)
		{
			return UtilTools.BinToHex(data, 0, data.Length);
		}

		public static string BinToHex(byte[] data, int start, int length)
		{
			string empty = string.Empty;
			if (start < 0 || length <= 0 || start + length > data.Length)
			{
				return empty;
			}
			StringBuilder stringBuilder = new StringBuilder(length * 4);
			for (int i = 0; i < length; i++)
			{
				stringBuilder.AppendFormat("{0,2:X2}", data[start + i]);
				if ((i + 1) % 16 == 0)
				{
					stringBuilder.AppendLine();
				}
				else
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public static bool StringIsNullOrEmpty(string str)
		{
			str = str.ToLowerInvariant();
			str = str.Trim();
			return str == null || str == string.Empty || str == "None" || str == "null";
		}

		public static void Xor(ref byte[] buffer, byte[] xor)
		{
			int num = xor.Length;
			int num2 = 0;
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] ^= xor[num2];
				num2 = (num2 + 1) % num;
			}
		}

		public static bool IsZero(float num)
		{
			return num <= float.Epsilon && num >= -1E-45f;
		}

		public static string GetStreamAssetsByPlatform(string subUrl)
		{
			string empty = string.Empty;
			return Application.streamingAssetsPath + subUrl;
		}

		public static string GetStreamAssetsByPlatform()
		{
			string empty = string.Empty;
			return Application.streamingAssetsPath;
		}

		public static float GetAngle360BetweenVector2(Vector2 from, Vector2 to)
		{
			float num;
			if (to.x != from.x)
			{
				num = Mathf.Atan((to.y - from.y) / (to.x - from.x)) * 180f / 3.1415927f;
				if (to.x <= from.x || to.y < from.y)
				{
					if (to.x < from.x && to.y >= from.y)
					{
						num += 180f;
					}
					else if (to.x < from.x && to.y <= from.y)
					{
						num += 180f;
					}
					else if (to.x > from.x && to.y <= from.y)
					{
						num = 360f + num;
					}
				}
			}
			else if (to.y < from.y)
			{
				num = 270f;
			}
			else
			{
				num = 90f;
			}
			return num;
		}
	}
}
