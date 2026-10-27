using System;

namespace Plugin
{
	public class Rand
	{
		public Rand(int seed)
		{
			this.holdSeed = seed;
		}

		public int seed
		{
			get
			{
				return this.holdSeed;
			}
			set
			{
				this.holdSeed = value;
			}
		}

		public int Random()
		{
			return (this.holdSeed = this.holdSeed * 214013 + 2531011) >> 16 & 32767;
		}

		public double RandomDouble()
		{
			return (double)this.Random() * Rand.MBIG;
		}

		public float RandomFloat()
		{
			return (float)this.Random() * (float)Rand.MBIG;
		}

		public int Range(int min, int max)
		{
			if (min > max)
			{
				throw new ArgumentOutOfRangeException();
			}
			return this.Random() % (max - min) + min;
		}

		public float Range(float min, float max)
		{
			if (min > max)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (max - min) * this.RandomFloat() + min;
		}

		private static double MBIG = 3.051850947599719E-05;

		private int holdSeed;
	}
}
