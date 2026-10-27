using System;
using Solarmax;
using UnityEngine;

public class VictoryWindowView : BaseWindow
{
	public override bool Init()
	{
		base.Init();
		UIEventListener.Get(this.returnButton).onClick = new UIEventListener.VoidDelegate(this.OnReturnButtonClick);
		UIEventListener.Get(this.nextButton).onClick = new UIEventListener.VoidDelegate(this.OnNextButtonClick);
		UIEventListener.Get(this.backButton).onClick = new UIEventListener.VoidDelegate(this.OnReturnButtonClick);
		return true;
	}

	public override void OnShow()
	{
		base.OnShow();
		this.ShowTestFriend();
		this.SetPlayerBaseInfo();
	}

	public override void OnHide()
	{
		Solarmax.Singleton<BattleSystem>.Instance.Reset();
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

	public void OnReturnButtonClick(GameObject go)
	{
	}

	public void OnNextButtonClick(GameObject go)
	{
	}

	public override void OnUIEventHandler(EventId eventId, params object[] args)
	{
	}

	private void SetPlayerBaseInfo()
	{
		if (Solarmax.Singleton<LocalPlayer>.Get().playerData != null)
		{
			this.moneyGo.GetComponent<UILabel>().text = Solarmax.Singleton<LocalPlayer>.Get().playerData.money.ToString();
			this.powerGo.GetComponent<UILabel>().text = this.FormatPower();
		}
	}

	private string FormatPower()
	{
		int power = Solarmax.Singleton<LocalPlayer>.Get().playerData.power;
		string empty = string.Empty;
		return string.Format("{0} / 1000", power);
	}

	private void ShowLeftStar()
	{
		if (this.starNum == 4)
		{
			LevelConfig data = Solarmax.Singleton<LevelConfigConfigProvider>.Instance.GetData(this.selectedCfgId);
			this.leftStar.SetActive(data.maxStar == 4);
		}
		else
		{
			this.leftStar.SetActive(false);
		}
	}

	public UIEventListener eventListener;

	public GameObject nextButton;

	public GameObject returnButton;

	public GameObject friendButton;

	public GameObject allButton;

	public GameObject friendButtonLine;

	public GameObject allButtonLine;

	public GameObject friendTemplate;

	public GameObject limitGo;

	public GameObject localPlayer;

	public GameObject threeStars;

	public GameObject fourStars;

	public GameObject twoStars;

	public GameObject oneStarInFour;

	public GameObject oneStarInThree;

	public GameObject twoStarInFour;

	public GameObject threeStarInFour;

	public GameObject twoStarInThree;

	public GameObject awardPower;

	public GameObject awardMoney;

	public GameObject scoreNum;

	public GameObject backButton;

	public GameObject moneyGo;

	public GameObject powerGo;

	public GameObject leftStar;

	private Team winTeam;

	private int starNum = 1;

	private string selectedCfgId;
}
