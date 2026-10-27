using System;
using Solarmax;
using UnityEngine;

public static class VoiceEngine
{
	public static void SetAppId(string appId)
	{
		VoiceEngine.mPlugin.SetAppId(appId);
	}

	public static void Init(int userId)
	{
		VoiceEngine.mPlugin.Init(userId);
	}

	public static void Destroy()
	{
		VoiceEngine.mPlugin.Destroy();
	}

	public static void EnterRoom(string roomId, int teamId, bool enableRoomChannel, byte[] authBuffer)
	{
		VoiceEngine.mPlugin.EnterRoom(roomId, teamId, enableRoomChannel, authBuffer);
	}

	public static void ExitRoom()
	{
		VoiceEngine.mPlugin.ExitRoom();
	}

	public static void OnReconnectResume()
	{
		VoiceEngine.mPlugin.OnReconnectResume();
	}

	public static void Poll()
	{
		VoiceEngine.mTimeSinceLastUpdate += Time.unscaledDeltaTime;
		if (VoiceEngine.mTimeSinceLastUpdate < 0.033333335f)
		{
			return;
		}
		VoiceEngine.mTimeSinceLastUpdate = 0f;
		try
		{
			VoiceEngine.mPlugin.Poll();
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("[Voice]Poll exception. \n{0}", new object[]
			{
				ex.ToString()
			});
		}
	}

	public static bool EnableMicrophone(bool b)
	{
		return VoiceEngine.mPlugin.EnableMicrophone(b);
	}

	public static bool EnableSpeaker(bool b)
	{
		return VoiceEngine.mPlugin.EnableSpeaker(b);
	}

	public static bool IsMicrophoneEnable()
	{
		return VoiceEngine.mPlugin.IsMicrophoneEnable();
	}

	public static bool IsSpeakerEnable()
	{
		return VoiceEngine.mPlugin.IsSpeakerEnable();
	}

	public static bool EnableRoomChannel(bool b)
	{
		return VoiceEngine.mPlugin.EnableRoomChannel(b);
	}

	public static bool IsRoomChannelEnable()
	{
		return VoiceEngine.mPlugin.IsRoomChannelEnable();
	}

	public static void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus)
		{
			VoiceEngine.mPlugin.Resume();
		}
		else
		{
			VoiceEngine.mPlugin.Pause();
		}
	}

	private const float TickInterval = 0.033333335f;

	private static float mTimeSinceLastUpdate;

	private static IVoicePlugin mPlugin = Solarmax.Singleton<VoicePluginGME>.Get();
}
