using System;

namespace Solarmax
{
	public class Logger : Lifecycle
	{
		public Logger()
		{
		}

		public Logger(Callback<string> callback)
		{
			this.mOutput = callback;
		}

		public virtual void Write(string message)
		{
			if (this.mOutput != null)
			{
				this.mOutput(message);
			}
		}

		public virtual bool Init()
		{
			return true;
		}

		public virtual void Tick(float interval)
		{
		}

		public virtual void Destroy()
		{
		}

		private Callback<string> mOutput;
	}
}
