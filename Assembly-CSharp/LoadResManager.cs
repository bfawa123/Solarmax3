using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadResManager : Solarmax.Singleton<LoadResManager>
{
	public bool Init()
	{
		if (!this.bInitLoadResManager)
		{
			LoadResManager.rootPathEditor = Application.dataPath;
			LoadResManager.rootPathStreamAssets = Application.streamingAssetsPath + "/res/";
			LoadResManager.rootPathPersistent = Application.persistentDataPath + "/res/";
			LoadResManager.m_ResourceInfos = new Dictionary<string, ResourceObject>();
			this.bInitLoadResManager = true;
		}
		return true;
	}

	public static void ReLoadManifest()
	{
		if (LoadResManager.m_manifestAB != null)
		{
			LoadResManager.m_manifestAB.Unload(true);
			LoadResManager.m_manifestAB = null;
			LoadResManager.m_BundleManifest = null;
		}
		string text = LoadResManager.rootPathPersistent + "manifest.ab";
		FileInfo fileInfo = new FileInfo(text);
		if (!fileInfo.Exists)
		{
			text = LoadResManager.rootPathStreamAssets + "manifest.ab";
		}
		LoadResManager.m_manifestAB = AssetBundle.LoadFromFile(text);
		LoadResManager.m_BundleManifest = LoadResManager.m_manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
	}

	private static void LoadDependencyBymainfest(string path, out string[] dependencyAsset)
	{
		dependencyAsset = null;
		if (!string.IsNullOrEmpty(path))
		{
			if (LoadResManager.m_BundleManifest == null)
			{
				string text = LoadResManager.rootPathPersistent + "manifest.ab";
				FileInfo fileInfo = new FileInfo(text);
				if (!fileInfo.Exists)
				{
					text = LoadResManager.rootPathStreamAssets + "manifest.ab";
				}
				LoadResManager.m_manifestAB = AssetBundle.LoadFromFile(text);
				LoadResManager.m_BundleManifest = LoadResManager.m_manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			}
			string assetBundleName = string.Format("{0}{1}", path, LoadResManager.FileExt);
			string[] allDependencies = LoadResManager.m_BundleManifest.GetAllDependencies(assetBundleName);
			if (allDependencies.Length <= 0)
			{
				return;
			}
			int num = allDependencies.Length;
			string[] array = new string[num];
			dependencyAsset = array;
			for (int i = 0; i < allDependencies.Length; i++)
			{
				dependencyAsset[i] = allDependencies[i];
			}
		}
	}

	private static void LoadDependency(string path, string name, bool bFrist = false)
	{
		if (!string.IsNullOrEmpty(path))
		{
			string[] array = null;
			LoadResManager.LoadDependencyBymainfest(path, out array);
			if (bFrist && array == null)
			{
				return;
			}
			if (array == null || array.Length <= 0)
			{
				ResourceObject resourceObject = null;
				LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
				if (resourceObject != null && resourceObject.m_Asset != null)
				{
					return;
				}
				resourceObject = new ResourceObject();
				LoadResManager.m_ResourceInfos.Add(path, resourceObject);
				resourceObject.LoadRes(path, name, LoadResManager.FileExt);
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					string name2 = string.Empty;
					string text = array[i];
					int num = text.LastIndexOf('.');
					int num2 = text.LastIndexOf('/') + 1;
					if (num > 0)
					{
						name2 = text.Substring(num2, num - num2);
						text = text.Substring(0, num);
					}
					LoadResManager.LoadDependency(text, name2, false);
				}
				if (!bFrist)
				{
					ResourceObject resourceObject2 = null;
					LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject2);
					if (resourceObject2 != null && resourceObject2.m_Asset != null)
					{
						return;
					}
					resourceObject2 = new ResourceObject();
					LoadResManager.m_ResourceInfos.Add(path, resourceObject2);
					resourceObject2.LoadRes(path, name, LoadResManager.FileExt);
				}
			}
		}
	}

	public static GameObject LoadRes(string path)
	{
		path = path.ToLower();
		string name = string.Empty;
		path = "assets/res/" + path;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			return resourceObject.m_Asset.LoadAsset<GameObject>(name);
		}
		LoadResManager.LoadDependency(path, name, true);
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		return resourceObject.LoadRes(path, name, LoadResManager.FileExt);
	}

	public static void LoadScene(string path)
	{
		string name = string.Empty;
		path = "assets/res/" + path;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			return;
		}
		LoadResManager.LoadDependency(path, name, true);
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		resourceObject.LoadScene(path, name, LoadResManager.FileExt);
	}

	public static Texture2D LoadTex(string path)
	{
		path = path.ToLower();
		string name = string.Empty;
		path = "assets/res/" + path;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			return resourceObject.m_Asset.LoadAsset<Texture2D>(name);
		}
		LoadResManager.LoadDependency(path, name, true);
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		return resourceObject.LoadTex(path, name, LoadResManager.FileExt);
	}

	public static string LoadConfig(string path, string namekey)
	{
		path = path.ToLower();
		path = "assets/res/" + path;
		int length = path.LastIndexOf('.');
		path = path.Substring(0, length);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject == null)
		{
			resourceObject = new ResourceObject();
			LoadResManager.m_ResourceInfos[path] = resourceObject;
			return resourceObject.LoadTxt(path.ToLower(), namekey, LoadResManager.FileExt);
		}
		if (resourceObject.m_Asset == null)
		{
			return string.Empty;
		}
		TextAsset textAsset = resourceObject.m_Asset.LoadAsset<TextAsset>(namekey);
		if (null == textAsset)
		{
			Debug.LogErrorFormat("Load {0} from {1} failed!", new object[]
			{
				namekey,
				path
			});
			return string.Empty;
		}
		return textAsset.text;
	}

	public static AudioClip LoadSound(string path)
	{
		path = path.ToLower();
		string name = string.Empty;
		path = "assets/res/" + path;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			return resourceObject.m_Asset.LoadAsset<AudioClip>(name);
		}
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		return resourceObject.LoadSound(path, name, LoadResManager.FileExt);
	}

	public static AudioClip LoadCustomSound(string path)
	{
		AudioClip result = null;
		string text = Application.streamingAssetsPath + "/res/data/audio/";
		string text2 = text + path;
		AudioType audioType = LoadResManager.GuessAudioType(path);
		WWW www = new WWW(text2);
		while (!www.isDone)
		{
		}
		if (www.error != null)
		{
			string text3 = text + WWW.EscapeURL(path).Replace("+", "%20");
			WWW www2 = new WWW(text3);
			while (!www2.isDone)
			{
			}
			if (www2.error != null)
			{
				Debug.LogError(string.Concat(new string[]
				{
					"Load Custom Sound ",
					text,
					path,
					" with Error: ",
					www.error,
					" | Escaped URL Error: ",
					www2.error
				}));
			}
			else
			{
				result = ((audioType == AudioType.UNKNOWN) ? www2.GetAudioClip() : www2.GetAudioClip(false, false, audioType));
			}
		}
		else
		{
			result = ((audioType == AudioType.UNKNOWN) ? www.GetAudioClip() : www.GetAudioClip(false, false, audioType));
		}
		return result;
	}

	private static AudioType GuessAudioType(string path)
	{
		string text = path.ToLowerInvariant();
		if (text.EndsWith(".wav"))
		{
			return AudioType.WAV;
		}
		if (text.EndsWith(".ogg"))
		{
			return AudioType.OGGVORBIS;
		}
		if (text.EndsWith(".mp3"))
		{
			return AudioType.MPEG;
		}
		return AudioType.UNKNOWN;
	}

	public static string LoadTxt(string path)
	{
		path = path.ToLower();
		path = "assets/res/" + path;
		string name = string.Empty;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			TextAsset textAsset = resourceObject.m_Asset.LoadAsset<TextAsset>(name);
			return textAsset.text;
		}
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		return resourceObject.LoadTxt(path.ToLower(), name, LoadResManager.FileExt);
	}

	public static string LoadTextFileFromPersistent(string path)
	{
		string text = string.Format("{0}{1}", LoadResManager.rootPathPersistent, path);
		if (!File.Exists(text))
		{
			return null;
		}
		string result;
		try
		{
			string text2 = File.ReadAllText(text);
			Debug.LogFormat("LoadTextFile {0} success", new object[]
			{
				text
			});
			result = text2;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("LoadTextFile {0} exception.\n{1}", new object[]
			{
				text,
				ex.ToString()
			});
			result = null;
		}
		return result;
	}

	public static string LoadTextFileFromStreamingAssets(string path)
	{
		string text = string.Format("{0}{1}", LoadResManager.rootPathStreamAssets, path);
		string result;
		using (WWW www = new WWW(text))
		{
			while (!www.isDone)
			{
			}
			if (www.error != null)
			{
				Debug.LogErrorFormat("LoadTextFile {0} error: {1}", new object[]
				{
					text,
					www.error
				});
				result = null;
			}
			else
			{
				Debug.LogFormat("LoadTextFile {0} success", new object[]
				{
					text
				});
				result = www.text;
			}
		}
		return result;
	}

	public static string LoadTextFile(string path, ResLoadPathType pathType = ResLoadPathType.PersistentFirst)
	{
		if (pathType == ResLoadPathType.PersistentFirst)
		{
			string text = LoadResManager.LoadTextFileFromPersistent(path);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return LoadResManager.LoadTextFileFromStreamingAssets(path);
		}
		else
		{
			if (pathType == ResLoadPathType.PersistentOnly)
			{
				return LoadResManager.LoadTextFileFromPersistent(path);
			}
			return LoadResManager.LoadTextFileFromStreamingAssets(path);
		}
	}

	public static void SaveTextFile(string path, string txt)
	{
		string path2 = string.Format("{0}/{1}", LoadResManager.rootPathPersistent, path);
		string directoryName = Path.GetDirectoryName(path2);
		if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		File.WriteAllText(path2, txt);
	}

	public static T LoadJson<T>(string path, ResLoadPathType pathType = ResLoadPathType.PersistentFirst) where T : class
	{
		string text = LoadResManager.LoadTextFile(path, pathType);
		if (string.IsNullOrEmpty(text))
		{
			return (T)((object)null);
		}
		T result;
		try
		{
			T t = JsonUtility.FromJson<T>(text);
			result = t;
		}
		catch (Exception ex)
		{
			Debug.LogErrorFormat("Parse config {0} exception\n{1}", new object[]
			{
				path,
				ex.ToString()
			});
			result = (T)((object)null);
		}
		return result;
	}

	public static void SaveJson<T>(T cfg, string path)
	{
		string txt = JsonUtility.ToJson(cfg);
		LoadResManager.SaveTextFile(path, txt);
	}

	public static Material LoadMat(string path)
	{
		path = path.ToLower();
		string name = string.Empty;
		path = "assets/res/" + path;
		int num = path.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			name = path.Substring(num2, num - num2);
		}
		path = path.Substring(0, num);
		ResourceObject resourceObject = null;
		LoadResManager.m_ResourceInfos.TryGetValue(path, out resourceObject);
		if (resourceObject != null && resourceObject.m_Asset != null)
		{
			return resourceObject.m_Asset.LoadAsset<Material>(name);
		}
		LoadResManager.LoadDependency(path, name, true);
		resourceObject = new ResourceObject();
		LoadResManager.m_ResourceInfos[path] = resourceObject;
		return resourceObject.LoadMat(path, name, LoadResManager.FileExt);
	}

	public static string LoadCustomTxt(string path)
	{
		string text = null;
		string text2 = path;
		string text3 = path.ToLower();
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		int num = text2.LastIndexOf('.');
		int num2 = path.LastIndexOf('/') + 1;
		if (num > 0)
		{
			text2.Substring(num2, num - num2);
			text2.Substring(num);
			text2.Substring(0, num);
		}
		else
		{
			text2.Substring(num2);
		}
		text = LoadResManager.LoadTextFileFromStreamingAssets(text3);
		if (text != null)
		{
			return text;
		}
		text = LoadResManager.LoadTextFileFromStreamingAssets(text2);
		if (text != null)
		{
			return text;
		}
		text = LoadResManager.LoadTextFileFromPersistent(text3);
		if (text != null)
		{
			return text;
		}
		text = LoadResManager.LoadTextFileFromPersistent(text2);
		if (text != null)
		{
			return text;
		}
		Debug.LogError("Custom load error with path " + text2);
		return null;
	}

	private static string rootPathEditor = string.Empty;

	public static string rootPathPersistent = string.Empty;

	public static string rootPathStreamAssets = string.Empty;

	public static string FileExt = ".ab";

	public static string TempRoot = "assets/res/";

	private static AssetBundle m_manifestAB;

	private static AssetBundleManifest m_BundleManifest;

	private static Dictionary<string, ResourceObject> m_ResourceInfos;

	private bool bInitLoadResManager;
}
