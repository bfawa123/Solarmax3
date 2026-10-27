using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class CollectionBuyWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnShowBuyWindowEvent);
		return true;
	}

	public void Awake()
	{
	}

	public override void OnShow()
	{
		base.OnShow();
		this.model = Solarmax.Singleton<CollectionModel>.Get();
		this.downloadLoading.SetActive(false);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnShowBuyWindowEvent)
		{
			CollectionBuyModel collectionBuyModel = (CollectionBuyModel)args[0];
			if (collectionBuyModel == null)
			{
				return;
			}
			this.buyModel = collectionBuyModel;
			this.UpdateUI();
		}
	}

	public void OnCloseClicked()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CollectionBuyWindow");
	}

	public void OnBuyClicked()
	{
		if (this.buyModel != null)
		{
			CSBuySkin csbuySkin = new CSBuySkin();
			csbuySkin.id = this.buyModel.buyId;
			Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSBuySkin>(267, csbuySkin);
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("CollectionBuyWindow");
	}

	private void UpdateUI()
	{
		if (this.buyModel.buyType == 0)
		{
			this.protrait.SetActive(true);
			this.bgView.SetActive(false);
			PortraitTemplate component = this.protrait.GetComponent<PortraitTemplate>();
			component.Load(this.buyModel.url, null, null);
		}
		else
		{
			this.protrait.SetActive(false);
			this.bgView.SetActive(true);
			this.bg.picUrl = this.buyModel.url;
		}
		this.buyNUM.text = this.buyModel.gold;
	}

	public GameObject downloadLoading;

	public GameObject bgView;

	public NetTexture bg;

	public GameObject protrait;

	public UILabel buyNUM;

	private CollectionModel model;

	private CollectionBuyModel buyModel;
}
