using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class BagWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnUpdateKnapsack);
		base.RegisterEvent(EventId.OnSelectBagItem);
		return true;
	}

	private void ShowBase()
	{
		this.iconSelect.gameObject.SetActive(false);
		this.nameSelect.text = string.Empty;
		this.descSelect.text = string.Empty;
		this.Frag.SetActive(false);
		this.numSelect.text = string.Format(LanguageDataProvider.GetValue(2292), string.Empty);
		List<PackItem> itemList = Solarmax.Singleton<ItemDataHandler>.Instance.itemList;
		if (itemList == null || itemList.Count <= 0)
		{
			return;
		}
		itemList.Sort(delegate(PackItem p1, PackItem p2)
		{
			if (p1.itemid < p2.itemid)
			{
				return -1;
			}
			if (p1.itemid > p2.itemid)
			{
				return 1;
			}
			return 0;
		});
		this.SelectOneItem(itemList[0].id);
	}

	public override void OnShow()
	{
		base.OnShow();
		this.ShowBase();
		this.ShowBag();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnUpdateKnapsack)
		{
			this.ShowBag();
			this.ShowBase();
		}
		else if (eventId == EventId.OnSelectBagItem)
		{
			this.SelectOneItem((int)args[0]);
		}
	}

	private void ShowBag()
	{
		this.parant.transform.DestroyChildren();
		this.scrollView.ResetPosition();
		List<PackItem> itemList = Solarmax.Singleton<ItemDataHandler>.Instance.itemList;
		if (itemList == null || itemList.Count <= 0)
		{
			return;
		}
		itemList.Sort(delegate(PackItem p1, PackItem p2)
		{
			if (p1.itemid < p2.itemid)
			{
				return -1;
			}
			if (p1.itemid > p2.itemid)
			{
				return 1;
			}
			return 0;
		});
		int num = itemList.Count / 6;
		int num2 = itemList.Count % 6;
		if (num2 > 0)
		{
			num++;
		}
		int num3 = 0;
		int count = itemList.Count;
		GameObject gameObject = null;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject2 = this.parant.gameObject.AddChild(this.template);
			BagWindowCellParent component = gameObject2.GetComponent<BagWindowCellParent>();
			if (component != null)
			{
				for (int j = 0; j < 6; j++)
				{
					if (num3 < count)
					{
						component.SetInfo(j, itemList[num3]);
					}
					else
					{
						component.SetInfo(j, null);
					}
					num3++;
				}
				if (gameObject == null)
				{
					component.cell[0].select.SetActive(true);
					gameObject = component.cell[0].select;
				}
			}
			gameObject2.SetActive(true);
		}
		Solarmax.Singleton<ItemDataHandler>.Get().curSelect = gameObject;
		this.parant.Reposition();
		if (num > 5)
		{
			this.scrollView.disableDragIfFits = false;
		}
		else
		{
			this.scrollView.disableDragIfFits = true;
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
				this.iconSelect.gameObject.SetActive(true);
				if (data.func == ITEMFUNCTION.Composite || data.func == ITEMFUNCTION.All)
				{
					this.Frag.SetActive(true);
				}
				else
				{
					this.Frag.SetActive(false);
				}
				this.iconSelect.spriteName = data.icon;
				this.nameSelect.text = LanguageDataProvider.GetValue(data.name);
				this.numSelect.text = string.Format(LanguageDataProvider.GetValue(2292), itemBySN.num);
				if (data.func == ITEMFUNCTION.Composite || data.func == ITEMFUNCTION.All)
				{
					this.descSelect.text = string.Format(LanguageDataProvider.GetValue(data.desc), data.needCount);
				}
				else
				{
					this.descSelect.text = LanguageDataProvider.GetValue(data.desc);
				}
				this.snSelect = itemBySN;
			}
		}
	}

	public void OnClickFenJie()
	{
		if (this.snSelect != null)
		{
			if (Solarmax.Singleton<ItemDataHandler>.Get().GetItemBySN(this.snSelect.id) == null)
			{
				return;
			}
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(this.snSelect.itemid);
			if (data == null || this.snSelect.num <= 0)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2306), 3f);
				return;
			}
			if (data.func != ITEMFUNCTION.Decomposition && data.func != ITEMFUNCTION.All)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2305), 3f);
				return;
			}
			Solarmax.Singleton<UISystem>.Get().ShowWindow("DecompositionWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSelectBagItem, new object[]
			{
				this.snSelect.id
			});
		}
	}

	public void OnClickShiYong()
	{
		if (this.snSelect != null)
		{
			if (Solarmax.Singleton<ItemDataHandler>.Get().GetItemBySN(this.snSelect.id) == null)
			{
				return;
			}
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(this.snSelect.itemid);
			if (data == null)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2306), 3f);
				return;
			}
			if (data.func == ITEMFUNCTION.Decomposition)
			{
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
				Solarmax.Singleton<NetSystem>.Get().helper.RequestUseItem(this.snSelect.itemid, 1);
			}
			else
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CompositeWindow");
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSelectBagItem, new object[]
				{
					this.snSelect.id
				});
			}
		}
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("BagWindow");
		Solarmax.Singleton<UISystem>.Get().ShowWindow("HomeWindow");
	}

	public UISprite iconSelect;

	public UILabel nameSelect;

	public UILabel descSelect;

	public UILabel numSelect;

	public GameObject Frag;

	public GameObject template;

	public UIScrollView scroll;

	public UITable parant;

	public UIScrollView scrollView;

	private PackItem snSelect;
}
