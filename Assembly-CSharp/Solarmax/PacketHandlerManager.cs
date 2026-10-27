using System;
using System.Collections.Generic;

namespace Solarmax
{
	public class PacketHandlerManager : IPacketHandlerManager, Lifecycle
	{
		public PacketHandlerManager()
		{
			this.mHandlerDict = new Dictionary<int, IPacketHandler>();
		}

		public bool Init()
		{
			return true;
		}

		public void Tick(float interval)
		{
		}

		public void Destroy()
		{
			this.mHandlerDict.Clear();
		}

		public void RegisterHandler(int packetType, MessageHandler handler)
		{
			if (handler != null)
			{
				PacketHandler packetHandler = new PacketHandler();
				packetHandler.iPacketType = packetType;
				packetHandler.mHandler = handler;
				this.mHandlerDict.Add(packetType, packetHandler);
			}
		}

		public bool DispatchHandler(int type, byte[] data)
		{
			if (data != null && this.mHandlerDict.ContainsKey(type))
			{
				IPacketHandler packetHandler = this.mHandlerDict[type];
				if (packetHandler != null)
				{
					return packetHandler.OnPacketHandler(data);
				}
			}
			return false;
		}

		private Dictionary<int, IPacketHandler> mHandlerDict;
	}
}
