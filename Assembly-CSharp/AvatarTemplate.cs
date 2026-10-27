using System;
using Solarmax;
using UnityEngine;

public class AvatarTemplate : MonoBehaviour
{
	public void EnsureInit(SkinConfig config, UIScrollView view)
	{
		if (config == null)
		{
			return;
		}
		this.scroll = view;
		this.config = config;
		this.moneyIcon.SetActive(!config.unlock);
		this.moneyNum.gameObject.SetActive(!config.unlock);
		this.moneyNum.text = config.goodValue.ToString();
		this.UpdateAvatar();
		string text = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		if (!Solarmax.Singleton<LocalPlayer>.Get().playerData.icon.EndsWith(".png"))
		{
			text += ".png";
		}
		if (config.skinImageName.Equals(text))
		{
			this.OnClicked();
		}
	}

	public void Unlock()
	{
	}

	public void OnClicked()
	{
		Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig = this.config;
		this.select.startsActive = true;
	}

	public void UpdateAvatar()
	{
		PortraitTemplate component = base.transform.Find("Portrait").GetComponent<PortraitTemplate>();
		component.Load(this.config.skinImageName, this.scroll, null);
		string text = this.config.skinImageName;
		if (!text.EndsWith(".png"))
		{
			text += ".png";
		}
		string text2 = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		if (!Solarmax.Singleton<LocalPlayer>.Get().playerData.icon.EndsWith(".png"))
		{
			text2 += ".png";
		}
		if (text.Equals(text2))
		{
			this.status.gameObject.SetActive(true);
			this.status.text = LanguageDataProvider.GetValue(2163);
		}
		else
		{
			this.status.gameObject.SetActive(false);
		}
		if (!this.config.unlock && this.config.type == SkinType.Avatar && this.config.bgImage != string.Empty)
		{
			this.moneyIcon.SetActive(false);
			this.moneyNum.gameObject.SetActive(false);
			this.status.text = LanguageDataProvider.GetValue(2234);
			this.status.gameObject.SetActive(true);
		}
	}

	public NetTexture avatar;

	public GameObject lockIcon;

	public UIToggle select;

	public GameObject moneyIcon;

	public UILabel moneyNum;

	public UILabel status;

	private SkinConfig config;

	private UIScrollView scroll;
}
