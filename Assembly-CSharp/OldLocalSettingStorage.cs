using System;
using Solarmax;

public class OldLocalSettingStorage : Solarmax.Singleton<OldLocalSettingStorage>, OldILocalStorage
{
	public string Name()
	{
		return "LocalSettingStorage";
	}

	public void Save(OldLocalStorageSystem manager)
	{
		manager.PutInt((!this.music) ? 0 : 1);
		manager.PutInt((!this.sound) ? 0 : 1);
		manager.PutInt(this.effectLevel);
		manager.PutInt(this.fightOption);
		manager.PutString(this.ip);
		manager.PutInt(this.sliderMode);
	}

	public void Load(OldLocalStorageSystem manager)
	{
		this.music = (manager.GetInt() > 0);
		this.sound = (manager.GetInt() > 0);
		this.effectLevel = manager.GetInt();
		this.fightOption = manager.GetInt();
		this.ip = manager.GetString();
		this.sliderMode = manager.GetInt();
	}

	public void Clear(OldLocalStorageSystem manager)
	{
	}

	public bool music = true;

	public bool sound = true;

	public int effectLevel;

	public int fightOption;

	public string ip = string.Empty;

	public int lobbyWindowType;

	public int sliderMode;
}
