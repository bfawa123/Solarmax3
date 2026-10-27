using System;
using UnityEngine;

public class EntityHouse : EntityNode
{
	public EntityHouse(string name) : base(name)
	{
	}

	public override bool Init()
	{
		base.Init();
		return true;
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_House");
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
