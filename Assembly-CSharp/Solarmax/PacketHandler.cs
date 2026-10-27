using System;
using System.IO;

namespace Solarmax
{
	public class PacketHandler : IPacketHandler
	{
		public int GetPacketType()
		{
			return this.iPacketType;
		}

		public bool OnPacketHandler(byte[] data)
		{
			using (MemoryStream memoryStream = new MemoryStream(data, 0, data.Length))
			{
				PacketEvent message = new PacketEvent(this.iPacketType, memoryStream);
				this.mHandler(this.iPacketType, message);
			}
			return true;
		}

		public int iPacketType;

		public MessageHandler mHandler;
	}
}
