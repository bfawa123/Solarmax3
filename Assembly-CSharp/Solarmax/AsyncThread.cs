using System;
using System.Threading;

namespace Solarmax
{
	public class AsyncThread
	{
		public AsyncThread(Callback<AsyncThread> cb)
		{
			this.mStatus = AsyncThread.ThreadStatus.NONE;
			this.mContext = cb;
			this.mFinishedContext = null;
			this.mThread = new Thread(new ThreadStart(this.ContextMask));
			this.mExtraData = null;
		}

		public AsyncThread(Callback<AsyncThread> cb, object extraData)
		{
			this.mStatus = AsyncThread.ThreadStatus.NONE;
			this.mContext = cb;
			this.mFinishedContext = null;
			this.mThread = new Thread(new ThreadStart(this.ContextMask));
			this.mExtraData = extraData;
		}

		private void ContextMask()
		{
			this.mStatus = AsyncThread.ThreadStatus.WORKING;
			this.mContext(this);
			if (this.mStatus == AsyncThread.ThreadStatus.STOP && this.mFinishedContext != null)
			{
				this.mFinishedContext();
			}
			this.mStatus = AsyncThread.ThreadStatus.FINISHED;
		}

		private void Release()
		{
			this.mContext = null;
			this.mFinishedContext = null;
			this.mThread = null;
			this.mExtraData = null;
		}

		public bool Start()
		{
			if (this.mThread != null)
			{
				this.mStatus = AsyncThread.ThreadStatus.START;
				this.mThread.Start();
				return true;
			}
			return false;
		}

		public void Stop()
		{
			if (this.IsWorking())
			{
				this.mFinishedContext = null;
				this.mStatus = AsyncThread.ThreadStatus.STOP;
			}
		}

		public void AsyncStop(Callback cb)
		{
			if (this.IsWorking())
			{
				this.mFinishedContext = cb;
				this.mStatus = AsyncThread.ThreadStatus.STOP;
			}
		}

		public void SyncStop(Callback cb)
		{
			if (this.IsWorking())
			{
				this.mFinishedContext = null;
				this.mStatus = AsyncThread.ThreadStatus.STOP;
				while (this.mStatus != AsyncThread.ThreadStatus.FINISHED)
				{
				}
				cb();
			}
		}

		public bool IsWorking()
		{
			return this.mStatus == AsyncThread.ThreadStatus.WORKING;
		}

		public int GetThreadId()
		{
			return (this.mThread != null) ? this.mThread.ManagedThreadId : -1;
		}

		public void SetExtraData(object data)
		{
			this.mExtraData = data;
		}

		public object GetExtraData()
		{
			return this.mExtraData;
		}

		private volatile AsyncThread.ThreadStatus mStatus;

		private Callback<AsyncThread> mContext;

		private Callback mFinishedContext;

		private Thread mThread;

		private object mExtraData;

		public enum ThreadStatus
		{
			NONE,
			START,
			WORKING,
			STOP,
			FINISHED
		}
	}
}
