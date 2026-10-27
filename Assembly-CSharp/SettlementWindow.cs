using System;
using System.Collections.Generic;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class SettlementWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.SettlementWindowShow);
		base.RegisterEvent(EventId.OnDoubleAdClicked);
		base.RegisterEvent(EventId.OnDoubleAdCanceled);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
		this.grid.transform.DestroyChildren();
		this.grid.Reposition();
	}

	private void CalculateBattleResult()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SettlementWindow  CalculateBattleResult", new object[0]);
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.selectedCfgId);
		this.Diffise = data.difficult + 1;
		this.score = this.ShowScoreNum(this.temphitships, this.tempproduces, data);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUnLockNextLevel, new object[0]);
		float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
		MonoSingleton<FlurryAnalytis>.Instance.FlurryBattleEndEvent(this.selectedCfgId, "0", this.score.ToString(), this.starNum.ToString(), this.temphitships.ToString(), this.tempdestorys.ToString(), battleTime.ToString());
		MiGameAnalytics.MiAnalyticsBattleEndEvent(this.selectedCfgId, "0", this.score.ToString(), this.starNum.ToString(), this.temphitships.ToString(), this.tempdestorys.ToString(), battleTime.ToString());
	}

	public int ShowScoreNum(int hitships, int produces, LevelConfig cfg)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SettlementWindow  ShowScoreNum", new object[0]);
		int num = this.Diffise * 10000;
		int num2 = (hitships <= 500) ? hitships : 500;
		int num3 = (produces <= 500) ? produces : 500;
		float num4 = (6000f - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() * 20f >= 0f) ? (6000f - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() * 20f) : 0f;
		int num5 = Convert.ToInt32(Math.Round((double)((float)cfg.scoreper * ((float)(num + num2 + num3) + num4))));
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsNeedSend(this.starNum, num5))
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestSetLevelSorce(cfg.id, cfg.levelGroup, Solarmax.Singleton<LocalAccountStorage>.Get().account, num5);
			Solarmax.Singleton<NetSystem>.Instance.helper.SetLevelStar(Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id, cfg.id, this.starNum, num5);
			Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id] = num5;
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalLevelScore();
		}
		Solarmax.Singleton<LevelDataHandler>.Instance.SetLevelStarToLocalStorage(this.starNum, num5);
		return num5;
	}

	private void UpdateUI()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SettlementWindow   UpdateUI", new object[0]);
		this.scoreLabel.text = this.score.ToString();
		Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.selectedCfgId);
		string currentGroupID = Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID();
		int num = 0;
		int num2 = 0;
		AchievementModel.GetDiffcultStar(currentGroupID, out num, out num2);
		Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("SettlementWindow   UpdateUI   star information: num {0} num2 {1}", num, num2), new object[0]);
		for (int i = 0; i < 3; i++)
		{
			this.stars[i].SetActive(false);
			this.unstars[i].SetActive(i < num);
		}
		this.starTable.Reposition();
		for (int j = 0; j < num; j++)
		{
			if (j < num2)
			{
				this.stars[j].SetActive(true);
				this.stars[j].transform.Find("ParticleSystem").gameObject.SetActive(false);
			}
			else
			{
				this.stars[j].SetActive(false);
				this.stars[j].transform.Find("ParticleSystem").gameObject.SetActive(true);
			}
		}
		this.victory.SetActive(false);
		this.defeat.SetActive(false);
		this.rewardGo.SetActive(false);
		if (this.success)
		{
			this.defeat.SetActive(false);
			this.integral.SetActive(true);
			this.passTimeContainer.SetActive(true);
			this.victory.SetActive(true);
			this.achieveContainer.SetActive(false);
			Solarmax.Singleton<AchievementManager>.Get().GetAdsAchievement();
			this.adsBtn.SetActive(false);
			Solarmax.Singleton<LoggerSystem>.Instance.Info(string.Format("SettlementWindow  UpdateUI  set num info:  destory {0}  time {1}  lost {2}", this.temphitships, this.tempTime, this.tempdestorys), new object[0]);
			this.destroyLabel.text = this.temphitships.ToString();
			this.timeLabel.text = string.Format("{0:N2}s", this.tempTime);
			this.lostLabel.text = this.tempdestorys.ToString();
			this.completeAchievementLable.gameObject.SetActive(false);
		}
		else
		{
			this.defeat.SetActive(true);
			this.integral.SetActive(false);
			this.passTimeContainer.SetActive(false);
			this.achieveContainer.SetActive(false);
			this.destroyLabel.text = this.temphitships.ToString();
			this.lostLabel.text = this.tempdestorys.ToString();
			this.achieveReward.text = string.Format("x0", new object[0]);
			this.adsBtn.SetActive(false);
			this.backBtn.SetActive(false);
			this.norBtn.SetActive(true);
			this.completeAchievementLable.gameObject.SetActive(false);
		}
		string id = Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.id;
		if (Solarmax.Singleton<LevelDataHandler>.Get().GetPayChapterInfo(id) != null)
		{
			this.adsBtn.SetActive(false);
			this.backBtn.SetActive(false);
			this.norBtn.SetActive(true);
			this.completeAchievementLable.gameObject.SetActive(false);
		}
		this.table.Reposition();
		this.viewTable.Reposition();
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("SettlementWindow   OnUIEventHandler", new object[0]);
		if (eventId == EventId.SettlementWindowShow)
		{
			this.selectedCfgId = (string)args[0];
			this.success = (bool)args[1];
			this.tempdestorys = (int)args[2];
			this.temphitships = (int)args[3];
			this.tempproduces = (int)args[4];
			this.tempTime = (float)args[5];
			this.bInitView = (bool)args[6];
			if (this.success)
			{
				this.CalculateBattleResult();
			}
			this.UpdateUI();
		}
	}

	public void OnClickBack()
	{
		if (Solarmax.Singleton<LevelDataHandler>.Get().currentChapter.type == 0)
		{
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow("LobbyWindowView");
			if (this.bInitView)
			{
				Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnGuiledEndStartGame, new object[0]);
			}
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSingleBattleEnd, new object[]
			{
				0
			});
		}
		else
		{
			Solarmax.Singleton<UISystem>.Get().HideAllWindow();
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ChapterWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnSingleBattleEnd, new object[]
			{
				0
			});
		}
	}

	public void OnBack()
	{
		this.OnClickBack();
	}

	public void OnClickAds()
	{
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
		{
			Solarmax.Singleton<AchievementManager>.Get().FinishADSAchievement(true);
			MonoSingleton<FlurryAnalytis>.Instance.LogPveResultLookAds();
			this.ClaimAdsReward();
			this.OnClickBack();
		});
	}

	private void ClaimAdsReward()
	{
		foreach (Achievement achievement in Solarmax.Singleton<AchievementManager>.Get().completeList)
		{
			if (achievement.types[0] == AchievementType.Ads && Solarmax.Singleton<TaskConfigProvider>.Get().achieveToTask.ContainsKey(achievement.taskId) && Solarmax.Singleton<TaskConfigProvider>.Get().achieveToTask[achievement.taskId].status != TaskStatus.Received)
			{
				TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Get().achieveToTask[achievement.taskId];
				Solarmax.Singleton<TaskModel>.Get().ClaimReward(taskConfig.id, null, 1);
			}
		}
	}

	public void OnOpenAchieve()
	{
		this.eff.Clear();
		for (int i = 0; i < 3; i++)
		{
			GameObject gameObject = this.stars[i].transform.Find("ParticleSystem").gameObject;
			this.eff.Add(gameObject.activeSelf);
			gameObject.SetActive(false);
		}
		this.achieveContainer.SetActive(true);
	}

	public void OnCloseAchieve()
	{
		for (int i = 0; i < this.eff.Count; i++)
		{
			GameObject gameObject = this.stars[i].transform.Find("ParticleSystem").gameObject;
			gameObject.SetActive(this.eff[i]);
		}
		this.eff.Clear();
		this.achieveContainer.SetActive(false);
	}

	public UILabel scoreLabel;

	public GameObject achieveContainer;

	public GameObject victory;

	public GameObject defeat;

	public GameObject integral;

	public GameObject passTimeContainer;

	public UITable table;

	public UILabel achieveTitle;

	public UILabel achieveReward;

	public UILabel destroyLabel;

	public UILabel timeLabel;

	public UILabel lostLabel;

	public GameObject[] stars;

	public GameObject[] unstars;

	public UIGrid grid;

	public GameObject achieveTemplate;

	public UIScrollView scroll;

	public UITable starTable;

	public GameObject rewardGo;

	public UITable viewTable;

	public GameObject norBtn;

	public GameObject backBtn;

	public GameObject adsBtn;

	public UILabel adsLable;

	private int starNum = 1;

	private int Diffise;

	private int starPower;

	private string selectedCfgId;

	private float tempTime;

	private int tempdestorys;

	private int temphitships;

	private int tempproduces;

	private int connectedNum;

	private bool success;

	public UILabel completeAchievementLable;

	private int completeAchievement;

	private int reward;

	private int score;

	private bool bInitView;

	private List<bool> eff = new List<bool>();
}
