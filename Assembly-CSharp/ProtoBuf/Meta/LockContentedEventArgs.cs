using System;

namespace ProtoBuf.Meta
{
	public sealed class LockContentedEventArgs : EventArgs
	{
		internal LockContentedEventArgs(string ownerStackTrace)
		{
			this.ownerStackTrace = ownerStackTrace;
		}

		public string OwnerStackTrace
		{
			get
			{
				return this.ownerStackTrace;
			}
		}

		private readonly string ownerStackTrace;
	}
}
