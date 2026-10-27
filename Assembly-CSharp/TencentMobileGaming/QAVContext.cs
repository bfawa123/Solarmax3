using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace TencentMobileGaming
{
	public class QAVContext : ITMGContext
	{
		private QAVContext()
		{
		}

		public new static QAVContext GetInstance()
		{
			if (QAVContext.sInstance == null)
			{
				QAVContext.sInstance = new QAVContext();
			}
			return QAVContext.sInstance;
		}

		~QAVContext()
		{
			UnityEngine.Debug.LogFormat("~QAVContext", new object[0]);
			QAVNative.QAVSDK_AVContext_Destroy();
		}

		public override int Poll()
		{
			return QAVNative.QAVSDK_Poll();
		}

		public override int Pause()
		{
			return QAVNative.QAVSDK_AVContext_Pause();
		}

		public override int Resume()
		{
			return QAVNative.QAVSDK_AVContext_Resume();
		}

		public override int Init(string sdkAppID, string openID)
		{
			if (QAVNative.QAVSDK_AVContext_IsContextStarted() && string.Equals(this.mSdkAppID, sdkAppID) && string.Equals(this.mOpenID, openID))
			{
				return QAVError.OK;
			}
			int num = QAVNative.QAVSDK_AVContext_Stop();
			if (num == QAVError.OK || num == QAVError.ERR_HAS_IN_THE_STATE)
			{
				this.mSdkAppID = sdkAppID;
				this.mOpenID = openID;
				QAVNative.QAVSDK_AVContext_Start(this.mSdkAppID, this.mOpenID);
				if (QAVContext.cache0 == null)
				{
					QAVContext.cache0 = new QAVAudioRouteChangeCallback(QAVAudioCtrl.s_OnAudioRouteChangeComplete);
				}
				QAVNative.QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(QAVContext.cache0);
				QAVPTT.GetInstance().SetAppInfo(sdkAppID, openID);
				return QAVError.OK;
			}
			return num;
		}

		public override int Uninit()
		{
			this.mSdkAppID = string.Empty;
			this.mOpenID = string.Empty;
			QAVNative.QAVSDK_AVAudioCtrl_SetAudioRouteChangeCallback(null);
			return QAVNative.QAVSDK_AVContext_Stop();
		}

		public override string GetSDKVersion()
		{
			return Marshal.PtrToStringAnsi(QAVNative.QAVSDK_GetSDKVersion());
		}

		public override void SetAppVersion(string sAppVersion)
		{
			QAVNative.QAVSDK_SetAppVersion(sAppVersion);
		}

		public override int SetLogLevel(int levelWrite, int levelPrint)
		{
			return QAVNative.QAVSDK_AVContext_SetLogLevel(levelWrite, levelPrint);
		}

		public override int SetLogPath(string logDir)
		{
			return QAVNative.QAVSDK_AVContext_SetLogPath(logDir);
		}

		public override int SetRecvMixStreamCount(int nCount)
		{
			return QAVNative.QAVSDK_AVContext_SetRecvMixStreamCount(nCount);
		}

		public override int SetRangeAudioMode(ITMGRangeAudioMode gameAudioMode)
		{
			return QAVNative.QAVSDK_AVContext_SetRangeAudioMode((int)gameAudioMode);
		}

		public override int SetRangeAudioTeamID(int teamID)
		{
			return QAVNative.QAVSDK_AVContext_SetRangeAudioTeamID(teamID);
		}

		public override int SetTestEnv(bool test)
		{
			UnityEngine.Debug.Log(string.Format("SetTestEnv================", new object[0]));
			return QAVNative.QAVSDK_AVContext_SetTestEnv(test);
		}

		public override bool IsRoomEntered()
		{
			return QAVNative.QAVSDK_AVContext_IsRoomEntered();
		}

		public override ITMG_RECORD_PERMISSION CheckMicPermission()
		{
			int num = QAVNative.QAVSDK_AVContext_CheckMicPermission();
			if (num < 0 || num > 3)
			{
				num = 3;
			}
			return (ITMG_RECORD_PERMISSION)num;
		}

		public override int EnterRoom(string roomID, ITMGRoomType roomtype, byte[] authBuffer)
		{
			int authBufferLen = authBuffer.Length;
			if (QAVContext.cache1 == null)
			{
				QAVContext.cache1 = new QAVEnterRoomComplete(QAVContext.s_OnEnterRoomComplete);
			}
			QAVEnterRoomComplete enterRoomComplete = QAVContext.cache1;
			if (QAVContext.cache2 == null)
			{
				QAVContext.cache2 = new QAVExitRoomComplete(QAVContext.s_OnExitRoomComplete);
			}
			QAVExitRoomComplete exitRoomComplete = QAVContext.cache2;
			if (QAVContext.cache3 == null)
			{
				QAVContext.cache3 = new QAVRoomDisconnect(QAVContext.s_OnRoomDisconnect);
			}
			QAVRoomDisconnect roomDisconnect = QAVContext.cache3;
			if (QAVContext.cache4 == null)
			{
				QAVContext.cache4 = new QAVEndpointsUpdateInfo(QAVContext.s_OnEndpointsUpdateInfo);
			}
			QAVEndpointsUpdateInfo endpointsUpdateInfo = QAVContext.cache4;
			if (QAVContext.cache5 == null)
			{
				QAVContext.cache5 = new QAVOnRoomTypeChangedEvent(QAVContext.s_QAVOnRoomTypeChangedEvent);
			}
			QAVOnRoomTypeChangedEvent onRoomtypeChangeEvent = QAVContext.cache5;
			if (QAVContext.cache6 == null)
			{
				QAVContext.cache6 = new QAVOnDeviceStateChangedEvent(QAVContext.s_OnDeviceStateChangedEvent);
			}
			QAVOnDeviceStateChangedEvent onDeviceStateChangedEvent = QAVContext.cache6;
			if (QAVContext.cache7 == null)
			{
				QAVContext.cache7 = new QAVAudioReadyCallback(QAVContext.s_OnAudioReady);
			}
			QAVAudioReadyCallback audioReadyCallback = QAVContext.cache7;
			if (QAVContext.cache8 == null)
			{
				QAVContext.cache8 = new QAVRoomChangeQualityCallback(QAVContext.s_AVRoomChangeQualityCallback);
			}
			QAVRoomChangeQualityCallback onRoomChangeQualityEvent = QAVContext.cache8;
			if (QAVContext.cache9 == null)
			{
				QAVContext.cache9 = new QAVCommonEventCallback(QAVContext.s_QAVCommonEventCallback);
			}
			QAVCommonEventCallback commonEventCallback = QAVContext.cache9;
			if (QAVContext.cacheA == null)
			{
				QAVContext.cacheA = new QAVOnEventCallBack(QAVContext.s_QAVOnEventCallBack);
			}
			return QAVNative.QAVSDK_AVContext_EnterRoom(roomID, authBuffer, authBufferLen, (int)roomtype, enterRoomComplete, exitRoomComplete, roomDisconnect, endpointsUpdateInfo, onRoomtypeChangeEvent, onDeviceStateChangedEvent, audioReadyCallback, onRoomChangeQualityEvent, commonEventCallback, QAVContext.cacheA);
		}

		public override int ExitRoom()
		{
			return QAVNative.QAVSDK_AVContext_ExitRoom();
		}

		public override ITMGRoom GetRoom()
		{
			return this.GetRoomInner();
		}

		public override string GetAdvanceParams(string KeyCode)
		{
			return Marshal.PtrToStringAnsi(QAVNative.QAVSDK_AVContext_GetAdvanceParams(KeyCode));
		}

		public override int SetAdvanceParams(string KeyCode, string value)
		{
			return QAVNative.QAVSDK_AVContext_SetAdvanceParams(KeyCode, value);
		}

		public QAVRoom GetRoomInner()
		{
			if (this.mAVRoom == null)
			{
				this.mAVRoom = new QAVRoom();
			}
			return this.mAVRoom;
		}

		public override ITMGAudioCtrl GetAudioCtrl()
		{
			return this.GetAudioCtrlInner();
		}

		public QAVAudioCtrl GetAudioCtrlInner()
		{
			if (this.mAVAudioCtrl == null)
			{
				this.mAVAudioCtrl = new QAVAudioCtrl();
			}
			return this.mAVAudioCtrl;
		}

		public override ITMGAudioEffectCtrl GetAudioEffectCtrl()
		{
			return this.GetAudioEffectCtrlInner();
		}

		public QAVAudioEffectCtrl GetAudioEffectCtrlInner()
		{
			if (this.mAVAudioEffectCtrl == null)
			{
				this.mAVAudioEffectCtrl = new QAVAudioEffectCtrl();
			}
			return this.mAVAudioEffectCtrl;
		}

		public override ITMGPTT GetPttCtrl()
		{
			return QAVPTT.GetInstance();
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVEnterRoomComplete OnEnterRoomCompleteEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVExitRoomComplete OnExitRoomCompleteEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVRoomDisconnect OnRoomDisconnectEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVEndpointsUpdateInfo OnEndpointsUpdateInfoEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVOnRoomTypeChangedEvent OnRoomTypeChangedEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVAudioReadyCallback OnAudioReadyEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVRoomChangeQualityCallback OnRoomChangeQualityEvent;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVCommonEventCallback OnCommonEventCallback;

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVOnEventCallBack onEventCallBack;

		[MonoPInvokeCallback(typeof(QAVAudioReadyCallback))]
		private static void s_OnAudioReady()
		{
			if (QAVContext.GetInstance().OnAudioReadyEvent != null)
			{
				QAVContext.GetInstance().OnAudioReadyEvent();
			}
		}

		[MonoPInvokeCallback(typeof(QAVRoomChangeQualityCallback))]
		private static void s_AVRoomChangeQualityCallback(int nQualityEVA, float fLostRate, int nDealy)
		{
			if (QAVContext.GetInstance().OnRoomChangeQualityEvent != null)
			{
				QAVContext.GetInstance().OnRoomChangeQualityEvent(nQualityEVA, fLostRate, nDealy);
			}
		}

		[MonoPInvokeCallback(typeof(QAVEnterRoomComplete))]
		private static void s_OnEnterRoomComplete(int result, string error_info)
		{
			UnityEngine.Debug.LogFormat("s_OnEnterRoomComplete:code:{0},Msg:{1},{2}", new object[]
			{
				result,
				error_info,
				QAVContext.GetInstance().OnEnterRoomCompleteEvent
			});
			if (QAVContext.GetInstance().OnEnterRoomCompleteEvent != null)
			{
				QAVContext.GetInstance().OnEnterRoomCompleteEvent(result, error_info);
			}
		}

		[MonoPInvokeCallback(typeof(QAVExitRoomComplete))]
		private static void s_OnExitRoomComplete()
		{
			UnityEngine.Debug.LogFormat("s_OnExitRoomComplete", new object[0]);
			if (QAVContext.GetInstance().OnExitRoomCompleteEvent != null)
			{
				QAVContext.GetInstance().OnExitRoomCompleteEvent();
			}
		}

		[MonoPInvokeCallback(typeof(QAVRoomDisconnect))]
		private static void s_OnRoomDisconnect(int result, string error_info)
		{
			UnityEngine.Debug.LogFormat("s_OnRoomDisconnect", new object[0]);
			if (QAVContext.GetInstance().OnRoomDisconnectEvent != null)
			{
				QAVContext.GetInstance().OnRoomDisconnectEvent(result, error_info);
			}
		}

		[MonoPInvokeCallback(typeof(QAVEndpointsUpdateInfo))]
		private static void s_OnEndpointsUpdateInfo(int eventID, int count, string[] openIdList)
		{
			if (QAVContext.GetInstance().OnEndpointsUpdateInfoEvent != null)
			{
				QAVContext.GetInstance().OnEndpointsUpdateInfoEvent(eventID, count, openIdList);
			}
		}

		[MonoPInvokeCallback(typeof(QAVOnDeviceStateChangedEvent))]
		private static void s_OnDeviceStateChangedEvent(int deviceType, string deviceId, bool openOrClose)
		{
		}

		[MonoPInvokeCallback(typeof(QAVOnRoomTypeChangedEvent))]
		private static void s_QAVOnRoomTypeChangedEvent(int roomtype)
		{
			UnityEngine.Debug.LogFormat("s_QAVOnRoomTypeChangedEvent:coderoomtype{0}", new object[]
			{
				roomtype
			});
			if (QAVContext.GetInstance().OnRoomTypeChangedEvent != null)
			{
				QAVContext.GetInstance().OnRoomTypeChangedEvent(roomtype);
			}
		}

		[MonoPInvokeCallback(typeof(QAVOnEventCallBack))]
		private static void s_QAVOnEventCallBack(int type, int subType, string data)
		{
			UnityEngine.Debug.LogFormat("s_QAVOnEventCallBack:type={0},subTtpe={1} info:{2}", new object[]
			{
				type,
				subType,
				data
			});
			if (QAVContext.GetInstance().onEventCallBack != null)
			{
				QAVContext.GetInstance().onEventCallBack(type, subType, data);
			}
		}

		[MonoPInvokeCallback(typeof(QAVCommonEventCallback))]
		private static void s_QAVCommonEventCallback(int type, int param0, int param1)
		{
			UnityEngine.Debug.LogFormat("s_AVRoomChangeQualityCallback:type={0}", new object[]
			{
				type
			});
			if (QAVContext.GetInstance().OnCommonEventCallback != null)
			{
				QAVContext.GetInstance().OnRoomChangeQualityEvent(type, (float)param0, param1);
			}
		}

		private QAVRoom mAVRoom;

		private QAVAudioCtrl mAVAudioCtrl;

		private QAVAudioEffectCtrl mAVAudioEffectCtrl;

		private static QAVContext sInstance;

		private static readonly object sLock = new object();

		private string mSdkAppID = string.Empty;

		public string mOpenID = string.Empty;

		[CompilerGenerated]
		private static QAVAudioRouteChangeCallback cache0;

		[CompilerGenerated]
		private static QAVEnterRoomComplete cache1;

		[CompilerGenerated]
		private static QAVExitRoomComplete cache2;

		[CompilerGenerated]
		private static QAVRoomDisconnect cache3;

		[CompilerGenerated]
		private static QAVEndpointsUpdateInfo cache4;

		[CompilerGenerated]
		private static QAVOnRoomTypeChangedEvent cache5;

		[CompilerGenerated]
		private static QAVOnDeviceStateChangedEvent cache6;

		[CompilerGenerated]
		private static QAVAudioReadyCallback cache7;

		[CompilerGenerated]
		private static QAVRoomChangeQualityCallback cache8;

		[CompilerGenerated]
		private static QAVCommonEventCallback cache9;

		[CompilerGenerated]
		private static QAVOnEventCallBack cacheA;
	}
}
