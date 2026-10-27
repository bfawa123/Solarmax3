using System;
using UnityEngine;

public class AchievementTemplate : MonoBehaviour
{
	public void UpdateUI(bool success, string text)
	{
		this.info.text = text;
		if (success)
		{
			this.info.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			this.info.color = new Color(1f, 1f, 1f, 0.6f);
		}
	}

	public UILabel info;

	public UISprite star;

	public UITexture unStar;
}
