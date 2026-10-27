using System;
using Solarmax;
using UnityEngine;

public class EntityPlanet : EntityNode
{
	public EntityPlanet(string name) : base(name)
	{
	}

	public override bool Init()
	{
		base.Init();
		this.RandomSprite();
		return true;
	}

	protected override GameObject CreateGameObject()
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Planet");
		return UnityEngine.Object.Instantiate(resources) as GameObject;
	}

	private void RandomSprite()
	{
		int num = Solarmax.Singleton<BattleSystem>.Instance.battleData.rand.Range(1, 9);
		string path = string.Format("gameres/sprites/sp_planet{0:D2}.prefab", num);
		SpriteRenderer component = LoadResManager.LoadRes(path).GetComponent<SpriteRenderer>();
		base.image.sprite = component.sprite;
	}

	public override void Resuming()
	{
		this.SetColor(this.color);
		this.SetPosition(this.position);
		base.SetScale(this.scale);
		base.SetRotation(this.eulerAngles);
	}
}
