using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVOnDeviceStateChangedEvent(int deviceType, string deviceId, bool openOrClose);
}
