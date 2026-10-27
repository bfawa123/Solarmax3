using System;
using UnityEngine;

public class GuideTipCell : MonoBehaviour
{
	public void SetLevelCell(string sprite, string msg)
	{
		this.desc.text = msg;
	}

	public UISprite build;

	public UILabel desc;

	public GameObject line1;

	public GameObject line2;
}
