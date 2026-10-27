using System;

namespace Solarmax
{
	public class JRandom
	{
		public JRandom()
		{
			this.setSeed((long)(DateTime.Now.Millisecond + this.GetHashCode()));
		}

		public JRandom(long seed)
		{
			this.setSeed(seed);
		}

		protected int next(int bits)
		{
			int result = -1;
			lock (this)
			{
				this.seed = (this.seed * 25214903917L + 11L & 281474976710655L);
				result = (int)(this.seed >> 48 - bits);
			}
			return result;
		}

		public bool nextBoolean()
		{
			return this.next(1) != 0;
		}

		public void nextBytes(byte[] buf)
		{
			int num = 0;
			int i = 0;
			int num2 = 0;
			while (i < buf.Length)
			{
				if (num2 == 0)
				{
					num = this.nextInt();
					num2 = 3;
				}
				else
				{
					num2--;
				}
				buf[i++] = (byte)num;
				num >>= 8;
			}
		}

		public double nextDouble()
		{
			return (double)(((long)this.next(26) << 27) + (long)this.next(27)) / 9007199254740992.0;
		}

		public float nextFloat()
		{
			return (float)this.next(24) / 16777216f;
		}

		public double nextGaussian()
		{
			double num;
			double num4;
			lock (this)
			{
				if (this.haveNextNextGaussian)
				{
					this.haveNextNextGaussian = false;
					return this.nextNextGaussian;
				}
				double num2;
				double num3;
				do
				{
					num = 2.0 * this.nextDouble() - 1.0;
					num2 = 2.0 * this.nextDouble() - 1.0;
					num3 = num * num + num2 * num2;
				}
				while (num3 >= 1.0 || num3 == 0.0);
				num4 = Math.Sqrt(-2.0 * Math.Log(num3) / num3);
				this.nextNextGaussian = num2 * num4;
				this.haveNextNextGaussian = true;
			}
			return num * num4;
		}

		public int nextInt()
		{
			return this.next(32);
		}

		public int nextInt(int n)
		{
			if (n <= 0)
			{
				throw new ArgumentException("n <= 0: " + n);
			}
			if ((n & -n) == n)
			{
				return (int)((long)n * (long)this.next(31) >> 31);
			}
			int num;
			int num2;
			do
			{
				num = this.next(31);
				num2 = num % n;
			}
			while (num - num2 + (n - 1) < 0);
			return num2;
		}

		public long nextLong()
		{
			return ((long)this.next(32) << 32) + (long)this.next(32);
		}

		public void setSeed(long seed)
		{
			lock (this)
			{
				this.seed = ((seed ^ 25214903917L) & 281474976710655L);
				this.haveNextNextGaussian = false;
			}
		}

		private const long serialVersionUID = 3905348978240129619L;

		private const long multiplier = 25214903917L;

		private bool haveNextNextGaussian;

		private long seed;

		private double nextNextGaussian;
	}
}
