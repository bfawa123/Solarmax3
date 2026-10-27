using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVRoomChangeQualityCallback(int nQualityEVA, float fLostRate, int nDealy);
}
