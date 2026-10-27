using System;
using System.Collections;
using Solarmax;
using UnityEngine;

public class AvatarView : MonoBehaviour
{
	private void Awake()
	{
		this.scroll = this.table.transform.parent.gameObject.GetComponent<UIScrollView>();
	}

	public void EnsureInit()
	{
		this.useGo.SetActive(false);
		this.buyGo.SetActive(false);
		this.model = Solarmax.Singleton<CollectionModel>.Get();
		CollectionModel collectionModel = Solarmax.Singleton<CollectionModel>.Get();
		collectionModel.onAvatarChanged = (CollectionModel.OnAvatarChanged)Delegate.Combine(collectionModel.onAvatarChanged, new CollectionModel.OnAvatarChanged(this.OnAvatarChanged));
		base.StartCoroutine(this.UpdateUI());
	}

	public void EnsureDestroy()
	{
		CollectionModel collectionModel = Solarmax.Singleton<CollectionModel>.Get();
		collectionModel.onAvatarChanged = (CollectionModel.OnAvatarChanged)Delegate.Remove(collectionModel.onAvatarChanged, new CollectionModel.OnAvatarChanged(this.OnAvatarChanged));
		Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig = null;
		if (Solarmax.Singleton<CollectionModel>.Get().NeedSyncAvatar())
		{
		}
		this.ClearTable();
	}

	public void SetButtonStatus()
	{
	}

	private IEnumerator UpdateUI()
	{
		this.scroll.ResetPosition();
		yield return null;
		foreach (SkinConfig config in this.model.avatarConfigs)
		{
			GameObject gameObject = this.table.gameObject.AddChild(this.avatarTemplate);
			gameObject.SetActive(true);
			AvatarTemplate component = gameObject.GetComponent<AvatarTemplate>();
			component.EnsureInit(config, this.scroll);
		}
		this.table.Reposition();
		this.scroll.ResetPosition();
		string url = Solarmax.Singleton<LocalPlayer>.Get().playerData.icon;
		if (!url.EndsWith(".png"))
		{
			url += ".png";
		}
		this.choosedAvatar.picUrl = url;
		this.playerName.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.name;
		yield break;
	}

	private void ClearTable()
	{
		this.table.transform.DestroyChildren();
		this.table.Reposition();
	}

	private void AfterChoosedAvatar()
	{
	}

	private void OnAvatarChanged()
	{
		SkinConfig choosedConfig = Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig;
		if (choosedConfig == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("Choosed avatar error", new object[0]);
			return;
		}
		this.choosedAvatar.picUrl = choosedConfig.skinImageName;
		if (choosedConfig.unlock)
		{
			this.useGo.SetActive(true);
			this.buyGo.SetActive(false);
			string text = choosedConfig.skinImageName;
			if (!this.choosedAvatar.picUrl.EndsWith(".png"))
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
				this.useGo.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(2163);
				this.buyButton.GetComponent<UIButton>().enabled = false;
				this.buyButton.SetActive(true);
			}
			else
			{
				this.useGo.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(2219);
				this.buyButton.GetComponent<UIButton>().enabled = true;
				this.buyButton.SetActive(true);
			}
		}
		else if (choosedConfig.type == SkinType.Avatar && choosedConfig.bgImage != string.Empty)
		{
			this.buyButton.GetComponent<UIButton>().enabled = false;
			this.useGo.SetActive(true);
			this.buyGo.SetActive(false);
			this.buyButton.SetActive(false);
			this.useGo.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(2240);
		}
		else
		{
			this.buyButton.GetComponent<UIButton>().enabled = true;
			this.useGo.SetActive(false);
			this.buyGo.SetActive(true);
			this.buyButton.SetActive(true);
			this.buyLabel.text = choosedConfig.goodValue.ToString();
		}
	}

	public void OnClickBUButton()
	{
		SkinConfig choosedConfig = Solarmax.Singleton<CollectionModel>.Get().ChoosedConfig;
		if (choosedConfig.unlock)
		{
			if (!Solarmax.Singleton<LocalPlayer>.Get().playerData.icon.Equals(choosedConfig.skinImageName))
			{
				Solarmax.Singleton<LocalPlayer>.Get().playerData.icon = choosedConfig.skinImageName;
				Solarmax.Singleton<NetSystem>.Instance.helper.ChangeIcon(choosedConfig.skinImageName);
				AvatarTemplate[] componentsInChildren = this.table.transform.GetComponentsInChildren<AvatarTemplate>();
				foreach (AvatarTemplate avatarTemplate in componentsInChildren)
				{
					avatarTemplate.UpdateAvatar();
				}
				this.useGo.GetComponent<UILabel>().text = LanguageDataProvider.GetValue(2163);
				this.buyButton.GetComponent<UIButton>().enabled = false;
			}
		}
		else
		{
			if ((double)Solarmax.Singleton<LocalPlayer>.Get().playerData.money < choosedConfig.goodValue)
			{
				Tips.Make(LanguageDataProvider.GetValue(1102));
				return;
			}
			CollectionBuyModel collectionBuyModel = new CollectionBuyModel
			{
				buyType = 0,
				buyId = choosedConfig.id,
				url = choosedConfig.skinImageName,
				gold = choosedConfig.goodValue.ToString()
			};
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CollectionBuyWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnShowBuyWindowEvent, new object[]
			{
				collectionBuyModel
			});
		}
	}

	public UITable table;

	public NetTexture choosedAvatar;

	public GameObject avatarTemplate;

	public GameObject useGo;

	public GameObject buyGo;

	public UILabel buyLabel;

	public GameObject buyButton;

	public UILabel playerName;

	private CollectionModel model;

	private UIScrollView scroll;
}
