using System;
using DG.Tweening;
using Solarmax;
using UnityEngine;

public class LotteryAwardItemBh : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetExtraId(int extarId)
	{
		AwardItem extraItem = Solarmax.Singleton<LuckModel>.Get().GetExtraItem(extarId);
		this.numLabel.text = "x" + extraItem.itemNum;
		this.awardInfo = Solarmax.Singleton<LotteryAddProvider>.Get().dataDict[extarId];
		this.cntLabel.text = LanguageDataProvider.Format(2316, new object[]
		{
			this.awardInfo.count
		});
		if (extraItem.type == 1)
		{
			this.iconSprite.spriteName = "icon_currency";
			this.fragMarkObj.SetActive(false);
		}
		else
		{
			this.iconSprite.spriteName = extraItem.itemIcon;
			if (extraItem.needCount > 1)
			{
				this.fragMarkObj.SetActive(true);
			}
			else
			{
				this.fragMarkObj.SetActive(false);
			}
		}
	}

	public void OnItemClick()
	{
		Debug.Log("OnItemClick");
		if (Solarmax.Singleton<LuckModel>.Get().tweener != null && Solarmax.Singleton<LuckModel>.Get().tweener.IsPlaying())
		{
			return;
		}
		Solarmax.Singleton<NetSystem>.Get().helper.LotteryAward(this.awardInfo.id);
	}

	public void SetHasTook(bool hasTook)
	{
		this.checkObj.SetActive(hasTook);
	}

	public void SetCanTake(bool canTake)
	{
		if (canTake)
		{
			this.bgSprite.spriteName = "B_kelingqu";
		}
		else
		{
			this.bgSprite.spriteName = "B_bukelingqu";
		}
	}

	public LotteryAddConfig awardInfo;

	public UISprite iconSprite;

	public UILabel numLabel;

	public UILabel cntLabel;

	public GameObject checkObj;

	public GameObject fragMarkObj;

	public UISprite bgSprite;
}
