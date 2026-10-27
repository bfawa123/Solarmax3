using System;

namespace Solarmax
{
	public class RemoteHost
	{
		public RemoteHost(string address, int port)
		{
			this.mAddress = address;
			this.mPort = port;
		}

		public string GetAddress()
		{
			return this.mAddress;
		}

		public int GetPort()
		{
			return this.mPort;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}", this.mAddress, this.mPort);
		}

		private string mAddress;

		private int mPort;
	}
}
