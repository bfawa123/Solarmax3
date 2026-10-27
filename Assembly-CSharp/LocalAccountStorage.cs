using System;
using Solarmax;
using UnityEngine;

public class LocalAccountStorage : Solarmax.Singleton<LocalAccountStorage>, ILocalStorage
{
	public string Name()
	{
		return "LocalAccountStorage";
	}

	public void Save(LocalStorageSystem manager)
	{
		manager.PutInt("Version", this.ver);
		manager.PutString("account", this.account);
		manager.PutString("name", this.name);
		manager.PutString("icon", this.icon);
		manager.PutString("singleCurrentLevel", this.singleCurrentLevel);
		manager.PutString("guideFightLevel", this.guideFightLevel);
		manager.PutInt("localLanguage", this.localLanguage);
		manager.PutLong("regtimeSaveFile", this.regtimeSaveFile);
	}

	public void Load(LocalStorageSystem manager)
	{
		this.ver = manager.GetInt("Version", 0);
		this.account = manager.GetString("account", string.Empty);
		this.name = manager.GetString("name", string.Empty);
		this.icon = manager.GetString("icon", string.Empty);
		this.singleCurrentLevel = manager.GetString("singleCurrentLevel", string.Empty);
		this.guideFightLevel = manager.GetString("guideFightLevel", string.Empty);
		this.regtimeSaveFile = manager.GetLong("regtimeSaveFile");
		int @int = manager.GetInt("localLanguage", -1);
		if (@int >= 0)
		{
			this.localLanguage = @int;
		}
	}

	public void Clear(LocalStorageSystem manager)
	{
		this.account = string.Empty;
		this.name = string.Empty;
		this.icon = string.Empty;
		this.singleCurrentLevel = string.Empty;
		this.guideFightLevel = string.Empty;
		this.localLanguage = 0;
		this.regtimeSaveFile = 0L;
	}

	public int GetLocalLaugue()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("Get Local Laugue", new object[0]);
		if (this.localLanguage >= 0)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Info("GetLocalLaugue: {0} from account", new object[]
			{
				this.localLanguage
			});
			return this.localLanguage;
		}
		string lastLoginAccountId = Solarmax.Singleton<LocalStorageSystem>.Instance.GetLastLoginAccountId();
		if (!string.IsNullOrEmpty(lastLoginAccountId))
		{
			string key = string.Format("{0}{1}localLanguage", this.Name(), lastLoginAccountId);
			this.localLanguage = PlayerPrefs.GetInt(key, -1);
			Solarmax.Singleton<LoggerSystem>.Instance.Info("GetLocalLaugue: {0} from LastLoginAccount", new object[]
			{
				this.localLanguage
			});
			return this.localLanguage;
		}
		Solarmax.Singleton<LoggerSystem>.Instance.Info("GetLocalLaugue: empty LastLoginAccount", new object[0]);
		return -1;
	}

	public string account = string.Empty;

	public string name = string.Empty;

	public int localLanguage = -1;

	public string icon = string.Empty;

	public long regtimeSaveFile;

	public string singleCurrentLevel = string.Empty;

	public string guideFightLevel = string.Empty;

	public string token = "unity_test";

	public bool webTest = true;

	public int ver = 1;
}
