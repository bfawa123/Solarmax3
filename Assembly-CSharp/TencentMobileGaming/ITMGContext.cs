using System;

namespace TencentMobileGaming
{
	public abstract class ITMGContext
	{
		static ITMGContext()
		{
			QAVSDKInit.InitSDK();
			QAVContext.GetInstance();
		}

		public static ITMGContext GetInstance()
		{
			return QAVContext.GetInstance();
		}

		public abstract int Poll();

		public abstract int Pause();

		public abstract int Resume();

		public abstract string GetSDKVersion();

		public abstract void SetAppVersion(string sAppVersion);

		public abstract int SetLogLevel(int levelWrite, int levelPrint);

		public abstract int SetLogPath(string logDir);

		public abstract int Init(string sdkAppID, string openID);

		public abstract int Uninit();

		public abstract bool IsRoomEntered();

		public abstract int EnterRoom(string roomID, ITMGRoomType roomType, byte[] authBuffer);

		public abstract int ExitRoom();

		public abstract int SetAdvanceParams(string KeyCode, string value);

		public abstract string GetAdvanceParams(string KeyCode);

		public abstract ITMGRoom GetRoom();

		public abstract ITMGAudioCtrl GetAudioCtrl();

		public abstract ITMGAudioEffectCtrl GetAudioEffectCtrl();

		public abstract ITMG_RECORD_PERMISSION CheckMicPermission();

		public abstract ITMGPTT GetPttCtrl();

		public abstract event QAVEnterRoomComplete OnEnterRoomCompleteEvent;

		public abstract event QAVExitRoomComplete OnExitRoomCompleteEvent;

		public abstract event QAVRoomDisconnect OnRoomDisconnectEvent;

		public abstract event QAVEndpointsUpdateInfo OnEndpointsUpdateInfoEvent;

		public abstract event QAVOnRoomTypeChangedEvent OnRoomTypeChangedEvent;

		public abstract event QAVAudioReadyCallback OnAudioReadyEvent;

		public abstract event QAVRoomChangeQualityCallback OnRoomChangeQualityEvent;

		public abstract event QAVCommonEventCallback OnCommonEventCallback;

		public abstract event QAVOnEventCallBack onEventCallBack;

		public abstract int SetTestEnv(bool test);

		public abstract int SetRecvMixStreamCount(int nCount);

		public abstract int SetRangeAudioMode(ITMGRangeAudioMode gameAudioMode);

		public abstract int SetRangeAudioTeamID(int teamID);

		public static int LOG_LEVEL_NONE;

		public static int LOG_LEVEL_ERROR = 1;

		public static int LOG_LEVEL_INFO = 2;

		public static int LOG_LEVEL_DEBUG = 3;

		public static int LOG_LEVEL_VERBOSE = 4;

		public static int EVENT_ID_ENDPOINT_ENTER = 1;

		public static int EVENT_ID_ENDPOINT_EXIT = 2;

		public static int EVENT_ID_ENDPOINT_HAS_CAMERA_VIDEO = 3;

		public static int EVENT_ID_ENDPOINT_NO_CAMERA_VIDEO = 4;

		public static int EVENT_ID_ENDPOINT_HAS_AUDIO = 5;

		public static int EVENT_ID_ENDPOINT_NO_AUDIO = 6;
	}
}
