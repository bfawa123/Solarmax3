using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PortraitManager : Solarmax.Singleton<PortraitManager>
{
	public Texture2D GetTexture2D(string url)
	{
		Texture2D texture2D = this.TryGetTexture2DFromCache(url);
		if (this.TryGetTexture2DFromCache(url) != null)
		{
			return texture2D;
		}
		texture2D = this.TryGetTexture2DFromLocal(url);
		if (texture2D != null)
		{
			return texture2D;
		}
		return null;
	}

	public void AddTexture2D(string url, Texture2D texture)
	{
		if (url.StartsWith("http"))
		{
			string[] array = url.Split(new char[]
			{
				'/'
			});
			if (array.Length > 1)
			{
				string text = array[array.Length - 1];
				string path = string.Format("{0}/{1}", this.GetDownloadPath(), text);
				string[] array2 = text.Split(new char[]
				{
					'.'
				});
				if (array2[array2.Length - 1] == "jpg")
				{
					File.WriteAllBytes(path, texture.EncodeToJPG());
				}
				else if (array2[array2.Length - 1] == "png")
				{
					File.WriteAllBytes(path, texture.EncodeToPNG());
				}
			}
		}
		this.dicAvartar2D[url] = texture;
	}

	public Texture2D TryGetTexture2DFromCache(string url)
	{
		if (this.dicAvartar2D.ContainsKey(url))
		{
			return this.dicAvartar2D[url];
		}
		return null;
	}

	public Texture2D TryGetTexture2DFromLocal(string url)
	{
		string text = string.Format("gameres/atlas/skin/{0}", url);
		Debug.Log(text);
		Texture2D texture2D = LoadResManager.LoadTex(text.ToLower());
		if (texture2D != null)
		{
			this.dicAvartar2D[url] = texture2D;
			return texture2D;
		}
		return null;
	}

	public Texture2D TryGetTexture2DFromSkin(string url, string strPath, string name)
	{
		if (string.IsNullOrEmpty(url))
		{
			return null;
		}
		Texture2D result;
		if (url.StartsWith("http"))
		{
			Debug.Log("AB：" + strPath);
			strPath = strPath.ToLower();
			AssetBundle assetBundle = AssetBundle.LoadFromFile(strPath);
			result = assetBundle.LoadAsset<Texture2D>(name);
			assetBundle.Unload(false);
		}
		else
		{
			Debug.Log("AB：" + strPath);
			result = LoadResManager.LoadTex(strPath);
		}
		return result;
	}

	private string GetDownloadPath()
	{
		string result = string.Format("{0}/Resources", Application.dataPath);
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			result = string.Format("{0}/Resources", Application.persistentDataPath);
		}
		return result;
	}

	private Dictionary<string, Texture2D> dicAvartar2D = new Dictionary<string, Texture2D>();
}
