using System;

namespace TencentMobileGaming
{
	public abstract class ITMGAudioCtrl
	{
		public abstract int EnableMic(bool isEnabled);

		public abstract int GetMicState();

		public abstract int EnableSpeaker(bool isEnabled);

		public abstract int GetSpeakerState();

		public abstract int EnableAudioCaptureDevice(bool enabled);

		public abstract int EnableAudioPlayDevice(bool enabled);

		public abstract bool IsAudioCaptureDeviceEnabled();

		public abstract bool IsAudioPlayDeviceEnabled();

		public abstract int EnableAudioSend(bool isEnabled);

		public abstract int EnableAudioRecv(bool isEnabled);

		public abstract bool IsAudioSendEnabled();

		public abstract bool IsAudioRecvEnabled();

		public abstract int GetMicLevel();

		public abstract int SetMicVolume(int volume);

		public abstract int GetMicVolume();

		public abstract int GetSpeakerLevel();

		public abstract int SetSpeakerVolume(int volume);

		public abstract int GetSpeakerVolume();

		public abstract int GetVolumeById(string openid);

		public abstract int EnableLoopBack(bool enable);

		public abstract event QAVAudioRouteChangeCallback OnAudioRouteChangeComplete;

		public abstract int AddAudioBlackList(string openId);

		public abstract int RemoveAudioBlackList(string openId);

		public abstract int GetSendStreamLevel();

		public abstract int GetRecvStreamLevel(string openId);

		public abstract int InitSpatializer(string modelPath);

		public abstract int EnableSpatializer(bool enable, bool applyTeam);

		public abstract bool IsEnableSpatializer();

		public static int AUDIOROUTE_OTHERS = -1;

		public static int AUDIOROUTE_BUILDINRECIEVER;

		public static int AUDIOROUTE_SPEAKER = 1;

		public static int AUDIOROUTE_HEADPHONE = 2;

		public static int AUDIOROUTE_BLUETOOTH = 3;
	}
}
