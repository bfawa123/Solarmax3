using System;

namespace Solarmax
{
	public class TimeSystem : Solarmax.Singleton<TimeSystem>, Lifecycle
	{
		public TimeSystem()
		{
			this.mFrames = 0;
			this.mTotalTickSeconds = 0.0;
			this.mLocalStartTime = 0.0;
			this.mRemoteTimeOffset = 0f;
		}

		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("TimeSystem    init  begin", new object[0]);
			this.mLocalStartTime = this.GetMillisecods();
			this.mTotalTickSeconds = 0.0;
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("TimeSystem    init  end", new object[0]);
			return true;
		}

		public void Tick(float interval)
		{
			this.mFrames++;
			this.mTotalTickSeconds += (double)interval;
		}

		public void Destroy()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("TimeSystem    destroy  begin", new object[0]);
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("TimeSystem    destroy  end", new object[0]);
		}

		public float GetRemoteTimeOffset()
		{
			return this.mRemoteTimeOffset;
		}

		public void SetRemoteTimeOffset(float timeOffset)
		{
			this.mRemoteTimeOffset = timeOffset;
		}

		public double GetMillisecods()
		{
			return Solarmax.Singleton<TimeSystem>.Instance.GetServerTime().Subtract(this._BaseTime).TotalMilliseconds;
		}

		public double GetLocalMilliseconds()
		{
			return this.GetMillisecods() - this.mLocalStartTime;
		}

		public double GetServerMilliseconds()
		{
			return this.GetLocalMilliseconds() - (double)this.mRemoteTimeOffset;
		}

		public void ResetFrame()
		{
			this.mFrames = 0;
		}

		public int GetFrame()
		{
			return this.mFrames;
		}

		public double GetRunTime()
		{
			return this.mTotalTickSeconds;
		}

		public DateTime GetClientTime()
		{
			return DateTime.Now;
		}

		public int GetIntervalDay(double start, double end)
		{
			double num = end - start;
			return (int)(num / 86400000.0);
		}

		public void SetServerTime(int timeStamp)
		{
			this.serverTimeStampBegin = (long)timeStamp;
			this.serverTimeBegin = new DateTime(1970, 1, 1);
			this.serverTimeBegin = this.serverTimeBegin.AddSeconds((double)timeStamp);
			this.localTimeBegin = DateTime.Now;
		}

		public DateTime GetServerTime()
		{
			TimeSpan value = DateTime.Now - this.localTimeBegin;
			return this.serverTimeBegin.Add(value);
		}

		public long GetTimeStamp()
		{
			TimeSpan timeSpan = DateTime.Now - this.localTimeBegin;
			return this.serverTimeStampBegin + (long)timeSpan.TotalSeconds;
		}

		public DateTime GetServerTimeEX()
		{
			TimeSpan value = DateTime.UtcNow - this.localTimeBegin;
			return this.serverTimeBegin.Add(value);
		}

		public DateTime GetServerTimeCST()
		{
			return TimeZone.CurrentTimeZone.ToLocalTime(this.GetServerTime());
		}

		public DateTime GetTime(int seconds)
		{
			DateTime result = new DateTime(1970, 1, 1);
			result = result.AddSeconds((double)seconds);
			return result;
		}

		public DateTime GetTimeCST(int seconds)
		{
			return TimeZone.CurrentTimeZone.ToLocalTime(this.GetTime(seconds));
		}

		public DateTime GetTimeCST(DateTime dt)
		{
			return TimeZone.CurrentTimeZone.ToLocalTime(dt);
		}

		private DateTime _BaseTime = new DateTime(1970, 1, 1);

		private int mFrames;

		private double mTotalTickSeconds;

		private double mLocalStartTime;

		private float mRemoteTimeOffset;

		private DateTime serverTimeBegin;

		private DateTime localTimeBegin;

		private long serverTimeStampBegin;
	}
}
