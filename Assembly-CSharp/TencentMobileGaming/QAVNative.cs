using System;
using System.Runtime.InteropServices;

namespace TencentMobileGaming
{
	public class QAVNative
	{
		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_Poll();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_GetSDKVersion();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern void QAVSDK_SetAppVersion([MarshalAs(UnmanagedType.LPStr)] string sAppVersion);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_CheckMicPermission();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetTestEnv(bool test);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetLogLevel(int levelWrite, int levelPrint);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetLogPath([MarshalAs(UnmanagedType.LPStr)] string logDir);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVContext_IsContextStarted();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Start([MarshalAs(UnmanagedType.LPStr)] string sdkAppId, [MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Stop();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Destroy();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVContext_IsRoomEntered();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_EnterRoom([MarshalAs(UnmanagedType.LPStr)] string roomID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] authBuffer, int authBufferLen, int roomtype, QAVEnterRoomComplete enterRoomComplete, QAVExitRoomComplete exitRoomComplete, QAVRoomDisconnect roomDisconnect, QAVEndpointsUpdateInfo endpointsUpdateInfo, QAVOnRoomTypeChangedEvent onRoomtypeChangeEvent, QAVOnDeviceStateChangedEvent onDeviceStateChangedEvent, QAVAudioReadyCallback audioReadyCallback, QAVRoomChangeQualityCallback onRoomChangeQualityEvent, QAVCommonEventCallback commonEventCallback, QAVOnEventCallBack onEventCallback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_ExitRoom();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetRecvMixStreamCount(int nMixCount);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Resume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_Pause();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetAdvanceParams([MarshalAs(UnmanagedType.LPStr)] string KeyCode, [MarshalAs(UnmanagedType.LPStr)] string value);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVContext_GetAdvanceParams([MarshalAs(UnmanagedType.LPStr)] string KeyCode);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr QAVSDK_AVRoom_GetQualityTips();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_ChangeRoomType(int roomtype, QAVCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_GetRoomType();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetRangeAudioMode(int audioMode);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVContext_SetRangeAudioTeamID(int teamID);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_UpdateAudioRecvRange(int range);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVRoom_UpdateSelfPosition([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] int[] position, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisForward, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisRight, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] float[] axisUp, int len);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioCaptureDevice(bool enabled);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioPlayDevice(bool enabled);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioCaptureDeviceEnabled();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioPlayDeviceEnabled();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioSend(bool enabled);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAudioRecv(bool enabled);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioSendEnabled();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_IsAudioRecvEnabled();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetMicLevel();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetMicVolume(int volume);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetMicVolume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetSpeakerLevel();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetSpeakerVolume(int volume);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetSpeakerVolume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetVolumeByUin([MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableLoopBack(bool enabled);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(QAVAudioRouteChangeCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetVoiceType(int voiceType);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetKaraokeType(int type);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_AddAudioBlackList([MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_RemoveAudioBlackList([MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetSendStreamLevel();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetRecvStreamLevel([MarshalAs(UnmanagedType.LPStr)] string openId);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_InitSpatializer([MarshalAs(UnmanagedType.LPStr)] string modelPath);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableSpatializer(bool enabled, bool applyTeam);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVAudioCtrl_IsEnableSpatializer();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StartRecord(int type, [MarshalAs(UnmanagedType.LPStr)] string dstFile, [MarshalAs(UnmanagedType.LPStr)] string accMixFile, [MarshalAs(UnmanagedType.LPStr)] string accPlayFile, QAVAudioRecordCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StopRecord();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_PauseRecord();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_ResumeRecord();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAccompanyFile([MarshalAs(UnmanagedType.LPStr)] string accPlayFile);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetAccompanyTotalTimeByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetRecordTimeByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetRecordTimeByMs(int timeMs);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetRecordKaraokeType(int type);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetRecordFileDurationByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StartPreview(QAVAudioRecordPreviewCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StopPreview();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_PausePreview();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_ResumePreview();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetPreviewTimeByMs(int time);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetPreviewTimeByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetMixWieghts(float mic, float acc);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_AdjustAudioTimeByMs(int time);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_MixRecordFile(QAVAudioRecordMixCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_CancelMixRecordFile();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StartAccompany(string filePath, bool loopBack, int loopCount, int duckerTimeMs, int fileSize, QAVAccompanyFileCompleteHandler callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_StopAccompany(int duckerTimeMs);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool QAVSDK_AVAudioCtrl_IsAccompanyPlayEnd();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_PauseAccompany();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_ResumeAccompany();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAccompanyPlay(bool enable);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_EnableAccompanyLoopBack(bool enable);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAccompanyVolume(int vol);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_GetAccompanyVolume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint QAVSDK_AVAudioCtrl_GetAccompanyFileTotalTimeByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_AVAudioCtrl_SetAccompanyFileCurrentPlayedTimeByMs(uint timeMs);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint QAVSDK_AVAudioCtrl_GetAccompanyFileCurrentPlayedTimeByMs();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_ApplyPTTAuthbuffer([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] authBuffer, int authBufferLen);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetAppInfo([MarshalAs(UnmanagedType.LPStr)] string appid, [MarshalAs(UnmanagedType.LPStr)] string openid);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetMaxMessageLength(int msTime);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StartRecording([MarshalAs(UnmanagedType.LPStr)] string filePath, QAVRecordFileCompleteCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StopRecording();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_CancelRecording();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_UploadRecordedFile([MarshalAs(UnmanagedType.LPStr)] string filePath, QAVUploadFileCompleteCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_DownloadRecordedFile([MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string filePath, QAVDownloadFileCompleteCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StartPlayFile([MarshalAs(UnmanagedType.LPStr)] string filePath, QAVPlayFileCompleteCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StopPlayFile();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetFileSize([MarshalAs(UnmanagedType.LPStr)] string filePath);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetVoiceFileDuration([MarshalAs(UnmanagedType.LPStr)] string filePath);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SpeechToText([MarshalAs(UnmanagedType.LPStr)] string fileID, [MarshalAs(UnmanagedType.LPStr)] string speechLanguage, [MarshalAs(UnmanagedType.LPStr)] string translateLanguage, QAVSpeechToTextCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_StartRecordingWithStreamingRecognition([MarshalAs(UnmanagedType.LPStr)] string filePath, [MarshalAs(UnmanagedType.LPStr)] string speechLanguage, [MarshalAs(UnmanagedType.LPStr)] string translatelanguage, QAVStreamingRecognitionCallback callback);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetMicLevel();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetMicVolume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetMicVolume(int vol);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetSpeakerLevel();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_GetSpeakerVolume();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_SetSpeakerVolume(int vol);

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_PauseRecording();

		[DllImport("gmesdk", CallingConvention = CallingConvention.Cdecl)]
		public static extern int QAVSDK_PTT_ResumeRecording();

		public const string MyLibName = "gmesdk";
	}
}
