using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class BattleLaserLineHalo
{
	public void InitHalo(Node hostNode)
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
		if (!(hostNode is LasergunNode))
		{
			return;
		}
		MapBuildingConfig mapBuildingConfig = Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTable.mbcList.Find((MapBuildingConfig n) => n.tag == hostNode.tag);
		if (mapBuildingConfig == null)
		{
			return;
		}
		this.count = (int)(mapBuildingConfig.lasergunRange / mapBuildingConfig.lasergunRotateSkip) + 1;
		if (this.count <= 0)
		{
			return;
		}
		this.gos.Clear();
		this.lines.Clear();
		float num = mapBuildingConfig.lasergunAngle;
		float num2 = 20f;
		Vector3 zero = Vector3.zero;
		UnityEngine.Object resources = Solarmax.Singleton<AssetManager>.Get().GetResources("Entity_LaserLineNew");
		for (int i = 0; i < this.count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(resources) as GameObject;
			gameObject.transform.SetParent(Solarmax.Singleton<BattleSystem>.Instance.battleData.root.transform);
			gameObject.SetActive(false);
			this.gos.Add(gameObject);
			LineRenderer componentInChildren = gameObject.GetComponentInChildren<LineRenderer>();
			if (componentInChildren)
			{
				zero.x = hostNode.GetPosition().x + num2 * Mathf.Cos(3.1415927f * num / 180f);
				zero.y = hostNode.GetPosition().y + num2 * Mathf.Sin(3.1415927f * num / 180f);
				componentInChildren.enabled = true;
				componentInChildren.sortingLayerID = SortingLayer.NameToID("Node");
				componentInChildren.sortingOrder = 1;
				componentInChildren.positionCount = 2;
				componentInChildren.SetPosition(0, hostNode.GetPosition());
				componentInChildren.SetPosition(1, zero);
				componentInChildren.widthMultiplier = 0.6f;
				LineRenderer lineRenderer = componentInChildren;
				Color white = Color.white;
				componentInChildren.endColor = white;
				lineRenderer.startColor = white;
				this.lines.Add(componentInChildren);
			}
			num += mapBuildingConfig.lasergunRotateSkip;
		}
	}

	public void ResetHost(Node hostNode)
	{
	}

	public void Show(Color color)
	{
		for (int i = 0; i < this.count; i++)
		{
			LineRenderer lineRenderer = this.lines[i];
			Color white = Color.white;
			this.lines[i].endColor = white;
			lineRenderer.startColor = white;
			this.gos[i].SetActive(true);
		}
	}

	public void Hide()
	{
		for (int i = 0; i < this.count; i++)
		{
			this.gos[i].SetActive(false);
		}
	}

	public void Destroy()
	{
		for (int i = 0; i < this.gos.Count; i++)
		{
			if (this.gos[i] != null)
			{
				UnityEngine.Object.Destroy(this.gos[i]);
			}
		}
		this.gos.Clear();
		this.lines.Clear();
		this.count = 0;
	}

	protected List<GameObject> gos = new List<GameObject>();

	private List<LineRenderer> lines = new List<LineRenderer>();

	private int count;
}
