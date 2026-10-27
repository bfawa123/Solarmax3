using System;
using System.Diagnostics;
using AOT;

namespace TencentMobileGaming
{
	public class QAVAudioCtrl : ITMGAudioCtrl
	{
		public override int EnableAudioCaptureDevice(bool enabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioCaptureDevice(enabled);
		}

		public override int EnableAudioPlayDevice(bool enabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioPlayDevice(enabled);
		}

		public override bool IsAudioCaptureDeviceEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioCaptureDeviceEnabled() != 0;
		}

		public override bool IsAudioPlayDeviceEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioPlayDeviceEnabled() != 0;
		}

		public override int EnableAudioSend(bool isEnabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioSend(isEnabled);
		}

		public override int EnableAudioRecv(bool isEnabled)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableAudioRecv(isEnabled);
		}

		public override bool IsAudioSendEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioSendEnabled() != 0;
		}

		public override bool IsAudioRecvEnabled()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsAudioRecvEnabled() != 0;
		}

		public override int EnableMic(bool isEnabled)
		{
			int num = this.EnableAudioCaptureDevice(isEnabled);
			int num2 = this.EnableAudioSend(isEnabled);
			if (num == QAVError.OK && num2 == QAVError.OK)
			{
				return QAVError.OK;
			}
			if (num != QAVError.OK)
			{
				return num;
			}
			return num2;
		}

		public override int GetMicState()
		{
			return (!this.IsAudioCaptureDeviceEnabled() || !this.IsAudioSendEnabled()) ? 0 : 1;
		}

		public override int EnableSpeaker(bool isEnabled)
		{
			int num = this.EnableAudioPlayDevice(isEnabled);
			int num2 = this.EnableAudioRecv(isEnabled);
			if (num == QAVError.OK && num2 == QAVError.OK)
			{
				return QAVError.OK;
			}
			if (num != QAVError.OK)
			{
				return num;
			}
			return num2;
		}

		public override int GetSpeakerState()
		{
			return (!this.IsAudioPlayDeviceEnabled() || !this.IsAudioRecvEnabled()) ? 0 : 1;
		}

		public override int GetMicLevel()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetMicLevel();
		}

		public override int SetMicVolume(int volume)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetMicVolume(volume);
		}

		public override int GetMicVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetMicVolume();
		}

		public override int GetSpeakerLevel()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetSpeakerLevel();
		}

		public override int SetSpeakerVolume(int volume)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_SetSpeakerVolume(volume);
		}

		public override int GetSpeakerVolume()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetSpeakerVolume();
		}

		public override int GetVolumeById(string openid)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetVolumeByUin(openid);
		}

		public override int EnableLoopBack(bool enable)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableLoopBack(enable);
		}

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public override event QAVAudioRouteChangeCallback OnAudioRouteChangeComplete;

		[MonoPInvokeCallback(typeof(QAVAudioRouteChangeCallback))]
		public static void s_OnAudioRouteChangeComplete(int code)
		{
			if (QAVContext.GetInstance().GetAudioCtrlInner().OnAudioRouteChangeComplete != null)
			{
				QAVContext.GetInstance().GetAudioCtrlInner().OnAudioRouteChangeComplete(code);
			}
		}

		public override int AddAudioBlackList(string openId)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_AddAudioBlackList(openId);
		}

		public override int RemoveAudioBlackList(string openId)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_RemoveAudioBlackList(openId);
		}

		public override int GetSendStreamLevel()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetSendStreamLevel();
		}

		public override int GetRecvStreamLevel(string openId)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_GetRecvStreamLevel(openId);
		}

		public override int InitSpatializer(string modelPath)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_InitSpatializer(modelPath);
		}

		public override int EnableSpatializer(bool enable, bool applyTeam)
		{
			return QAVNative.QAVSDK_AVAudioCtrl_EnableSpatializer(enable, applyTeam);
		}

		public override bool IsEnableSpatializer()
		{
			return QAVNative.QAVSDK_AVAudioCtrl_IsEnableSpatializer();
		}

		~QAVAudioCtrl()
		{
		}
	}
}
