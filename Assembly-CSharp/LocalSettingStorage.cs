using System;
using Solarmax;
using UnityEngine;

public class LocalSettingStorage : Solarmax.Singleton<LocalSettingStorage>, ILocalStorage
{
	public string Name()
	{
		return "LocalSettingStorage1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutFloat("music", this.music);
		manager.PutFloat("sound", this.sound);
		manager.PutInt("effectLevel", this.effectLevel);
		manager.PutInt("fightOption", this.fightOption);
		manager.PutString("serverUrl", this.serverUrl);
		manager.PutInt("sliderMode", this.sliderMode);
		manager.PutInt("bg", this.bg);
		manager.PutBool("enableSpeaker", this.enableSpeaker);
		manager.PutBool("enableMicrophone", this.enableMicrophone);
		manager.PutBool("enableRoomChannel", this.enableRoomChannel);
		manager.PutBool("enableTeamChannel", this.enableTeamChannel);
		this.SaveMusicWithoutAccount();
		this.SaveSoundWithoutAccount();
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.music = manager.GetFloat("music", 0f);
		this.sound = manager.GetFloat("sound", 0f);
		this.effectLevel = manager.GetInt("effectLevel", 0);
		this.fightOption = manager.GetInt("fightOption", 0);
		this.serverUrl = manager.GetString("serverUrl", string.Empty);
		this.sliderMode = manager.GetInt("sliderMode", 0);
		this.bg = manager.GetInt("bg", BGManager.DEFAULT_BG_ID);
		this.enableSpeaker = manager.GetBool("enableSpeaker", true);
		this.enableMicrophone = manager.GetBool("enableMicrophone", true);
		this.enableRoomChannel = manager.GetBool("enableRoomChannel", false);
		this.enableTeamChannel = manager.GetBool("enableTeamChannel", false);
		if (string.IsNullOrEmpty(this.serverUrl))
		{
			this.music = this.GetMusicWithoutAccount();
			this.sound = this.GetSoundWithoutAccount();
		}
	}

	public void Clear(LocalStorageSystem manager)
	{
	}

	public int GetSkinId()
	{
		if (Solarmax.Singleton<LocalStorageSystem>.Instance.IsAccountRelatedDataLoaded)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("GetSkinId(LocalSetting): {0}", new object[]
			{
				this.bg
			});
			return this.bg;
		}
		string lastLoginAccountId = Solarmax.Singleton<LocalStorageSystem>.Instance.GetLastLoginAccountId();
		if (string.IsNullOrEmpty(lastLoginAccountId))
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("GetSkinId(EmptyLastLoginAccountId): {0}", new object[]
			{
				0
			});
			return 0;
		}
		string key = string.Format("{0}{1}bg", this.Name(), lastLoginAccountId);
		int @int = PlayerPrefs.GetInt(key, 0);
		Solarmax.Singleton<LoggerSystem>.Instance.Info("GetSkinId(LastLoginAccountId): {0}", new object[]
		{
			@int
		});
		return @int;
	}

	public void LoadSoundStorage()
	{
		string lastLoginAccountId = Solarmax.Singleton<LocalStorageSystem>.Instance.GetLastLoginAccountId();
		if (string.IsNullOrEmpty(lastLoginAccountId))
		{
			return;
		}
		this.music = PlayerPrefs.GetFloat(string.Format("{0}{1}music", this.Name(), lastLoginAccountId), 0f);
		this.sound = PlayerPrefs.GetFloat(string.Format("{0}{1}sound", this.Name(), lastLoginAccountId), 0f);
	}

	public void SaveMusicWithoutAccount()
	{
		PlayerPrefs.SetFloat("LocalSettingStorage1_Music", this.music);
	}

	public float GetMusicWithoutAccount()
	{
		return PlayerPrefs.GetFloat("LocalSettingStorage1_Music", 0.8f);
	}

	public void SaveSoundWithoutAccount()
	{
		PlayerPrefs.SetFloat("LocalSettingStorage1_Sound", this.sound);
	}

	public float GetSoundWithoutAccount()
	{
		return PlayerPrefs.GetFloat("LocalSettingStorage1_Sound", 1f);
	}

	public float music = 1f;

	public float sound = 1f;

	public int effectLevel;

	public int fightOption;

	public string serverUrl = string.Empty;

	public int lobbyWindowType;

	public int sliderMode;

	public int bg;

	public int ver = 1;

	public bool enableSpeaker;

	public bool enableMicrophone;

	public bool enableRoomChannel;

	public bool enableTeamChannel;
}
