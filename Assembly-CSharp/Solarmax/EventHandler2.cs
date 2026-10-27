using System;

namespace Solarmax
{
	public class EventHandler2
	{
		public EventHandler2()
		{
			this.mKey = -1;
			this.mHoster = null;
			this.mData = null;
			this.mHandler = null;
			this.mFireCount = 0;
		}

		public void Init(int key, object hoster, object data, Callback<int, object, object[]> handler)
		{
			this.mKey = key;
			this.mHoster = hoster;
			this.mData = data;
			this.mHandler = handler;
		}

		public void test()
		{
		}

		public void Fire(Event2 e)
		{
			if (this.mHandler != null && this.mHoster != null)
			{
				this.mHandler(e.GetKey(), this.mData, e.GetArgs());
				this.mFireCount++;
			}
			else
			{
                Solarmax.Singleton<LoggerSystem>.Instance.Error(string.Concat(new object[]
				{
					"EventLisener Error! hoster:",
					this.mHoster,
					", handler:",
					this.mHandler
				}), new object[0]);
			}
		}

		public bool IsEvent(int key, object hoster)
		{
			return this.mKey == key && this.mHoster == hoster;
		}

		private int mKey;

		private object mHoster;

		private object mData;

		private Callback<int, object, object[]> mHandler;

		private int mFireCount;
	}
}
