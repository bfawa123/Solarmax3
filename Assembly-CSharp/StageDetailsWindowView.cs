using System;
using System.Collections.Generic;
using Solarmax;
using UnityEngine;

public class StageDetailsWindowView : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.ShowTestFriend();
		UIEventListener.Get(this.startButton).onClick = new UIEventListener.VoidDelegate(this.OnStartButtonClick);
		UIEventListener.Get(this.closeButton).onClick = new UIEventListener.VoidDelegate(this.OnCloseButtonClick);
		UIEventListener.Get(this.friendButton).onClick = new UIEventListener.VoidDelegate(this.OnFriendTabSelected);
		UIEventListener.Get(this.allButton).onClick = new UIEventListener.VoidDelegate(this.OnAllTabSelected);
		UIEventListener.Get(this.firstGoodsItem).onClick = new UIEventListener.VoidDelegate(this.OnGoodsItemClicked);
		UIEventListener.Get(this.secondGoodsItem).onClick = new UIEventListener.VoidDelegate(this.OnGoodsItemClicked);
		UIEventListener.Get(this.thirdGoodsItem).onClick = new UIEventListener.VoidDelegate(this.OnGoodsItemClicked);
		this.goodsItem.Clear();
		this.goodsItem.Add(this.firstGoodsItem);
		this.goodsItem.Add(this.secondGoodsItem);
		this.goodsItem.Add(this.thirdGoodsItem);
	}

	public override void OnHide()
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	public void OnCloseClick()
	{
	}

	public void ShowTestFriend()
	{
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = this.limitGo.AddChild(this.friendTemplate);
			gameObject.SetActive(true);
		}
		this.limitGo.GetComponent<UIGrid>().repositionNow = true;
	}

	public void OnStartButtonClick(GameObject go)
	{
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestStartLevel(this.selectLevelConfig.id);
	}

	public void OnCloseButtonClick(GameObject go)
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().ShowWindow("LobbyWindowView");
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.UpdateChapterWindow, new object[]
		{
			0
		});
	}

	private void OnStartLevelResult()
	{
		string map = this.selectLevelConfig.map;
		string id = this.selectLevelConfig.id;
		Solarmax.Singleton<LocalPlayer>.Get().playerData.singleFightNext = (this.selectLevelConfig.id == this.levelList[this.levelList.Count - 1]);
		if (this.difficultyLevel == 0)
		{
			this.difficultyLevel = 1;
		}
		Solarmax.Singleton<BattleSystem>.Instance.battleData.difficultyLevel = this.difficultyLevel;
		Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = 1;
		Solarmax.Singleton<LevelDataHandler>.Instance.SetSelectLevel(id, 0);
		LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Get().GetData(id);
		if (data != null)
		{
			if (this.difficultyLevel == 1)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.easyAI;
			}
			else if (this.difficultyLevel == 2)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.generalAI;
			}
			else if (this.difficultyLevel == 3)
			{
				Solarmax.Singleton<BattleSystem>.Instance.battleData.aiLevel = data.hardAI;
			}
			Solarmax.Singleton<BattleSystem>.Instance.battleData.aiParam = data.aiParam;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.dyncDiffType = data.dyncDiffType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winType = data.winType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam1 = data.winTypeParam1;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.winTypeParam2 = data.winTypeParam2;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseType = data.loseType;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam1 = data.loseTypeParam1;
			Solarmax.Singleton<BattleSystem>.Instance.battleData.loseTypeParam2 = data.loseTypeParam2;
		}
		Solarmax.Singleton<NetSystem>.Instance.helper.RequestSingleMatch(map, GameType.SingleLevel, true);
	}

	public void OnStartStageBattle()
	{
		Solarmax.Singleton<EventSystem>.Instance.FireEvent(EventId.NoticeSelfTeam, new object[0]);
		Solarmax.Singleton<ShipFadeManager>.Get().SetShipAlpha(0f);
		TweenAlpha tweenAlpha = base.gameObject.GetComponent<TweenAlpha>();
		if (tweenAlpha == null)
		{
			tweenAlpha = base.gameObject.AddComponent<TweenAlpha>();
		}
		tweenAlpha.ResetToBeginning();
		tweenAlpha.from = 1f;
		tweenAlpha.to = 0f;
		tweenAlpha.duration = 0.6f;
		tweenAlpha.SetOnFinished(delegate()
		{
			this.StartSingleBattle();
		});
	}

	public void StartSingleBattle()
	{
		Solarmax.Singleton<UISystem>.Get().HideAllWindow();
		Solarmax.Singleton<UISystem>.Get().FadeBattle(true, new EventDelegate(delegate()
		{
			Solarmax.Singleton<BattleSystem>.Instance.StartLockStep();
		}));
		Solarmax.Singleton<UISystem>.Get().ShowWindow("BattleWindow_off");
		GuideManager.StartGuide(GuildCondition.GC_Level, Solarmax.Singleton<BattleSystem>.Instance.battleData.matchId, null);
	}

	private void OnFriendTabSelected(GameObject go)
	{
		this.allButtonLine.SetActive(false);
		this.friendButtonLine.SetActive(true);
	}

	private void OnAllTabSelected(GameObject go)
	{
		this.allButtonLine.SetActive(true);
		this.friendButtonLine.SetActive(false);
	}

	private void OnGoodsItemClicked(GameObject go)
	{
		foreach (GameObject gameObject in this.goodsItem)
		{
			gameObject.GetComponent<StageDetailsWindowViewGoodsItemBehavior>().Show(gameObject == go);
		}
	}

	private void ShowStarBehavior(int maxStars)
	{
		this.stageThreeStar.SetActive(maxStars == 3);
		this.stageFourStar.SetActive(maxStars != 3);
	}

	public void ShowScore()
	{
	}

	public void ShowStar()
	{
	}

	public UIEventListener eventListener;

	public GameObject startButton;

	public GameObject firstGoodsItem;

	public GameObject secondGoodsItem;

	public GameObject thirdGoodsItem;

	public GameObject friendButton;

	public GameObject allButton;

	public GameObject friendButtonLine;

	public GameObject allButtonLine;

	public GameObject friendTemplate;

	public GameObject limitGo;

	public GameObject localPlayer;

	public GameObject stageName;

	public GameObject closeButton;

	public GameObject stageThreeStar;

	public GameObject stageFourStar;

	public GameObject threeStars;

	public GameObject oneStarInFour;

	public GameObject twoStarInFour;

	public GameObject threeStarInFour;

	public GameObject oneStarInThree;

	public GameObject twoStarInThree;

	public GameObject fourFuzzyStar;

	public GameObject threeFuzzyStar;

	public GameObject totalScore;

	public GameObject costPower;

	public GameObject leftStar;

	private LevelConfig selectLevelConfig;

	private int difficultyLevel;

	private List<string> mapList = new List<string>();

	private List<string> levelList = new List<string>();

	private List<GameObject> goodsItem = new List<GameObject>();
}
