using System;
using UnityEngine;

public class EntityClouds : EntityNode
{
	public EntityClouds(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Clouds");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	public override void Resuming()
	{
		this.SetColor(this.color);
		this.SetPosition(this.position);
		base.SetScale(this.scale);
		base.SetRotation(this.eulerAngles);
	}
}
