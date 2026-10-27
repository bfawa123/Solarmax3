using System;
using UnityEngine;

public class DyWith : MonoBehaviour
{
	private void Start()
	{
		this.texture.width = this.root.manualWidth;
	}

	public UIRoot root;

	public UITexture texture;
}
