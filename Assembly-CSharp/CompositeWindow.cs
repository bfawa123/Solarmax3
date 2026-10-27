using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;

public class CompositeWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnSelectBagItem);
		return true;
	}

	private void ShowBase()
	{
		this.itemicon.gameObject.SetActive(false);
		this.curandMax.text = string.Empty;
		this.fenjiePrice.text = string.Empty;
		this.buqimoney.text = string.Empty;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.ShowBase();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnSelectBagItem)
		{
			this.SelectOneItem((int)args[0]);
		}
	}

	private void ShowBag()
	{
		List<PackItem> itemList = Solarmax.Singleton<ItemDataHandler>.Instance.itemList;
		if (itemList == null || itemList.Count <= 0)
		{
			return;
		}
	}

	private void SelectOneItem(int sn)
	{
		PackItem itemBySN = Solarmax.Singleton<ItemDataHandler>.Instance.GetItemBySN(sn);
		if (itemBySN != null)
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(itemBySN.itemid);
			if (data != null)
			{
				this.itemicon.gameObject.SetActive(true);
				this.itemicon.spriteName = data.icon;
				this.curandMax.text = string.Format("{0}/{1}", itemBySN.num, data.needCount);
				this.fenjiePrice.text = data.Coprice.ToString();
				int num = data.Coprice * ((data.needCount - itemBySN.num >= 0) ? (data.needCount - itemBySN.num) : 0);
				this.buqimoney.text = num.ToString();
				this.SelectNum = itemBySN.num;
				this.snSelect = itemBySN;
			}
		}
	}

	public void OnNorComposite()
	{
		if (this.snSelect != null)
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(this.snSelect.itemid);
			if (data == null || this.snSelect.num <= 0)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2306), 3f);
				return;
			}
			if (data.func != ITEMFUNCTION.Composite && data.func != ITEMFUNCTION.All)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2305), 3f);
				return;
			}
			if (this.SelectNum < data.needCount)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2308), 3f);
				return;
			}
			if (data.resultType == ProductType.Chapter && Solarmax.Singleton<LevelDataHandler>.Get().IsBuyChapter(data.resultID.ToString()))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2328), 3f);
				return;
			}
			if (data.resultType == ProductType.Icon && Solarmax.Singleton<CollectionModel>.Get().IsUnLock(data.resultID))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2329), 3f);
				return;
			}
			if (data.resultType == ProductType.Bg && Solarmax.Singleton<CollectionModel>.Get().IsUnLock(data.resultID))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2330), 3f);
				return;
			}
			Solarmax.Singleton<NetSystem>.Get().helper.RequestUseItem(this.snSelect.itemid, this.SelectNum);
		}
	}

	public void OnMoneyComposite()
	{
		if (this.snSelect != null)
		{
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(this.snSelect.itemid);
			if (data == null || this.snSelect.num <= 0)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2306), 3f);
				return;
			}
			if (data.func != ITEMFUNCTION.Composite && data.func != ITEMFUNCTION.All)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2305), 3f);
				return;
			}
			int num = data.Coprice * ((data.needCount - this.snSelect.num >= 0) ? (data.needCount - this.snSelect.num) : 0);
			if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < num)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(1102), 3f);
				return;
			}
			if (data.resultType == ProductType.Chapter && Solarmax.Singleton<LevelDataHandler>.Get().IsBuyChapter(data.resultID.ToString()))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2328), 3f);
				return;
			}
			if (data.resultType == ProductType.Icon && Solarmax.Singleton<CollectionModel>.Get().IsUnLock(data.resultID))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2329), 3f);
				return;
			}
			if (data.resultType == ProductType.Bg && Solarmax.Singleton<CollectionModel>.Get().IsUnLock(data.resultID))
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2330), 3f);
				return;
			}
			Solarmax.Singleton<NetSystem>.Get().helper.RequestUseItem(this.snSelect.itemid, this.SelectNum);
		}
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("CompositeWindow");
	}

	public UISprite itemicon;

	public UILabel curandMax;

	public UILabel fenjiePrice;

	public UILabel buqimoney;

	private int SelectNum;

	private PackItem snSelect;
}
