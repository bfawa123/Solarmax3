using System;
using System.Collections.Generic;
using NetMessage;
using Solarmax;

public class PaidLevelBuyTOPWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnShowActiveChapters);
		base.RegisterEvent(EventId.OnHaveNewChapterUnlocked);
		return true;
	}

	public override void OnShow()
	{
		Solarmax.Singleton<NetSystem>.Get().helper.StartRequestActiveChapter(Solarmax.Singleton<LevelDataHandler>.Instance.payChapterList);
		Solarmax.Singleton<NetSystem>.Get().helper.StartRequestActiveChapterPrice();
		for (int i = 0; i < this.activePanel.Length; i++)
		{
			this.activePanel[i].go.SetActive(false);
		}
		base.OnShow();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnShowActiveChapters)
		{
			this.RefrushTop3();
		}
		else if (eventId == EventId.OnHaveNewChapterUnlocked)
		{
			this.RefrushTop3();
		}
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Instance.HideWindow("PaidLevelBuyTOPWindow");
	}

	private void RefrushTop3()
	{
		if (this.activePanel.Length != 3)
		{
			return;
		}
		List<string> topChapterList = Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList;
		if (topChapterList.Count != this.activePanel.Length)
		{
			return;
		}
		for (int i = 0; i < topChapterList.Count; i++)
		{
			string chapterId = topChapterList[i];
			ChapterInfo payChapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetPayChapterInfo(chapterId);
			if (payChapterInfo != null)
			{
				this.activePanel[i].go.SetActive(true);
				this.activePanel[i].SetInfo(payChapterInfo);
			}
		}
	}

	public void OnBuyTop1()
	{
		int count = Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList.Count;
		if (count >= 1)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[0]))
			{
				Tips.Make(LanguageDataProvider.GetValue(1149));
				return;
			}
			MonoSingleton<FlurryAnalytis>.Instance.LogTopBuyChapters("hot_buy1");
			this.OnBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[0]);
		}
	}

	public void OnBuyTop2()
	{
		int count = Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList.Count;
		if (count >= 2)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[1]))
			{
				Tips.Make(LanguageDataProvider.GetValue(1149));
				return;
			}
			MonoSingleton<FlurryAnalytis>.Instance.LogTopBuyChapters("hot_buy2");
			this.OnBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[1]);
		}
	}

	public void OnBuyTop3()
	{
		int count = Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList.Count;
		if (count >= 3)
		{
			if (Solarmax.Singleton<LevelDataHandler>.Instance.IsBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[2]))
			{
				Tips.Make(LanguageDataProvider.GetValue(1149));
				return;
			}
			MonoSingleton<FlurryAnalytis>.Instance.LogTopBuyChapters("hot_buy3");
			this.OnBuyChapter(Solarmax.Singleton<LevelDataHandler>.Instance.topChapterList[2]);
		}
	}

	public void OnBuyChapter(string chapterID)
	{
		this.mBuyChapterID = chapterID;
		ChapterInfo payChapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetPayChapterInfo(chapterID);
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(payChapterInfo.id);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		string value = LanguageDataProvider.GetValue(data.name);
		string value2 = LanguageDataProvider.GetValue(data.describe);
		Solarmax.Singleton<UISystem>.Get().ShowWindow("ActiveBuyWidnow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnCommonDialog1, new object[]
		{
			LanguageDataProvider.GetValue(1100),
			new EventDelegate(new EventDelegate.Callback(this.BuyChapter)),
			chapterID
		});
	}

	private void BuyChapter()
	{
		if (string.IsNullOrEmpty(this.mBuyChapterID))
		{
			return;
		}
		ChapterInfo payChapterInfo = Solarmax.Singleton<LevelDataHandler>.Instance.GetPayChapterInfo(this.mBuyChapterID);
		if (Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.mBuyChapterID) == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData.money < payChapterInfo.nPromotionPrice)
		{
			Tips.Make(LanguageDataProvider.GetValue(1102));
			Coroutine.DelayDo(0.2f, new EventDelegate(delegate()
			{
				Solarmax.Singleton<UISystem>.Get().ShowWindow("GoldTipsWindow");
			}));
		}
		else
		{
			CSBuyChapter csbuyChapter = new CSBuyChapter();
			csbuyChapter.chapter = payChapterInfo.id;
			Solarmax.Singleton<NetSystem>.Instance.helper.SendProto<CSBuyChapter>(219, csbuyChapter);
		}
	}

	public ActiveChapterObject[] activePanel;

	private string mBuyChapterID = string.Empty;
}
