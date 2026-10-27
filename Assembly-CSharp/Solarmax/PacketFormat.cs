using System;
using System.Net;

namespace Solarmax
{
	public class PacketFormat : IPacketFormat
	{
		public int GetLength(int dataLength)
		{
			return 8 + dataLength;
		}

		public void GenerateBuffer(ref byte[] dest, IPacket packet)
		{
			byte[] data = packet.GetData();
			int length = this.GetLength(data.Length);
			dest = new byte[length];
			byte[] bytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length - 4));
			Array.Copy(bytes, 0, dest, 0, 4);
			int packetType = packet.GetPacketType();
			byte[] bytes2 = BitConverter.GetBytes(packetType);
			Array.Copy(bytes2, 0, dest, 4, 4);
			Array.Copy(data, 0, dest, 8, data.Length);
		}

		public bool CheckHavePacket(byte[] buffer, int offset)
		{
			int num = BitConverter.ToInt32(buffer, 0);
			num = IPAddress.NetworkToHostOrder(num);
			return num + 4 <= offset;
		}

		public bool DecodePacket(byte[] buffer, ref int packetLength, ref int packetType, ref byte[] proto)
		{
			int num = BitConverter.ToInt32(buffer, 0);
			num = IPAddress.NetworkToHostOrder(num);
			if (num >= 0)
			{
				packetType = BitConverter.ToInt32(buffer, 4);
				if (packetType >= 0)
				{
					proto = new byte[num - 4];
					Array.Copy(buffer, 8, proto, 0, num - 4);
					if (proto != null)
					{
						packetLength = num + 4;
						return true;
					}
				}
			}
            Solarmax.Singleton<LoggerSystem>.Instance.Debug(string.Concat(new object[]
			{
				"解包错误。。。。。。。。。。。。",
				packetLength,
				"  ",
				packetType
			}), new object[0]);
			return false;
		}

		public static byte[] PACKET_HEAD = new byte[]
		{
			99,
			99
		};
	}
}
