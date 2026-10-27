using System;

namespace Solarmax
{
	public interface IPacket
	{
		int GetPacketType();

		byte[] GetData();
	}
}
