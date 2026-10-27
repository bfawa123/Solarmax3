using System;
using Solarmax;
using UnityEngine;

public class EntityGunturret : EntityNode
{
	public EntityGunturret(string name) : base(name)
	{
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Speedship");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	protected override void InitGameObject()
	{
		base.InitGameObject();
		base.glow = base.go.transform.Find("shape").GetComponent<SpriteRenderer>();
		if (Solarmax.Singleton<BattleSystem>.Instance.battleData.vertical)
		{
			base.glow.transform.Rotate(Vector3.forward, 90f);
		}
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

	public override void CalcGlowShape(Color color)
	{
		base.glow.color = color;
	}
}
