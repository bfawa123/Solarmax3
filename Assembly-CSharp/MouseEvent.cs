using System;
using UnityEngine;

public class MouseEvent : MonoBehaviour
{
	private void LateUpdate()
	{
		if (this.battlteObject == null)
		{
			return;
		}
		if (!this.battlteObject.activeSelf)
		{
			return;
		}
		if (Input.GetMouseButtonUp(1))
		{
			Vector3 position = Input.mousePosition;
			position.z = 0f;
			position = this.uiCamera.ScreenToWorldPoint(position);
			this.nodeMenu.transform.position = position;
			this.nodeMenu.SetActive(true);
		}
	}

	private void OnClick()
	{
		float num = Time.realtimeSinceStartup - this.nodeAttribute.GetComponent<NodeAttribute>().clickTime;
		if (num < 0.1f)
		{
			return;
		}
		this.nodeAttribute.SetActive(false);
		this.nodeMenu.SetActive(false);
	}

	private void OnMouseDown()
	{
	}

	public GameObject nodeMenu;

	public Camera uiCamera;

	public GameObject battlteObject;

	public GameObject nodeAttribute;
}
