using System;
using System.IO;
using ProtoBuf;

namespace Solarmax
{
	public class NetPacket : IPacket
	{
		public NetPacket(int type)
		{
			this.mType = type;
			this.mProtoBytes = null;
		}

		public int GetPacketType()
		{
			return this.mType;
		}

		public byte[] GetData()
		{
			return this.mProtoBytes;
		}

		public void EncodeProto<T>(T proto)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					Serializer.Serialize<T>(memoryStream, proto);
					byte[] buffer = memoryStream.GetBuffer();
					int num = (int)memoryStream.Length;
					this.mProtoBytes = new byte[num];
					Array.Copy(buffer, 0, this.mProtoBytes, 0, num);
				}
				catch (Exception)
				{
				}
			}
		}

		private int mType;

		private byte[] mProtoBytes;
	}
}
