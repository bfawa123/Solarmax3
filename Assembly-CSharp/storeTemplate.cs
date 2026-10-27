using System;
using Solarmax;
using UnityEngine;

public class storeTemplate : MonoBehaviour
{
	public void EnsureInit(StoreConfig config)
	{
		if (config == null)
		{
			return;
		}
		this.config = config;
		this.UpdateAvatar();
	}

	public void Unlock()
	{
	}

	public void OnClicked()
	{
	}

	private void UpdateAvatar()
	{
		this.icon.spriteName = this.config.Icon;
		this.desc.text = this.config.GoldValue.ToString();
		this.num.text = this.config.GetPriceDesc();
	}

	public UISprite icon;

	public UILabel desc;

	public UILabel num;

	private StoreConfig config;
}
