using System;

namespace Solarmax
{
	public interface IPacketFormat
	{
		int GetLength(int dataLength);

		void GenerateBuffer(ref byte[] dest, IPacket packet);

		bool CheckHavePacket(byte[] buffer, int offset);

		bool DecodePacket(byte[] buffer, ref int packetLength, ref int packetType, ref byte[] data);
	}
}
