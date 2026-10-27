using System;
using Solarmax;
using UnityEngine;

public class ActiveBuyWidnow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonDialog1);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		base.gameObject.SetActive(false);
	}

	public override void OnHide()
	{
		this.onYes = null;
		this.yesBtn1.gameObject.SetActive(false);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialog1)
		{
			string str = (string)args[0];
			this.onYes = (EventDelegate)args[1];
			string chapterID = (string)args[2];
			int layout = 0;
			if (args.Length >= 4)
			{
				layout = (int)args[3];
			}
			this.SetInfo(str, chapterID, layout);
		}
	}

	private void SetInfo(string str, string chapterID, int layout)
	{
		ChapterInfo payChapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetPayChapterInfo(chapterID);
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(payChapterInfo.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		string text = LanguageDataProvider.GetValue(data.name);
		string text2 = LanguageDataProvider.GetValue(data.describe);
		this.tips.text = str;
		this.title.text = text;
		this.tips.text = text2;
		this.buyCount.text = string.Format(LanguageDataProvider.GetValue(2267), payChapterInfo.nBuyCount);
		this.value[1].text = string.Format("{0:F1}", payChapterInfo.fStrategyStart);
		this.value[0].text = string.Format("{0:F1}", payChapterInfo.fInterestStart);
		this.value[2].text = string.Format("{0:F1}", payChapterInfo.ftotalStart);
		if (payChapterInfo.nPromotionPrice > 0 && payChapterInfo.nPromotionPrice < data.costGold)
		{
			this.oldPrice.gameObject.SetActive(true);
			this.yesBtnLabel1.color = Color.red;
			this.yesBtnLabel1.text = payChapterInfo.nPromotionPrice.ToString();
			this.oldPrice.text = "(" + data.costGold.ToString() + ")";
		}
		else
		{
			this.yesBtnLabel1.color = Color.white;
			this.yesBtnLabel1.text = data.costGold.ToString();
			this.oldPrice.gameObject.SetActive(false);
		}
		if (layout == 1)
		{
			this.yesBtn1.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(true);
	}

	public void OnYesClick()
	{
		if (this.onYes != null)
		{
			this.onYes.Execute();
		}
		Solarmax.Singleton<UISystem>.Get().HideWindow("ActiveBuyWidnow");
	}

	public void OnNoClick()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("ActiveBuyWidnow");
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("ActiveBuyWidnow");
	}

	public UIButton yesBtn1;

	public UILabel yesBtnLabel1;

	public UILabel tips;

	public UILabel[] value;

	public UILabel title;

	public UILabel buyCount;

	public UILabel oldPrice;

	private EventDelegate onYes;
}
