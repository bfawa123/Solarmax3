using System;
using System.Collections.Generic;
using NetMessage;
using UnityEngine;

namespace Solarmax
{
	public sealed class OldLocalStorageSystem : Solarmax.Singleton<OldLocalStorageSystem>, Lifecycle
	{
		public OldLocalStorageSystem()
		{
			this.appVersion = string.Empty;
			this.storageList = new List<OldILocalStorage>();
			this.saveTempIndex = 0;
			this.saveTempName = string.Empty;
		}

		public bool Init()
		{
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("LocalStorageSystem    init   begin", new object[0]);
			this.RegisterLocalStorage(Solarmax.Singleton<OldLocalAccountStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<OldLocalSettingStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<OldLocalChaptersStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<OldLocalLevelStorage>.Get());
			this.RegisterLocalStorage(Solarmax.Singleton<OldLocalPvpStorage>.Get());
			this.LoadLocalStorage();
            Solarmax.Singleton<LoggerSystem>.Instance.Debug("OldLocalStorageSystem    init   end", new object[0]);
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

		private void RegisterLocalStorage(OldILocalStorage storage)
		{
			if (!this.storageList.Contains(storage))
			{
				this.storageList.Add(storage);
			}
		}

		public void LoadLocalStorage()
		{
			OldILocalStorage oldILocalStorage = Solarmax.Singleton<OldLocalAccountStorage>.Get();
			this.saveTempName = oldILocalStorage.Name();
			this.saveTempIndex = 0;
			oldILocalStorage.Load(this);
			oldILocalStorage = Solarmax.Singleton<OldLocalSettingStorage>.Get();
			this.saveTempName = oldILocalStorage.Name();
			this.saveTempIndex = 0;
			oldILocalStorage.Load(this);
		}

		public CSUploadOldVersionData LoadUploadStorage()
		{
			OldILocalStorage oldILocalStorage = Solarmax.Singleton<OldLocalChaptersStorage>.Get();
			this.saveTempName = oldILocalStorage.Name();
			this.saveTempIndex = 0;
			oldILocalStorage.Load(this);
			return OldLocalStorageSystem.oldData;
		}

		public void SaveStorage()
		{
			if (!this.needSaveDisk)
			{
				return;
			}
			PlayerPrefs.SetString("_LocalStorageVersion_", this.appVersion);
			for (int i = 0; i < this.storageList.Count; i++)
			{
				OldILocalStorage oldILocalStorage = this.storageList[i];
				this.saveTempName = oldILocalStorage.Name();
				this.saveTempIndex = 0;
				oldILocalStorage.Save(this);
			}
			PlayerPrefs.Save();
			this.needSaveDisk = false;
		}

		public void SaveLocalAccount(bool bSaveTime = false)
		{
			this.saveTempName = Solarmax.Singleton<LocalAccountStorage>.Get().Name();
			this.saveTempIndex = 0;
			if (bSaveTime)
			{
				DateTime d = new DateTime(1970, 1, 1);
				long regtimeSaveFile = (long)(DateTime.Now - d).TotalSeconds;
                Solarmax.Singleton<OldLocalAccountStorage>.Get().regtimeSaveFile = regtimeSaveFile;
			}
            Solarmax.Singleton<OldLocalAccountStorage>.Get().Save(this);
		}

		public void SaveLocalChapters()
		{
			this.saveTempName = Solarmax.Singleton<OldLocalChaptersStorage>.Get().Name();
			this.saveTempIndex = 0;
            Solarmax.Singleton<OldLocalChaptersStorage>.Get().Save(this);
		}

		public void SavePvp(int days, int win, int destroy)
		{
			this.saveTempIndex = 0;
			this.saveTempName = Solarmax.Singleton<OldLocalPvpStorage>.Get().Name();
            Solarmax.Singleton<OldLocalPvpStorage>.Get().days = days;
            Solarmax.Singleton<OldLocalPvpStorage>.Get().pvpWin = win;
            Solarmax.Singleton<OldLocalPvpStorage>.Get().pvpDestroy = destroy;
            Solarmax.Singleton<OldLocalPvpStorage>.Get().Save(this);
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
				OldILocalStorage oldILocalStorage = this.storageList[i];
				this.saveTempIndex = 0;
				oldILocalStorage.Clear(this);
			}
		}

		public void NeedSaveToDisk()
		{
			this.needSaveDisk = true;
		}

		public void SetAppVersion(string version)
		{
			this.appVersion = version;
		}

		public bool VarifyVersion()
		{
			Debug.LogFormat("游戏版本：[{0}]  本地数据版本：[{1}]", new object[]
			{
				this.appVersion,
				this.storageVersion
			});
			bool result = true;
			string[] array = this.storageVersion.Split(new char[]
			{
				'.'
			});
			string[] array2 = this.appVersion.Split(new char[]
			{
				'.'
			});
			if (array.Length != array2.Length)
			{
				result = false;
			}
			for (int i = 0; i < array2.Length - 1; i++)
			{
				if (!array[i].Equals(array2[i]))
				{
					result = false;
					break;
				}
			}
			return result;
		}

		public int GetInt()
		{
			return PlayerPrefs.GetInt(this.saveTempName + ++this.saveTempIndex);
		}

		public float GetFloat()
		{
			return PlayerPrefs.GetFloat(this.saveTempName + ++this.saveTempIndex);
		}

		public long GetLong()
		{
			long num = (long)PlayerPrefs.GetInt(this.saveTempName + ++this.saveTempIndex);
			long num2 = (long)PlayerPrefs.GetInt(this.saveTempName + ++this.saveTempIndex);
			return num << 32 | num2;
		}

		public string GetString()
		{
			return PlayerPrefs.GetString(this.saveTempName + ++this.saveTempIndex);
		}

		public void PutChar(char data)
		{
			PlayerPrefs.SetInt(this.saveTempName + ++this.saveTempIndex, (int)data);
		}

		public void PutShort(short data)
		{
			PlayerPrefs.SetInt(this.saveTempName + ++this.saveTempIndex, (int)data);
		}

		public void PutInt(int data)
		{
			PlayerPrefs.SetInt(this.saveTempName + ++this.saveTempIndex, data);
		}

		public void PutFloat(float data)
		{
			PlayerPrefs.SetFloat(this.saveTempName + ++this.saveTempIndex, data);
		}

		public void PutLong(long data)
		{
			PlayerPrefs.SetInt(this.saveTempName + ++this.saveTempIndex, (int)(data >> 32));
			PlayerPrefs.SetInt(this.saveTempName + ++this.saveTempIndex, (int)data);
		}

		public void PutString(string data)
		{
			PlayerPrefs.SetString(this.saveTempName + ++this.saveTempIndex, data);
		}

		public static string OLD_LOCAL_SAVE_TAG = "------老版本存储数据标志{0}";

		public static CSUploadOldVersionData oldData = new CSUploadOldVersionData();

		private bool needSaveDisk;

		private string appVersion;

		private string storageVersion;

		public List<OldILocalStorage> storageList;

		private int saveTempIndex;

		private string saveTempName;

		private const string StorageVersionMark = "_LocalStorageVersion_";
	}
}
