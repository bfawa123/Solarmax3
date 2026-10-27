using System;
using System.Collections.Generic;
using MiGameSDK;
using Solarmax;
using UnityEngine;

public class DailyTaskTemplate : MonoBehaviour
{
	public void EnsureInit(TaskConfig config, bool month = false)
	{
		if (config == null)
		{
			return;
		}
		this.task = config;
		this.model = Solarmax.Singleton<TaskModel>.Get();
		this.isMonthCardTask = month;
		this.UpdateUI();
	}

	public void EnsureDestroy()
	{
	}

	private void UpdateUI()
	{
		this.title.text = LanguageDataProvider.GetValue(this.task.title);
		if (this.isMonthCardTask)
		{
			this.desc.text = string.Format("{0}{1}", LanguageDataProvider.GetValue(1154), LanguageDataProvider.GetValue(this.task.descId));
		}
		else
		{
			this.desc.text = LanguageDataProvider.GetValue(this.task.descId);
		}
		if (!string.IsNullOrEmpty(this.task.icon))
		{
			this.taskIcon.spriteName = this.task.icon;
		}
		if (this.task.rewardType == Solarmax.RewardType.Gold && this.task.rewardValue > 0)
		{
			this.moneyObject.SetActive(true);
			this.moneyReward.text = this.task.rewardValue.ToString();
		}
		else
		{
			this.moneyObject.SetActive(false);
			this.moneyReward.text = "0";
		}
		if (this.task.reward2Type == Solarmax.RewardType.Degree && this.task.reward2Value > 0)
		{
			this.degreeObject.SetActive(true);
			this.degreeReward.text = this.task.reward2Value.ToString();
		}
		else
		{
			this.degreeObject.SetActive(false);
			this.degreeReward.text = "0";
		}
		TaskStatus status = this.task.status;
		if (status != TaskStatus.Completed)
		{
			if (status != TaskStatus.Received)
			{
				if (status == TaskStatus.Unfinished)
				{
					this.unfinished.SetActive(true);
					this.finished.SetActive(false);
					if (this.task.subType == FinishConntion.Level)
					{
						int num = Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin;
						if (num > this.task.taskParameter)
						{
							num = this.task.taskParameter;
						}
						this.unfinishedLabel.text = Solarmax.Singleton<LocalPvpStorage>.Get().pvpWin.ToString() + "/" + this.task.taskParameter.ToString();
					}
					else if (this.task.subType == FinishConntion.Pve)
					{
						int num2 = Solarmax.Singleton<LocalPvpStorage>.Get().pve;
						if (num2 > this.task.taskParameter)
						{
							num2 = this.task.taskParameter;
						}
						this.unfinishedLabel.text = num2.ToString() + "/" + this.task.taskParameter.ToString();
					}
					else if (this.task.subType == FinishConntion.OnLine)
					{
						int num3 = (int)Solarmax.Singleton<LocalPlayer>.Get().mOnLineTime / 60;
						if (num3 > this.task.taskParameter)
						{
							num3 = this.task.taskParameter;
						}
						this.unfinishedLabel.text = num3.ToString() + "/" + this.task.taskParameter.ToString();
					}
					else if (this.task.subType == FinishConntion.Ads)
					{
						int num4 = Solarmax.Singleton<LocalPvpStorage>.Get().lookAds;
						if (num4 > this.task.taskParameter)
						{
							num4 = this.task.taskParameter;
						}
						this.unfinishedLabel.text = num4 + "/" + this.task.taskParameter.ToString();
					}
					else
					{
						int num5 = Solarmax.Singleton<LocalPvpStorage>.Get().pvpDestroy;
						if (num5 > this.task.taskParameter)
						{
							num5 = this.task.taskParameter;
						}
						this.unfinishedLabel.text = num5 + "/" + this.task.taskParameter.ToString();
					}
				}
			}
			else
			{
				this.table.gameObject.SetActive(false);
				this.unfinished.SetActive(false);
				this.finished.SetActive(true);
			}
		}
		else
		{
			this.unfinished.SetActive(false);
			this.showAdBtn.SetActive(!this.isMonthCardTask);
			this.finished.SetActive(false);
		}
	}

	public void Update()
	{
		this.fRefrushTime += Time.deltaTime;
		if (this.fRefrushTime <= 30f)
		{
			return;
		}
		this.fRefrushTime = 0f;
		if (this.task != null)
		{
			if (this.task.status == TaskStatus.Unfinished)
			{
				if (this.task.subType == FinishConntion.OnLine)
				{
					int num = (int)Solarmax.Singleton<LocalPlayer>.Get().mOnLineTime / 60;
					if (num > this.task.taskParameter)
					{
						num = this.task.taskParameter;
					}
					this.unfinishedLabel.text = num.ToString() + "/" + this.task.taskParameter.ToString();
				}
				if (!this.norReward.enabled && !this.adsReward.enabled)
				{
					return;
				}
				this.norReward.enabled = false;
				this.norSprite.spriteName = "Button_s2";
				this.adsSprite.spriteName = "Button_s2";
				this.adsReward.enabled = false;
			}
			else if (this.task.status == TaskStatus.Completed)
			{
				if (this.norReward.enabled && this.adsReward.enabled)
				{
					return;
				}
				this.norReward.enabled = true;
				this.norSprite.spriteName = "Button_s3";
				this.adsReward.enabled = true;
				this.adsSprite.spriteName = "Ad-button";
			}
		}
	}

	public void OnClickedDone()
	{
		if (!this.canClick)
		{
			return;
		}
		this.onCDTime = 2f;
		this.canClick = false;
		if (this.isMonthCardTask)
		{
			if (Solarmax.Singleton<LocalPlayer>.Get().IsRechargeRewardCard() && Solarmax.Singleton<LocalPlayer>.Get().IsMonthCardReceive)
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Info("领取月卡", new object[0]);
				Solarmax.Singleton<NetSystem>.Instance.helper.CSReceiveMonthlyCard();
			}
		}
		else
		{
			Solarmax.Singleton<UISystem>.Instance.ShowWindow(new ShowWindowParams("RewardTipsWindow", EventId.OnShowRewardTipsWindow, new object[]
			{
				RewardTipsWindow.ViewType.AD
			}));
			Solarmax.Singleton<TaskModel>.Get().claimTask = this.task;
			Solarmax.Singleton<TaskModel>.Get().claimReward = new RewardTipsModel(this.task.rewardValue, global::RewardType.Money, false, 0);
		}
	}

	public void OnClickedShowAdBtn()
	{
		if (!this.canClick)
		{
			return;
		}
		this.onCDTime = 2f;
		this.canClick = false;
		AdManager.ShowAd(AdManager.ShowAdType.DoubleAd, delegate(object[] param)
		{
			if (this.task != null)
			{
				MonoSingleton<FlurryAnalytis>.Instance.LogTaskLookAds();
				Solarmax.Singleton<TaskModel>.Get().claimTask = this.task;
				Solarmax.Singleton<TaskModel>.Get().claimReward = new RewardTipsModel(this.task.rewardValue * 2, global::RewardType.Money, false, 0);
				Solarmax.Singleton<TaskModel>.Get().ClaimReward(this.task.id, null, 2);
			}
			else
			{
				Solarmax.Singleton<LoggerSystem>.Instance.Error("双倍领取任务失败，task为空", new object[0]);
			}
		});
	}

	private void OnRequestTaskOk(List<string> success, List<string> failure)
	{
		foreach (string value in success)
		{
			if (this.task.id.Equals(value))
			{
				TaskConfig taskConfig = Solarmax.Singleton<TaskConfigProvider>.Instance.GetTask(this.task.id);
				taskConfig.status = TaskStatus.Received;
				this.unfinished.SetActive(false);
				this.done.SetActive(false);
				this.finished.SetActive(true);
				return;
			}
		}
		foreach (string value2 in failure)
		{
			if (this.task.id.Equals(value2))
			{
				break;
			}
		}
	}

	private void FixedUpdate()
	{
		if (this.onCDTime > 1E-45f)
		{
			this.onCDTime += Time.deltaTime;
			if (this.onCDTime > 0.5f)
			{
				this.canClick = true;
				this.onCDTime = float.Epsilon;
			}
		}
	}

	public UILabel title;

	public UILabel desc;

	public UITable table;

	public GameObject unfinished;

	public GameObject done;

	public GameObject finished;

	public UILabel unfinishedLabel;

	public UILabel doneLabel;

	public UILabel finishedLabel;

	public GameObject showAdBtn;

	public UISprite taskIcon;

	public GameObject degreeObject;

	public UILabel degreeReward;

	public UIButton norReward;

	public UISprite norSprite;

	public UIButton adsReward;

	public UISprite adsSprite;

	public GameObject moneyObject;

	public UILabel moneyReward;

	private TaskConfig task;

	private TaskModel model;

	private const float CD_TIME = 0.5f;

	private bool isMonthCardTask;

	private float onCDTime;

	private bool canClick = true;

	private float fRefrushTime = 30f;
}
