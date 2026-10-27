using System;
using System.IO;

namespace ProtoBuf
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				FriendData friendData = new FriendData();
				friendData.DbID = 11111;
				MemoryStream memoryStream = new MemoryStream();
				Serializer.Serialize<FriendData>(memoryStream, friendData);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Position = 0L;
				memoryStream.Read(array, 0, array.Length);
				memoryStream.Dispose();
			}
			catch (Exception ex)
			{
			}
		}
	}
}
