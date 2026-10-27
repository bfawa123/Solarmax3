using System;
using UnityEngine;

public class FX_texture_ani_Script : MonoBehaviour
{
	private void Update()
	{
		if (this.textures.Length == 0)
		{
			return;
		}
		if (this.timer <= this.changeInterval)
		{
			this.timer += Time.deltaTime;
		}
		else
		{
			this.timer = 0f;
			base.GetComponent<MeshRenderer>().material.mainTexture = this.textures[this.index];
			this.index++;
			if (this.index > this.textures.Length - 1)
			{
				this.index = 0;
			}
		}
	}

	public Texture[] textures;

	public float changeInterval = 0.05f;

	private float timer;

	private int index;
}
