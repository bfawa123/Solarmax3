using System;

namespace Solarmax
{
	public class BitOperator
	{
		public static bool GetBitFlag(byte[] data, int index)
		{
			if (data == null)
			{
				return false;
			}
			if (index < 0 || index >= data.Length * 8)
			{
				return false;
			}
			byte b = (byte)(1 << index % 8);
			return (b & data[index / 8]) == b;
		}

		public static bool GetBitFlag(int data, int index)
		{
			if (index < 0 || index >= 32)
			{
				return false;
			}
			int num = 1 << index;
			return (num & data) == num;
		}

		public static bool SetBitFlag(byte[] data, int index, bool flag)
		{
			if (data == null)
			{
				return false;
			}
			if (index < 0 || index >= data.Length * 8)
			{
				return false;
			}
			byte b = (byte)(1 << index % 8);
			if (flag)
			{
				int num = index / 8;
				data[num] |= b;
			}
			else
			{
				int num2 = index / 8;
				data[num2] &= (byte) ~b;
			}
			return true;
		}

		private const int BYTESIZE = 8;
	}
}
