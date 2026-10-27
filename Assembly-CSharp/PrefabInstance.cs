using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstance
{
	public string Name { get; private set; }

	public void Init(string name, GameObject prefab)
	{
		this.Name = name;
		this.mGo = UnityEngine.Object.Instantiate<GameObject>(prefab);
		UnityEngine.Object.DontDestroyOnLoad(this.mGo);
		this.mGo.name = name;
		this.mTrans = this.mGo.transform;
	}

	public void Destroy()
	{
		if (null != this.mGo)
		{
			UnityEngine.Object.Destroy(this.mGo);
			this.mGo = null;
		}
		this.mTrans = null;
		this.mMaterials = null;
	}

	public void SetActive(bool b)
	{
		if (null == this.mGo)
		{
			return;
		}
		this.mGo.SetActive(b);
	}

	public void SetPosition(Vector3 p)
	{
		if (null == this.mTrans)
		{
			return;
		}
		this.mTrans.localPosition = p;
	}

	public void SetName(string name)
	{
		if (null == this.mGo)
		{
			return;
		}
		this.mGo.name = name;
	}

	public void SetParent(Transform t)
	{
		if (null == this.mTrans)
		{
			return;
		}
		this.mTrans.SetParent(t, false);
	}

	public void SetColor(Color c)
	{
		if (null == this.mGo)
		{
			return;
		}
		if (this.mMaterials == null)
		{
			this.mMaterials = new List<Material>();
			Renderer[] componentsInChildren = this.mGo.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] materials = componentsInChildren[i].materials;
				this.mMaterials.AddRange(materials);
			}
		}
		foreach (Material material in this.mMaterials)
		{
			material.color = c;
		}
	}

	public T GetOrAddComponent<T>() where T : Component
	{
		if (null == this.mGo)
		{
			return (T)((object)null);
		}
		T t = this.mGo.GetComponent<T>();
		if (null == t)
		{
			t = this.mGo.AddComponent<T>();
		}
		return t;
	}

	private GameObject mGo;

	private Transform mTrans;

	private List<Material> mMaterials;
}
