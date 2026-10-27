using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace TencentMobileGaming
{
	public class QAVRoom : ITMGRoom
	{
		public override string GetQualityTips()
		{
			return Marshal.PtrToStringAnsi(QAVNative.QAVSDK_AVRoom_GetQualityTips());
		}

		[MonoPInvokeCallback(typeof(QAVCallback))]
		private static void s_ChangeRoomtypeCallback(int result, string error_info)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"s_ChangeRoomtypeCallback, result=",
				result,
				"err:",
				error_info
			}));
			if (QAVContext.GetInstance().GetRoomInner().OnChangeRoomtypeCallback != null)
			{
				QAVContext.GetInstance().GetRoomInner().OnChangeRoomtypeCallback(result, error_info);
			}
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVCallback OnChangeRoomtypeCallback;

		public override int ChangeRoomType(ITMGRoomType roomType)
		{
			if (QAVRoom.cache0 == null)
			{
				QAVRoom.cache0 = new QAVCallback(QAVRoom.s_ChangeRoomtypeCallback);
			}
			return QAVNative.QAVSDK_AVRoom_ChangeRoomType((int)roomType, QAVRoom.cache0);
		}

		public override int GetRoomType()
		{
			int num = QAVNative.QAVSDK_AVRoom_GetRoomType();
			if (num > 0 && num < 4)
			{
				return num;
			}
			return 0;
		}

		public override int UpdateAudioRecvRange(int range)
		{
			return QAVNative.QAVSDK_AVRoom_UpdateAudioRecvRange(range);
		}

		public override int UpdateSelfPosition(int[] position, float[] axisForward, float[] axisRight, float[] axisUp)
		{
			return QAVNative.QAVSDK_AVRoom_UpdateSelfPosition(position, axisForward, axisRight, axisUp, 3);
		}

		[CompilerGenerated]
		private static QAVCallback cache0;
	}
}
