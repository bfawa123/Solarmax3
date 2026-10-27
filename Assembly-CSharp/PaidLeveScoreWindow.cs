using System;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class PaidLeveScoreWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnCommonDialogLayout);
		base.RegisterEvent(EventId.OnDoubleAdClicked);
		base.RegisterEvent(EventId.OnDoubleAdCanceled);
		base.RegisterEvent(EventId.OnPingFenSunccess);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		for (int i = 0; i < this.interestListenter.Length; i++)
		{
			this.interestListenter[i].onClick = new UIEventListener.VoidDelegate(this.OnInterestScore);
		}
		for (int j = 0; j < this.strategyListenter.Length; j++)
		{
			this.strategyListenter[j].onClick = new UIEventListener.VoidDelegate(this.OnStrategyScore);
		}
		for (int k = 0; k < this.totalListenter.Length; k++)
		{
			this.totalListenter[k].onClick = new UIEventListener.VoidDelegate(this.OnTotalScore);
		}
		this.InitPage();
	}

	public override void OnHide()
	{
	}

	private void InitPage()
	{
		this.RefrushPage(0, this.interest_score);
		this.RefrushPage(1, this.strategy_score);
		this.RefrushPage(2, this.total_score);
	}

	private void RefrushPage(int type, int value)
	{
		if (type == 0)
		{
			this.interestLable.text = value.ToString();
			for (int i = 0; i < this.interestSprite.Length; i++)
			{
				this.interestSprite[i].spriteName = "Btn_comment_likeA";
			}
			for (int j = 0; j < value; j++)
			{
				this.interestSprite[j].spriteName = "Btn_comment_likeAC";
			}
		}
		if (type == 1)
		{
			this.strategyLable.text = value.ToString();
			for (int k = 0; k < this.interestSprite.Length; k++)
			{
				this.strategySprite[k].spriteName = "Btn_comment_likeA";
			}
			for (int l = 0; l < value; l++)
			{
				this.strategySprite[l].spriteName = "Btn_comment_likeAC";
			}
		}
		if (type == 2)
		{
			this.totalLable.text = value.ToString();
			for (int m = 0; m < this.interestSprite.Length; m++)
			{
				this.totalSprite[m].spriteName = "Btn_comment_likeA";
			}
			for (int n = 0; n < value; n++)
			{
				this.totalSprite[n].spriteName = "Btn_comment_likeAC";
			}
		}
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnCommonDialogLayout)
		{
			if ((int)args[0] == 0)
			{
				this.ok.SetActive(true);
				this.ad.SetActive(true);
			}
			else
			{
				this.ok.SetActive(true);
				this.ad.SetActive(true);
			}
			this.chapterID = (string)args[1];
			ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterID);
			if (data == null)
			{
				Tips.Make(LanguageDataProvider.GetValue(1101));
				return;
			}
			this.orgMoney.text = data.comment.ToString();
			this.beiMoney.text = data.commentMultiple.ToString();
		}
		else if (eventId == EventId.OnDoubleAdClicked)
		{
			ChapterConfig config = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterID);
			if (config == null)
			{
				Tips.Make(LanguageDataProvider.GetValue(1101));
				return;
			}
			this.IsLookAds = true;
			AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogCommentLookAds();
				Solarmax.Singleton<NetSystem>.Get().helper.StartSetChapterScore(this.chapterID, this.strategy_score, this.interest_score, this.total_score, config.commentMultiple);
			});
		}
		else if (eventId == EventId.OnDoubleAdCanceled)
		{
			this.IsLookAds = false;
			Solarmax.Singleton<NetSystem>.Get().helper.StartSetChapterScore(this.chapterID, this.strategy_score, this.interest_score, this.total_score, 1);
		}
		else if (eventId == EventId.OnPingFenSunccess)
		{
			ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterID);
			if (data2 == null)
			{
				Tips.Make(LanguageDataProvider.GetValue(1101));
				return;
			}
			if (this.IsLookAds)
			{
				Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
				{
					RewardTipsWindow.ViewType.Reward,
					new RewardTipsModel(data2.comment * data2.commentMultiple, global::RewardType.Money, false, 0)
				}));
			}
			else
			{
				Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
				{
					RewardTipsWindow.ViewType.Reward,
					new RewardTipsModel(data2.comment, global::RewardType.Money, false, 0)
				}));
			}
			Solarmax.Singleton<UISystem>.Get().HideWindow("PaidLeveScoreWindow");
		}
	}

	public void OnClose()
	{
		Solarmax.Singleton<UISystem>.Get().HideWindow("PaidLeveScoreWindow");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnRefrushPingFenBtn, new object[0]);
	}

	public void OnOK()
	{
		if (!this.IsCommitScore())
		{
			return;
		}
		ChapterConfig data = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterID);
		if (data == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
		{
			RewardTipsWindow.ViewType.Reward2,
			new RewardTipsModel(data.commentMultiple, global::RewardType.Money, false, 0)
		}));
	}

	private bool IsCommitScore()
	{
		if (this.strategy_score <= 0 || this.interest_score <= 0 || this.total_score <= 0)
		{
			Tips.Make(LanguageDataProvider.GetValue(2275));
			return false;
		}
		return true;
	}

	public void OnAds()
	{
		if (!this.IsCommitScore())
		{
			return;
		}
		ChapterConfig config = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(this.chapterID);
		if (config == null)
		{
			Tips.Make(LanguageDataProvider.GetValue(1101));
			return;
		}
		this.IsLookAds = true;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
		{
			MonoSingleton<FlurryAnalytis>.Instance.LogCommentLookAds();
			Solarmax.Singleton<NetSystem>.Get().helper.StartSetChapterScore(this.chapterID, this.strategy_score, this.interest_score, this.total_score, config.commentMultiple);
		});
	}

	public void OnStrategyScore(GameObject go)
	{
		string name = go.name;
		int num = int.Parse(name);
		if (num == this.strategy_score)
		{
			return;
		}
		this.strategy_score = num;
		this.RefrushPage(1, this.strategy_score);
	}

	public void OnInterestScore(GameObject go)
	{
		string name = go.name;
		int num = int.Parse(name);
		if (num == this.interest_score)
		{
			return;
		}
		this.interest_score = num;
		this.RefrushPage(0, this.interest_score);
	}

	public void OnTotalScore(GameObject go)
	{
		string name = go.name;
		int num = int.Parse(name);
		if (num == this.total_score)
		{
			return;
		}
		this.total_score = num;
		this.RefrushPage(2, this.total_score);
	}

	public GameObject ok;

	public GameObject ad;

	public UILabel interestLable;

	public UILabel strategyLable;

	public UILabel totalLable;

	public UIEventListener[] interestListenter;

	public UIEventListener[] strategyListenter;

	public UIEventListener[] totalListenter;

	public UISprite[] interestSprite;

	public UISprite[] strategySprite;

	public UISprite[] totalSprite;

	public UILabel orgMoney;

	public UILabel beiMoney;

	private string chapterID = string.Empty;

	private int interest_score = 5;

	private int strategy_score = 5;

	private int total_score = 5;

	private bool IsLookAds;
}
