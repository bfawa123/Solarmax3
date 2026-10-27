using System;
using Solarmax;
using UnityEngine;

public class EntityFixedWarpDoor : EntityNode
{
	public EntityFixedWarpDoor(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_FixedWarpDoor");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	protected override void InitGameObject()
	{
		base.InitGameObject();
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			this.ImageTransform.Rotate(Vector3.forward, -90f);
		}
	}
}
