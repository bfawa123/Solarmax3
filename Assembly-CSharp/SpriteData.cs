using System;
using UnityEngine;

[Serializable]
public class SpriteData
{
	public Color color = Color.white;

	[Range(0f, 1f)]
	public float rate = 0.1f;
}
