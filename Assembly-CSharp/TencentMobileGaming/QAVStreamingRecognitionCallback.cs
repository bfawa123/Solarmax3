using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void QAVStreamingRecognitionCallback(int code, string fileid, string filepath, string result);
}
