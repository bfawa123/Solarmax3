using System;

namespace Solarmax
{
	public class Event2 : IPoolObject<Event2>
	{
		public void Set(int key, object[] args)
		{
			this.mKey = key;
			this.mArgs = args;
		}

		public int GetKey()
		{
			return this.mKey;
		}

		public object[] GetArgs()
		{
			return this.mArgs;
		}

		private int mKey;

		private object[] mArgs;
	}
}
