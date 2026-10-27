using System;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
	private void Start()
	{
		this.roate = false;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			float y = Input.GetAxis("Mouse X") * this.RoatedSpeed * Time.deltaTime;
			if (this.roate)
			{
				base.gameObject.transform.Rotate(new Vector3(0f, y, 0f));
			}
		}
	}

	private void OnMouseDown()
	{
		this.roate = true;
		Debug.Log("collider");
	}

	private void OnMouseUp()
	{
		this.roate = false;
		Debug.Log("Out of collider");
	}

	private bool roate;

	private float RoatedSpeed = 1000f;
}
