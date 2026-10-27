using System;
using UnityEngine;

public class BattleWarningHalo
{
	protected GameObject go { get; set; }

	private SpriteRenderer sprite { get; set; }

	public void InitHalo(Node hostNode, float radius)
	{
		if (hostNode == null)
		{
			return;
		}
		hostNode = hostNode.nodeManager.GetNode(hostNode.tag);
		if (hostNode == null)
		{
			return;
		}
		if (!hostNode.CanShowRange())
		{
			return;
		}
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_Warning");
		this.go = (UnityEngine.Object.Instantiate(resources) as GameObject);
		this.sprite = this.go.GetComponentInChildren<SpriteRenderer>();
		this.go.transform.position = hostNode.GetPosition();
		this.go.transform.SetParent(hostNode.GetGO().transform);
		float d = radius / 2f;
		this.go.transform.localScale = Vector3.one * d;
		this.go.SetActive(false);
	}

	public void ResetHost(Node hostNode)
	{
		if (hostNode == null)
		{
			return;
		}
		hostNode = hostNode.nodeManager.GetNode(hostNode.tag);
		if (hostNode == null)
		{
			return;
		}
		if (this.go != null)
		{
			this.go.transform.SetParent(hostNode.GetGO().transform);
			this.go.transform.position = hostNode.GetPosition();
		}
	}

	public void Show(Color color)
	{
		if (this.go != null)
		{
			color.a = 0.3f;
			this.sprite.color = color;
			this.go.SetActive(true);
		}
	}

	public void Hide()
	{
		if (this.go != null)
		{
			this.go.SetActive(false);
			this.sprite.color = Color.white;
		}
	}
}
