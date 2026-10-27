using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class BattleEndWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnFinished);
		base.RegisterEvent(EventId.OnFinishedColor);
		return true;
	}

	public override void OnShow()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleEndWindow  OnShow", new object[0]);
		base.OnShow();
		this.gameType = Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType;
		if (this.gameType == GameType.Single || this.gameType == GameType.PayLevel || this.gameType == GameType.TestLevel || this.gameType == GameType.GuildeLevel || this.gameType == GameType.SingleLevel)
		{
			Solarmax.Singleton<BattleSystem>.Instance.lockStep.Reset();
			base.Invoke("FinishSingle", 3f);
			this.lineSingle.gameObject.SetActive(false);
			TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
			if (tweenAlpha == null)
			{
				tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
			}
			tweenAlpha.ResetToBeginning();
			tweenAlpha.from = 1f;
			tweenAlpha.to = 0f;
			tweenAlpha.duration = 2f;
			tweenAlpha.Play(true);
		}
		else
		{
			this.haveResult = false;
			base.Invoke("FinishPvp", 3f);
			this.linePvp.gameObject.SetActive(true);
		}
		LoggerSystem.CodeComments("Unfinished problem: why there are bug when show the twenalpha?");
	}

	public override void OnHide()
	{
		base.CancelInvoke("FinishPvp");
		base.CancelInvoke("FinishSingle");
		this.lineSingle.gameObject.SetActive(false);
		this.linePvp.gameObject.SetActive(false);
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnFinished)
		{
			this.haveResult = true;
			if (args.Length > 0)
			{
				this.proto = (args[0] as SCFinishBattle);
			}
		}
		else if (eventId == EventId.OnFinishedColor)
		{
			Color color = (Color)args[0];
			this.lineSingleRenderer.color = color;
			this.linePvpRenderer.color = color;
			this.winTeam = (args[1] as Team);
		}
	}

	public void FinishPvp()
	{
		if (this.haveResult)
		{
			Solarmax.Singleton<UISystem>.Get().HideWindow("BattleEndWindow");
			Solarmax.Singleton<UISystem>.Get().HideWindow("BattleWindow");
			Solarmax.Singleton<UISystem>.Get().HideWindow("ReplayBattleWindow");
			Solarmax.Singleton<UISystem>.Get().HideWindow("CommonDialogWindow");
			Solarmax.Singleton<UISystem>.Get().ShowWindow("ResultWindow");
			Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.OnFinished, new object[]
			{
				this.proto
			});
		}
		else
		{
			base.Invoke("FinishPvpBattle", 0.5f);
		}
	}

	public void FinishPvpBattle()
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow(new ShowWindowParams("LobbyWindowView", EventId.UpdateChaptersWindow, new object[]
		{
			1
		}));
	}

	/// <summary>
	/// 结算使用的关卡ID：地图切换后 matchId 会变成第二张地图ID，这里返回原始关卡ID
	/// </summary>
	private string GetSettlementLevelId()
	{
		BattleData battleData = Solarmax.Singleton<BattleSystem>.Instance.battleData;
		return string.IsNullOrEmpty(battleData.levelId) ? battleData.matchId : battleData.levelId;
	}

	public void FinishSingle()
	{
		Solarmax.Singleton<LoggerSystem>.Instance.Info("BattleEndWindow  FinishSingle", new object[0]);
		Solarmax.Singleton<ShipFadeManager>.Get().SetFadeType(ShipFadeManager.FADETYPE.OUT, 0.25f);
		Solarmax.Singleton<UISystem>.Get().FadeBattle(false, new EventDelegate(delegate()
		{
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.SingleLevel || Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.GuildeLevel)
			{
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType == BattlePlayType.Replay)
				{
					Solarmax.Singleton<UISystem>.Get().HideAllWindow();
					bool flag = false;
					LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.replayManager.battleData.matchId);
					if (data != null)
					{
						ChapterConfig data2 = Solarmax.Singleton<ChapterConfigProvider>.Instance.GetData(data.chapter);
						if (data2 != null && data2.type == 1)
						{
							flag = true;
						}
					}
					if (Solarmax.Singleton<BattleSystem>.Instance.replayManager.battleData.isLevelReplay)
					{
						if (flag)
						{
							Solarmax.Singleton<UISystem>.Get().ShowWindow("ChapterWindow");
						}
						else
						{
							Solarmax.Singleton<UISystem>.Get().ShowWindow("LobbyWindowView");
						}
					}
					else
					{
						Solarmax.Singleton<UISystem>.Get().ShowWindow("ReplayWindow");
					}
				}
				else if (this.winTeam.team == Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
				{
					int destory = this.winTeam.destory;
					int hitships = this.winTeam.hitships;
					int produces = this.winTeam.produces;
					int star = this.winTeam.star;
					float battleTime = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
					bool flag2 = false;
					if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.GuildeLevel)
					{
						flag2 = true;
					}
					Solarmax.Singleton<UISystem>.Get().ShowWindow("SettlementWindow");
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.SettlementWindowShow, new object[]
					{
						this.GetSettlementLevelId(),
						true,
						destory,
						hitships,
						produces,
						battleTime,
						flag2
					});
				}
				else
				{
					Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
					int destory2 = team.destory;
					int hitships2 = team.hitships;
					int produces2 = team.produces;
					float battleTime2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
					bool flag3 = false;
					if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.GuildeLevel)
					{
						flag3 = true;
					}
					Solarmax.Singleton<UISystem>.Get().ShowWindow("SettlementWindow");
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.SettlementWindowShow, new object[]
					{
						this.GetSettlementLevelId(),
						false,
						destory2,
						hitships2,
						produces2,
						battleTime2,
						flag3
					});
				}
				Solarmax.Singleton<BattleSystem>.Instance.Reset();
				return;
			}
			if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.PayLevel)
			{
				if (this.winTeam.team == Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam)
				{
					int destory3 = this.winTeam.destory;
					int hitships3 = this.winTeam.hitships;
					int produces3 = this.winTeam.produces;
					int star2 = this.winTeam.star;
					float battleTime3 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
					Solarmax.Singleton<UISystem>.Get().ShowWindow("SettlementWindow");
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.SettlementWindowShow, new object[]
					{
						this.GetSettlementLevelId(),
						true,
						destory3,
						hitships3,
						produces3,
						battleTime3,
						false
					});
				}
				else
				{
					Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam(Solarmax.Singleton<BattleSystem>.Instance.battleData.currentTeam);
					int destory4 = team2.destory;
					int hitships4 = team2.hitships;
					int produces4 = team2.produces;
					float battleTime4 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime();
					Solarmax.Singleton<UISystem>.Get().ShowWindow("SettlementWindow");
					Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.SettlementWindowShow, new object[]
					{
						this.GetSettlementLevelId(),
						false,
						destory4,
						hitships4,
						produces4,
						battleTime4,
						false
					});
				}
				Solarmax.Singleton<BattleSystem>.Instance.Reset();
			}
		}));
		RapidBlurEffect behavior = Camera.main.GetComponent<RapidBlurEffect>();
		if (behavior != null)
		{
			behavior.enabled = true;
			behavior.MainBgScale(true, 4.5f, 0.035f);
			global::Coroutine.DelayDo(0.55f, new EventDelegate(delegate()
			{
				behavior.enabled = false;
			}));
		}
	}

	public SpriteRenderer lineSingleRenderer;

	public TweenAlpha lineSingle;

	public SpriteRenderer linePvpRenderer;

	public TweenAlpha linePvp;

	private bool haveResult;

	private SCFinishBattle proto;

	private Team winTeam;

	private GameType gameType;
}
