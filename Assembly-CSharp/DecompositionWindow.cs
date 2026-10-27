using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class DecompositionWindow : BaseWindow
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
		this.itemname.text = string.Empty;
		this.fenjiePrice.text = string.Empty;
		this.itemnum.text = string.Empty;
		this.huodeMoney.text = "0";
		this.SelectNum = 1;
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
			this.UpdateLable(true);
		}
	}

	private void Update()
	{
		if (this.IsSlider)
		{
			this.UpdateLable(false);
			this.IsSlider = false;
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
				this.itemname.text = LanguageDataProvider.GetValue(data.name);
				this.fenjiePrice.text = data.Deprice.ToString();
				this.itemnum.text = string.Format(LanguageDataProvider.GetValue(2292), itemBySN.num);
				this.huodeMoney.text = "0";
				if (data.func == ITEMFUNCTION.Composite || data.func == ITEMFUNCTION.All)
				{
					this.mark.SetActive(true);
				}
				else
				{
					this.mark.SetActive(false);
				}
				this.snSelect = itemBySN;
			}
		}
	}

	public void OnClickFenJie()
	{
		if (this.snSelect != null)
		{
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
			if (this.SelectNum <= 0)
			{
				Tips.Make(Tips.TipsType.FlowUp, LanguageDataProvider.GetValue(2306), 3f);
				return;
			}
			Solarmax.Singleton<NetSystem>.Get().helper.RequestDecomposeItem(this.snSelect.id, this.SelectNum);
			Solarmax.Singleton<UISystem>.Get().HideWindow("DecompositionWindow");
		}
	}

	public void OnClickCancle()
	{
		this.OnClose();
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("DecompositionWindow");
	}

	public void OnAdd()
	{
		if (this.snSelect != null)
		{
			int num = this.snSelect.num;
			if (this.SelectNum >= num)
			{
				return;
			}
			this.SelectNum++;
			this.UpdateLable(true);
		}
	}

	public void OnDel()
	{
		if (this.snSelect != null)
		{
			int num = this.snSelect.num;
			if (this.SelectNum <= 1)
			{
				return;
			}
			this.SelectNum--;
			this.UpdateLable(true);
		}
	}

	public void OnMax()
	{
		if (this.snSelect != null)
		{
			this.SelectNum = this.snSelect.num;
			this.UpdateLable(true);
		}
	}

	public void OnUiSliderValueChange()
	{
		float value = this.curSlider.value;
		this.SelectNum = (int)((float)this.snSelect.num * value);
		if (value >= 1f)
		{
			this.SelectNum = this.snSelect.num;
		}
		if (value <= 0.0001f)
		{
			this.SelectNum = 1;
		}
		if (this.SelectNum <= 0)
		{
			this.SelectNum = 1;
		}
		this.IsSlider = true;
	}

	private void UpdateLable(bool bMonifyProc = true)
	{
		if (this.snSelect != null)
		{
			int num = this.snSelect.num;
			this.fenjieNum.text = string.Format("{0}/{1}", this.SelectNum, num);
			if (bMonifyProc)
			{
				float value = (float)this.SelectNum / ((float)num * 1f);
				this.curSlider.value = value;
			}
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(this.snSelect.itemid);
			if (data != null)
			{
				int num2 = this.SelectNum * data.Deprice;
				this.huodeMoney.text = num2.ToString();
			}
		}
	}

	public UISprite itemicon;

	public UILabel itemname;

	public UILabel fenjiePrice;

	public UILabel itemnum;

	public UILabel huodeMoney;

	public UILabel fenjieNum;

	public UISlider curSlider;

	public GameObject mark;

	private int SelectNum;

	private bool IsSlider;

	private PackItem snSelect;
}
