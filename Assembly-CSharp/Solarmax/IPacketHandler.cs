using System;

namespace Solarmax
{
	public interface IPacketHandler
	{
		int GetPacketType();

		bool OnPacketHandler(byte[] data);
	}
}
