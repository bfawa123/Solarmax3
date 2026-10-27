using System;
using Solarmax;

public class OldLocalLevelStorage : Solarmax.Singleton<OldLocalLevelStorage>, OldILocalStorage
{
	public string Name()
	{
		return "LocalLevelfails";
	}

	public void Save(OldLocalStorageSystem manager)
	{
		manager.PutInt(this.Levelfails);
		manager.PutInt(this.LevelID);
	}

	public void Load(OldLocalStorageSystem manager)
	{
		this.Levelfails = manager.GetInt();
		this.LevelID = manager.GetInt();
	}

	public void Clear(OldLocalStorageSystem manager)
	{
	}

	public void SetLevelInfo(string strLevel)
	{
		string s = strLevel.Substring(1);
		this.LevelID = int.Parse(s);
	}

	public int Levelfails;

	public int LevelID;
}
