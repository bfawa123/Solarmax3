using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public class FileReader
	{
		private static void _reset()
		{
			FileReader._line_ptr = 0;
			FileReader._line_array.Clear();
			FileReader.lines_temp.Clear();
			FileReader._element_ptr = 0;
			FileReader._element_array = null;
			FileReader._element_title_array = null;
			FileReader._element_dict.Clear();
		}

		private static string _next_element()
		{
			return FileReader._element_array[FileReader._element_ptr++];
		}

		public static bool LoadPath(string filePath)
		{
			if (!string.IsNullOrEmpty(filePath))
			{
				string text = string.Empty;
				try
				{
					text = LoadResManager.LoadTxt(filePath);
				}
				catch (Exception ex)
				{
					Debug.LogErrorFormat("Error Loading File [{0}]", new object[]
					{
						filePath
					});
					return false;
				}
				return FileReader.LoadText(text);
			}
			return false;
		}

		public static bool LoadText(string text)
		{
			FileReader._reset();
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(new char[]
				{
					'\n'
				});
				if (array.Length == 1)
				{
					array = text.Split(new char[]
					{
						'\r'
					});
				}
				foreach (string text2 in array)
				{
					string texttmp = text2;
					if (text2.EndsWith("\r"))
					{
						texttmp = text2.Substring(0, text2.Length - 1);
					}
					FileReader.lines_temp.Add(texttmp);
				}
				FileReader.DeleteComments();
				return true;
			}
			return false;
		}

		public static void UnLoad()
		{
			FileReader._reset();
		}

		private static void DeleteComments()
		{
			bool flag = true;
			foreach (string text in FileReader.lines_temp)
			{
				if (!text.StartsWith("#") && !string.IsNullOrEmpty(text.Trim()))
				{
					FileReader._line_array.Add(text);
				}
				else if (flag && text.StartsWith("#"))
				{
					FileReader.GenerateElementDict(text);
					flag = false;
				}
			}
			FileReader.lines_temp.Clear();
		}

		private static void GenerateElementDict(string titleLine)
		{
			if (titleLine.StartsWith("#"))
			{
				titleLine = titleLine.Substring(1);
			}
			if (titleLine.EndsWith("#"))
			{
				titleLine = titleLine.Substring(0, titleLine.Length - 1);
			}
			FileReader._element_title_array = titleLine.Split(new char[]
			{
				'\t'
			});
			for (int i = 0; i < FileReader._element_title_array.Length; i++)
			{
				FileReader._element_dict.Add(FileReader._element_title_array[i], string.Empty);
			}
		}

		public static bool IsEnd()
		{
			if (FileReader._line_ptr >= FileReader._line_array.Count)
			{
				return true;
			}
			string text = FileReader._line_array[FileReader._line_ptr];
			return text == null || string.IsNullOrEmpty(text) || text.StartsWith("\t");
		}

		public static void ReadLine()
		{
			FileReader._element_ptr = 0;
			FileReader._element_array = FileReader._line_array[FileReader._line_ptr].Split(new char[]
			{
				'\t'
			});
			FileReader._line_ptr++;
			for (int i = 0; i < FileReader._element_title_array.Length; i++)
			{
				string key = FileReader._element_title_array[i];
				string value = (i >= FileReader._element_array.Length) ? string.Empty : FileReader._element_array[i];
				FileReader._element_dict[key] = value;
			}
		}

		public static int ReadInt()
		{
			string text = FileReader._next_element();
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static int ReadInt(string tag)
		{
			string text = FileReader._element_dict[tag];
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			return int.Parse(text);
		}

		public static float ReadFloat()
		{
			string text = FileReader._next_element();
			if (string.IsNullOrEmpty(text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		public static float ReadFloat(string tag)
		{
			string text = FileReader._element_dict[tag];
			if (string.IsNullOrEmpty(text))
			{
				return 0f;
			}
			return float.Parse(text);
		}

		public static string ReadString()
		{
			string text = FileReader._next_element();
			if (text.StartsWith("\""))
			{
				text = text.Substring(1);
			}
			if (text.EndsWith("\""))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text.Replace("\\n", "\n");
		}

		public static string ReadString(string tag)
		{
			string text = FileReader._element_dict[tag];
			if (text.StartsWith("\""))
			{
				text = text.Substring(1);
			}
			if (text.EndsWith("\""))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text.Replace("\\n", "\n");
		}

		public static bool ReadBoolean()
		{
			string a = FileReader._next_element().ToLowerInvariant();
			return a == "1" || a == "true";
		}

		public static bool ReadBoolean(string tag)
		{
			string a = FileReader._element_dict[tag].ToLowerInvariant();
			return a == "1" || a == "true";
		}

		private static int _line_ptr = 0;

		private static List<string> _line_array = new List<string>();

		private static int _element_ptr = 0;

		private static string[] _element_array = null;

		private static List<string> lines_temp = new List<string>();

		private static string[] _element_title_array = null;

		private static Dictionary<string, string> _element_dict = new Dictionary<string, string>();
	}
}
