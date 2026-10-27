using System;
using System.Collections.Generic;
using UnityEngine;

namespace Solarmax
{
	public sealed class LocalStorageSystem : Solarmax.Singleton<LocalStorageSystem>, Lifecycle
	{
		public LocalStorageSystem()
		{
			this.storageList = new List<ILocalStorage>();
			this.saveTempName = string.Empty;
		}

		public bool IsAccountRelatedDataLoaded { get; private set; }

		public bool Init()
		{
			this.RegisterLocalStorage(Solarmax.Singleton<LocalAccountStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalSettingStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalChaptersStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalPvpStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalOrderStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalAchievementStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalTaskStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalLevelScoreStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<LocalPvpSeasonSystem>.Get());
			this.IsAccountRelatedDataLoaded = false;
			return true;
		}

		public void Tick(float interval)
		{
			if (!this.needSaveDisk)
			{
				return;
			}
			this.SaveStorage();
			this.needSaveDisk = false;
		}

		public void Destroy()
		{
			this.SaveStorage();
			this.storageList.Clear();
		}

		private void RegisterLocalStorage(ILocalStorage storage)
		{
			if (!this.storageList.Contains(storage))
			{
				this.storageList.Add(storage);
			}
		}

		public bool LoadAccountRelated(string account)
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info("LoadAccountRelated", new object[0]);
			if (string.IsNullOrEmpty(account))
			{
				return false;
			}
			this.acountTempName = account;
			for (int i = 0; i < this.storageList.Count; i++)
			{
				ILocalStorage localStorage = this.storageList[i];
				this.saveTempName = localStorage.Name() + this.acountTempName;
				localStorage.Load(this);
			}
			this.IsAccountRelatedDataLoaded = true;
			return true;
		}

		public void SaveStorage()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Info("LocalStorageSystem  SaveStorage", new object[0]);
			if (!this.needSaveDisk)
			{
				return;
			}
			for (int i = 0; i < this.storageList.Count; i++)
			{
				ILocalStorage localStorage = this.storageList[i];
				this.saveTempName = localStorage.Name() + this.acountTempName;
				localStorage.Save(this);
			}
			PlayerPrefs.Save();
			this.needSaveDisk = false;
		}

		public void SaveLocalAccount(bool bSaveTime = false)
		{
			this.saveTempName = Solarmax.Singleton<LocalAccountStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalAccountStorage>.Get().Save(this);
		}

		public void SaveLocalChapters()
		{
			this.saveTempName = Solarmax.Singleton<LocalChaptersStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalChaptersStorage>.Get().Save(this);
		}

		public void SaveLocalSetting()
		{
			this.saveTempName = Solarmax.Singleton<LocalSettingStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalSettingStorage>.Get().Save(this);
		}

		public void SaveLocalOrder()
		{
			this.saveTempName = Solarmax.Singleton<LocalOrderStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalOrderStorage>.Get().Save(this);
		}

		public void SaveLocalPvp(int days, int win, int destroy)
		{
			this.saveTempName = Solarmax.Singleton<LocalPvpStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalPvpStorage>.Get().days = days;
            Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin = win;
            Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy = destroy;
            Solarmax.Singleton<LocalPvpStorage>.Get().Save(this);
		}

		public void SaveLocalPvp(int win, int destroy)
		{
			this.saveTempName = Solarmax.Singleton<LocalPvpStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin = win;
            Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy = destroy;
            Solarmax.Singleton<LocalPvpStorage>.Get().Save(this);
		}

		public void SaveLocalAchievement()
		{
			this.saveTempName = Solarmax.Singleton<LocalAchievementStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalAchievementStorage>.Get().Save(this);
		}

		public void SaveLocalTask()
		{
			this.saveTempName = Solarmax.Singleton<LocalTaskStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalTaskStorage>.Get().Save(this);
		}

		public void SaveLocalLevelScore()
		{
			this.saveTempName = Solarmax.Singleton<LocalLevelScoreStorage>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalLevelScoreStorage>.Get().Save(this);
		}

		public void SaveLocalSeason()
		{
			this.saveTempName = Solarmax.Singleton<LocalPvpSeasonSystem>.Get().Name() + this.acountTempName;
            Solarmax.Singleton<LocalPvpSeasonSystem>.Get().Save(this);
		}

		public void DeleteStorage()
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.Save();
		}

		public void ClearStorage()
		{
			for (int i = 0; i < this.storageList.Count; i++)
			{
				ILocalStorage localStorage = this.storageList[i];
				localStorage.Clear(this);
			}
		}

		public void SetLastLoginAccountId(string accountId, bool isVisitor)
		{
			if (string.IsNullOrEmpty(accountId))
			{
				return;
			}
			PlayerPrefs.SetString(LocalStorageSystem.LastLoginAccountIdKey, accountId);
			PlayerPrefs.SetInt(LocalStorageSystem.LastLoginAsVisitorKey, (!isVisitor) ? 0 : 1);
            Solarmax.Singleton<LoggerSystem>.Instance.Info("SetLastLoginAccountId: {0}, isVisitor: {1}", new object[]
			{
				accountId,
				isVisitor
			});
		}

		public string GetLastLoginAccountId()
		{
			string @string = PlayerPrefs.GetString(LocalStorageSystem.LastLoginAccountIdKey, string.Empty);
            Solarmax.Singleton<LoggerSystem>.Instance.Info("GetLastLoginAccountId: {0}", new object[]
			{
				@string
			});
			return @string;
		}

		public void SetLastLoginAsVisitor(bool b)
		{
			PlayerPrefs.SetInt(LocalStorageSystem.LastLoginAsVisitorKey, (!b) ? 0 : 1);
            Solarmax.Singleton<LoggerSystem>.Instance.Info("SetLastLoginAsVisitor: {0}", new object[]
			{
				b
			});
		}

		public bool IsLastLoginAsVisitor()
		{
			bool flag = PlayerPrefs.GetInt(LocalStorageSystem.LastLoginAsVisitorKey, 0) == 1;
            Solarmax.Singleton<LoggerSystem>.Instance.Info("IsLastLoginAsVisitor: {0}", new object[]
			{
				flag
			});
			return flag;
		}

		public void NeedSaveToDisk()
		{
			this.needSaveDisk = true;
		}

		public int GetInt(string key, int defaultValue = 0)
		{
			return PlayerPrefs.GetInt(this.saveTempName + key, defaultValue);
		}

		public bool GetBool(string key, bool defaultValue = false)
		{
			return PlayerPrefs.GetInt(this.saveTempName + key, (!defaultValue) ? 0 : 1) != 0;
		}

		public float GetFloat(string key, float defaultValue = 0f)
		{
			return PlayerPrefs.GetFloat(this.saveTempName + key, defaultValue);
		}

		public long GetLong(string key)
		{
			long num = (long)PlayerPrefs.GetInt(this.saveTempName + key + "0");
			long num2 = (long)PlayerPrefs.GetInt(this.saveTempName + key + "1");
			return num << 32 | num2;
		}

		public string GetString(string key, string defaultValue = "")
		{
			return PlayerPrefs.GetString(this.saveTempName + key, defaultValue);
		}

		public void PutInt(string key, int data)
		{
			PlayerPrefs.SetInt(this.saveTempName + key, data);
		}

		public void PutBool(string key, bool data)
		{
			PlayerPrefs.SetInt(this.saveTempName + key, (!data) ? 0 : 1);
		}

		public void PutFloat(string key, float data)
		{
			PlayerPrefs.SetFloat(this.saveTempName + key, data);
		}

		public void PutLong(string key, long data)
		{
			PlayerPrefs.SetInt(this.saveTempName + key + "0", (int)(data >> 32));
			PlayerPrefs.SetInt(this.saveTempName + key + "1", (int)data);
		}

		public void PutString(string key, string data)
		{
			PlayerPrefs.SetString(this.saveTempName + key, data);
		}

		public void LoadStorage()
		{
			this.LoadAccountRelated("0");
		}

		private static readonly string LastLoginAccountIdKey = "LastLoginAccountId";

		private static readonly string LastLoginAsVisitorKey = "LastLoginAsVisitor";

		private bool needSaveDisk;

		public List<ILocalStorage> storageList;

		private string saveTempName;

		private string acountTempName = string.Empty;
	}
}
