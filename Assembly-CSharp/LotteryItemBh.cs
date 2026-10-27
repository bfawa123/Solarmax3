using System;
using Solarmax;
using UnityEngine;

public class LotteryItemBh : MonoBehaviour
{
	public void SetWheelId(int wheelId)
	{
		this.wheelID = wheelId;
		LotteryConfig lotteryConfig = Solarmax.Singleton<LotteryConfigProvider>.Get().dataDict[wheelId];
		AwardItem lotteryWheelItem = Solarmax.Singleton<LuckModel>.Get().GetLotteryWheelItem(wheelId);
		this.numLabel.text = "x" + lotteryWheelItem.itemNum;
		if (lotteryWheelItem.type == 1)
		{
			this.iconSprite.spriteName = "icon_currency";
		}
		else
		{
			this.iconSprite.spriteName = lotteryWheelItem.itemIcon;
			if (lotteryWheelItem.needCount > 1)
			{
				this.fragMarkSprite.gameObject.SetActive(true);
			}
		}
		if ((double)lotteryConfig.percent <= 0.01)
		{
		}
	}

	public UISprite iconSprite;

	public UISprite fragMarkSprite;

	public UITexture iconTextrue;

	public UILabel numLabel;

	public UISprite bgSprite;

	public int wheelID;
}
