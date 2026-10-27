using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ItemDataHandler : Solarmax.Singleton<ItemDataHandler>, IDataHandler, Lifecycle
{
	public ItemDataHandler()
	{
		this.curSelect = null;
	}

	public bool Init()
	{
		this.itemList = new List<PackItem>();
		return true;
	}

	public void InitPage(Pack data)
	{
		this.itemList.Clear();
		for (int i = 0; i < data.items.Count; i++)
		{
			PackItem item = data.items[i];
			this.ModifyItem(item);
		}
	}

	public void ModifyItem(PackItem item)
	{
		PackItem packItem = null;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].id == item.id)
			{
				packItem = this.itemList[i];
				break;
			}
		}
		if (packItem == null)
		{
			packItem = new PackItem();
			packItem.id = item.id;
			packItem.itemid = item.itemid;
			packItem.num = item.num;
			this.itemList.Add(packItem);
		}
		else if (item.num == 0)
		{
			this.itemList.Remove(packItem);
		}
		else
		{
			packItem.num = item.num;
		}
	}

	public PackItem GetItemBySN(int sn)
	{
		PackItem result = null;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].id == sn)
			{
				result = this.itemList[i];
				break;
			}
		}
		return result;
	}

	public PackItem GetItemByTypeID(int id)
	{
		PackItem result = null;
		for (int i = 0; i < this.itemList.Count; i++)
		{
			if (this.itemList[i].itemid == id)
			{
				result = this.itemList[i];
				break;
			}
		}
		return result;
	}

	public void Reset()
	{
		this.itemList.Clear();
	}

	public void Tick(float interval)
	{
	}

	public void Destroy()
	{
	}

	private void OnEventHandler(int eventId, object data, params object[] args)
	{
	}

	public List<PackItem> itemList;

	public GameObject curSelect;
}
