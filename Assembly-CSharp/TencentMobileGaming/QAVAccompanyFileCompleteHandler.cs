using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVAccompanyFileCompleteHandler(int code, bool isfinished, string filepath);
}
