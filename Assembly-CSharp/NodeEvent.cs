using System;
using UnityEngine;

public class NodeEvent : MonoBehaviour
{
	private void Start()
	{
		this.mapEdit = base.gameObject.GetComponentInParent<MapEdit>();
	}

	private void OnMouseDown()
	{
		this.OnPress(true);
	}

	private void OnMouseUp()
	{
	}

	private void OnMouseDrag()
	{
		this.OnDrag(Vector2.zero);
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed && base.enabled)
		{
			this.mapEdit.menuRoot.SetActive(true);
			this.mapEdit.nodeAttribute.SetNode(this.node);
			this.mapEdit.UpdateMenu(this.node.GetPosition());
			this.clicktime = 0f;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (!base.enabled)
		{
			return;
		}
		this.clicktime += Time.deltaTime;
		if (this.clicktime < 0.5f)
		{
			return;
		}
		this.node.SetPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		this.mapEdit.nodeAttribute.UpdatePosition(this.node);
		this.mapEdit.UpdateMenu(this.node.GetPosition());
	}

	public Node node;

	public MapEdit mapEdit;

	private float clicktime;
}
