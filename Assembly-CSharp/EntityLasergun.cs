using System;
using Solarmax;
using UnityEngine;

public class EntityLasergun : EntityNode
{
	public EntityLasergun(string name) : base(name)
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
		this.image0 = base.go.transform.Find("image/image0").GetComponent<SpriteRenderer>();
		this.image1 = base.go.transform.Find("image/image0/image1").GetComponent<SpriteRenderer>();
		this.image2 = base.go.transform.Find("image/image0/image2").GetComponent<SpriteRenderer>();
	}

	public override void SetColor(Color color)
	{
		base.SetColor(color);
		this.image0.color = new Color(color.r, color.g, color.b, 1f);
		this.image1.color = new Color(1f, 1f, 1f, 1f);
		this.image2.color = new Color(1f, 1f, 1f, 1f);
		if (base.glow == null)
		{
			return;
		}
		base.glow.color = color;
	}

	public override void CalcGlowShape(Color color)
	{
		if (base.glow != null)
		{
			base.glow.color = color;
		}
	}

	private SpriteRenderer image0;

	private SpriteRenderer image1;

	private SpriteRenderer image2;
}
