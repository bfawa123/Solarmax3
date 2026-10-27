using System;
using NetMessage;
using Solarmax;
using UnityEngine;

public class ReplayBattleWindow : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		base.RegisterEvent(EventId.OnBattleReplayFrame);
		base.RegisterEvent(EventId.ShowForWatchMode);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.showForWatch = false;
		this.battleTimeMax = this.GetBattleTimeMaxSec();
		this.populationTemplate = LanguageDataProvider.GetValue(200);
		for (int i = 0; i < this.playerNode.Length; i++)
		{
			this.playerNode[i].SetActive(false);
		}
		int num = 0;
		for (int j = 1; j < LocalPlayer.MaxTeamNum; j++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)j);
			if (team != null && team.Valid())
			{
				num++;
			}
		}
		int num2 = 0;
		for (int k = 1; k < LocalPlayer.MaxTeamNum; k++)
		{
			Team team2 = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)k);
			if (team2 != null && team2.Valid() && num2 <= num && num2 <= 5)
			{
				num2++;
				this.teamIndex[num2 - 1] = k;
				this.playerNode[num2 - 1].SetActive(true);
				this.populations[num2 - 1].gameObject.SetActive(true);
				string text = team2.playerData.icon;
				if (!text.EndsWith(".png"))
				{
					text += ".png";
				}
				this.raceIcons[num2 - 1].picUrl = text;
				Color color = team2.color;
				color.a = 1f;
				this.raceBgs[num2 - 1].color = color;
				this.names[num2 - 1].color = color;
				this.populations[num2 - 1].color = color;
				if (Solarmax.Singleton<BattleSystem>.Instance.battleData.gameType == GameType.SingleLevel && team2.playerData.userId < 0)
				{
					this.names[num2 - 1].text = LanguageDataProvider.GetValue(19);
				}
				else
				{
					this.names[num2 - 1].text = team2.playerData.name;
				}
				this.populations[num2 - 1].text = string.Format(this.populationTemplate, team2.current, team2.currentMax);
			}
		}
		this.playSpeed = 2;
		this.SetPlaySpeedBtnStatus();
		base.InvokeRepeating("TimerProc", 0.5f, 0.5f);
		RapidBlurEffect component = Camera.main.GetComponent<RapidBlurEffect>();
		if (component != null)
		{
			component.enabled = true;
			this.cameraDepth = Camera.main.orthographicSize;
			component.MainBgScale(false, 5.5f, 5.5f);
		}
	}

	private int GetBattleTimeMaxSec()
	{
		int num = int.Parse(Solarmax.Singleton<GameVariableConfigProvider>.Instance.GetData(4)) * 60;
		CooperationType battleSubType = Solarmax.Singleton<BattleSystem>.Instance.battleData.battleSubType;
		if (battleSubType == CooperationType.CT_4vPC || battleSubType == CooperationType.CT_3vPC || battleSubType == CooperationType.CT_2vPC)
		{
			MapConfig data = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId.ToString());
			if (data != null && data.coopTimeLimitMin > 0)
			{
				num = data.coopTimeLimitMin * 60;
			}
		}
		if (battleSubType == CooperationType.CT_3v1)
		{
			MapConfig data2 = Solarmax.Singleton<MapConfigProvider>.Instance.GetData(Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId.ToString());
			if (data2 != null && data2.pvpTimeLimitMin > 0)
			{
				num = data2.pvpTimeLimitMin * 60;
			}
		}
		return num;
	}

	public override void OnHide()
	{
		RapidBlurEffect component = Camera.main.GetComponent<RapidBlurEffect>();
		if (component != null)
		{
			component.enabled = true;
			component.MainBgScale(true, this.cameraDepth, this.cameraDepth);
		}
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
		if (eventId == EventId.OnBattleReplayFrame)
		{
			this.nowFrame = (int)args[0];
			this.totalFrame = (int)args[1];
		}
		else if (eventId == EventId.ShowForWatchMode)
		{
			for (int i = 0; i < this.playSpeedButton.Length; i++)
			{
				this.playSpeedButton[i].gameObject.SetActive(false);
			}
			this.pauseBtn.gameObject.SetActive(false);
			this.showForWatch = true;
		}
	}

	private void TimerProc()
	{
		for (int i = 1; i < 6; i++)
		{
			Team team = Solarmax.Singleton<BattleSystem>.Instance.sceneManager.teamManager.GetTeam((TEAM)this.teamIndex[i - 1]);
			if (team != null)
			{
				this.populations[i - 1].text = string.Format(this.populationTemplate, team.current, team.currentMax);
			}
		}
		if (this.showForWatch)
		{
			int num = Mathf.RoundToInt((float)this.battleTimeMax - Solarmax.Singleton<BattleSystem>.Instance.sceneManager.GetBattleTime());
			if (num >= 0)
			{
				this.time.text = string.Format("{0:D2}:{1:D2}", num / 60, num % 60);
			}
		}
		else if (Solarmax.Singleton<BattleSystem>.Instance.battleData.battleType == BattlePlayType.Replay)
		{
			int num2 = (this.totalFrame - this.nowFrame) / 30;
			this.time.text = string.Format("{0:D2}:{1:D2}", num2 / 60, num2 % 60);
		}
		else
		{
			int num3 = (this.totalFrame * 3 - this.nowFrame) / 30;
			this.time.text = string.Format("{0:D2}:{1:D2}", num3 / 60, num3 % 60);
		}
	}

	public void OnCloseClick()
	{
		Solarmax.Singleton<AudioManger>.Get().PlayAudioBG("Empty", 0.5f);
		if (this.showForWatch)
		{
			Solarmax.Singleton<UISystem>.Get().ShowWindow("CommonDialogWindow");
			EventSystem instance = Solarmax.Singleton<EventSystem>.Instance;
			EventId id = EventId.OnCommonDialog;
			object[] array = new object[3];
			array[0] = 2;
			array[1] = LanguageDataProvider.GetValue(913);
			array[2] = new EventDelegate(delegate()
			{
				Solarmax.Singleton<BattleSystem>.Instance.OnPlayerDirectQuit();
				Solarmax.Singleton<BattleSystem>.Instance.Reset();
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("CreateRoomWindow");
			});
			instance.FireEvent(id, array);
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.replayManager.PlayRecordOver();
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
			Solarmax.Singleton<BattleSystem>.Instance.Reset();
			if (Solarmax.Singleton<BattleSystem>.Instance.replayManager.battleData.isLevelReplay)
			{
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
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
				Solarmax.Singleton<UISystem>.Get().HideAllWindow();
				Solarmax.Singleton<UISystem>.Get().ShowWindow("ReplayWindow");
			}
		}
	}

	public void OnPauseClick()
	{
		if (Solarmax.Singleton<BattleSystem>.Instance.IsPause())
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(false);
			this.pausePage.SetActive(false);
			this.pauseBtn.normalSprite = "button_zanting";
		}
		else
		{
			Solarmax.Singleton<BattleSystem>.Instance.SetPause(true);
			this.pausePage.SetActive(true);
			this.pauseBtn.normalSprite = "button_sudix1";
		}
	}

	public void OnSpeedClick1()
	{
		if (this.playSpeed == 1)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 0.5f;
		this.playSpeed = 1;
		this.SetPlaySpeedBtnStatus();
	}

	public void OnSpeedClick2()
	{
		if (this.playSpeed == 2)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 1f;
		this.playSpeed = 2;
		this.SetPlaySpeedBtnStatus();
	}

	public void OnSpeedClick3()
	{
		if (this.playSpeed == 3)
		{
			return;
		}
		Solarmax.Singleton<BattleSystem>.Instance.lockStep.playSpeed = 2f;
		this.playSpeed = 3;
		this.SetPlaySpeedBtnStatus();
	}

	private void SetPlaySpeedBtnStatus()
	{
		for (int i = 0; i < this.playSpeedButton.Length; i++)
		{
			this.playSpeedButton[i].isEnabled = (i != this.playSpeed - 1);
		}
	}

	public UIButton pauseBtn;

	public GameObject[] playerNode;

	public UISprite[] raceBgs;

	public NetTexture[] raceIcons;

	public UILabel[] names;

	public UILabel[] populations;

	public GameObject pausePage;

	private int playSpeed = 2;

	public UIButton[] playSpeedButton;

	public UILabel time;

	private string populationTemplate;

	private int nowFrame;

	private int totalFrame;

	private bool showForWatch;

	private int battleTimeMax;

	private int[] teamIndex = new int[5];

	private float cameraDepth;
}
