using System;
using UnityEngine;

public class test_uv : MonoBehaviour
{
	private void Start()
	{
		this.v2 = Vector2.zero;
	}

	private void Update()
	{
		this.v2.x = this.v2.x + Time.fixedDeltaTime * this.xspeed;
		this.v2.y = this.v2.y + Time.fixedDeltaTime * this.yspeed;
		base.GetComponent<Renderer>().materials[0].mainTextureOffset = this.v2;
	}

	public float xspeed;

	public float yspeed;

	private Vector2 v2;
}
