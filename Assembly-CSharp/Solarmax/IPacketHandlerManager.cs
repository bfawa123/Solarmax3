using System;

namespace Solarmax
{
	public interface IPacketHandlerManager : Lifecycle
	{
		void RegisterHandler(int packetType, MessageHandler handler);

		bool DispatchHandler(int type, byte[] data);
	}
}
