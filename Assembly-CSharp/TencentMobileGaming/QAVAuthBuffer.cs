using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	public class QAVAuthBuffer
	{
		public static byte[] GenAuthBuffer(int appId, string roomID, string openId, string key)
		{
			QAVSDKInit.InitSDK();
			int num = 1024;
			byte[] array = new byte[num];
			num = QAVAuthBuffer.QAVSDK_AuthBuffer_GenAuthBuffer(appId, roomID, openId, key, array, num);
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, num);
			return array2;
		}

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		private static extern int QAVSDK_AuthBuffer_GenAuthBuffer(int appId, [MarshalAs(UnmanagedType.LPStr)] string roomID, [MarshalAs(UnmanagedType.LPStr)] string openID, [MarshalAs(UnmanagedType.LPStr)] string key, [MarshalAs(UnmanagedType.LPArray)] byte[] retAuthBuff, int buffLength);

		public const string MyLibName = "gmesdk";
	}
}
