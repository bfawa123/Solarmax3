using System;
using System.Collections.Generic;
using System.IO;
using Solarmax;
using UnityEngine;

public class ResourceObject
{
	public ResourceObject()
	{
		this.m_LastUseTime = Solarmax.Singleton<TimeSystem>.Instance.GetServerTime();
		this.m_DependencyList = new List<object>();
	}

	public void AddDependency(object dependency)
	{
		if (this.m_DependencyList.Contains(dependency))
		{
			return;
		}
		this.m_DependencyList.Add(dependency);
	}

	public void Release(bool isShutdown)
	{
	}

	public void OnSpawn()
	{
	}

	public void OnUnspawn()
	{
	}

	public void LoadPacker(string SpritePacker, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + SpritePacker + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + SpritePacker + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
		}
	}

	public GameObject LoadRes(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		GameObject result;
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
			result = this.m_Asset.LoadAsset<GameObject>(name);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
			if (this.m_Asset == null)
			{
				return null;
			}
			result = this.m_Asset.LoadAsset<GameObject>(name);
		}
		return result;
	}

	public void LoadScene(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		if (!fileInfo.Exists)
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
		}
		this.m_Asset = AssetBundle.LoadFromFile(text);
		if (this.m_Asset == null)
		{
			Debug.LogErrorFormat("Load scene bundle {0} failed!", new object[]
			{
				text
			});
		}
	}

	public Material LoadMat(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		Material result;
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
			result = this.m_Asset.LoadAsset<Material>(name);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
			if (this.m_Asset == null)
			{
				return null;
			}
			result = this.m_Asset.LoadAsset<Material>(name);
		}
		return result;
	}

	public AudioClip LoadSound(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		AudioClip result;
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
			result = this.m_Asset.LoadAsset<AudioClip>(name);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
			if (this.m_Asset == null)
			{
				return null;
			}
			result = this.m_Asset.LoadAsset<AudioClip>(name);
		}
		return result;
	}

	public Texture2D LoadTex(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		Texture2D result;
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
			result = this.m_Asset.LoadAsset<Texture2D>(name);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
			if (this.m_Asset == null)
			{
				return null;
			}
			result = this.m_Asset.LoadAsset<Texture2D>(name);
		}
		return result;
	}

	public string LoadTxt(string Path, string name, string fileExt)
	{
		string text = LoadResManager.rootPathPersistent + Path + fileExt;
		FileInfo fileInfo = new FileInfo(text);
		TextAsset textAsset;
		if (fileInfo.Exists)
		{
			this.m_Asset = AssetBundle.LoadFromFile(text);
			textAsset = this.m_Asset.LoadAsset<TextAsset>(name);
		}
		else
		{
			text = LoadResManager.rootPathStreamAssets + Path + fileExt;
			this.m_Asset = AssetBundle.LoadFromFile(text);
			if (this.m_Asset == null)
			{
				Debug.LogErrorFormat("Load {0} failed!", new object[]
				{
					text
				});
				return null;
			}
			textAsset = this.m_Asset.LoadAsset<TextAsset>(name);
		}
		if (textAsset == null)
		{
			Debug.LogErrorFormat("Load {0} from {1} failed!", new object[]
			{
				name,
				text
			});
			return string.Empty;
		}
		return textAsset.text;
	}

	public AssetBundle m_Asset;

	private DateTime m_LastUseTime;

	private readonly List<object> m_DependencyList;
}
