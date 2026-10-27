using System;
using UnityEngine;

public class EntityBarrier : EntityNode
{
	public EntityBarrier(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Barrier");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}
}
