using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class BagWindowCell : MonoBehaviour
{
	public void SetInfo(PackItem pi)
	{
		if (pi == null)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.SetActive(true);
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Get().GetData(pi.itemid);
			if (data != null)
			{
				this.icon.spriteName = data.icon;
				this.num.text = pi.num.ToString();
				this.desc.text = LanguageDataProvider.GetValue(data.name);
				this.sn = pi.id;
				if (data.func == ITEMFUNCTION.Composite || data.func == ITEMFUNCTION.All)
				{
					this.mark.SetActive(true);
				}
				else
				{
					this.mark.SetActive(false);
				}
			}
		}
	}

	public void OnClickItem(GameObject go)
	{
		if (Solarmax.Singleton<ItemDataHandler>.Get().curSelect != null)
		{
			Solarmax.Singleton<ItemDataHandler>.Get().curSelect.SetActive(false);
		}
		this.select.SetActive(true);
		Solarmax.Singleton<ItemDataHandler>.Get().curSelect = this.select;
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSelectBagItem, new object[]
		{
			this.sn
		});
	}

	public UISprite icon;

	public UILabel desc;

	public UILabel num;

	public GameObject mark;

	public GameObject select;

	public int sn;
}
