using System;
using Solarmax;

public class OldLocalPvpStorage : Solarmax.Singleton<OldLocalPvpStorage>, OldILocalStorage
{
	public string Name()
	{
		return "PVP_RECORD";
	}

	public void Save(OldLocalStorageSystem manager)
	{
		manager.PutInt(this.days);
		manager.PutInt(this.pvpWin);
		manager.PutInt(this.pvpDestroy);
	}

	public void Load(OldLocalStorageSystem manager)
	{
		this.days = manager.GetInt();
		this.pvpWin = manager.GetInt();
		this.pvpDestroy = manager.GetInt();
	}

	public void Clear(OldLocalStorageSystem manager)
	{
	}

	public int days;

	public int pvpWin;

	public int pvpDestroy;
}
