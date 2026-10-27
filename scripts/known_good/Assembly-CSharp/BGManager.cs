using System;
using System.IO;
using Solarmax;
using UnityEngine;

public class BGManager : MonoBehaviour
{
	public static BGManager Inst { get; private set; }

	private void Awake()
	{
		if (null != BGManager.Inst)
		{
			Debug.LogError("Create BGManager repeat!");
			UnityEngine.Object.Destroy(BGManager.Inst);
			BGManager.Inst = null;
		}
		BGManager.Inst = this;
		this.mBackgrounds[0] = GameObject.Find("Battle/Sky/BG").AddComponent<BGGroup>();
		this.mBackgrounds[0].Init();
		this.mBackgrounds[1] = GameObject.Find("Battle/Sky/CG").AddComponent<BGGroup>();
		this.mBackgrounds[1].Init();
		this.mAirShipPrefab = LoadResManager.LoadRes("gameres/sprites/airship.prefab");
		this.mAirShipGo = UnityEngine.Object.Instantiate<GameObject>(this.mAirShipPrefab);
		this.mAirShipGo.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		this.mAirShip = this.mAirShipGo.GetComponent<AirShip>();
		Solarmax.Singleton<LocalSettingStorage>.Get().LoadSoundStorage();
	}

	private void Start()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BGManager Start", new object[0]);
		this.ApplySkinConfig(null, false);
	}

	private void OnDestroy()
	{
		BGManager.Inst = null;
	}

	private void Update()
	{
	}

	public void Scroll(float delta)
	{
		this.mBackgrounds[(int)this.mBackgroundType].Scroll(delta);
	}

	public void ChangeBackground(BackgroundType t)
	{
		if (t == this.mBackgroundType)
		{
			return;
		}
		this.mBackgrounds[(int)this.mBackgroundType].SetVisible(false);
		this.mBackgroundType = t;
		this.mBackgrounds[(int)this.mBackgroundType].SetVisible(true);
	}

	public void SetTexture(BackgroundType t, int imageIndex, Texture2D tex)
	{
		Solarmax.Singleton<LoggerSystem>.Get().Info("BGManager Set Texture", new object[0]);
		if (null == tex)
		{
			return;
		}
		this.mBackgrounds[(int)t].SetTexture(imageIndex, tex);
	}

	public void SetTextureAll(BackgroundType t, Texture2D tex)
	{
		if (null == tex)
		{
			return;
		}
		BGGroup bggroup = this.mBackgrounds[(int)t];
		if (bggroup == null)
		{
			return;
		}
		bggroup.SetTextureAll(tex);
	}

	public bool DownloadBG(SkinConfig config)
	{
		if (config == null)
		{
			return false;
		}
		bool flag = false;
		string[] array = config.bgImage.Split(new char[]
		{
			','
		});
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string text = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[i] + ".ab";
				FileInfo fileInfo = new FileInfo(text);
				if (config.url.StartsWith("http") && !fileInfo.Exists)
				{
					Debug.LogFormat("DownLoad skin to {0}", new object[]
					{
						text
					});
					MonoSingleton<UpdateSystem>.Instance.AddDownLoad(config.url.ToLower() + "/android/" + array[i].ToLower() + ".ab", text, 0L);
					flag = true;
				}
			}
		}
		if (flag)
		{
			MonoSingleton<UpdateSystem>.Instance.StartDownLoad();
		}
		return flag;
	}

	public void ApplyLastSkinConfig()
	{
		if (this.lastSkinConfig == null || !this.lastSkinConfig.unlock)
		{
			this.ApplySkinConfig(null, false);
			return;
		}
		if (this.mSkinConfig == this.lastSkinConfig)
		{
			return;
		}
		this.ApplySkinConfig(this.lastSkinConfig, false);
	}

	public void ApplySkinConfig(SkinConfig config, bool force)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BGManager  ApplySkinConfig", new object[0]);
		if (config == null)
		{
			int skinId = Solarmax.Singleton<LocalSettingStorage>.Get().GetSkinId();
			config = Solarmax.Singleton<SkinConfigProvider>.Get().GetData(skinId);
		}
		if (config == null)
		{
			return;
		}
		int bg = Solarmax.Singleton<LocalSettingStorage>.Get().bg;
		if (config.unlock)
		{
			Solarmax.Singleton<LocalSettingStorage>.Get().bg = config.id;
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
		}
		if (!force && this.mSkinConfig == config)
		{
			return;
		}
		SkinConfig skinConfig = this.mSkinConfig;
		SkinConfig skinConfig2 = this.lastSkinConfig;
		this.mSkinConfig = config;
		if (this.lastSkinConfig == null || this.mSkinConfig.unlock)
		{
			this.lastSkinConfig = this.mSkinConfig;
		}
		bool flag = false;
		string[] array = config.bgImage.Split(new char[]
		{
			','
		});
		GameObject gameObject = GameObject.Find("Sky/BG/game_bg1");
		foreach (string text in array)
		{
			string value = gameObject.GetComponent<SpriteRenderer>().sprite.texture.name.ToLower();
			if (text.Contains(value))
			{
				return;
			}
		}
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				string text2 = MonoSingleton<UpdateSystem>.Instance.saveSkin + array[j] + ".ab";
				FileInfo fileInfo = new FileInfo(text2);
				if (config.url.StartsWith("http") && !fileInfo.Exists)
				{
					Debug.LogFormat("DownLoad skin to {0}", new object[]
					{
						text2
					});
					MonoSingleton<UpdateSystem>.Instance.AddDownLoad(config.url.ToLower() + "/android/" + array[j].ToLower() + ".ab", text2, 0L);
					flag = true;
				}
				else
				{
					if (!config.url.StartsWith("http"))
					{
						text2 = config.url + array[j];
					}
					Debug.LogFormat("Load skin from {0}", new object[]
					{
						text2
					});
					Texture2D tex = Solarmax.Singleton<PortraitManager>.Get().TryGetTexture2DFromSkin(config.url, text2, array[j].ToLower());
					this.SetTexture(BackgroundType.Normal, j, tex);
				}
			}
		}
		if (flag)
		{
			MonoSingleton<UpdateSystem>.Instance.StartDownLoad();
			this.mSkinConfig = skinConfig;
			this.lastSkinConfig = skinConfig2;
			Solarmax.Singleton<LocalSettingStorage>.Get().bg = bg;
			Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalSetting();
		}
	}

	public void AirShipFly(int trackIndex)
	{
		this.SetAirShipVisible(true);
		if (null == this.mAirShip)
		{
			return;
		}
		this.mAirShip.Fly(trackIndex, 0f, ShowWindowParams.None);
	}

	public void AirShipFly(int trackIndex, float showWindowDelayTime, ShowWindowParams showWindowParams)
	{
		this.SetAirShipVisible(true);
		if (null == this.mAirShip)
		{
			return;
		}
		this.mAirShip.Fly(trackIndex, showWindowDelayTime, showWindowParams);
	}

	public void UpdateAirShipFly(float nt)
	{
		if (null == this.mAirShip)
		{
			return;
		}
		this.mAirShip.UpdateFly(nt);
	}

	public void SetAirShipVisible(bool b)
	{
		if (null == this.mAirShipGo)
		{
			return;
		}
		this.mAirShipGo.SetActive(b);
		if (!b)
		{
			this.SetMotionVectorLength(0f);
		}
	}

	public void SetMotionVectorLength(float f)
	{
		this.mBackgrounds[(int)this.mBackgroundType].SetMotionVectorLength(f);
	}

	public SkinConfig CurrentSkinConfig()
	{
		return this.mSkinConfig;
	}

	public void EmptySkinConfig()
	{
		if (this.mSkinConfig == null)
		{
			return;
		}
		this.mSkinConfig = null;
	}

	public static readonly int SPIdMotionVectorLength = Shader.PropertyToID("_MotionVectorLength");

	public static readonly int DEFAULT_BG_ID = 103;

	private BackgroundType mBackgroundType;

	private readonly BGGroup[] mBackgrounds = new BGGroup[2];

	private GameObject mAirShipPrefab;

	private GameObject mAirShipGo;

	private AirShip mAirShip;

	private SkinConfig lastSkinConfig;

	private SkinConfig mSkinConfig;
}
