using System;
using System.Collections.Generic;
using System.Text;
using Solarmax;
using UnityEngine;

public class AudioManger : Solarmax.Singleton<AudioManger>
{
	private AudioSource bgAudio { get; set; }

	private AudioSource effectAudio { get; set; }

	public void Init()
	{
		this.bgAudio = GameObject.Find("bg").GetComponent<AudioSource>();
		if (this.bgAudio == null)
		{
			Debug.LogError("Find bg audio source is error!");
			return;
		}
		this.bgAudio.volume = Solarmax.Singleton<LocalSettingStorage>.Get().GetMusicWithoutAccount();
		this.effectAudio = GameObject.Find("effect").GetComponent<AudioSource>();
		if (this.bgAudio == null)
		{
			Debug.LogError("Find bg audio source is error!");
			return;
		}
		this.effectAudio.volume = Solarmax.Singleton<LocalSettingStorage>.Get().GetSoundWithoutAccount();
		this.effectAudioParent = this.effectAudio.transform.parent.gameObject;
		for (int i = 0; i < 2; i++)
		{
			this.CreateNewEffectAudio();
		}
	}

	public void Pause()
	{
		if (this.bgAudio != null)
		{
			this.bgAudio.Pause();
		}
		if (this.effectAudio != null)
		{
			this.effectAudio.Pause();
		}
		for (int i = 0; i < this.effectAudioList.Count; i++)
		{
			this.effectAudioList[i].Pause();
		}
	}

	public void UnPause()
	{
		if (this.bgAudio != null)
		{
			this.bgAudio.UnPause();
		}
		if (this.effectAudio != null)
		{
			this.effectAudio.UnPause();
		}
		for (int i = 0; i < this.effectAudioList.Count; i++)
		{
			this.effectAudioList[i].UnPause();
		}
	}

	private void PlayAudioEffect(string audioName, float volume = 1f, float pan = 0f)
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.silent)
		{
			return;
		}
		AudioSource freeEffectAudioSource = this.GetFreeEffectAudioSource();
		if (freeEffectAudioSource == null)
		{
			return;
		}
		AudioClip audioClip = Solarmax.Singleton<AssetManager>.Get().GetResources(audioName) as AudioClip;
		if (audioClip == null)
		{
			return;
		}
		freeEffectAudioSource.panStereo = pan;
		freeEffectAudioSource.loop = false;
		freeEffectAudioSource.PlayOneShot(audioClip);
	}

	private void PlayAudioEffect(AudioClip clip, float volume)
	{
		if (clip == null)
		{
			return;
		}
		AudioSource freeEffectAudioSource = this.GetFreeEffectAudioSource();
		if (freeEffectAudioSource == null)
		{
			return;
		}
		freeEffectAudioSource.panStereo = 0f;
		freeEffectAudioSource.loop = false;
		freeEffectAudioSource.PlayOneShot(clip);
	}

	private AudioSource GetFreeEffectAudioSource()
	{
		AudioSource audioSource = null;
		int i = 0;
		int count = this.effectAudioList.Count;
		while (i < count)
		{
			if (!this.effectAudioList[i].isPlaying)
			{
				audioSource = this.effectAudioList[i];
				break;
			}
			i++;
		}
		if (audioSource == null)
		{
			if (this.effectAudioList.Count < 6)
			{
				audioSource = this.CreateNewEffectAudio();
			}
			else
			{
				if (this.effectAudioIndex >= 6)
				{
					this.effectAudioIndex = 0;
				}
				audioSource = this.effectAudioList[this.effectAudioIndex++];
			}
		}
		return audioSource;
	}

	private AudioSource CreateNewEffectAudio()
	{
		GameObject gameObject = this.effectAudioParent.AddChild(this.effectAudio.gameObject);
		gameObject.name = "effect-" + this.effectAudioList.Count;
		AudioSource component = gameObject.GetComponent<AudioSource>();
		component.volume = Solarmax.Singleton<LocalSettingStorage>.Get().sound;
		this.effectAudioList.Add(component);
		return component;
	}

	public void MuteEffectAudio(bool mute)
	{
		if (this.effectAudio == null)
		{
			return;
		}
		this.effectAudio.mute = mute;
	}

	public void PlayAudioBG(string audio, float volume = 0.5f)
	{
		if (this.bgAudio.isPlaying && !this.bgAudio.mute && this.nowPlayAudioBG.Equals(audio))
		{
			return;
		}
		this.nowPlayAudioBG = audio;
		this.PlayAudioBGA(audio, volume);
	}

	public void ChangeBGVolume(float audioVolume)
	{
		if (this.bgAudio != null)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().music = audioVolume;
			this.bgAudioVolume = audioVolume;
			this.bgAudio.volume = audioVolume;
		}
	}

	public void ChangeSoundVolume(float audioVolume)
	{
		this.soundVolume = audioVolume;
		Solarmax.Singleton<LocalSettingStorage>.Get().sound = audioVolume;
		int i = 0;
		int count = this.effectAudioList.Count;
		while (i < count)
		{
			this.effectAudioList[i].volume = audioVolume;
			i++;
		}
	}

	public float GetBGVolume()
	{
		if (this.bgAudio != null)
		{
			return this.bgAudio.volume;
		}
		return -1f;
	}

	private void PlayAudioBGA(string audio, float volume)
	{
		AudioClip clip = this.bgAudio.clip;
		string audioLower = audio.ToLower();
		this.bgAudio.clip = LoadResManager.LoadSound(string.Format("gameres/sounds/{0}.mp3", audioLower));
		if (this.bgAudio.clip == null)
		{
			bool flag = audio.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase) || audio.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) || audio.EndsWith(".wav", StringComparison.OrdinalIgnoreCase);
			if (flag)
			{
				this.bgAudio.clip = LoadResManager.LoadCustomSound(audio);
				if (this.bgAudio.clip == null)
				{
					this.bgAudio.clip = LoadResManager.LoadCustomSound(audioLower);
				}
			}
			else
			{
				string[] array = new string[]
				{
					".ogg",
					".wav",
					".mp3"
				};
				for (int i = 0; i < array.Length; i++)
				{
					this.bgAudio.clip = LoadResManager.LoadCustomSound(audio + array[i]);
					if (this.bgAudio.clip != null)
					{
						break;
					}
					this.bgAudio.clip = LoadResManager.LoadCustomSound(audioLower + array[i]);
					if (this.bgAudio.clip != null)
					{
						break;
					}
				}
			}
		}

		Resources.UnloadAsset(clip);
		if (this.bgAudio.clip != null)
		{
			this.bgAudio.loop = true;
			this.bgAudio.Play();
		}
	}

	public void MuteBGAudio(bool mute)
	{
		if (this.bgAudio == null)
		{
			return;
		}
		this.bgAudio.Stop();
		this.bgAudio.mute = mute;
	}

	private float GetAudioPan(Vector3 pos)
	{
		if (UICamera.currentCamera != null)
		{
			pos = UICamera.currentCamera.WorldToScreenPoint(pos);
		}
		return (pos.x / (float)Screen.width - 0.5f) * 2f;
	}

	public void PlayJumpCharge(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_JumpCharge))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("jumpCharge", 0.6f, audioPan);
	}

	public void PlayJumpStart(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_JumpStart))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("jumpStart", 0.6f, audioPan);
	}

	public void PlayJumpEnd(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_JumpEnd))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("jumpEnd", 0.6f, audioPan);
	}

	public void PlayCapture(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Capture))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("capture", 1f, audioPan);
	}

	public void PlayTwist(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Twist))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("T_Twist_tra", 1f, audioPan);
	}

	public void PlayClone(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Clone))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("T_Clone_tra", 1f, audioPan);
	}

	public void PlayLaser(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Laser))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("laser", 1f, audioPan);
	}

	public void PlayExlposion(Vector3 pos)
	{
		float audioPan = this.GetAudioPan(pos);
		int value = UnityEngine.Random.Range(1, 8);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("explosion0");
		stringBuilder.Append(value);
		this.PlayAudioEffect(stringBuilder.ToString(), 0.7f, audioPan);
	}

	public void PlayWarpCharge(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_WarpCharge))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("warp_charge", 0.7f, audioPan);
	}

	public void PlayWarp(Vector3 pos)
	{
		if (!Solarmax.Singleton<GameTimeManager>.Get().CheckTimer(TimerType.T_Warp))
		{
			return;
		}
		float audioPan = this.GetAudioPan(pos);
		this.PlayAudioEffect("warp", 0.7f, audioPan);
	}

	public void PlayEffect(string name)
	{
		this.PlayAudioEffect(name, 1f, 0f);
	}

	public void PlayEffect(AudioClip clip, float volumn)
	{
		this.PlayAudioEffect(clip, volumn);
	}

	public void PlayPlanetExplosion(Vector3 pos)
	{
		this.PlayAudioEffect("PlanetExplosion", 1f, 0f);
	}

	public float bgAudioVolume = 1f;

	public float soundVolume = 1f;

	private GameObject effectAudioParent;

	private List<AudioSource> effectAudioList = new List<AudioSource>();

	private int effectAudioIndex;

	private const int DefaultEffectNum = 2;

	private const int DefaultEffectMax = 6;

	private string nowPlayAudioBG = string.Empty;
}
