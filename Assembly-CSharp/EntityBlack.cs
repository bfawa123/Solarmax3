using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityBlack : EntityNode
{
	public EntityBlack(string name) : base(name)
	{
	}

	public override bool Init()
	{
		return base.Init();
	}

	public override void Tick(int frame, float interval)
	{
		base.Tick(frame, interval);
	}

	public override void Destroy()
	{
		base.Destroy();
		List<string> list = new List<string>(this.deformations.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = this.deformations[list[i]];
			if (gameObject != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		base.go = null;
	}

	protected override GameObject CreateGameObject()
	{
		GameObject result = this.CreateGO("Entity_BlackHole");
		this.nowShape = "Entity_BlackHole";
		return result;
	}

	private GameObject CreateGO(string name)
	{
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources(name);
		GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
		this.deformations.Add(name, gameObject);
		return gameObject;
	}

	public void Deformating(string res, out bool newAsset)
	{
		newAsset = false;
		if (this.nowShape.Equals(res))
		{
			return;
		}
		GameObject gameObject;
		if (this.deformations.ContainsKey(res))
		{
			gameObject = this.deformations[res];
		}
		else
		{
			gameObject = this.CreateGO(res);
			newAsset = true;
		}
		Transform transform = gameObject.transform;
		Transform transform2 = base.go.transform;
		transform.parent = transform2.parent;
		transform.localPosition = transform2.localPosition;
		transform.localRotation = transform2.localRotation;
		transform.localScale = transform2.localScale;
		gameObject.layer = base.go.layer;
		gameObject.name = base.go.name;
		this.SetPosition(EntityBlack.FARPOS);
		base.go = gameObject;
		base.image = base.go.transform.Find("image").GetComponent<SpriteRenderer>();
		base.halo = base.go.transform.Find("halo").GetComponent<SpriteRenderer>();
		this.nowShape = res;
		Animator component = base.go.GetComponent<Animator>();
		component.Play(res + "_in");
	}

	private Dictionary<string, GameObject> deformations = new Dictionary<string, GameObject>();

	private static Vector3 FARPOS = new Vector3(100000f, 100000f, 100000f);

	private string nowShape;
}
