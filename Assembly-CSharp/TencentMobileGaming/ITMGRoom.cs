using System;

namespace TencentMobileGaming
{
	public abstract class ITMGRoom
	{
		public abstract string GetQualityTips();

		public abstract int ChangeRoomType(ITMGRoomType roomType);

		public abstract event QAVCallback OnChangeRoomtypeCallback;

		public abstract int GetRoomType();

		public abstract int UpdateAudioRecvRange(int range);

		public abstract int UpdateSelfPosition(int[] position, float[] axisForward, float[] axisRight, float[] axisUp);
	}
}
