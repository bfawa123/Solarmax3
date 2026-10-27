using System;
using UnityEngine;

public class EntityCreature : EntityNode
{
	public EntityCreature(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(base.perfab);
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
