using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Solarmax
{
	public class UpdateSystem : MonoSingleton<UpdateSystem>
	{
		public static string GetDllPath()
		{
			return string.Format("/data/data/{0}/files/{1}", Application.identifier, UpdateSystem.DllName);
		}

		public bool Init()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				this.saveRoot = Application.dataPath + "/cache";
				this.saveVideo = Application.dataPath + "/video2/";
				this.saveSkin = Application.dataPath + "/skin/";
			}
			else if (Application.platform == RuntimePlatform.OSXEditor)
			{
				this.saveRoot = Application.persistentDataPath;
				this.saveVideo = Application.persistentDataPath + "/video2/";
				this.saveSkin = Application.persistentDataPath + "/skin/";
			}
			else if (Application.platform == RuntimePlatform.Android)
			{
				this.saveRoot = Application.persistentDataPath;
				this.saveVideo = Application.persistentDataPath + "/video2/";
				this.saveSkin = Application.persistentDataPath + "/skin/";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				this.saveRoot = Application.persistentDataPath;
				this.saveVideo = Application.persistentDataPath + "/video2/";
				this.saveSkin = Application.persistentDataPath + "/skin/";
			}
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
			{
				this.saveRoot = Application.dataPath + "/cache";
				this.saveVideo = Application.dataPath + "/video2/";
				this.saveSkin = Application.dataPath + "/skin/";
			}
			if (!this.saveRoot.EndsWith("/"))
			{
				this.saveRoot += "/";
			}
			return true;
		}

		public void Tick(float interval)
		{
		}

		public void Destroy()
		{
		}

		public void CheckUpgrade(bool forceRequest)
		{
			UpdateSystem.IsLoginUpdate = false;
			this.dllChanged = false;
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("CheckUpgradeStart");
			UpgradeUtil.CheckUpgrade(forceRequest, new Action(this.OnGetUpgradeInfoResponse));
		}

		private void OnGetUpgradeInfoResponse()
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("CheckUpgradeResponse");
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("NeedNotUppgrade");
			Debug.Log("OnGetUpgradeInfoResponse: Already newest version.");
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnABDownloadingFinished, new object[0]);
		}

		private void SaveAsset2Local(string path, WWW www)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			int length = path.LastIndexOf('/');
			string path2 = path.Substring(0, length);
			if (!Directory.Exists(path2))
			{
				Directory.CreateDirectory(path2);
			}
			FileStream fileStream = new FileStream(path, FileMode.Create);
			fileStream.Write(www.bytes, 0, www.bytes.Length);
			fileStream.Flush();
			fileStream.Close();
			fileStream.Dispose();
		}

		private void LoadFileListLocal()
		{
			this.localFiles.Clear();
			string text = LoadResManager.LoadTextFile("filelist.xml", ResLoadPathType.PersistentFirst);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			UpgradeUtil.ParseFileList(text, this.localFiles);
		}

		private void LoadFileListServer()
		{
			this.serverFiles.Clear();
			string downloadUrl = this.GetDownloadUrl("filelist.xml");
			using (WWW www = new WWW(downloadUrl))
			{
				while (!www.isDone)
				{
				}
				if (www.error != null)
				{
					Debug.LogErrorFormat("Download {0} failed", new object[]
					{
						downloadUrl
					});
					this.CheckUpgrade(true);
				}
				else
				{
					UpgradeUtil.ParseFileList(www.text, this.serverFiles);
				}
			}
		}

		private void SaveVersionConfig()
		{
			Debug.Log("UpdateSystem: Save VersionConfig...");
			if (UpgradeRequest.Response == null)
			{
				Debug.LogError("SaveVersionConfig: Null UpgradeRequest.Response");
				return;
			}
			VersionConfig cfg = new VersionConfig
			{
				VersionName = UpgradeRequest.Response.version_name,
				VersionCode = UpgradeRequest.Response.version_code
			};
			UpgradeUtil.SaveVersionConfig(cfg);
			Debug.Log("UpdateSystem: Save VersionConfig complete.");
		}

		private void SaveFileList2Xml()
		{
			Debug.Log("UpdateSystem: Save FileList...");
			string path = string.Format("{0}/filelist.xml", LoadResManager.rootPathPersistent);
			string directoryName = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			UpgradeUtil.SaveFileList(path, this.serverFiles);
			Debug.Log("UpdateSystem: Save FileList complete.");
		}

		private string GetDownloadUrl(string path)
		{
			if (UpgradeRequest.Response == null)
			{
				Debug.LogError("GetDownloadUrl: Null UpgradeRequest.Response");
				return null;
			}
			return Path.Combine(UpgradeRequest.Response.update_url, path);
		}

		private void GeneriUpdateFiles()
		{
			this.downList.Clear();
			foreach (KeyValuePair<string, TableAsset> keyValuePair in this.serverFiles)
			{
				string key = keyValuePair.Key;
				TableAsset tableAsset = null;
				this.localFiles.TryGetValue(key, out tableAsset);
				if (tableAsset == null)
				{
					this.AddAssetDownload(keyValuePair.Value);
				}
				else if (tableAsset != null && !tableAsset.GUID.Equals(keyValuePair.Value.GUID))
				{
					this.AddAssetDownload(keyValuePair.Value);
				}
			}
			Debug.LogFormat("UpdateSystem: Will download {0} files.", new object[]
			{
				this.downList.Count
			});
		}

		public void ReloadClient()
		{
			Debug.Log("UpdateSystem: ReloadClient...");
			UpdateSystem.IsLoginUpdate = false;
			this.localFiles.Clear();
			this.serverFiles.Clear();
            Solarmax.Singleton<DataProviderSystem>.Instance.Destroy();
            Solarmax.Singleton<DataProviderSystem>.Instance.Init();
			UpgradeUtil.ReLoadGameCofig();
			LoadResManager.ReLoadManifest();
			Debug.Log("UpdateSystem: ReloadClient complete.");
		}

		private IEnumerator DownloadAssets(string url, string path)
		{
			Debug.LogFormat("Download {0} -> {1} start...", new object[]
			{
				url,
				path
			});
			using (WWW www = new WWW(url))
			{
				while (!www.isDone)
				{
					yield return 1;
				}
				if (www.error != null)
				{
					Debug.LogWarningFormat("Download asset {0} error: {1}. Download paused", new object[]
					{
						url,
						www.error
					});
					UpdateSystem.IsPauseDownLoad = true;
					yield break;
				}
				UpdateSystem.downLoadIndex++;
				byte[] bytes = www.bytes;
				this.SaveAsset2Local(path, www);
				Debug.LogFormat("Download {0} -> {1} success.", new object[]
				{
					url,
					path
				});
                Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUpdateDownLoad, new object[]
				{
					(long)bytes.Length
				});
                Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUpdateDownLoad2, new object[]
				{
					1
				});
			}
			yield break;
		}

		private void AddAssetDownload(TableAsset remoteAsset)
		{
			string text;
			if (remoteAsset.assetBundlePath == UpdateSystem.DllName)
			{
				text = UpdateSystem.GetDllPath();
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				this.dllChanged = true;
			}
			else
			{
				text = LoadResManager.rootPathPersistent + remoteAsset.assetBundlePath;
			}
			UpdateSystem.IsLoginUpdate = true;
			string downloadUrl = this.GetDownloadUrl(remoteAsset.assetBundlePath);
			this.AddDownLoad(downloadUrl, text, remoteAsset.fileSize);
		}

		public void AddDownLoad(string url, string savePath, long nSize = 0L)
		{
			if (string.IsNullOrEmpty(url))
			{
				Debug.LogError("AddDownLoad: Null url");
				return;
			}
			if (this.IsExit(url))
			{
				return;
			}
			DownTable item = new DownTable
			{
				Url = url,
				path = savePath
			};
			this.nDownAllSize += nSize;
			this.downList.Add(item);
			Debug.LogFormat("Add download file: {0}", new object[]
			{
				url
			});
		}

		public bool IsDownLoadIng()
		{
			return this.downList.Count > 0;
		}

		private bool IsExit(string url)
		{
			for (int i = 0; i < this.downList.Count; i++)
			{
				if (this.downList[i].Equals(url))
				{
					return true;
				}
			}
			return false;
		}

		public void StartDownLoad()
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("HotUpdateStartDownload");
			long num = this.nDownAllSize;
			long num2 = 0L;
			UpdateSystem.downLoadIndex = 0;
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartDownLoad, new object[]
			{
				num2,
				num
			});
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnStartDownLoad2, new object[]
			{
				num2,
				this.downList.Count
			});
			base.StartCoroutine(this.StartUpdate());
		}

		public IEnumerator StartUpdate()
		{
			Debug.Log("UpdateSystem: Update start...");
			this.downList.Clear();
			while (UpdateSystem.downLoadIndex < this.downList.Count)
			{
				DownTable v = this.downList[UpdateSystem.downLoadIndex];
				if (UpdateSystem.IsPauseDownLoad)
				{
					if (Application.internetReachability != NetworkReachability.NotReachable)
					{
						UpdateSystem.IsPauseDownLoad = false;
					}
					yield return new WaitForSeconds(2f);
				}
				yield return this.DownloadAssets(v.Url, v.path);
				v = null;
				v = null;
			}
			this.downList.Clear();
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("HotUpdateDownloadComplete");
			Debug.Log("UpdateSystem: Download files complete.");
			Debug.Log("UpdateSystem: Update complete.");
			MonoSingleton<FlurryAnalytis>.Instance.LogEvent("HotUpdateSuccess");
            Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnABDownloadingFinished, new object[0]);
			yield break;
		}

		public void DownloadEditMapInfo()
		{
		}

		public static readonly string DllName = "game_ass.dll";

		private static bool IsLoginUpdate;

		public static bool IsPauseDownLoad;

		public static int downLoadIndex;

		private bool dllChanged;

		public string saveRoot;

		public string saveVideo;

		public string saveSkin;

		private long nDownAllSize;

		private List<DownTable> downList = new List<DownTable>();

		private Dictionary<string, TableAsset> localFiles = new Dictionary<string, TableAsset>();

		private Dictionary<string, TableAsset> serverFiles = new Dictionary<string, TableAsset>();
	}
}
