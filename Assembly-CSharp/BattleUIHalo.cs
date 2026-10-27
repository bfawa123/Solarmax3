using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class BattleUIHalo
{
	public void InitHalo(Node node)
	{
		if (node == null)
		{
			return;
		}
		this.hostNode = node.nodeManager.GetNode(node.tag);
		if (this.hostNode == null)
		{
			return;
		}
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_UI_Halo");
		this.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		this.image = this.go.GetComponent<UISprite>();
		this.back = this.go.transform.Find("BackGround").GetComponent<UISprite>();
		UISprite uisprite = this.back;
		string spriteName = string.Format("waihuan-{0}", node.GetScale());
		this.image.spriteName = spriteName;
		uisprite.spriteName = spriteName;
		this.back.MakePixelPerfect();
		this.image.MakePixelPerfect();
		this.go.transform.localScale = TouchHandler.haloScaleV;
		if (this.hostNode.GetGO())
		{
			this.go.transform.SetParent(this.hostNode.GetGO().transform);
		}
		else
		{
			this.go.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
		}
		this.go.transform.localPosition = Vector3.zero;
		this.go.SetActive(false);
	}

	public void ResetHost(Node node)
	{
		if (node == null || node.nodeManager == null)
		{
			return;
		}
		this.hostNode = node.nodeManager.GetNode(node.tag);
		if (this.hostNode == null)
		{
			return;
		}
		UISprite uisprite = this.back;
		string spriteName = string.Format("3px-{0}", node.GetScale());
		this.image.spriteName = spriteName;
		uisprite.spriteName = spriteName;
		this.back.MakePixelPerfect();
		this.image.MakePixelPerfect();
		this.go.transform.localScale = TouchHandler.haloScaleV;
		this.go.transform.SetParent(this.hostNode.GetGO().transform);
		this.go.transform.localPosition = Vector3.zero;
	}

	public void ShutHalo()
	{
		this.go.SetActive(false);
		this.image.color = Color.white;
		this.back.color = Color.white;
	}

	public void SetColor(List<Color> clrArray, List<float> phaseArray)
	{
		if (clrArray.Count != phaseArray.Count)
		{
			return;
		}
		if (!this.go.activeSelf)
		{
			this.go.SetActive(true);
		}
		switch (clrArray.Count)
		{
		case 2:
			this.image.fillDirection = UIBasicSprite.FillDirection.RadialCircular;
			this.image.fillAmount = phaseArray[1];
			this.image.invert = true;
			this.back.color = clrArray[0];
			this.image.color = clrArray[1];
			this.back.invert = false;
			this.back.fillDirection = UIBasicSprite.FillDirection.RadialCircular;
			this.back.fillAmount = phaseArray[0];
			return;
		}
		this.back.color = Color.white;
		this.image.fillDirection = UIBasicSprite.FillDirection.RadialMuti;
		List<SpriteData> list = new List<SpriteData>();
		for (int i = 0; i < clrArray.Count; i++)
		{
			list.Add(new SpriteData
			{
				color = clrArray[i],
				rate = phaseArray[i]
			});
		}
		this.image.AddSpriteData(list);
	}

	private Node hostNode;

	private GameObject go;

	private UISprite image;

	private UISprite back;
}
