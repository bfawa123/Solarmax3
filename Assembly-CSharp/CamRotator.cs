using System;
using UnityEngine;

public class CamRotator : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (this.isActive)
		{
			base.transform.Rotate(Vector3.up, this.speed * Time.deltaTime);
		}
	}

	public bool isActive;

	public float speed = 0.33f;
}
