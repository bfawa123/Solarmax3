using System;

public interface IVoicePlugin
{
	void SetAppId(string appId);

	void Init(int userId);

	void Destroy();

	void Poll();

	void EnterRoom(string roomId, int teamId, bool enableRoomChannel, byte[] authBuffer);

	void ExitRoom();

	void OnReconnectResume();

	bool EnableMicrophone(bool b);

	bool EnableSpeaker(bool b);

	bool IsMicrophoneEnable();

	bool IsSpeakerEnable();

	bool EnableRoomChannel(bool b);

	bool IsRoomChannelEnable();

	void Pause();

	void Resume();
}
