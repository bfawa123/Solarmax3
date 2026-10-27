using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool
{
	public static PrefabInstance NewInstane(string name)
	{
		PrefabPool prefabPool;
		if (!PrefabPool.sPrefabs.TryGetValue(name, out prefabPool))
		{
			prefabPool = new PrefabPool();
			prefabPool.Init(name);
			PrefabPool.sPrefabs.Add(name, prefabPool);
		}
		return prefabPool.New();
	}

	public static void FreeInstance(ref PrefabInstance inst)
	{
		if (inst == null)
		{
			return;
		}
		PrefabPool prefabPool;
		if (!PrefabPool.sPrefabs.TryGetValue(inst.Name, out prefabPool))
		{
			return;
		}
		prefabPool.Free(ref inst);
	}

	public string Name { get; private set; }

	public void Init(string name)
	{
		this.Name = name;
		this.mPrefab = LoadResManager.LoadRes(string.Format("gameres/sprites/{0}.prefab", name));
		if (null == this.mPrefab)
		{
			Debug.LogErrorFormat("Load prefab {0} failed!", new object[]
			{
				name
			});
		}
		this.mRootGo = new GameObject(name);
		this.mRootTrans = this.mRootGo.transform;
	}

	public void Destroy()
	{
		foreach (PrefabInstance prefabInstance in this.mInstances)
		{
			prefabInstance.Destroy();
		}
		this.mInstances.Clear();
		if (null != this.mRootGo)
		{
			UnityEngine.Object.Destroy(this.mRootGo);
			this.mRootTrans = null;
			this.mRootGo = null;
		}
		if (null != this.mPrefab)
		{
			Resources.UnloadAsset(this.mPrefab);
			this.mPrefab = null;
		}
	}

	public PrefabInstance New()
	{
		if (null == this.mPrefab)
		{
			return null;
		}
		int count = this.mInstances.Count;
		PrefabInstance prefabInstance;
		if (count < 1)
		{
			prefabInstance = new PrefabInstance();
			prefabInstance.Init(this.Name, this.mPrefab);
			return prefabInstance;
		}
		prefabInstance = this.mInstances[count - 1];
		this.mInstances.RemoveAt(count - 1);
		prefabInstance.SetActive(true);
		return prefabInstance;
	}

	public void Free(ref PrefabInstance inst)
	{
		if (inst == null)
		{
			return;
		}
		inst.SetActive(false);
		inst.SetParent(this.mRootTrans);
		this.mInstances.Add(inst);
		inst = null;
	}

	private static Dictionary<string, PrefabPool> sPrefabs = new Dictionary<string, PrefabPool>();

	private GameObject mPrefab;

	private GameObject mRootGo;

	private Transform mRootTrans;

	private List<PrefabInstance> mInstances = new List<PrefabInstance>();
}
