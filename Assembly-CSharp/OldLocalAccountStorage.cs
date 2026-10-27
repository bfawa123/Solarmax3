using System;
using Solarmax;

public class OldLocalAccountStorage : Solarmax.Singleton<OldLocalAccountStorage>, OldILocalStorage
{
	public string Name()
	{
		return "LocalAccountStorage";
	}

	public void Save(OldLocalStorageSystem manager)
	{
		manager.PutString(this.account);
		manager.PutString(this.name);
		manager.PutString(this.icon);
		manager.PutString(this.singleCurrentLevel);
		manager.PutString(this.guideFightLevel);
		manager.PutLong(this.timeStamp);
		manager.PutLong(this.regtimeStamp);
		manager.PutInt(this.localLanguage);
		manager.PutLong(this.regtimeSaveFile);
	}

	public void Load(OldLocalStorageSystem manager)
	{
		this.account = manager.GetString();
		this.name = manager.GetString();
		this.icon = manager.GetString();
		this.singleCurrentLevel = manager.GetString();
		this.guideFightLevel = manager.GetString();
		this.timeStamp = manager.GetLong();
		this.regtimeStamp = manager.GetLong();
		this.localLanguage = manager.GetInt();
		this.regtimeSaveFile = manager.GetLong();
		if (string.IsNullOrEmpty(Solarmax.Singleton<LocalAccountStorage>.Get().name))
		{
			Solarmax.Singleton<LocalAccountStorage>.Get().name = this.name;
		}
		if (Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage == -1)
		{
			Solarmax.Singleton<LocalAccountStorage>.Get().localLanguage = this.localLanguage;
		}
		if (string.IsNullOrEmpty(Solarmax.Singleton<LocalAccountStorage>.Get().icon))
		{
			Solarmax.Singleton<LocalAccountStorage>.Get().icon = this.icon;
		}
		Solarmax.Singleton<LocalStorageSystem>.Get().SaveLocalAccount(false);
	}

	public void Clear(OldLocalStorageSystem manager)
	{
		this.account = string.Empty;
		this.name = string.Empty;
		this.icon = string.Empty;
		this.singleCurrentLevel = string.Empty;
		this.guideFightLevel = string.Empty;
		this.timeStamp = 0L;
		this.regtimeStamp = 0L;
		this.localLanguage = 0;
		this.regtimeSaveFile = 0L;
	}

	public int GetDailysRewardMoney()
	{
		if (this.regtimeStamp > 0L)
		{
			DateTime d = new DateTime(1970, 1, 1);
			d = d.AddSeconds((double)this.regtimeStamp);
			TimeSpan timeSpan = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d;
			if (timeSpan.Days > 0)
			{
				return timeSpan.Days * 6;
			}
		}
		return 10000;
	}

	public string account = string.Empty;

	public string name = string.Empty;

	public int localLanguage = -1;

	public string icon = string.Empty;

	public long timeStamp;

	public long regtimeStamp;

	public long regtimeSaveFile;

	public string singleCurrentLevel = string.Empty;

	public string guideFightLevel = string.Empty;

	public string token = "unity_test";

	public bool webTest = true;
}
