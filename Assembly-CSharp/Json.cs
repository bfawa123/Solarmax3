using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Json
{
	public static void Test()
	{
	}

	public static byte[] EnCodeBytes(object msg)
	{
		byte[] buffer;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(memoryStream, msg);
			buffer = memoryStream.GetBuffer();
		}
		return buffer;
	}

	public static T DeCode<T>(byte[] bytes)
	{
		T result;
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			result = (T)((object)binaryFormatter.Deserialize(memoryStream));
		}
		return result;
	}
}
