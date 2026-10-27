using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using GameCore.Loader;
using MiGameSDK;
using UnityEngine;

namespace Solarmax
{
	public static class UpgradeUtil
	{
		public static bool CleanupHotUpdateRes(bool restartIfCleanup)
		{
			bool flag = false;
			if (Directory.Exists(LoadResManager.rootPathPersistent))
			{
				Directory.Delete(LoadResManager.rootPathPersistent, true);
				Debug.LogFormat("CleanupHotUpdateRes: Cleanup {0}", new object[]
				{
					LoadResManager.rootPathPersistent
				});
				flag = true;
				UpgradeUtil.mVersionConfig = null;
				UpgradeUtil.mChannelConfig = null;
				UpgradeUtil.mGameConfig = null;
			}
			string dllPath = UpdateSystem.GetDllPath();
			if (!string.IsNullOrEmpty(dllPath) && File.Exists(dllPath))
			{
				File.Delete(dllPath);
				Debug.LogFormat("CleanupHotUpdateRes: Cleanup {0}", new object[]
				{
					dllPath
				});
				flag = true;
				if (restartIfCleanup)
				{
					Debug.LogFormat("CleanupHotUpdateRes: Need restart", new object[0]);
					UpgradeUtil.RestartGame();
				}
			}
			if (!flag)
			{
				Debug.LogFormat("CleanupHotUpdateRes: Nothing to cleanup", new object[0]);
			}
			return flag;
		}

		public static void CheckPersistentResVersion(bool restartIfCleanup)
		{
		}

		public static VersionConfig GetVersionConfig()
		{
			if (UpgradeUtil.mVersionConfig == null)
			{
				UpgradeUtil.mVersionConfig = LoadResManager.LoadJson<VersionConfig>("version_cfg.txt", ResLoadPathType.PersistentFirst);
			}
			return UpgradeUtil.mVersionConfig;
		}

		public static void SaveVersionConfig(VersionConfig cfg)
		{
			LoadResManager.SaveJson<VersionConfig>(cfg, "version_cfg.txt");
			UpgradeUtil.mVersionConfig = LoadResManager.LoadJson<VersionConfig>("version_cfg.txt", ResLoadPathType.PersistentFirst);
			Debug.LogFormat("SaveVersionConfig success. VersionName = {0}, VersionCode = {1}", new object[]
			{
				UpgradeUtil.mVersionConfig.VersionName,
				UpgradeUtil.mVersionConfig.VersionCode
			});
		}

		public static void ReLoadGameCofig()
		{
			UpgradeUtil.mGameConfig = LoadResManager.LoadJson<GameConfig>("cfg/game_cfg.txt", ResLoadPathType.PersistentFirst);
		}

		public static GameConfig GetGameConfig()
		{
			if (UpgradeUtil.mGameConfig == null)
			{
				UpgradeUtil.mGameConfig = LoadResManager.LoadJson<GameConfig>("cfg/game_cfg.txt", ResLoadPathType.PersistentFirst);
			}
			UpgradeUtil.mGameConfig.EnablePay = true;
			UpgradeUtil.mGameConfig.EnableFetchProducts = true;
			UpgradeUtil.mGameConfig.EnableVisitorLogin = true;
			return UpgradeUtil.mGameConfig;
		}

		public static ChannelConfig GetChannelConfig()
		{
			if (UpgradeUtil.mChannelConfig == null)
			{
				UpgradeUtil.mChannelConfig = LoadResManager.LoadJson<ChannelConfig>("channel_cfg.txt", ResLoadPathType.PersistentFirst);
			}
			return UpgradeUtil.mChannelConfig;
		}

		public static string GetAppVersion()
		{
			VersionConfig versionConfig = UpgradeUtil.GetVersionConfig();
			if (versionConfig == null)
			{
				return "UnknowVersion";
			}
			return UpgradeUtil.FormatAppVersion(versionConfig.VersionName, versionConfig.VersionCode);
		}

		public static string FormatAppVersion(string versionName, int versionCode)
		{
			return string.Format("{0}.{1}", versionName, versionCode);
		}

		public static string AesEncrypt(string key, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			string result;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged
			{
				Key = Encoding.UTF8.GetBytes(key),
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			})
			{
				using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor())
				{
					byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
					result = Convert.ToBase64String(array, 0, array.Length);
				}
			}
			return result;
		}

		public static string AesDecrypt(string key, string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			byte[] array = Convert.FromBase64String(text);
			string @string;
			using (RijndaelManaged rijndaelManaged = new RijndaelManaged
			{
				Key = Encoding.UTF8.GetBytes(key),
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			})
			{
				using (ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor())
				{
					byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
					@string = Encoding.UTF8.GetString(bytes);
				}
			}
			return @string;
		}

		private static void OpenAppUpgradeUrl()
		{
			if (UpgradeRequest.Response == null)
			{
				Debug.LogErrorFormat("OpenAppUpgradeUrl: Null UpgradeRequest.Response", new object[0]);
				return;
			}
			if (string.IsNullOrEmpty(UpgradeRequest.Response.update_url))
			{
				Debug.LogErrorFormat("OpenAppUpgradeUrl: Empty UpdateUrl", new object[0]);
				return;
			}
			Application.OpenURL(UpgradeRequest.Response.update_url);
			UpgradeUtil.QuitGame();
		}

		public static void QuitGame()
		{
			Application.Quit();
		}

		public static void RestartGame()
		{
			Debug.LogFormat("Restart game...", new object[0]);
			MiPlatformSDK.Restart(100);
		}

		public static void CheckNetwork(EventDelegate.Callback onSuccessCallback, long downloadSize)
		{
			onSuccessCallback();
		}

		public static void ShowAppUpgradeWindow()
		{
            Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
			string value = LanguageDataProvider.GetValue(2225);
			EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
			EventId id = EventId.OnCommonDialog;
			object[] array = new object[4];
			array[0] = 2;
			array[1] = value;
			int num = 2;
			if (UpgradeUtil.tmp2 == null)
			{
				UpgradeUtil.tmp2 = new EventDelegate.Callback(UpgradeUtil.OpenAppUpgradeUrl);
			}
			array[num] = new EventDelegate(UpgradeUtil.tmp2);
			int num2 = 3;
			if (UpgradeUtil.tmp3 == null)
			{
				UpgradeUtil.tmp3 = new EventDelegate.Callback(UpgradeUtil.QuitGame);
			}
			array[num2] = new EventDelegate(UpgradeUtil.tmp3);
			instance.FireEvent(id, array);
		}

		public static void ShowRestartAppWindow(int textKey)
		{
            Solarmax.Singleton<UISystem>.Instance.ShowWindow("CommonDialogWindow");
			string value = LanguageDataProvider.GetValue(textKey);
			EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
			EventId id = EventId.OnCommonDialog;
			object[] array = new object[4];
			array[0] = 2;
			array[1] = value;
			int num = 2;
			if (UpgradeUtil.tmp4 == null)
			{
				UpgradeUtil.tmp4 = new EventDelegate.Callback(UpgradeUtil.RestartGame);
			}
			array[num] = new EventDelegate(UpgradeUtil.tmp4);
			int num2 = 3;
			if (UpgradeUtil.tmp5 == null)
			{
				UpgradeUtil.tmp5 = new EventDelegate.Callback(UpgradeUtil.QuitGame);
			}
			array[num2] = new EventDelegate(UpgradeUtil.tmp5);
			instance.FireEvent(id, array);
		}

		public static void CheckUpgrade(bool forceRequest, Action onResponse)
		{
			onResponse();
		}

		public static bool ParseFileList(string txt, Dictionary<string, TableAsset> files)
		{
			files.Clear();
			if (string.IsNullOrEmpty(txt))
			{
				Debug.LogError("ParseFileList: Empty");
				return false;
			}
			XDocument xdocument = XDocument.Parse(txt);
			XElement xelement = xdocument.Element("Assets");
			if (xelement == null)
			{
				return false;
			}
			IEnumerable<XElement> enumerable = xelement.Elements("Asset");
			foreach (XElement em in enumerable)
			{
				TableAsset tableAsset = new TableAsset
				{
					assetPath = em.GetAttribute("id", string.Empty),
					GUID = em.GetAttribute("md5", string.Empty),
					fileSize = long.Parse(em.GetAttribute("fileSize", "0")),
					assetBundlePath = em.GetAttribute("bundlePath", string.Empty)
				};
				files.Add(tableAsset.assetPath, tableAsset);
			}
			Debug.LogFormat("ParseFileList success, fileCount: {0}", new object[]
			{
				files.Count
			});
			return true;
		}

		public static void SaveFileList(string path, Dictionary<string, TableAsset> files)
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			FileStream fileStream = new FileStream(path, FileMode.Create);
			XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, new UTF8Encoding(false));
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlTextWriter.WriteStartDocument();
			xmlTextWriter.WriteStartElement("Assets");
			foreach (KeyValuePair<string, TableAsset> keyValuePair in files)
			{
				xmlTextWriter.WriteStartElement("Asset");
				xmlTextWriter.WriteAttributeString("id", keyValuePair.Value.assetPath);
				xmlTextWriter.WriteAttributeString("md5", keyValuePair.Value.GUID);
				xmlTextWriter.WriteAttributeString("fileSize", keyValuePair.Value.fileSize.ToString());
				xmlTextWriter.WriteAttributeString("bundlePath", keyValuePair.Value.assetBundlePath);
				xmlTextWriter.WriteEndElement();
			}
			xmlTextWriter.WriteEndElement();
			xmlTextWriter.WriteEndDocument();
			xmlTextWriter.Close();
			fileStream.Close();
			Debug.LogFormat("SaveFileList {0} success, fileCount: {1}", new object[]
			{
				path,
				files.Count
			});
		}

		private static VersionConfig mVersionConfig;

		private static GameConfig mGameConfig;

		private static ChannelConfig mChannelConfig;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp0;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp1;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp2;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp3;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp4;

		[CompilerGenerated]
		private static EventDelegate.Callback tmp5;
	}
}
