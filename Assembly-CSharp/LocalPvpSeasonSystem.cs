using System;
using Solarmax;

public class LocalPvpSeasonSystem : Solarmax.Singleton<LocalPvpSeasonSystem>, ILocalStorage
{
	public string Name()
	{
		return "SEASON_RECORD1";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutInt("pvpType", this.pvpType);
		manager.PutInt("seasonStart", this.seasonStart);
		manager.PutInt("seasonEnd", this.seasonEnd);
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.pvpType = manager.GetInt("pvpType", 0);
		this.seasonStart = manager.GetInt("seasonStart", 0);
		this.seasonEnd = manager.GetInt("seasonEnd", 0);
	}

	public void Clear(LocalStorageSystem manager)
	{
	}

	public int pvpType;

	public int seasonStart;

	public int seasonEnd;

	public int ver = 1;
}
