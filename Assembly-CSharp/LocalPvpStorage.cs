using System;
using Solarmax;

public class LocalPvpStorage : Solarmax.Singleton<LocalPvpStorage>, ILocalStorage
{
	public string Name()
	{
		return "PVP_RECORD1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutInt("days", this.days);
		manager.PutInt("pvpWin", this.pvpWin);
		manager.PutInt("pvpDestroy", this.pvpDestroy);
		manager.PutInt("lookAds", this.lookAds);
		manager.PutInt("pve", this.pve);
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.days = manager.GetInt("days", 0);
		this.pvpWin = manager.GetInt("pvpWin", 0);
		this.pvpDestroy = manager.GetInt("pvpDestroy", 0);
		this.lookAds = manager.GetInt("lookAds", 0);
		this.pve = manager.GetInt("pve", 0);
	}

	public void Clear(LocalStorageSystem manager)
	{
		this.lookAds = 0;
		this.pve = 0;
	}

	public void Clear()
	{
		this.lookAds = 0;
		this.pve = 0;
	}

	public int days;

	public int pvpWin;

	public int pvpDestroy;

	public int pvpType;

	public int seasonStart;

	public int seasonEnd;

	public int lookAds;

	public int pve;

	public int ver = 1;
}
