using System;
using System.Collections.Generic;
using MiGameSDK;
using NetMessage;
using Solarmax;
using UnityEngine;

public class storeView : MonoBehaviour
{
	public void EnsureInit()
	{
		this.cdList.Clear();
		this.model = Solarmax.Singleton<CollectionModel>.Get();
		List<StoreConfig> storeConfigs = this.model.storeConfigs;
		for (int i = 0; i < storeConfigs.Count; i++)
		{
			if (storeConfigs[i].type == 0)
			{
				this.cdList.Add(storeConfigs[i]);
			}
		}
		int num = this.productTemplate.Length;
		this.product = new string[num];
		this.money.text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
		this.monthCardEffect.SetActive(false);
		this.UpdateUI();
		if (!UpgradeUtil.GetGameConfig().EnablePay)
		{
			this.productTemplate[1].SetActive(false);
			this.productTemplate[2].SetActive(false);
			this.productTemplate[3].SetActive(false);
			this.productTemplate[4].SetActive(false);
			this.productTemplate[0].transform.localPosition = new Vector3(8.6f, -34f, 0f);
			this.rawardMonth.SetActive(false);
		}
	}

	public void EnsureDestroy()
	{
	}

	public void Update()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().showAdsRefreshTime > 0f)
		{
			int nLookAdsNum = Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum;
			if (nLookAdsNum >= 0 && nLookAdsNum < Solarmax.Singleton<storeConfigProvider>.Get().MAX_SHOW_ADS_NUM)
			{
				this.adProcess.value = Solarmax.Singleton<LocalPlayer>.Get().showAdsRefreshTime / (float)this.cdList[nLookAdsNum].cd;
			}
		}
		else
		{
			this.adProcess.value = 0f;
			this.adCD.gameObject.SetActive(false);
		}
	}

	private void InitTemplateUI(string prodcutID, GameObject go)
	{
		StoreConfig data = Solarmax.Singleton<storeConfigProvider>.Get().GetData(prodcutID);
		if (data == null)
		{
			go.SetActive(false);
			return;
		}
		if (data.type != 0 && Solarmax.Singleton<ThirdPartySystem>.Instance.GettingProductsInfo)
		{
			go.SetActive(false);
			return;
		}
		go.SetActive(true);
		bool flag = !Solarmax.Singleton<LocalPlayer>.Get().IsBuyed(prodcutID);
		UITable[] componentsInChildren = go.GetComponentsInChildren<UITable>();
		if (data.type == 1)
		{
			UILabel component = go.transform.Find("num").GetComponent<UILabel>();
			component.text = data.GetPriceDesc();
			UILabel component2 = go.transform.Find("moneyLabel").transform.Find("Label").GetComponent<UILabel>();
			component2.text = data.GoldValue.ToString();
			Transform transform = go.transform.Find("FirstPay");
			transform.gameObject.SetActive(flag);
			if (flag)
			{
				GameObject gameObject = go.transform.Find("FirstPay").transform.Find("song").gameObject;
				UILabel component3 = go.transform.Find("FirstPay").transform.Find("money").GetComponent<UILabel>();
				component3.text = data.GoldValue.ToString();
			}
		}
		else
		{
			this.monthGo = go;
			this.monthCardConfigId = prodcutID;
			this.RefreshMothGo();
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Reposition();
		}
	}

	public void RefreshMothGo()
	{
		if (this.monthGo == null && this.monthCardConfigId == null)
		{
			Solarmax.Singleton<LoggerSystem>.Instance.Error("月卡实体为空", new object[0]);
			return;
		}
		this.RefreshMonthCard();
		StoreConfig data = Solarmax.Singleton<storeConfigProvider>.Get().GetData(this.monthCardConfigId);
		GameObject gameObject = this.monthGo;
		UILabel component = gameObject.transform.Find("DailyRefresh").GetComponent<UILabel>();
		DateTime dateTime = new DateTime(1970, 1, 1);
		string arg = dateTime.ToLocalTime().ToString("hh:mm");
		component.text = string.Format(LanguageDataProvider.GetValue(2076), arg);
		UILabel component2 = gameObject.transform.Find("num").GetComponent<UILabel>();
		DateTime d = new DateTime(1970, 1, 1);
		long num = (long)(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - d).TotalSeconds;
		long num2 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) / 86400L;
		long num3 = (Solarmax.Singleton<LocalPlayer>.Get().month_card_end - num) % 86400L;
		if (num3 > 0L)
		{
			num2 += 1L;
		}
		UILabel component3 = gameObject.transform.Find("moneyLabel").transform.Find("day").GetComponent<UILabel>();
		component3.gameObject.SetActive(false);
		if (Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard())
		{
			if (Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive)
			{
				UILabel component4 = gameObject.transform.Find("moneyLabel").transform.Find("Label").GetComponent<UILabel>();
				component4.text = LanguageDataProvider.GetValue(2023);
				component3.gameObject.SetActive(true);
				component3.text = "(" + num2.ToString() + LanguageDataProvider.GetValue(2149) + ")";
				component2.text = data.GetPriceDesc();
			}
			else
			{
				UILabel component5 = gameObject.transform.Find("moneyLabel").transform.Find("Label").GetComponent<UILabel>();
				component5.text = LanguageDataProvider.GetValue(2023);
				component3.gameObject.SetActive(true);
				component3.text = "(" + num2.ToString() + LanguageDataProvider.GetValue(2149) + ")";
				component2.text = data.GetPriceDesc();
			}
		}
		if (!Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard())
		{
			UILabel component6 = gameObject.transform.Find("moneyLabel").transform.Find("Label").GetComponent<UILabel>();
			component6.text = LanguageDataProvider.GetValue(1154);
			component2.text = data.GetPriceDesc();
		}
	}

	private void UpdateUI()
	{
		int num = Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum;
		if (num >= 0 && num < this.cdList.Count)
		{
			this.money.text = this.cdList[num].GoldValue.ToString();
		}
		else
		{
			this.money.text = "0";
		}
		num = 1;
		foreach (StoreConfig storeConfig in this.model.storeConfigs)
		{
			if (storeConfig != null && (storeConfig.type == 1 || storeConfig.type == 2) && num >= 0 && num < this.product.Length)
			{
				this.product[num] = storeConfig.id;
				num++;
			}
		}
		this.RefreshMonthCard();
		this.RefreshProducts();
	}

	private void RefreshMonthCard()
	{
		UIButton component = this.rawardMonth.GetComponent<UIButton>();
		if (component != null)
		{
			if (Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive)
			{
				component.enabled = true;
				component.SetState(UIButtonColor.State.Normal, true);
			}
			else
			{
				component.enabled = false;
				component.SetState(UIButtonColor.State.Disabled, true);
			}
		}
	}

	public void RefreshProducts()
	{
		for (int i = 1; i < this.product.Length; i++)
		{
			if (!string.IsNullOrEmpty(this.product[i]))
			{
				this.InitTemplateUI(this.product[i], this.productTemplate[i]);
			}
		}
	}

	public void OnClickedAds()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum < Solarmax.Singleton<storeConfigProvider>.Get().MAX_SHOW_ADS_NUM)
		{
			if (Solarmax.Singleton<LocalPlayer>.Get().showAdsRefreshTime <= 0f)
			{
				AdManager.ShowAd(AdManager.ShowAdType.ShopAd, null);
			}
		}
		else
		{
			Tips.Make(LanguageDataProvider.GetValue(2153));
		}
	}

	public void OnBuyCard()
	{
		if (!this.CheckTimeLimit())
		{
			return;
		}
		Solarmax.Singleton<LoggerSystem>.Instance.Info("购买月卡", new object[0]);
		int num = 4;
		string text = this.product[num];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.BuyProduct(text);
	}

	public void OnBuyProdcut1()
	{
		if (!this.CheckTimeLimit())
		{
			return;
		}
		int num = 1;
		string text = this.product[num];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.BuyProduct(text);
	}

	public void OnBuyProdcut2()
	{
		if (!this.CheckTimeLimit())
		{
			return;
		}
		int num = 2;
		string text = this.product[num];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.BuyProduct(text);
	}

	public void OnBuyProdcut3()
	{
		if (!this.CheckTimeLimit())
		{
			return;
		}
		int num = 3;
		string text = this.product[num];
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		this.BuyProduct(text);
	}

	private void BuyProduct(string prodcutID)
	{
		CSGenerateOrderID csgenerateOrderID = new CSGenerateOrderID();
		csgenerateOrderID.productID = prodcutID;
		csgenerateOrderID.num = 1;
		Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSGenerateOrderID>(293, csgenerateOrderID);
	}

	public void CoodDown()
	{
		int nLookAdsNum = Solarmax.Singleton<LocalPlayer>.Get().playerData.nLookAdsNum;
		if (nLookAdsNum >= 0 && nLookAdsNum < Solarmax.Singleton<storeConfigProvider>.Get().MAX_SHOW_ADS_NUM)
		{
			Solarmax.Singleton<LocalPlayer>.Get().showAdsRefreshTime = (float)this.cdList[nLookAdsNum].cd;
			this.money.text = this.cdList[nLookAdsNum].GoldValue.ToString();
		}
		else
		{
			this.money.text = "0";
		}
	}

	private bool CheckTimeLimit()
	{
		DateTime? dateTime = this.searchTime;
		if (dateTime != null && (Solarmax.Singleton<TimeSystem>.Instance.GetServerTime() - this.searchTime.Value).TotalSeconds < 5.0)
		{
			return false;
		}
		this.searchTime = new DateTime?(Solarmax.Singleton<TimeSystem>.Instance.GetServerTime());
		return true;
	}

	public void ShowMonthCardEffect()
	{
		this.monthCardEffect.SetActive(true);
	}

	public void OnRewardMonthCard()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard() && Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.CSReceiveMonthlyCard();
		}
	}

	public UILabel money;

	public GameObject[] productTemplate;

	public string[] product;

	public GameObject monthCardEffect;

	private List<StoreConfig> cdList = new List<StoreConfig>();

	private CollectionModel model;

	public UILabel adCD;

	public UISlider adProcess;

	private DateTime? searchTime;

	private GameObject monthGo;

	public GameObject rawardMonth;

	private string monthCardConfigId;

	private enum ProdcutType
	{
		Pro1 = 1,
		Pro2,
		Pro3,
		Card
	}
}
