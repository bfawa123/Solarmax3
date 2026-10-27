using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ChestWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnChestNotify);
		base.RegisterEvent(EventId.OnChestTime);
		base.RegisterEvent(EventId.OnChestBattle);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.chest1.SetActive(false);
		this.itemTemp.SetActive(false);
		string[] array = base.gameObject.name.Split(new char[]
		{
			'('
		});
		if (!string.IsNullOrEmpty(array[0]))
		{
			GuideManager.StartGuide(GuildCondition.GC_Ui, array[0], base.gameObject);
		}
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId != EventId.OnChestNotify)
		{
			if (eventId != EventId.OnChestTime)
			{
				if (eventId == EventId.OnChestBattle)
				{
					this.msg2 = (args[0] as SCGainBattleChest);
					int battlechestid = Solarmax.Singleton<LocalPlayer>.Get().playerData.battlechestid;
					ChestConfig data = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(battlechestid);
					if (data == null)
					{
						return;
					}
					this.curChestIcon0.spriteName = data.iconopen;
					this.chestType = 2;
				}
			}
			else
			{
				this.msg1 = (args[0] as SCGainTimerChest);
				int timechestid = Solarmax.Singleton<LocalPlayer>.Get().playerData.timechestid;
				ChestConfig data2 = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(timechestid);
				if (data2 == null)
				{
					return;
				}
				this.curChestIcon0.spriteName = data2.iconopen;
				this.chestType = 1;
			}
		}
		else
		{
			this.msg = (args[0] as SCGainChest);
			if (this.msg == null)
			{
				return;
			}
			int slot = this.msg.slot;
			ChessItem chessItem = Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot];
			ChestConfig data3 = Solarmax.Singleton<ChestConfigProvider>.Instance.GetData(chessItem.id);
			if (data3 == null)
			{
				return;
			}
			this.curChestIcon0.spriteName = data3.iconopen;
			this.chestType = 0;
		}
		this.OnWindownClicked();
	}

	private void OnWindownClicked()
	{
		if (this.itemCount > 0)
		{
			this.chest1.SetActive(false);
			this.itemCount--;
			this.curChestLast.spriteName = this.itemCount.ToString();
			PackItem item = null;
			int curvalue = 0;
			int maxvalue = 0;
			int skill = 0;
			int num = this.chestType;
			if (num != 0)
			{
				if (num != 1)
				{
					if (num == 2)
					{
						item = this.msg2.items[this.itemCount];
						curvalue = this.msg2.add_num[this.itemCount];
						maxvalue = this.msg2.levelup_num[this.itemCount];
						skill = this.msg2.skillids[this.itemCount];
					}
				}
				else
				{
					item = this.msg1.items[this.itemCount];
					curvalue = this.msg1.add_num[this.itemCount];
					maxvalue = this.msg1.levelup_num[this.itemCount];
					skill = this.msg1.skillids[this.itemCount];
				}
			}
			else
			{
				item = this.msg.items[this.itemCount];
				curvalue = this.msg.add_num[this.itemCount];
				maxvalue = this.msg.levelup_num[this.itemCount];
				skill = this.msg.skillids[this.itemCount];
			}
			this.ShowItem(item, curvalue, maxvalue, skill);
		}
		else if (this.itemCount == 0)
		{
			this.chest1.SetActive(true);
			this.ShowAllItem();
			this.itemCount = -1;
		}
		else
		{
			if (this.chestType == 0 && this.msg.slot > 0)
			{
				int slot = this.msg.slot;
				Solarmax.Singleton<LocalPlayer>.Get().playerData.chesses[slot] = null;
			}
			Solarmax.Singleton<NetSystem>.Instance.helper.LoadClientStorage();
			Solarmax.Singleton<UISystem>.Get().HideWindow("ChestWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnChestNotify, new object[0]);
		}
	}

	private void ShowItem(PackItem item, int curvalue, int maxvalue, int skill)
	{
	}

	private void ShowAllItem()
	{
		int num = 0;
		int num2 = this.chestType;
		if (num2 != 0)
		{
			if (num2 != 1)
			{
				if (num2 == 2)
				{
					num = this.msg2.items.Count;
				}
			}
			else
			{
				num = this.msg1.items.Count;
			}
		}
		else
		{
			num = this.msg.items.Count;
		}
		for (int i = 0; i < num; i++)
		{
			PackItem packItem = null;
			int num3 = 0;
			int num4 = this.chestType;
			if (num4 != 0)
			{
				if (num4 != 1)
				{
					if (num4 == 2)
					{
						packItem = this.msg2.items[i];
						num3 = this.msg2.add_num[i];
					}
				}
				else
				{
					packItem = this.msg1.items[i];
					num3 = this.msg1.add_num[i];
				}
			}
			else
			{
				packItem = this.msg.items[i];
				num3 = this.msg.add_num[i];
			}
			ItemConfig data = Solarmax.Singleton<ItemConfigProvider>.Instance.GetData(packItem.itemid);
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.itemTemp);
			gameObject.SetActive(true);
			gameObject.name = string.Format("item{0}", i);
			gameObject.transform.parent = this.itemTemp.transform.parent;
			gameObject.transform.localScale = Vector3.one;
			Vector3 localPosition = this.itemTemp.transform.localPosition;
			if (num % 2 == 0)
			{
				if (i < num / 2)
				{
					localPosition.x += (float)(i - num / 2 + 1) * 220f;
				}
				else
				{
					localPosition.x = -localPosition.x + (float)(i - num / 2) * 220f;
				}
			}
			else
			{
				localPosition.x += (float)(i - num / 2) * 256f;
			}
			gameObject.transform.localPosition = localPosition;
			UISprite component = gameObject.transform.Find("icon").GetComponent<UISprite>();
			UILabel component2 = gameObject.transform.Find("level").GetComponent<UILabel>();
			component.spriteName = data.icon;
			component2.text = string.Format("X{0}", num3);
		}
	}

	public GameObject chest0;

	public UISprite curItemIcon;

	public UILabel curItemNum;

	public UILabel curItemName;

	public UILabel curItemDesc;

	public UISlider curItemSlider;

	public UILabel curItemPersent;

	public UISprite curChestIcon0;

	public UISprite curChestLast;

	public GameObject chest1;

	public GameObject itemTemp;

	private int itemCount;

	private SCGainChest msg;

	private SCGainTimerChest msg1;

	private SCGainBattleChest msg2;

	private int chestType;
}
