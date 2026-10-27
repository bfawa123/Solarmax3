using System;
using UnityEngine;

public class EntityDefense : EntityNode
{
	public EntityDefense(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Defense");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	protected override void InitGameObject()
	{
		base.InitGameObject();
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		if (base.glow == null)
		{
			return;
		}
		base.glow.color = color;
	}
}
