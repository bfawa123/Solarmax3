using System;
using System.Collections;
using Solarmax;
using UnityEngine;

public class VictoryWindowViewNew : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.VictioryWindowViewShow);
		base.RegisterEvent(EventId.RequestUserResult);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.RequestUserResult)
		{
			this.CaleBattleResult();
		}
		else if (eventId == EventId.VictioryWindowViewShow)
		{
			this.selectedCfgId = (args[0] as string);
			this.tempdestorys = (int)args[1];
			this.temphitships = (int)args[2];
			this.tempproduces = (int)args[3];
			this.starNum = AchievementModel.GetCompletedStars(Solarmax.Singleton<LevelDataHandler>.Get().GetCurrentGroupID());
			this.CaleBattleResult();
		}
	}

	public void PlaySingleStarSound()
	{
		Solarmax.Singleton<AudioManger>.Get().PlayEffect("starSound");
	}

	public int ShowScoreNum(int hitships, int produces, LevelConfig cfg)
	{
		int num = this.Diffise * 10000;
		int num2 = (hitships <= 500) ? hitships : 500;
		int num3 = (produces <= 500) ? produces : 500;
		float num4 = (6000f - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() * 20f >= 0f) ? (6000f - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime() * 20f) : 0f;
		int num5 = (int)((float)cfg.scoreper * ((float)(num + num2 + num3) + num4));
		if (Solarmax.Singleton<LevelDataHandler>.Instance.IsNeedSend(this.starNum, num5))
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestSetLevelSorce(cfg.id, cfg.levelGroup, Solarmax.Singleton<LocalAccountStorage>.Get().account, num5);
			Solarmax.Singleton<NetSystem>.Instance.helper.SetLevelStar(Solarmax.Singleton<LevelDataHandler>.Instance.currentChapter.id, cfg.id, this.starNum, num5);
			Solarmax.Singleton<LocalLevelScoreStorage>.Get().levelScore[Solarmax.Singleton<LevelDataHandler>.Instance.currentLevel.id] = num5;
			Solarmax.Singleton<LocalStorageSystem>.Instance.SaveLocalLevelScore();
		}
		Solarmax.Singleton<LevelDataHandler>.Instance.SetLevelStarToLocalStorage(this.starNum, num5);
		this.scoreNum.text = num5.ToString();
		return num5;
	}

	public void ShowStarBehavior(int nDif)
	{
		this.Diffise = nDif;
		for (int i = 0; i < this.starList.Length; i++)
		{
			this.starList[i].SetActive(false);
		}
	}

	private void AnimatorPlayEnd()
	{
		this.aniPlayer.onFinished.Clear();
		Solarmax.Singleton<UISystem>.Get().HideWindow("VictoryWindowView");
	}

	private void PlayAnimation(string strAni, float speed = 1f)
	{
		this.aniPlayer.clipName = strAni;
		this.aniPlayer.resetOnPlay = true;
		this.aniPlayer.Play(true, false, 1f, false);
	}

	private IEnumerator LoginServer()
	{
		this.connectedNum++;
		yield return Solarmax.Singleton<NetSystem>.Instance.helper.ConnectServer(false);
		if (Solarmax.Singleton<NetSystem>.Instance.GetConnector().GetConnectStatus() == ConnectionStatus.CONNECTED)
		{
			Solarmax.Singleton<NetSystem>.Instance.helper.RequestUser();
		}
		else
		{
			yield return new WaitForSeconds(1f);
			if (this.connectedNum <= 2)
			{
				global::Coroutine.Start(this.LoginServer());
			}
			else
			{
				this.CaleBattleResult();
			}
		}
		yield break;
	}

	private void CaleBattleResult()
	{
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.selectedCfgId);
		this.ShowStarBehavior(data.difficult + 1);
		int num = this.ShowScoreNum(this.temphitships, this.tempproduces, data);
		this.aniPlayer.onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.AnimatorPlayEnd)));
		this.PlayAnimation("RewardsWindowView_in", 1f);
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnUnLockNextLevel, new object[0]);
		float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
		MonoSingleton<FlurryAnalytis>.Instance.FlurryBattleEndEvent(this.selectedCfgId, "0", num.ToString(), this.starNum.ToString(), this.temphitships.ToString(), this.tempdestorys.ToString(), battleTime.ToString());
		MiGameAnalytics.MiAnalyticsBattleEndEvent(this.selectedCfgId, "0", num.ToString(), this.starNum.ToString(), this.temphitships.ToString(), this.tempdestorys.ToString(), battleTime.ToString());
	}

	public UIPlayAnimation aniPlayer;

	public UILabel awardPower;

	public UILabel awardMoney;

	public UILabel scoreNum;

	public GameObject[] starList;

	private int starNum = 1;

	private int Diffise;

	private int starPower;

	private string selectedCfgId;

	private int tempdestorys;

	private int temphitships;

	private int tempproduces;

	private int tempstars;

	private int connectedNum;
}
