using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVEndpointsUpdateInfo(int eventID, int count, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] string[] openIdList);
}
