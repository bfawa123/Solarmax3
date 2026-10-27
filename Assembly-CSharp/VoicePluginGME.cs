using System;
using Solarmax;
using TencentMobileGaming;
using UnityEngine;

public class VoicePluginGME : Solarmax.Singleton<VoicePluginGME>, IVoicePlugin
{
	private static string UserIdToOpenId(int userId)
	{
		return ((long)userId * 10000L).ToString();
	}

	private static int OpenIdToUserId(string openId)
	{
		long num = 0L;
		if (!long.TryParse(openId, out num))
		{
			return 0;
		}
		num /= 10000L;
		return (int)num;
	}

	public void SetAppId(string appId)
	{
		this.mAppId = appId;
	}

	public void Init(int userId)
	{
		if (string.IsNullOrEmpty(this.mAppId))
		{
			Debug.LogError("[VoicePluginGME]Empty app id!");
			return;
		}
		if (this.mInited)
		{
			return;
		}
		string logPath = string.Format("{0}/gme", Application.persistentDataPath);
		ITMGContext.GetInstance().SetLogPath(logPath);
		ITMGContext.GetInstance().OnEnterRoomCompleteEvent += this.OnEnterRoomComplete;
		ITMGContext.GetInstance().OnExitRoomCompleteEvent += this.OnExitRoomComplete;
		ITMGContext.GetInstance().OnRoomDisconnectEvent += this.OnRoomDisconnect;
		ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent += this.OnEndpointsUpdateInfo;
		string appVersion = UpgradeUtil.GetAppVersion();
		ITMGContext.GetInstance().SetAppVersion(appVersion);
		string openID = VoicePluginGME.UserIdToOpenId(userId);
		int num = ITMGContext.GetInstance().Init(this.mAppId, openID);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]Init failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		this.mUserId = userId;
		this.mOpenId = openID;
		this.mInited = true;
		Debug.LogFormat("[VoicePluginGME]Init success, appId: {0}, userId: {1}, openId: {2}", new object[]
		{
			this.mAppId,
			this.mUserId,
			this.mOpenId
		});
	}

	public void Destroy()
	{
		if (!this.mInited)
		{
			return;
		}
		ITMGContext.GetInstance().OnEnterRoomCompleteEvent -= this.OnEnterRoomComplete;
		ITMGContext.GetInstance().OnExitRoomCompleteEvent -= this.OnExitRoomComplete;
		ITMGContext.GetInstance().OnRoomDisconnectEvent -= this.OnRoomDisconnect;
		ITMGContext.GetInstance().OnEndpointsUpdateInfoEvent -= this.OnEndpointsUpdateInfo;
		ITMGContext.GetInstance().Uninit();
		this.mUserId = 0;
		this.mOpenId = null;
		this.mRoomId = null;
		this.mTeamId = 0;
		this.mInited = false;
	}

	public void Poll()
	{
		if (!this.mInited)
		{
			return;
		}
		QAVNative.QAVSDK_Poll();
	}

	public void EnterRoom(string roomId, int teamId, bool enableRoomChannel, byte[] authBuffer)
	{
		this.ExitRoom();
		if (authBuffer == null || authBuffer.Length < 1)
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnterRoom null authBuffer", new object[0]);
			return;
		}
		teamId++;
		int num = ITMGContext.GetInstance().SetRangeAudioTeamID(teamId);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]SetRangeAudioTeamID {0} failed with code {1}", new object[]
			{
				this.mTeamId,
				num
			});
			return;
		}
		this.EnableRoomChannel(enableRoomChannel);
		num = ITMGContext.GetInstance().EnterRoom(roomId, ITMGRoomType.ITMG_ROOM_TYPE_FLUENCY, authBuffer);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnterRoom {0} failed with code {1}", new object[]
			{
				roomId,
				num
			});
			return;
		}
		this.mRoomId = roomId;
		this.mTeamId = teamId;
		this.mAuthBuffer = authBuffer;
		this.mEnableSpeaker = false;
		this.mEnableMicrophone = false;
		Debug.LogFormat("[VoicePluginGME]EnterRoom {0}, teameId: {1}", new object[]
		{
			this.mRoomId,
			this.mTeamId
		});
	}

	public void ExitRoom()
	{
		int num = ITMGContext.GetInstance().ExitRoom();
		if (QAVError.OK != num && QAVError.ERR_HAS_IN_THE_STATE != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]ExitRoom failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		this.mRoomId = null;
		this.mTeamId = 0;
		this.mAuthBuffer = null;
		this.mEnableSpeaker = false;
		this.mEnableMicrophone = false;
		Debug.Log("[VoicePluginGME]ExitRoom");
	}

	public void OnReconnectResume()
	{
		if (string.IsNullOrEmpty(this.mRoomId))
		{
			Debug.Log("[VoicePluginGME]OnReconnectResume has not enter room.");
			return;
		}
		int num = ITMGContext.GetInstance().ExitRoom();
		if (QAVError.OK != num && QAVError.ERR_HAS_IN_THE_STATE != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]OnReconnectResume ExitRoom failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		num = ITMGContext.GetInstance().SetRangeAudioTeamID(this.mTeamId);
		if (QAVError.OK != num)
		{
			Debug.LogWarningFormat("[VoicePluginGME]OnReconnectResume SetRangeAudioTeamID {0} failed with code {1}", new object[]
			{
				this.mTeamId,
				num
			});
		}
		num = ITMGContext.GetInstance().SetRangeAudioMode(this.mRangeAudioMode);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]OnReconnectResume SetRangeAudioMode {0} failed with code {1}", new object[]
			{
				this.mRangeAudioMode,
				num
			});
			return;
		}
		num = ITMGContext.GetInstance().EnterRoom(this.mRoomId, ITMGRoomType.ITMG_ROOM_TYPE_FLUENCY, this.mAuthBuffer);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]OnReconnectResume EnterRoom {0} failed with code {1}", new object[]
			{
				this.mRoomId,
				num
			});
			return;
		}
		Debug.LogFormat("[VoicePluginGME]OnReconnectResume EnterRoom {0}, teameId: {1}", new object[]
		{
			this.mRoomId,
			this.mTeamId
		});
	}

	public bool EnableMicrophone(bool b)
	{
		if (!ITMGContext.GetInstance().IsRoomEntered())
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnableMicrophone {0} failed, not in room", new object[]
			{
				b
			});
			return false;
		}
		if (!this.EnableMicrophoneImpl(b))
		{
			return false;
		}
		this.mEnableMicrophone = b;
		return true;
	}

	public bool EnableSpeaker(bool b)
	{
		if (!ITMGContext.GetInstance().IsRoomEntered())
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnableSpeaker {0} failed, not in room", new object[]
			{
				b
			});
			return false;
		}
		if (!this.EnableSpeakerImpl(b))
		{
			return false;
		}
		this.mEnableSpeaker = b;
		return true;
	}

	private bool EnableMicrophoneImpl(bool b)
	{
		if (b && !ITMGContext.GetInstance().GetAudioCtrl().IsAudioCaptureDeviceEnabled())
		{
			int num = ITMGContext.GetInstance().GetAudioCtrl().EnableAudioCaptureDevice(true);
			if (QAVError.OK != num)
			{
				Debug.LogErrorFormat("[VoicePluginGME]EnableAudioCaptureDevice {0} failed with code {1}", new object[]
				{
					b,
					num
				});
				return false;
			}
		}
		int num2 = ITMGContext.GetInstance().GetAudioCtrl().EnableAudioSend(b);
		if (QAVError.OK != num2)
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnableMicrophone {0} failed with code {1}", new object[]
			{
				b,
				num2
			});
			return false;
		}
		Debug.LogFormat("[VoicePluginGME]EnableMicrophone {0} success", new object[]
		{
			b
		});
		return true;
	}

	private bool EnableSpeakerImpl(bool b)
	{
		int num = ITMGContext.GetInstance().GetAudioCtrl().EnableAudioRecv(b);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnableSpeaker {0} failed with code {1}", new object[]
			{
				b,
				num
			});
			return false;
		}
		Debug.LogFormat("[VoicePluginGME]EnableSpeaker {0} success", new object[]
		{
			b
		});
		return true;
	}

	public bool IsMicrophoneEnable()
	{
		return ITMGContext.GetInstance().GetAudioCtrl().IsAudioSendEnabled();
	}

	public bool IsSpeakerEnable()
	{
		return ITMGContext.GetInstance().GetAudioCtrl().IsAudioRecvEnabled();
	}

	public bool EnableRoomChannel(bool b)
	{
		ITMGRangeAudioMode rangeAudioMode = (!b) ? ITMGRangeAudioMode.ITMG_RANGE_AUDIO_MODE_TEAM : ITMGRangeAudioMode.ITMG_RANGE_AUDIO_MODE_WORLD;
		int num = ITMGContext.GetInstance().SetRangeAudioMode(rangeAudioMode);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]EnableRoomChannel {0} failed with code {1}", new object[]
			{
				b,
				num
			});
			return false;
		}
		this.mRangeAudioMode = rangeAudioMode;
		Debug.LogFormat("[VoicePluginGME]EnableRoomChannel {0} success", new object[]
		{
			b
		});
		return true;
	}

	public bool IsRoomChannelEnable()
	{
		return this.mRangeAudioMode == ITMGRangeAudioMode.ITMG_RANGE_AUDIO_MODE_WORLD;
	}

	public void Pause()
	{
		if (string.IsNullOrEmpty(this.mRoomId))
		{
			return;
		}
		int num = ITMGContext.GetInstance().Pause();
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]Pause failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		Debug.Log("[VoicePluginGME]Pause");
	}

	public void Resume()
	{
		if (string.IsNullOrEmpty(this.mRoomId))
		{
			return;
		}
		int num = ITMGContext.GetInstance().Resume();
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]Resume failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		Debug.Log("[VoicePluginGME]Resume");
	}

	public void OnEnterRoomComplete(int result, string error_info)
	{
		Debug.LogFormat("[VoicePluginGME]OnEnterRoomComplete, result: {0}, error_info: {1}", new object[]
		{
			result,
			error_info
		});
		ITMGContext.GetInstance().GetAudioCtrl().EnableAudioPlayDevice(true);
		this.EnableSpeakerImpl(this.mEnableSpeaker);
		this.EnableMicrophoneImpl(this.mEnableMicrophone);
		int num = ITMGContext.GetInstance().GetRoom().UpdateAudioRecvRange(1);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]UpdateAudioRecvRange 1 failed with code {0}", new object[]
			{
				num
			});
			return;
		}
		Debug.Log("[VoicePluginGME]UpdateAudioRecvRange 1");
		for (int i = 0; i < VoicePluginGME.Pos.Length; i++)
		{
			VoicePluginGME.Pos[i] = this.mTeamId * 100;
		}
		num = ITMGContext.GetInstance().GetRoom().UpdateSelfPosition(VoicePluginGME.Pos, VoicePluginGME.AxisForward, VoicePluginGME.AxisRight, VoicePluginGME.AxisUp);
		if (QAVError.OK != num)
		{
			Debug.LogErrorFormat("[VoicePluginGME]UpdateSelfPosition ({0}, {1}, {2}) failed with code {3}", new object[]
			{
				VoicePluginGME.Pos[0],
				VoicePluginGME.Pos[1],
				VoicePluginGME.Pos[2],
				num
			});
			return;
		}
		Debug.LogFormat("[VoicePluginGME]UpdateSelfPosition ({0}, {1}, {2})", new object[]
		{
			VoicePluginGME.Pos[0],
			VoicePluginGME.Pos[1],
			VoicePluginGME.Pos[2]
		});
	}

	public void OnExitRoomComplete()
	{
		Debug.Log("[VoicePluginGME]OnExitRoomComplete");
	}

	public void OnRoomDisconnect(int result, string error_info)
	{
		Debug.Log("[VoicePluginGME]OnRoomDisconnect");
	}

	public void OnEndpointsUpdateInfo(int eventID, int count, string[] openIdList)
	{
		if (eventID == ITMGContext.EVENT_ID_ENDPOINT_ENTER)
		{
			foreach (string text in openIdList)
			{
				int num = VoicePluginGME.OpenIdToUserId(text);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerJoinVoiceRoom, new object[]
				{
					num
				});
				Debug.LogFormat("[VoicePluginGME]Memeber {0} enter room", new object[]
				{
					text
				});
			}
		}
		else if (eventID == ITMGContext.EVENT_ID_ENDPOINT_EXIT)
		{
			foreach (string text2 in openIdList)
			{
				int num2 = VoicePluginGME.OpenIdToUserId(text2);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerLeaveVoiceRoom, new object[]
				{
					num2
				});
				Debug.LogFormat("[VoicePluginGME]Member {0} exit room", new object[]
				{
					text2
				});
			}
		}
		else if (eventID == ITMGContext.EVENT_ID_ENDPOINT_HAS_AUDIO)
		{
			foreach (string openId in openIdList)
			{
				int num3 = VoicePluginGME.OpenIdToUserId(openId);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerSpeekStart, new object[]
				{
					num3
				});
				Debug.LogFormat("[VoicePluginGME]Member {0} exit speaking...", new object[]
				{
					num3
				});
			}
		}
		else if (eventID == ITMGContext.EVENT_ID_ENDPOINT_NO_AUDIO)
		{
			foreach (string openId2 in openIdList)
			{
				int num4 = VoicePluginGME.OpenIdToUserId(openId2);
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.PlayerSpeekEnd, new object[]
				{
					num4
				});
				Debug.LogFormat("[VoicePluginGME]Member {0} speak stoped", new object[]
				{
					num4
				});
			}
		}
	}

	// Note: this type is marked as 'beforefieldinit'.
	static VoicePluginGME()
	{
		float[] array = new float[3];
		array[0] = 1f;
		VoicePluginGME.AxisRight = array;
		float[] array2 = new float[3];
		array2[1] = 1f;
		VoicePluginGME.AxisUp = array2;
		VoicePluginGME.Pos = new int[]
		{
			1,
			1,
			1
		};
	}

	private static readonly float[] AxisForward = new float[]
	{
		0f,
		0f,
		1f
	};

	private static readonly float[] AxisRight;

	private static readonly float[] AxisUp;

	private static int[] Pos;

	private string mAppId;

	private int mUserId;

	private string mOpenId;

	private string mRoomId;

	private int mTeamId;

	private ITMGRangeAudioMode mRangeAudioMode;

	private byte[] mAuthBuffer;

	private bool mEnableSpeaker;

	private bool mEnableMicrophone;

	private bool mInited;
}
