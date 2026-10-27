using System;
using UnityEngine;

public class HeadEffect : IPoolGo
{
	public HeadEffect CreateEffect(string name)
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(name);
		this.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		return this;
	}

	public override void Recycle()
	{
		base.Recycle();
		if (this.go != null)
		{
			this.go.SetActive(false);
		}
		else
		{
			this.Destory();
		}
	}

	public override void Destory()
	{
		base.Destory();
	}

	public GameObject go;
}
